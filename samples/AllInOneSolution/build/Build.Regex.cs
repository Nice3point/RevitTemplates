using System.Text.RegularExpressions;

sealed partial class Build
{
    readonly Regex YearRegex = YearRegexGenerator();
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
}