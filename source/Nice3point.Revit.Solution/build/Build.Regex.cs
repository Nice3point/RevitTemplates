using System.Text.RegularExpressions;

sealed partial class Build
{
<!--#if (Bundle)
    readonly Regex YearRegex = YearRegexGenerator();
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
}