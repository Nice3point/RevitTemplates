using System.IO.Enumeration;
using JetBrains.Annotations;

[PublicAPI]
sealed partial class Build : NukeBuild
{
    /// <summary>
    ///     Pipeline entry point.
    /// </summary>
    public static int Main() => Execute<Build>(build => build.Compile);

    /// <summary>
    ///     Extract solution configuration names from the solution file.
    /// </summary>
    List<string> GlobBuildConfigurations()
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Select(config => config.Remove(config.LastIndexOf('|')))
            .Where(config => Configurations.Any(wildcard => FileSystemName.MatchesSimpleExpression(wildcard, config)))
            .ToList();

        Assert.NotEmpty(configurations, $"No solution configurations have been found. Pattern: {string.Join(" | ", Configurations)}");
        return configurations;
    }
}