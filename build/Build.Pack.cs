using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Pack => _ => _
        .DependsOn(Clean)
        .Requires(() => ReleaseVersion)
        .Executes(() =>
        {
            var readme = CreateNugetReadme();
            try
            {
                var changelog = CreateNugetChangelog();
                DotNetPack(settings => settings
                    .SetConfiguration("Release")
                    .SetVersion(ReleaseVersion)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetVerbosity(DotNetVerbosity.minimal)
                    .SetPackageReleaseNotes(changelog));
            }
            finally
            {
                RestoreReadme(readme);
            }
        });

    string CreateNugetReadme()
    {
        var readmePath = Solution.Directory / "Readme.md";
        var readme = File.ReadAllText(readmePath);

        const string startSymbol = "<p";
        const string endSymbol = "</p>\r\n\r\n";

        var logoStartIndex = readme.IndexOf(startSymbol, StringComparison.Ordinal);
        var logoEndIndex = readme.IndexOf(endSymbol, StringComparison.Ordinal);

        var nugetReadme = readme.Remove(logoStartIndex, logoEndIndex - logoStartIndex + endSymbol.Length);
        File.WriteAllText(readmePath, nugetReadme);

        return readme;
    }

    void RestoreReadme(string readme)
    {
        var readmePath = Solution.Directory / "Readme.md";

        File.WriteAllText(readmePath, readme);
    }
}