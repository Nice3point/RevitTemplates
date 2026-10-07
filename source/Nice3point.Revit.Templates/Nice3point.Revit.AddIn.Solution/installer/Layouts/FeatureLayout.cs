using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using WixSharp;

namespace Installer.Layouts;

/// <summary>
///     Provides extension methods for a list of <see cref="Manifest.AddinContent" /> to create one installation feature per Revit version.
/// </summary>
public static class FeatureLayout
{
    /// <param name="content">The add-in content to install.</param>
    extension(IReadOnlyList<Manifest.AddinContent> content)
    {
        /// <summary>
        ///     Creates the <see cref="Dir" /> tree that installs the add-in for every Revit version.
        /// </summary>
        /// <param name="contentRoot">The root directory of the relative content paths.</param>
        /// <param name="scope">The installation scope of the packages.</param>
        /// <param name="mediaLayout">The cabinet layout of the packages.</param>
        /// <returns>The directories to install.</returns>
        /// <exception cref="DirectoryNotFoundException">The base directory of a file set doesn't exist.</exception>
        /// <exception cref="InvalidDataException">A file set matches no files.</exception>
        /// <remarks>The feature tree contains one feature per Revit version, and the user can change the installation directory of each feature.</remarks>
        public Dir[] CreateFeatureLayout(DirectoryInfo contentRoot, InstallScope scope, MediaLayout mediaLayout)
        {
            var revitFeature = new Feature
            {
                Name = "Revit add-in",
                Description = "Installs the add-in for the selected Revit versions.",
                Display = FeatureDisplay.expand
            };

            return
            [
                .. content
                    .GroupBy(addin => ResolveAddinsRoot(addin.RevitVersion, scope))
                    .Select(addinsRoot => new Dir(addinsRoot.Key,
                    [
                        .. addinsRoot.Select(addin => CreateVersionDirectory(addin, revitFeature, contentRoot, mediaLayout))
                    ]))
            ];
        }
    }

    /// <summary>
    ///     Creates the installation directory of the add-in for a single Revit version.
    /// </summary>
    private static Dir CreateVersionDirectory(Manifest.AddinContent addin, Feature revitFeature, DirectoryInfo contentRoot, MediaLayout mediaLayout)
    {
        var fileVersion = addin.RevitVersion.ToString();
        var feature = new Feature
        {
            Name = fileVersion,
            Description = $"Installs the add-in for Revit {fileVersion}.",
            ConfigurableDir = $"INSTALL{fileVersion}"
        };

        revitFeature.Add(feature);

        var fileSets = addin.Files
            .Select(fileSet => CreateFiles(fileSet, feature, contentRoot, mediaLayout))
            .Cast<WixEntity>()
            .ToArray();

        return new Dir(new Id($"INSTALL{fileVersion}"), fileVersion, fileSets);
    }

    /// <summary>
    ///     Selects the files of a single file set and assigns them to the specified feature and cabinet.
    /// </summary>
    private static Files CreateFiles(Manifest.FileSet fileSet, Feature feature, DirectoryInfo contentRoot, MediaLayout mediaLayout)
    {
        var basePath = Path.GetFullPath(Path.Combine(contentRoot.FullName, fileSet.BasePath));
        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"Cannot create the installer. The base directory '{basePath}' of the '{fileSet.Role}' file set does not exist.");
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddIncludePatterns(fileSet.Include);
        matcher.AddExcludePatterns(fileSet.Exclude);

        var matchingResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(basePath)));
        if (!matchingResult.HasMatches)
        {
            throw new InvalidDataException($"Cannot create the installer. The '{fileSet.Role}' file set selects no files in '{basePath}'. Check the include and exclude patterns of the file set.");
        }

        var selectedFiles = matchingResult.Files
            .Select(match => Path.GetFullPath(Path.Combine(basePath, match.Path)))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        LogSelectedFiles(fileSet, selectedFiles);

        var diskId = mediaLayout.ResolveDiskId(fileSet.Role);
        var selectedPaths = selectedFiles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return new Files(feature, Path.Combine(basePath, "*.*"), path => selectedPaths.Contains(Path.GetFullPath(path)))
        {
            OnProcess = file => file.AttributesDefinition = $"DiskId={diskId}"
        };
    }

    /// <summary>
    ///     Gets the Revit add-ins directory for the specified Revit version and installation scope.
    /// </summary>
    private static string ResolveAddinsRoot(int revitVersion, InstallScope scope)
    {
        if (scope is InstallScope.perUser)
        {
            return @"%AppDataFolder%\Autodesk\Revit\Addins";
        }

        return revitVersion switch
        {
            >= 2027 => @"%ProgramFiles%\Autodesk\Revit\Addins",
            _ => @"%CommonAppDataFolder%\Autodesk\Revit\Addins"
        };
    }

    /// <summary>
    ///     Writes the selected files of a file set to the console.
    /// </summary>
    private static void LogSelectedFiles(Manifest.FileSet fileSet, string[] selectedFiles)
    {
        Console.WriteLine($"{fileSet.Role} files for Revit add-in ({selectedFiles.Length}):");

        foreach (var selectedFile in selectedFiles)
        {
            Console.WriteLine($"- {selectedFile}");
        }
    }
}
