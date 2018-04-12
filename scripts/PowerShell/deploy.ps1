Param(
   [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
   [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
   [int]    [Parameter(Mandatory=$true)] $numberOfDays,
   [string] [Parameter(Mandatory=$true)] $keyVaultName,
   [string] [Parameter(Mandatory=$true)] $keyVaultResourceGroup,
   [string] [Parameter(Mandatory=$true)] $thumbprint
)

# Variables
$repoRoot = "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY" + "\Azure-Samples_openhack-devops"
$sub = (Get-AzureRmContext).Subscription.Id

[string] $PreReqTemplateFile = "$repoRoot\scripts\ARM\prerequisites.json"
[string] $TemplateFile = "$repoRoot\scripts\ARM\scenario_complete.json"
[string] $ParametersFile = "$repoRoot\scripts\ARM\scenario_complete.params.json"
[string] $dbSchemaDB = "$repoRoot\src\SQLDatabase\MyDrivingDB.sql" 
[string] $dbSchemaSQLAnalytics = "$repoRoot\src\SQLDatabase\MyDrivingAnalyticsDB.sql"
[string] $DeploymentName = ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm'))

$deployment1 = $null
$deployment2 = $null

# Edit the ARM template with Key Vault values
(Get-Content $ParametersFile) -replace "this-sub-id-will-be-changed-by-vsts",$sub | out-file $ParametersFile
(Get-Content $ParametersFile) -replace "this-rg-name-will-be-changed-by-vsts",$keyVaultResourceGroup | out-file $ParametersFile
(Get-Content $ParametersFile) -replace "this-key-vault-name-will-be-changed-by-vsts",$keyVaultName | out-file $ParametersFile

# Create or update the resource group using the specified template file and template parameters file
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Creating the resource group..."
Write-Output "**************************************************************************************************"
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

# Create storage account
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Deploying the prerequisites..."
Write-Output "**************************************************************************************************"
$deployment1 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-0" `
                                                 -ResourceGroupName $ResourceGroupName `
                                                 -TemplateFile $PreReqTemplateFile `
                                                 -Mode Complete -Force -Verbose

if ($deployment1.ProvisioningState -ne "Succeeded") {
	Write-Error "Failed to provision the prerequisites storage account."
	exit 1
}

# Upload the HQL queries to the storage account container
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Uploading files to blob storage..."
Write-Output "**************************************************************************************************"
Write-Output "Uploading hive scripts..."
. "$repoRoot\scripts\PowerShell\scripts\Copy-ArtifactsToBlobStorage.ps1" -StorageAccountName $deployment1.Outputs.storageAccountName.Value -StorageAccountKey $deployment1.Outputs.storageAccountKey.Value -StorageContainerName $deployment1.Outputs.assetsContainerName.Value -ArtifactsPath "$repoRoot\src\HDInsight"

# Create required services
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Deploying the resources in the ARM template. This operation may take several minutes..."
Write-Output "**************************************************************************************************"

$deployment2 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-1" -ResourceGroupName $ResourceGroupName -TemplateFile $TemplateFile -TemplateParameterFile $ParametersFile -Force -Verbose

if ($deployment2.ProvisioningState -ne "Succeeded") {
	Write-Warning "Skipping the storage and database initialization..."
	Write-Error "At least one resource could not be provisioned successfully. Review the output above to correct any errors and then run the deployment script again."
	exit 2
}

# Configure blob storage
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Initializing blob storage..."
Write-Output "**************************************************************************************************"
$storageAccountName = $deployment2.Outputs.storageAccountNameAnalytics.Value
$storageAccountKey = $deployment2.Outputs.storageAccountKeyAnalytics.Value
. "$repoRoot\scripts\PowerShell\scripts\setupStorage.ps1" -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.rawdataContainerName.Value
. "$repoRoot\scripts\PowerShell\scripts\setupStorage.ps1" -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.tripdataContainerName.Value
. "$repoRoot\scripts\PowerShell\scripts\setupStorage.ps1" -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.referenceContainerName.Value

Write-Output "Uploading sample data..."
. $repoRoot\scripts\PowerShell\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $storageAccountName `
											-StorageAccountKey $storageAccountKey `
											-StorageContainerName $deployment2.Outputs.tripdataContainerName.Value `
											-ArtifactsPath "$repoRoot\scripts\Assets"


# Initialize SQL databases
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Preparing the SQL databases..."
Write-Output "**************************************************************************************************"
$databaseName = $deployment2.Outputs.sqlDBName.Value
Write-Output "Initializing the '$databaseName' database..."

. "$repoRoot\scripts\PowerShell\scripts\setupDb.ps1" -ServerName $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value `
    					-AdminLogin $deployment2.Outputs.sqlServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlDBName.Value `
						-ScriptPath $dbSchemaDB

$databaseName = $deployment2.Outputs.sqlAnalyticsDBName.Value
Write-Output "Initializing the '$databaseName' database..."

. "$repoRoot\scripts\PowerShell\scripts\setupDb.ps1" -ServerName $deployment2.Outputs.sqlAnalyticsFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlAnalyticsServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlAnalyticsServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlAnalyticsDBName.Value `
						-ScriptPath $dbSchemaSQLAnalytics

Write-Output ""

# Set variables that will be passed to VSTS
$owner = 'oguzp@microsoft.com'
#$owner = (Get-AzureRmContext).Account.Id
$mlStorageAccountName = $deployment1.Outputs.mlStorageAccountName.Value
$mlStorageAccountKey = $deployment1.Outputs.mlStorageAccountKey.Value
$sqlServerFullyQualifiedDomainName = $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value
$sqlServerAdminLogin = $deployment2.Outputs.sqlServerAdminLogin.Value
$sqlDBName = $deployment2.Outputs.sqlDBName.Value
$sqlAnalyticsFullyQualifiedDomainName = $deployment2.Outputs.sqlAnalyticsFullyQualifiedDomainName.Value
$sqlAnalyticsServerAdminLogin = $deployment2.Outputs.sqlAnalyticsServerAdminLogin.Value
$sqlAnalyticsDBName = $deployment2.Outputs.sqlAnalyticsDBName.Value

# VSTS variables to be used in the next task
Write-Host "##vso[task.setvariable variable=sub]$sub"
Write-Host "##vso[task.setvariable variable=owner]$owner"
Write-Host "##vso[task.setvariable variable=mlStorageAccountName]$mlStorageAccountName"
Write-Host "##vso[task.setvariable variable=mlStorageAccountKey]$mlStorageAccountKey"
Write-Host "##vso[task.setvariable variable=sqlServerFullyQualifiedDomainName]$sqlServerFullyQualifiedDomainName"
Write-Host "##vso[task.setvariable variable=sqlServerAdminLogin]$sqlServerAdminLogin"
Write-Host "##vso[task.setvariable variable=sqlDBName]$sqlDBName"
Write-Host "##vso[task.setvariable variable=sqlAnalyticsFullyQualifiedDomainName]$sqlAnalyticsFullyQualifiedDomainName"
Write-Host "##vso[task.setvariable variable=sqlAnalyticsServerAdminLogin]$sqlAnalyticsServerAdminLogin"
Write-Host "##vso[task.setvariable variable=sqlAnalyticsDBName]$sqlAnalyticsDBName"

