sealed partial class Build
{
    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                var bundlePath = ArtifactsDirectory / $"{project.Name}.bundle";
                var bundleName = bundlePath.WithExtension(".zip");
                bundlePath.CompressTo(bundleName);
                bundlePath.DeleteDirectory();

                Log.Information("Bundle: {Name}", bundleName);
            }
        });
}