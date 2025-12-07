using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WixSharp;

namespace Installer;

public static class Generator
{
    /// <summary>
    ///     Generates Wix entities, features and directories for the installer.
    /// </summary>
    public static WixEntity[] GenerateWixEntities(IEnumerable<string> args)
    {
        var versionRegex = new Regex(@"\d+");
        var versionStorages = new Dictionary<string, List<WixEntity>>();

        var revitFeature = new Feature
        {
            Name = "Revit Add-in",
            Description = "Revit add-in installation files",
            Display = FeatureDisplay.expand
        };
        
        foreach (var directory in args)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var fileVersion = versionRegex.Match(directoryInfo.Name).Value;
            var feature = new Feature
            {
                Name = fileVersion,
                Description = $"Install add-in for Revit {fileVersion}",
                ConfigurableDir = $"INSTALL{fileVersion}"
            };

            revitFeature.Add(feature);

            var files = new Files(feature, $@"{directory}\*.*", FilterEntities);
            if (versionStorages.TryGetValue(fileVersion, out var storage))
            {
                storage.Add(files);
            }
            else
            {
                versionStorages.Add(fileVersion, [files]);
            }

            LogFeatureFiles(directory, fileVersion);
        }

        return versionStorages
            .Select(storage => new Dir(new Id($"INSTALL{storage.Key}"), storage.Key, storage.Value.ToArray()))
            .Cast<WixEntity>()
            .ToArray();
    }

    /// <summary>
    ///     Filter installer files and exclude from output. 
    /// </summary>
    private static bool FilterEntities(string file)
    {
        return !file.EndsWith(".pdb");
    }

    /// <summary>
    ///    Write a list of installer files.
    /// </summary>
    private static void LogFeatureFiles(string directory, string fileVersion)
    {
        var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        Console.WriteLine($"Installer files for version '{fileVersion}':");

        foreach (var assembly in assemblies.Where(FilterEntities))
        {
            Console.WriteLine($"'{assembly}'");
        }
    }
}