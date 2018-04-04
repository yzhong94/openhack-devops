Param(
   [string] [Parameter(Mandatory=$true)] $ResourceGroupName
)

Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "mydriving-archive" -OutputStartMode "JobStartTime"
Start-AzureRmStreamAnalyticsJob -ResourceGroupName $ResourceGroupName -Name "mydriving-vinlookup" -OutputStartMode "JobStartTime"
