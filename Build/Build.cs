using Nuke.Common.Git;
using Nuke.Common.ProjectModel;

partial class Build : NukeBuild
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / ArtifactsFolder;
    [GitRepository] readonly GitRepository GitRepository;
    [Solution] readonly Solution Solution;

    public static int Main() => Execute<Build>(x => x.Cleaning);

    List<string> GetConfigurations(params string[] startPatterns)
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Where(s => startPatterns.Any(s.StartsWith))
            .Select(s =>
            {
                var platformIndex = s.LastIndexOf('|');
                return s.Remove(platformIndex);
            })
            .ToList();
        if (configurations.Count == 0) throw new Exception($"Can't find configurations in the solution by patterns: {string.Join(" | ", startPatterns)}.");
        return configurations;
    }
}