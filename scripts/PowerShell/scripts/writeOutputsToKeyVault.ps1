# MyDriving DB
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerFullyQualifiedDomainName' -SecretValue (ConvertTo-SecureString $sqlServerFullyQualifiedDomainName -AsPlainText -Force)
Write-Host "1"
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerAdminLogin' -SecretValue (ConvertTo-SecureString $sqlServerAdminLogin -AsPlainText -Force)
Write-Host "2"
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlDBName' -SecretValue (ConvertTo-SecureString $sqlDBName -AsPlainText -Force)
Write-Host "3"

# MyDriving Analytics DB
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsFullyQualifiedDomainName' -SecretValue (ConvertTo-SecureString $sqlAnalyticsFullyQualifiedDomainName -AsPlainText -Force)
Write-Host "4"
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsServerAdminLogin' -SecretValue (ConvertTo-SecureString $sqlAnalyticsServerAdminLogin -AsPlainText -Force)
Write-Host "5"
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsDBName' -SecretValue (ConvertTo-SecureString $sqlAnalyticsDBName -AsPlainText -Force)
Write-Host "6"