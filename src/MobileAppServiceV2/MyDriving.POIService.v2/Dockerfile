#FROM microsoft/azure-functions-runtime:2.0.0-jessie

FROM azurefunctionstest/azure-functions-dotnet2.0:dev-jessie

ENV AzureWebJobsScriptRoot=/home/site/wwwroot
COPY ./bin/Debug/netstandard2.0/ /home/site/wwwroot
