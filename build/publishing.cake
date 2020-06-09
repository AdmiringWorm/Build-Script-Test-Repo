var owner = "AdmiringWorm";
var repo = "Build-Script-Test-Repo";

var createReleaseNotesTask = Task("Create-MilestoneReleaseNotes")
    .WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
    .WithCriteria(() => BuildSystem.GitHubActions.IsRunningOnGitHubActions)
    .WithCriteria(() => !BuildSystem.GitHubActions.Environment.PullRequest.IsPullRequest)
    .WithCriteria(() => BuildSystem.GitHubActions.Environment.Workflow.Ref != BuildSystem.GitHubActions.Environment.Workflow.HeadRef)
    .Does<BuildVersion>((buildVersion) =>
{
    var token = EnvironmentVariable("GITHUB_TOKEN");

    GitReleaseManagerCreate(token, owner, repo, new GitReleaseManagerCreateSettings {
        Milestone = buildVersion.MajorMinorPatch,
        Prerelease = buildVersion.SemVersion.Contains('-'),
        TargetCommitish = BuildSystem.GitHubActions.Environment.Workflow.Ref,
    });
});

var createTagTask = Task("Create-Tag")
    .Does<BuildVersion>((buildVersion) =>
{
    string message;
    if (buildVersion.MajorMinorPatch == buildVersion.SemVersion) {
        message = $"New official release of {buildVersion.MajorMinorPatch}";
    } else {
        message = $"Unstable pre-release of upcoming {buildVersion.MajorMinorPatch} ({buildVersion.SemVersion})";
    }

    StartProcess("git", $"tag {buildVersion.SemVersion} --sign --message \"{message}\"");

    if (HasArgument("push")) {
        StartProcess("git", $"push --follow-tags");
    }
});

var uploadReleaseArtifactsTask = Task("Upload-ReleaseArtifacts")
    .WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
    .WithCriteria(() => BuildSystem.AppVeyor.IsRunningOnAppVeyor)
    .WithCriteria(() => BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
    .WithCriteria(IsRunningOnWindows)
    .Does<BuildVersion>((buildVersion) =>
{
    var filesArr = GetFiles("*.md") + GetFiles("*.ps1") + GetFiles("*.sh");

    var files = string.Join(',', filesArr.Select(a => a.ToString()));
    var token = EnvironmentVariable("GITHUB_TOKEN");

    GitReleaseManagerAddAssets(token, owner, repo, BuildSystem.AppVeyor.Environment.Repository.Tag.Name, files);
});

var closeMilestoneTask = Task("Close-Milestones")
    .WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
    .WithCriteria(() => BuildSystem.GitHubActions.IsRunningOnGitHubActions)
    .Does(() =>
{
    var token = EnvironmentVariable("GITHUB_TOKEN");
    string tag = BuildSystem.GitHubActions.Environment.Workflow.Ref;
    int index = tag.LastIndexOf('/');
    if (index > -1) {
        tag = tag.Substring(index+1);
    }

    GitReleaseManagerClose(token, owner, repo, tag);
});

var publishReleaseTask = Task("Publish-Release")
    .WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
    .WithCriteria(() => BuildSystem.AppVeyor.IsRunningOnAppVeyor)
    .WithCriteria(() => BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
    .IsDependentOn(uploadReleaseArtifactsTask)
    .Does<BuildVersion>((buildVersion) =>
{
    var token = EnvironmentVariable("GITHUB_TOKEN");

    GitReleaseManagerPublish(token, owner, repo, BuildSystem.AppVeyor.Environment.Repository.Tag.Name);
});
