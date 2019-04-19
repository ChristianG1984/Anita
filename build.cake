#tool GitVersion.CommandLine
#tool coveralls.io

#addin Cake.Coveralls
#addin Cake.Coverlet

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var rootPath = new DirectoryPath("./");
var artifactsPath = rootPath.Combine("artifacts");
var testResultsPath = artifactsPath.Combine("TestResults");

Task("name")
    .Does(() => {
        var openCoverPath = new DirectoryPath("../").Combine(testResultsPath);
        Information(openCoverPath);
        Information(artifactsPath.FullPath);
        Information(DirectoryExists(artifactsPath));
        EnsureDirectoryExists(testResultsPath);
        Information(DirectoryExists(artifactsPath));
        DeleteDirectory(artifactsPath, new DeleteDirectorySettings {
            Recursive = true
        });
    });

var cleanTask = Task("Clean")
    .Does(() => {
        DotNetCoreClean("./Anita.sln", new DotNetCoreCleanSettings {
            Configuration = configuration
        });
        CleanDirectories("**/bin/" + configuration);
        CleanDirectories("**/obj/" + configuration);
        CleanDirectories("./Anita.Api.Tests/TestResults");
        if (DirectoryExists(artifactsPath))
            DeleteDirectory(artifactsPath, new DeleteDirectorySettings {
                Recursive = true
            });
    });

var restoreTask = Task("Restore")
    .IsDependentOn(cleanTask)
    .Does(() => {
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
    });

var testTask = Task("Test")
    .IsDependentOn(buildTask)
    .Does(() => {
        EnsureDirectoryExists(testResultsPath);
        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.opencover,
            CoverletOutputDirectory = testResultsPath,
            CoverletOutputName = $"results-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss-FFF}",
            Exclude = new List<string>{
                "[*.Tests]*"
            }
        };
        DotNetCoreTest("./Anita.Api.Tests/Anita.Api.Tests.csproj",
        new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true,
            ArgumentCustomization = args => {
                if (BuildSystem.IsRunningOnAppVeyor)
                    return args.Append("--logger").Append("Appveyor");
                else
                    return args;
            }
        },
        coverletSettings);
    });

var uploadCoverageReport = Task("Upload-Coverage-Report")
    .IsDependentOn(testTask)
    .Does(() => {
        var dInfo = new DirectoryInfo(testResultsPath.FullPath);
        var files = dInfo.GetFiles("*.opencover.xml");
        foreach (var file in files) {
            CoverallsIo(file.FullName, new CoverallsIoSettings() {
                RepoToken = EnvironmentVariable("COVERALLS_ANITA_TOKEN")
            });
        }
    });

var defaultTask = Task("Default")
    .IsDependentOn(uploadCoverageReport);

RunTarget(target);