#tool "GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var cleanTask = Task("Clean")
    .Does(() => {
        CleanDirectories("**/bin/" + configuration);
        CleanDirectories("**/obj/" + configuration);
        CleanDirectories("./Anita.Api.Tests/TestResults");
    });

var restoreTask = Task("Restore")
    .IsDependentOn(cleanTask)
    .Does(() => {
        // DotNetCoreRestore();
        NuGetRestore("./Anita.sln");
    });

var updateVersionInfos = Task("UpdateVersionInfos")
    .Does(() => {
        GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true
        });
    });

var buildTask = Task("Build")
    .IsDependentOn(restoreTask)
    .IsDependentOn(updateVersionInfos)
    .Does(() => {
        MSBuild("./Anita.sln", new MSBuildSettings {
            Configuration = configuration
        });
        // DotNetCoreBuild("./Anita.sln", new DotNetCoreBuildSettings {
        //     Configuration = configuration
        // });
    });

var testTask = Task("Test")
    .IsDependentOn(buildTask)
    .Does(() => {
        DotNetCoreTest("./Anita.Api.Tests/Anita.Api.Tests.csproj", new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true,
            ArgumentCustomization = args => args
                .Append("--collect").AppendQuoted("Code Coverage")
                .Append("--logger").Append("Appveyor")
        });
    });

var defaultTask = Task("Default")
    .IsDependentOn(testTask);

RunTarget(target);