using System.IO.Compression;
using Nuke.Common;
using Serilog;

partial class Build
{
    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Executes(() =>
        {
            var bundleDirectory = Solution.GetBundleDirectory(ArtifactsDirectory);
            if (Directory.Exists(bundleDirectory))
            {
                var archiveName = $"{bundleDirectory}.zip";
                Log.Information("Archive creation: {Directory}", archiveName);
                ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
                Log.Information("Deletion directory: {Directory}", bundleDirectory);
                Directory.Delete(bundleDirectory, true);
            }
            else
            {
                throw new Exception($"Directory not found for archiving: {bundleDirectory}");
            }
        });
}