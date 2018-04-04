Param(
   [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
   [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
   [string] [Parameter(Mandatory=$true)] $keyVaultName,
   [string] [Parameter(Mandatory=$true)] $vstsSPNClientID,
   [string] [Parameter(Mandatory=$true)] $subscriptionID,
   [string] [Parameter(Mandatory=$true)] $keyVaultAdminUser
)

Select-AzureRmSubscription -SubscriptionId $subscriptionID
Register-AzureRmResourceProvider -ProviderNamespace Microsoft.KeyVault
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

$spn = (Get-AzureRmADServicePrincipal -SPN $vstsSPNClientID)
$spnObjectId = $spn.Id
$UserObjectId = (Get-AzureRmADUser -SearchString $keyVaultAdminUser).Id

New-AzureRmKeyVault -VaultName $keyVaultName -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation -EnabledForTemplateDeployment  

Set-AzureRmKeyVaultAccessPolicy -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -ObjectId $UserObjectId -PermissionsToSecrets all
Set-AzureRmKeyVaultAccessPolicy -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -ObjectId $spnObjectId -PermissionsToSecrets all

$sqlServerAdminPassword = "OpenHack" + "-" + (-join (48..57 | ForEach-Object {[char]$_} | Get-Random -Count 8))
$sqlAnalyticsServerAdminPassword = "OpenHack" + "-" + (-join (48..57 | ForEach-Object {[char]$_} | Get-Random -Count 8))
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerAdminPassword' -SecretValue (ConvertTo-SecureString $sqlServerAdminPassword -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsServerAdminPassword' -SecretValue (ConvertTo-SecureString $sqlAnalyticsServerAdminPassword -AsPlainText -Force)
