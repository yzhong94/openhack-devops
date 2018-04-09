$sqlServerFullyQualifiedDomainName = $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value
$sqlServerAdminLogin = $deployment2.Outputs.sqlServerAdminLogin.Value
$sqlDBName = $deployment2.Outputs.sqlDBName.Value

Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerFullyQualifiedDomainName' -SecretValue (ConvertTo-SecureString $sqlServerFullyQualifiedDomainName -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlServerAdminLogin' -SecretValue (ConvertTo-SecureString $sqlServerAdminLogin -AsPlainText -Force)
Set-AzureKeyVaultSecret -VaultName $keyVaultName -Name 'sqlDBName' -SecretValue (ConvertTo-SecureString $sqlDBName -AsPlainText -Force)

