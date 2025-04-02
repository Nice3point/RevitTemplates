using System.Text.RegularExpressions;

sealed partial class Build
{
    /// <summary>
    ///     Regex for parsing the product version.
    /// </summary>
    readonly Regex YearRegex = YearRegexGenerator();

    /// <summary>
    ///     Regex for parsing Process arguments from the output.
    /// </summary>
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
}