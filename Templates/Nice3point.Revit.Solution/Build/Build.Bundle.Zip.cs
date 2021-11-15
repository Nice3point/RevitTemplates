using System;
using System.IO;
using System.IO.Compression;
using Nuke.Common;

partial class Build
{
    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Produces(ArtifactsDirectory / "*.zip")
        .Executes(() =>
        {
            var bundleDirectory = Solution.GetBundleDirectory(ArtifactsDirectory);
            if (Directory.Exists(bundleDirectory))
            {
                var archiveName = $"{bundleDirectory}.zip";
                Logger.Normal($"Archive creation: {archiveName}");
                ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
                Logger.Normal($"Deletion directory: {bundleDirectory}");
                Directory.Delete(bundleDirectory, true);
            }
            else
            {
                throw new Exception($"Directory not found for archiving: {bundleDirectory}");
            }
        });
}