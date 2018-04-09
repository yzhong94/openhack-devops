# MyDriving DB
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerFullyQualifiedDomainName' -SecretValue (ConvertTo-SecureString $sqlServerFullyQualifiedDomainName -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerAdminLogin' -SecretValue (ConvertTo-SecureString $sqlServerAdminLogin -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlDBName' -SecretValue (ConvertTo-SecureString $sqlDBName -AsPlainText -Force)

# MyDriving Analytics DB

Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsFullyQualifiedDomainName' -SecretValue (ConvertTo-SecureString $sqlAnalyticsFullyQualifiedDomainName -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsServerAdminLogin' -SecretValue (ConvertTo-SecureString $sqlAnalyticsServerAdminLogin -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlAnalyticsDBName' -SecretValue (ConvertTo-SecureString $sqlAnalyticsDBName -AsPlainText -Force)

