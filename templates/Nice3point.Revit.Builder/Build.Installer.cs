using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Nuke.Common;

partial class Build
{
    Target CreateInstaller => _ => _
        .TriggeredBy(Compile)
        .Produces(ArtifactsDirectory / "*.msi")
        .Executes(() =>
        {
            var mainProject = BuilderExtensions.GetProject(Solution, MainProjectName);
            var installerProject = BuilderExtensions.GetProject(Solution, InstallerProjectName);

            var releaseAddInDirectories = GetReleaseAddInDirectories();
            var configurations = GetConfigurations(InstallerConfiguration);
            var configurationPattern = new Regex(@".*");

            foreach (var directoryGroup in releaseAddInDirectories)
            {
                var installerConfigurations = new List<string>();
                var directories = directoryGroup.ToList();
                IterateDirectoriesRegex(directories, configurationPattern, (_, value) => installerConfigurations.Add(value));

                var mainParameters = new string[] { mainProject.GetBinDirectory(), installerProject.Directory };
                var exeArguments = BuildExeArguments(mainParameters.Concat(installerConfigurations).ToList());

                var exeFile = installerProject.GetExecutableFile(configurations, directories);
                if (string.IsNullOrEmpty(exeFile))
                {
                    Logger.Warn($"No installer executable was found for these packages:\n {string.Join("\n", directories)}");
                    continue;
                }

                var proc = new Process();
                proc.StartInfo.FileName  = exeFile;
                proc.StartInfo.Arguments = exeArguments;
                proc.Start();
                if (IsServerBuild || releaseAddInDirectories.Count > 1)
                {
                    Logger.Normal($"Waiting {InstallerCreationTime} seconds to create installer on another thread");
                    Thread.Sleep(TimeSpan.FromSeconds(InstallerCreationTime));
                }
            }
        });

    string BuildExeArguments(IReadOnlyList<string> args)
    {
        var argumentBuilder = new StringBuilder();
        for (var i = 0; i < args.Count; i++)
        {
            if (i > 0) argumentBuilder.Append(' ');
            var value = args[i];
            if (value.Contains(' ')) value = $"\"{value}\"";
            argumentBuilder.Append(value);
        }

        return argumentBuilder.ToString();
    }
}