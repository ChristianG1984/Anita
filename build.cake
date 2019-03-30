#tool "GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanDirectories("**/bin/" + configuration);
    CleanDirectories("**/obj/" + configuration);
    CleanDirectories("./Anita.Api.Tests/TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    // DotNetCoreRestore();
    NuGetRestore("./Anita.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild("./Anita.sln", new MSBuildSettings {
        Configuration = configuration
    });
    // DotNetCoreBuild("./Anita.sln", new DotNetCoreBuildSettings {
    //     Configuration = configuration
    // });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("./Anita.Api.Tests/Anita.Api.Tests.csproj", new DotNetCoreTestSettings {
        Configuration = configuration,
        NoBuild = true,
        ArgumentCustomization = args => args
            .Append("--collect").AppendQuoted("Code Coverage")
            .Append("--logger").Append("Appveyor")
    });
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);