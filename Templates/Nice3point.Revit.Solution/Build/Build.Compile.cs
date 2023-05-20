using System.IO.Enumeration;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    Target Compile => _ => _
        .TriggeredBy(Clean)
        .Executes(() =>
        {
            foreach (var configuration in GlobBuildConfigurations())
                DotNetBuild(settings => settings
                    .SetConfiguration(configuration)
                    .SetVersion(Version)
                    .SetVerbosity(DotNetVerbosity.Minimal));
        });

    List<string> GlobBuildConfigurations()
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Select(config => config.Remove(config.LastIndexOf('|')))
            .Where(config => Configurations.Any(wildcard => FileSystemName.MatchesSimpleExpression(wildcard, config)))
            .ToList();

        if (configurations.Count == 0)
            throw new Exception($"No solution configurations have been found. Pattern: {string.Join(" | ", Configurations)}");

        return configurations;
    }
}