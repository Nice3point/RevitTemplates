using System.IO.Compression;
using Nuke.Common;
using Serilog;

partial class Build
{
    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                var bundleDirectory = ArtifactsDirectory / $"{project.Name}.bundle";

                var archiveName = $"{bundleDirectory}.zip";
                ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
                Directory.Delete(bundleDirectory, true);

                Log.Information("Bundle: {Directory}", archiveName);
            }
        });
}