using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

[CheckBuildProjectConfigurations]
class Build : NukeBuild
{
    const string ProjectName = "FamilyUpdater"; /*caret*/
    const string InstallerName = "Installer";

    [Solution] readonly Solution Solution;

    Target Cleaning => _ => _
        .Executes(() =>
        {
            var outputDirectory = GetOutputDirectory();
            if (!Directory.Exists(outputDirectory)) return;
            var directoryInfo = new DirectoryInfo(outputDirectory);
            foreach (var file in directoryInfo.GetFiles()) file.Delete();
            foreach (var dir in directoryInfo.GetDirectories()) dir.Delete(true);
        });

    Target Compile => _ => _
        .TriggeredBy(Cleaning)
        .Executes(() =>
        {
            var releaseConfigurations = GetReleaseConfigurations();
            if (releaseConfigurations.Count == 0) throw new Exception("There are no configurations in the project.");

            foreach (var configuration in releaseConfigurations) BuildProject(configuration);
        });

    Target CreateInstaller => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var installerDirectory = GetProjectDirectory(InstallerName);
            var installerExe = GetExeDirectory(InstallerName);
            var projectDirectory = GetBinDirectory(ProjectName);

            var addInsDirectory = GetAddInsDirectory();
            var versions = new List<string>();
            IterateVersions(addInsDirectory, (_, version) =>
            {
                versions.Add(version);
            });

            var argumentBuilder = new StringBuilder();
            argumentBuilder.Append($"\"{projectDirectory}\"");
            argumentBuilder.Append(' ');
            argumentBuilder.Append($"\"{installerDirectory}\"");
            foreach (var version in versions)
            {
                argumentBuilder.Append(' ');
                argumentBuilder.Append(version);
            }

            var proc = new Process();
            proc.StartInfo.FileName  = installerExe;
            proc.StartInfo.Arguments = argumentBuilder.ToString();
            proc.Start();
        });

    Target CreateBundle => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var addInsDirectory = GetAddInsDirectory();
            var bundleDirectory = GetBundleDirectory();
            var contentDirectory = Path.Combine(bundleDirectory, "Contents");

            IterateVersions(addInsDirectory, (directoryInfo, version) =>
            {
                var buildDirectory = Path.Combine(contentDirectory, version);
                CopyFilesContent(directoryInfo.FullName, buildDirectory);
            });
        });

    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Executes(() =>
        {
            var bundleDirectory = GetBundleDirectory();
            var archiveName = $"{bundleDirectory}.zip";
            ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
        });

    public static int Main() => Execute<Build>(x => x.Cleaning);

    List<string> GetReleaseConfigurations()
    {
        return Solution.Configurations
            .Select(pair => pair.Key)
            .Where(s => s.StartsWith("Release"))
            .Select(s => s.Replace("|Any CPU", ""))
            .ToList();
    }

    void CopyFilesContent(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }

    void IterateVersions(List<DirectoryInfo> directories, Action<DirectoryInfo, string> action)
    {
        var versionPattern = new Regex(@"\d+");
        foreach (var directoryInfo in directories)
        {
            var version = versionPattern.Match(directoryInfo.Name).Value;
            if (string.IsNullOrEmpty(version))
            {
                Logger.Warn($"Missing version number for build \"{directoryInfo.Name}\"");
                continue;
            }

            action?.Invoke(directoryInfo, version);
        }
    }

    string GetOutputDirectory() => Path.Combine(Solution.Directory!, "output");

    List<DirectoryInfo> GetAddInsDirectory()
    {
        var projectDirectory = GetBinDirectory(ProjectName);
        var addInsDirectory = new DirectoryInfo(projectDirectory).GetDirectories()
            .Where(dir => dir.Name.StartsWith("AddIn"))
            .ToList();

        if (addInsDirectory.Count == 0) throw new Exception("There are no packaged assemblies in the project. Try to build the project again.");
        return addInsDirectory;
    }

    string GetBundleDirectory()
    {
        var bundleName = $"{ProjectName}.bundle";
        var bundleDirectory = Path.Combine(GetOutputDirectory(), bundleName);
        return bundleDirectory;
    }

    string GetBinDirectory(string projectName)
    {
        var projectDirectory = GetProjectDirectory(projectName);
        return Path.Join(projectDirectory, "bin");
    }

    string GetProjectDirectory(string projectName)
    {
        var project = Solution.GetProject(projectName);
        if (project == null) throw new NullReferenceException($"Cannon find project \"{projectName}\"");
        return project.Directory;
    }

    string GetExeDirectory(string projectName)
    {
        var binDirectory = GetBinDirectory(projectName);
        var releaseDirectory = Path.Join(binDirectory, "Release");
        return Path.Join(releaseDirectory, $"{projectName}.exe");
    }

    void BuildProject(string configuration) =>
        MSBuild(s => s
            .SetTargetPath(Solution)
            .SetConfiguration(configuration)
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetMaxCpuCount(Environment.ProcessorCount)
            .DisableNodeReuse());
}