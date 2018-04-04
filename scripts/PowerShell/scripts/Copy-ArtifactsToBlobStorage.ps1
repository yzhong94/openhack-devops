Param ([string] $StorageAccountName, [string] $StorageAccountKey, [string] $StorageContainerName, [string] $ArtifactsPath = "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY\Azure-Samples_openhack-devops\src\HDInsight", [string] $AzCopyPath = "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY\Azure-Samples_openhack-devops\scripts\PowerShell\tools\AzCopy.exe")

& $AzCopyPath """$ArtifactsPath""", "https://$StorageAccountName.blob.core.windows.net/$StorageContainerName", "/DestKey:$StorageAccountKey", "/S", "/Y"
