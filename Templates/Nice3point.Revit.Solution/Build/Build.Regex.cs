using System.Text.RegularExpressions;

partial class Build
{
<!--#if (Bundle)
    readonly Regex YearRegex = YearRegexGenerator();
#endif-->
<!--#if (GitHubPipeline)
    readonly Regex VersionRegex = VersionRegexGenerator();
#endif-->
<!--#if (Installer)
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();
#endif-->
<!--#if (Bundle)

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();
#endif-->
<!--#if (Installer)

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
#endif-->
<!--#if (GitHubPipeline)

    [GeneratedRegex(@"(\d+\.)+\d+", RegexOptions.Compiled)]
    private static partial Regex VersionRegexGenerator();
#endif-->
}