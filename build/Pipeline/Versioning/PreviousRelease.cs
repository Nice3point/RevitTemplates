using JetBrains.Annotations;

namespace Build.Pipeline.Versioning;

/// <summary>
///     Represents the release the version is compared with: the tag of the previous release, or the root commit if no earlier commit has a tag.
/// </summary>
[PublicAPI]
public union PreviousRelease(ReleaseTag, RootCommit);

/// <summary>
///     Represents the tag of the previous release.
/// </summary>
/// <param name="Name">The name of the tag.</param>
/// <example><c>6.3.2</c> or <c>7.0.0-rc.1</c>.</example>
[PublicAPI]
public sealed record ReleaseTag(string Name);

/// <summary>
///     Represents the root commit of the repository, the start of the history when no earlier commit has a release tag.
/// </summary>
/// <param name="Sha">The SHA of the commit.</param>
/// <example><c>2f5c3e1a9b7d4c6e8f0a1b2c3d4e5f6a7b8c9d0e</c>.</example>
[PublicAPI]
public sealed record RootCommit(string Sha);
