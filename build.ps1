$TargetSolution = ".\FunctionsDevDemo.sln"

$TestsPattern = "FunctionsDevDemo.*.Tests"

$ArtifactPath = ".\artifacts"
$ArtifactName = "FunctionsDevDemo.zip"
$ArtifactSource = ".\src\FunctionsDevDemo.Functions\publish\ReleaseArtifact\*"

# Stop if we hit an error

$ErrorActionPreference = "Stop"
trap {
    Write-Error @($Error)[0]
    exit 1
}

# Nuget restore


# Build the solution via MSBuild

& msbuild "$TargetSolution" /m /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile="Artifact"

# Run all of our tests

$TestContainers = (Get-ChildItem "tests/$TestsPattern/bin/Release/$TestsPattern.dll" |
                    ForEach-Object { 
                        "`/testcontainer:`"$_`"" 
                    }) -join " "

& mstest $TestContainers

# Produce an artifact

$ArtifactTarget = "$ArtifactPath\$ArtifactName"
if (!(Test-Path -Path $ArtifactPath)) { New-Item -Path $ArtifactPath -ItemType Directory }
if (Test-Path -Path $ArtifactTarget) { Remove-Item -Path $ArtifactTarget }

Compress-Archive -Path $ArtifactSource -DestinationPath $ArtifactTarget