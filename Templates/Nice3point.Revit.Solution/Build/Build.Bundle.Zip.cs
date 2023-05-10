using System.IO.Compression;

sealed partial class Build
{
    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                var bundleDirectory = ArtifactsDirectory / "{project.Name}.bundle";
                bundleDirectory.CompressTo(bundleDirectory / ".zip");
                bundleDirectory.DeleteDirectory();

                Log.Information("Bundle: {Name}", archiveName);
            }
        });
}