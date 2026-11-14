using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Wix;

/// <summary>
///     Represents the options that every command of the WiX Toolset has.
/// </summary>
/// <remarks>If <see cref="CommandLineToolOptions.Tool" /> is not set, the command runs the <c>wix</c> executable found in the <c>PATH</c> environment variable.</remarks>
[CliTool("wix")]
public abstract record WixOptions : CommandLineToolOptions;
