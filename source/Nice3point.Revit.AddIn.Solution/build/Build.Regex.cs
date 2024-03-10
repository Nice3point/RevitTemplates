using System.Text.RegularExpressions;

sealed partial class Build
{
#if (bundle)
    readonly Regex YearRegex = YearRegexGenerator();
#endif
#if (installer)
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