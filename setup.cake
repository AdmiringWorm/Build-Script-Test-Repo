#load "./build/*.cake"


var target = Argument("target", "Default");

Task("Report")
    .Does(() =>
{
    var envVars = new[] {
        "CI",
        "HOME",
    };
    if (BuildSystem.IsRunningOnGitHubActions)
    {
        envVars = new[] {
            "CI",
            "HOME",
            "GITHUB_WORKFLOW",
            "GITHUB_RUN_ID",
            "GITHUB_RUN_NUMBER",
            "GITHUB_ACTION",
            "GITHUB_ACTIONS",
            "GITHUB_ACTOR",
            "GITHUB_REPOSITORY",
            "GITHUB_EVENT_NAME",
            "GITHUB_EVENT_PATH",
            "GITHUB_WORKSPACE",
            "GITHUB_SHA",
            "GITHUB_REF",
            "GITHUB_HEAD_REF",
            "GITHUB_BASE_REF",
        };
    }
    else if (BuildSystem.IsRunningOnAppVeyor)
    {
        envVars = new[] {
            "CI",
            "APPVEYOR",
            "APPVEYOR_URL",
            "APPVEYOR_API_URL",
            "APPVEYOR_ACCOUNT_NAME",
            "APPVEYOR_PROJECT_ID",
            "APPVEYOR_PROJECT_NAME",
            "APPVEYOR_PROJECT_SLUG",
            "APPVEYOR_BUILD_FOLDER",
            "APPVEYOR_BUILD_ID",
            "APPVEYOR_BUILD_NUMBER",
            "APPVEYOR_BUILD_VERSION",
            "APPVEYOR_BUILD_WORKER_IMAGE",
            "APPVEYOR_PULL_REQUEST_NUMBER",
            "APPVEYOR_PULL_REQUEST_TITLE",
            "APPVEYOR_PULL_REQUEST_HEAD_REPO_NAME",
            "APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH",
            "APPVEYOR_PULL_REQUEST_HEAD_COMMIT",
            "APPVEYOR_JOB_ID",
            "APPVEYOR_JOB_NAME",
            "APPVEYOR_JOB_NUMBER",
            "APPVEYOR_REPO_PROVIDER",
            "APPVEYOR_REPO_TAG",
            "APPVEYOR_REPO_TAG_NAME",
            "APPVEYOR_REPO_COMMIT",
            "APPVEYOR_REPO_COMMIT_AUTHOR",
            "APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL",
            "APPVEYOR_REPO_COMMIT_TIMESTAMP",
            "APPVEYOR_REPO_COMMIT_MESSAGE",
            "APPVEYOR_REPO_COMMIT_EXTENDED",
            "APPVEYOR_SCHEDULED_BUILD",
            "APPVEYOR_FORCED_BUILD",
            "APPVEYOR_RE_BUILD",
            "APPVEYOR_RE_RUN_INCOMPLETE",
            "PLATFORM",
            "CONFIGURATION",
        };
    }

    foreach (var env in envVars.OrderBy(s => s))
    {
        Information($"{env,17}: '{EnvironmentVariable(env)}'");
    }
});

Task("Default")
    .IsDependentOn("Report");

Task("AppVeyor")
    .IsDependentOn("Default")
    .IsDependentOn(publishReleaseTask);

RunTarget(target);
