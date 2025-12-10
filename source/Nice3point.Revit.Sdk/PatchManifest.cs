using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Nice3point.Revit.Sdk;

[PublicAPI]
public class PatchManifest : Task
{
    [Required] public required ITaskItem[] Manifests { get; set; }
    public string? RevitVersion { get; set; }

    public override bool Execute()
    {
        try
        {
            if (!int.TryParse(RevitVersion, out var targetVersion)) return true;

            foreach (var manifest in Manifests)
            {
                var path = manifest.GetMetadata("FullPath");

                PatchManifestSettings(path, targetVersion);
            }

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);
            return false;
        }
    }

    private void PatchManifestSettings(string manifestPath, int targetVersion)
    {
        if (targetVersion >= 2026) return;

        var xmlDocument = XDocument.Load(manifestPath);

        var manifestSettings = xmlDocument.Root?.Elements("ManifestSettings").FirstOrDefault();
        if (manifestSettings is null) return;

        Log.LogMessage(MessageImportance.High, $"Patching {manifestPath}: removing 'ManifestSettings'");

        manifestSettings.Remove();
        xmlDocument.Save(manifestPath);
    }
}