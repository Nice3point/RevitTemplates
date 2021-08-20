using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
partial class Build : NukeBuild
{
    const string WixTargetPath = @"%USERPROFILE%\.nuget\packages\wixsharp\1.18.1\build\WixSharp.targets";
    const string IlRepackTargetPath = @"%USERPROFILE%\.nuget\packages\ilrepack.lib.msbuild.task\2.0.18.2\build\ILRepack.Lib.MSBuild.Task.targets";
    readonly AbsolutePath OutputDirectory = RootDirectory / "output";

    [Solution] readonly Solution Solution;
    AbsolutePath BundleDirectory;
    ProjectInfo InstallerInfo;
    ProjectInfo ProjectInfo;
    Regex VersionPattern;

    Target InitializeBuilder => _ => _
        .Executes(() =>
        {
            InstallerInfo   = new ProjectInfo(Solution, "Installer"); /*caret*/
            ProjectInfo     = new ProjectInfo(Solution, "Nice3point.FrameworkAddIn");
            BundleDirectory = OutputDirectory / $"{ProjectInfo.ProjectName}.bundle";
            VersionPattern  = new Regex(@"\d+");
        });

    Target Restore => _ => _
        .TriggeredBy(InitializeBuilder)
        .Executes(() =>
        {
            if (IsLocalBuild) return;
            var releaseConfigurations = GetReleaseConfigurations();
            foreach (var configuration in releaseConfigurations) RestoreProject(configuration);
        });

    Target Cleaning => _ => _
        .TriggeredBy(Restore)
        .Executes(() =>
        {
            if (Directory.Exists(OutputDirectory))
            {
                var directoryInfo = new DirectoryInfo(OutputDirectory);
                foreach (var file in directoryInfo.GetFiles()) file.Delete();
                foreach (var dir in directoryInfo.GetDirectories()) dir.Delete(true);
            }
            else
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            var wixTargetPath = Environment.ExpandEnvironmentVariables(WixTargetPath);
            var ilTargetPath = Environment.ExpandEnvironmentVariables(IlRepackTargetPath);

            if (File.Exists(wixTargetPath)) ReplaceFileText("<Target Name=\"MSIAuthoring\">", wixTargetPath, 3);
            if (File.Exists(ilTargetPath)) ReplaceFileText("<Target Name=\"ILRepack\">", ilTargetPath, 13);
        });

    Target Compile => _ => _
        .TriggeredBy(Cleaning)
        .Executes(() =>
        {
            var releaseConfigurations = GetReleaseConfigurations();
            foreach (var configuration in releaseConfigurations) BuildProject(configuration);
        });

    Target CreateInstaller => _ => _
        .TriggeredBy(Compile)
        .Produces(OutputDirectory / "*.msi")
        .Executes(() =>
        {
            var addInsDirectory = GetAddInsDirectory();
            var versions = new List<string>();
            IterateVersions(addInsDirectory, (_, version) => versions.Add(version));
            var exeArguments = BuildExeArguments(ProjectInfo.BinDirectory, InstallerInfo.Project.Directory, string.Join(' ', versions));
            var proc = new Process();
            proc.StartInfo.FileName  = InstallerInfo.ExecutableFile;
            proc.StartInfo.Arguments = exeArguments;
            proc.Start();
            if (!IsServerBuild) return;
            Logger.Normal("Waiting 10 seconds to create installer on another thread");
            Thread.Sleep(TimeSpan.FromSeconds(10));
        });

    Target CreateBundle => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var addInsDirectory = GetAddInsDirectory();
            var contentDirectory = BundleDirectory / "Contents";
            Directory.CreateDirectory(contentDirectory);

            IterateVersions(addInsDirectory, (directoryInfo, version) =>
            {
                var buildDirectory = contentDirectory / version;
                Logger.Normal($"Copy files from: {directoryInfo.FullName} to {buildDirectory}");
                CopyFilesContent(directoryInfo.FullName, buildDirectory);
            });
        });

    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Produces(OutputDirectory / "*.zip")
        .Executes(() =>
        {
            var archiveName = $"{BundleDirectory}.zip";
            Logger.Normal($"Archive creation: {BundleDirectory}\\{archiveName}");
            ZipFile.CreateFromDirectory(BundleDirectory, archiveName);
        });

    public static int Main() => Execute<Build>(x => x.InitializeBuilder);

    List<string> GetReleaseConfigurations()
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Where(s => s.StartsWith("Release"))
            .Select(s => s.Replace("|Any CPU", ""))
            .ToList();
        if (configurations.Count == 0) throw new Exception("There are no release configurations in the project.");
        return configurations;
    }

    List<DirectoryInfo> GetAddInsDirectory()
    {
        var addInsDirectory = new DirectoryInfo(ProjectInfo.BinDirectory).GetDirectories()
            .Where(dir => dir.Name.StartsWith("AddIn"))
            .ToList();

        if (addInsDirectory.Count == 0) throw new Exception("There are no packaged assemblies in the project. Try to build the project again.");
        return addInsDirectory;
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

    void CopyFilesContent(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }

    static void ReplaceFileText(string newText, string fileName, int lineNumber)
    {
        var arrLine = File.ReadAllLines(fileName);
        var lineText = arrLine[lineNumber - 1];
        if (lineText.Equals(newText)) return;
        arrLine[lineNumber - 1] = newText;
        File.WriteAllLines(fileName, arrLine);
    }

    string BuildExeArguments(params string[] args)
    {
        var argumentBuilder = new StringBuilder();
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0) argumentBuilder.Append(' ');
            var value = args[i];
            if (value.Contains(' ')) value = $"\"{value}\"";
            argumentBuilder.Append(value);
        }

        return argumentBuilder.ToString();
    }

    void RestoreProject(string configuration) =>
        MSBuild(s => s
            .SetTargetPath(Solution)
            .SetTargets("Restore")
            .SetConfiguration(configuration)
        );

    void BuildProject(string configuration) =>
        MSBuild(s => s
            .SetTargetPath(Solution)
            .SetTargets("Rebuild")
            .SetConfiguration(configuration)
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetMaxCpuCount(Environment.ProcessorCount)
            .DisableNodeReuse());
}