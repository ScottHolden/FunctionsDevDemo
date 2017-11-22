param(
    $Name = "dev",
    $ResourceGroupName = "dev-automated-publish",
    $ResourceGroupLocation = "Australia South East",
    $SubscriptionID = "19a82f81-88d1-4c33-8511-8abe7e30fc94"
)

# Stop if we hit an error

$ErrorActionPreference = "Stop"
trap {
    Write-Error @($Error)[0]
    exit 1
}

$TemplatePath = ".\deploy\azuredeploy.json"
$ArtifactPath = ".\artifacts\FunctionsDevDemo.zip"

# Log in to our resources

try
{
    Set-AzureRmContext -SubscriptionId $SubscriptionID -ErrorAction Stop
}
catch
{
    Login-AzureRmAccount -SubscriptionId $SubscriptionID
}

# Ensure that our resource group exists
Write-Host "Creating resource group"
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Confirm:$false -Force

# Deploy our template

$parameters = @{
    Name = $Name
}

Write-Host "Deploying template"
$output = New-AzureRmResourceGroupDeployment -Name "funcdevdemo$((Get-Date).ToString('yyyyMMddHHmmss'))" `
                                                -ResourceGroupName $ResourceGroupName `
                                                -TemplateFile $TemplatePath `
                                                -TemplateParameterObject $parameters

$output | Select-Object -Property DeploymentName,CorrelationId,ResourceGroupName,ProvisioningState,Timestamp,Mode


$PingUrl = $output.Outputs.pingUrl.Value
$PublishUrl = $output.Outputs.publishUrl.Value
$PublishAuthHeader = $output.Outputs.publishAuthenticationHeader.Value

# Wake up our functions (optional, but makes everything run faster)
Write-Host "Triggering wakeup"
Invoke-WebRequest -UseBasicParsing `
                    -Uri $PingUrl `
                    -Method Post | Out-Null

# Upload our binaries
Write-Host "Uploading binaries"
Invoke-WebRequest -UseBasicParsing `
                    -Uri $PublishUrl `
                    -Headers @{Authorization=$PublishAuthHeader} `
                    -Method Post `
                    -InFile $ArtifactPath `
                    -ContentType "multipart/form-data" | Select-Object -Property StatusCode,StatusDescription