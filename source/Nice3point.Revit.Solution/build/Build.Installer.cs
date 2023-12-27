using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
<!--#if (!NoPipeline)
using Nuke.Common.Git;
#endif-->
using Nuke.Common.Utilities;
using Serilog.Events;

sealed partial class Build
{
    Target CreateInstaller => _ => _
        .DependsOn(Compile)
<!--#if (!NoPipeline)
        .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnMainOrMasterBranch())
#endif-->
        .Executes(() =>
        {
            foreach (var (installer, project) in InstallersMap)
            {
                Log.Information("Project: {Name}", project.Name);

                var exePattern = $"*{installer.Name}.exe";
                var exeFile = Directory.EnumerateFiles(installer.Directory, exePattern, SearchOption.AllDirectories)
                    .FirstOrDefault()
                    .NotNull($"No installer file was found for the project: {installer.Name}");

                var directories = Directory.GetDirectories(project.Directory, "* Release *", SearchOption.AllDirectories);
                Assert.NotEmpty(directories, "No files were found to create an installer");

                var process = new Process();
                process.StartInfo.FileName = exeFile;
                process.StartInfo.Arguments = directories.Select(path => path.DoubleQuoteIfNeeded()).JoinSpace();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                RedirectStream(process.StandardOutput, LogEventLevel.Information);
                RedirectStream(process.StandardError, LogEventLevel.Error);

                process.WaitForExit();
                if (process.ExitCode != 0) throw new InvalidOperationException($"The installer creation failed. ExitCode {process.ExitCode}");
            }
        });

    [SuppressMessage("ReSharper", "TemplateIsNotCompileTimeConstantProblem")]
    void RedirectStream(StreamReader reader, LogEventLevel eventLevel)
    {
        while (!reader.EndOfStream)
        {
            var value = reader.ReadLine();
            if (value is null) continue;

            var matches = ArgumentsRegex.Matches(value);
            if (matches.Count > 0)
            {
                var parameters = matches
                    .Select(match => match.Value.Substring(1, match.Value.Length - 2))
                    .Cast<object>()
                    .ToArray();

                var line = ArgumentsRegex.Replace(value, match => $"{{Parameter{match.Index}}}");
                Log.Write(eventLevel, line, parameters);
            }
            else
            {
                Log.Debug(value);
            }
        }
    }
}