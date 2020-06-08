#load "./build/*.cake"


var target = Argument("target", "Default");

Task("Default");

Task("AppVeyor")
    .IsDependentOn("Default")
    .IsDependentOn(uploadReleaseArtifactsTask);

RunTarget(target);
