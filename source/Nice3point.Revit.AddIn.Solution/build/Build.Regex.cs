using System.Text.RegularExpressions;

sealed partial class Build
{
#if (bundle)
    /// <summary>
    ///     Regex for parsing the product version.
    /// </summary>
    readonly Regex YearRegex = YearRegexGenerator();
#endif
#if (installer)
    /// <summary>
    ///     Regex for parsing Process arguments from the output.
    /// </summary>
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();
#endif
#if (bundle)

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();
#endif
#if (installer)

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
#endif
}