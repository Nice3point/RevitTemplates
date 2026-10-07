using System.Xml.Linq;

namespace Installer.Layouts;

/// <summary>
///     Represents the cabinet files of the installer packages, one cabinet per file set role.
/// </summary>
/// <remarks>
///     Windows Installer copies the files of the cabinets in the order of their disk IDs.
///     The roles are installed in the order in which they appear in the manifest.
/// </remarks>
public sealed class MediaLayout
{
    private readonly string[] _cabinets;
    private readonly string[] _roles;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaLayout" /> class.
    /// </summary>
    /// <param name="content">The add-in content that defines the file set roles.</param>
    public MediaLayout(IReadOnlyList<Manifest.AddinContent> content)
    {
        _roles =
        [
            .. content
                .SelectMany(addin => addin.Files)
                .Select(fileSet => fileSet.Role)
                .Distinct(StringComparer.OrdinalIgnoreCase)
        ];

        _cabinets = [.. _roles.Select(role => $"{role}.cab")];
    }

    /// <summary>
    ///     Gets the disk ID of the cabinet that contains the specified file set role.
    /// </summary>
    /// <param name="role">The role of the file set.</param>
    /// <returns>The one-based disk ID of the cabinet.</returns>
    /// <exception cref="InvalidDataException">The manifest has no file set with the specified role.</exception>
    public int ResolveDiskId(string role)
    {
        var index = Array.FindIndex(_roles, candidate => string.Equals(candidate, role, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            throw new InvalidDataException($"Cannot resolve the cabinet of the '{role}' file set. The manifest has no file set with this role.");
        }

        return index + 1;
    }

    /// <summary>
    ///     Adds the cabinets to the generated WiX source.
    /// </summary>
    /// <param name="document">The generated WiX source document.</param>
    /// <exception cref="InvalidDataException">The document has no <c>Package</c> element.</exception>
    /// <remarks>A WixSharp project defines a single <c>Media</c> element. The method replaces it with one <c>Media</c> element per cabinet.</remarks>
    public void WriteToWixSource(XDocument document)
    {
        var package = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Package");
        if (package is null)
        {
            throw new InvalidDataException("Cannot write the cabinets to the WiX source. The generated source has no Package element.");
        }

        var wixNamespace = package.Name.Namespace;
        var generatedMedia = package.Elements(wixNamespace + "Media").ToList();
        var replacement = _cabinets
            .Select((cabinet, index) => new XElement(wixNamespace + "Media",
                new XAttribute("Id", index + 1),
                new XAttribute("Cabinet", cabinet),
                new XAttribute("EmbedCab", "yes")))
            .ToList();

        var anchor = generatedMedia.LastOrDefault() ?? package.Elements(wixNamespace + "SummaryInformation").LastOrDefault();
        if (anchor is null)
        {
            package.AddFirst(replacement);
        }
        else
        {
            anchor.AddAfterSelf(replacement);
        }

        foreach (var element in generatedMedia)
        {
            element.Remove();
        }
    }
}
