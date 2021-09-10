using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
partial class Build : NukeBuild
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / ArtifactsFolder;

    [Solution] public readonly Solution Solution;


    Target Restore => _ => _
        .Executes(() =>
        {
            var releaseConfigurations = GetConfigurations(BuildConfiguration, InstallerConfiguration);
            foreach (var configuration in releaseConfigurations) RestoreProject(configuration);
        });

    Target Cleaning => _ => _
        .TriggeredBy(Restore)
        .Executes(() =>
        {
            if (Directory.Exists(ArtifactsDirectory))
            {
                var directoryInfo = new DirectoryInfo(ArtifactsDirectory);
                foreach (var file in directoryInfo.GetFiles()) file.Delete();
                foreach (var dir in directoryInfo.GetDirectories()) dir.Delete(true);
            }
            else
            {
                Directory.CreateDirectory(ArtifactsDirectory);
            }

            var wixTargetPath = Environment.ExpandEnvironmentVariables(WixTargetPath);
            var ilTargetPath = Environment.ExpandEnvironmentVariables(IlRepackTargetPath);

            if (File.Exists(wixTargetPath)) ReplaceFileText("<Target Name=\"MSIAuthoring\">", wixTargetPath, 3);
            if (File.Exists(ilTargetPath)) ReplaceFileText("<Target Name=\"ILRepack\">", ilTargetPath, 13);

            if (IsServerBuild) return;

            var project = BuilderExtensions.GetProject(Solution, MainProjectName);
            var binDirectoryInfo = new DirectoryInfo(project.GetBinDirectory());
            if (!binDirectoryInfo.Exists) return;
            var addInDirectories = binDirectoryInfo.EnumerateDirectories()
                .Where(info => info.Name.StartsWith(AddInBinPrefix))
                .ToList();

            foreach (var addInDirectory in addInDirectories)
            {
                foreach (var file in addInDirectory.GetFiles()) file.Delete();
                addInDirectory.Delete(true);
            }
        });

    Target Compile => _ => _
        .TriggeredBy(Cleaning)
        .Executes(() =>
        {
            var configurations = GetConfigurations(BuildConfiguration, InstallerConfiguration);
            foreach (var configuration in configurations) BuildProject(configuration);
        });

    public static int Main() => Execute<Build>(x => x.Restore);

    List<string> GetConfigurations(params string[] startPatterns)
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Where(s => startPatterns.Any(s.StartsWith))
            .Select(s => s.Replace("|Any CPU", string.Empty))
            .ToList();
        if (configurations.Count == 0) throw new Exception("There are no release configurations in the project.");
        return configurations;
    }

    List<IGrouping<int, DirectoryInfo>> GetReleaseAddInDirectories()
    {
        var project = BuilderExtensions.GetProject(Solution, MainProjectName);
        var addInsDirectory = new DirectoryInfo(project.GetBinDirectory()).GetDirectories()
            .Where(dir => dir.Name.StartsWith(AddInBinPrefix))
            .Where(dir => dir.Name.Contains(BuildConfiguration))
            .GroupBy(info => info.Name.Length)
            .ToList();

        if (addInsDirectory.Count == 0) throw new Exception("There are no packaged assemblies in the project. Try to build the project again.");
        return addInsDirectory;
    }

    static void ReplaceFileText(string newText, string fileName, int lineNumber)
    {
        var arrLine = File.ReadAllLines(fileName);
        var lineText = arrLine[lineNumber - 1];
        if (lineText.Equals(newText)) return;
        arrLine[lineNumber - 1] = newText;
        File.WriteAllLines(fileName, arrLine);
    }

    void IterateDirectoriesRegex(List<DirectoryInfo> directories, Regex dirRegex, Action<DirectoryInfo, string> action)
    {
        foreach (var directoryInfo in directories)
        {
            var subName = dirRegex.Match(directoryInfo.Name).Value;
            if (string.IsNullOrEmpty(subName))
            {
                Logger.Warn($"Missing substring for directory: \"{directoryInfo.Name}\"");
                continue;
            }

            action?.Invoke(directoryInfo, subName);
        }
    }

    void RestoreProject(string configuration) =>
        MSBuild(s => s
            .SetTargetPath(Solution)
            .SetTargets("Restore")
            .SetVerbosity(MSBuildVerbosity.Minimal)
            .SetConfiguration(configuration)
        );

    void BuildProject(string configuration) =>
        MSBuild(s => s
            .SetTargetPath(Solution)
            .SetTargets("Rebuild")
            .SetVerbosity(MSBuildVerbosity.Minimal)
            .SetConfiguration(configuration)
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetMaxCpuCount(Environment.ProcessorCount)
            .DisableNodeReuse());
}