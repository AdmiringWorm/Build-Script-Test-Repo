#load "./build/*.cake"


var target = Argument("target", "Default");

Task("Report")
    .Does(() =>
{
    var githubActions = EnvironmentVariable("GITHUB_ACTIONS");
    var githubAction = EnvironmentVariable("GITHUB_ACTION");

    Information($"GITHUB_ACTIONS: {githubActions}");
    Information($"GITHUB_ACTION:  {githubAction}");
});

Task("Default")
    .IsDependentOn("Report");

Task("AppVeyor")
    .IsDependentOn("Default")
    .IsDependentOn(publishReleaseTask);

RunTarget(target);
