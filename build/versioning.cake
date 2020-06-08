public class BuildVersion
{
    public string SemVersion { get; set; }
    public string SemVersionPadded { get; set; }
    public string MajorMinorPatch { get; set; }
    public string InformationalVersion { get; set; }
}

Setup<BuildVersion>((ctx) => {
        var gitVersion = ctx.GitVersion();

        ctx.Information($"Building Codecove.exe version {gitVersion.SemVer}!");

        return new BuildVersion
        {
            MajorMinorPatch      = gitVersion.MajorMinorPatch,
            SemVersion           = gitVersion.SemVer,
            SemVersionPadded     = gitVersion.LegacySemVerPadded,
            InformationalVersion = gitVersion.InformationalVersion,
        };
});
