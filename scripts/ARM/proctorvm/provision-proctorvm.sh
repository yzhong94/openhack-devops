#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

usage() { echo "Usage: setup.sh -i <subscriptionId> -n <resourceGroupName> -l <resourceGroupLocation> -a <newStorageAccountName> -u <adminUsername> -d <dnsNameForPublicIP>"  1>&2; exit 1; }

declare subscriptionId=""
declare resourceGroupName=""
declare resourceGroupLocation=""

declare newStorageAccountName=""
declare adminUsername=""
declare dnsNameForPublicIP=""

# Initialize parameters specified from command line
while getopts ":i:n:l:a:u:d:" arg; do
    case "${arg}" in
        i)
            subscriptionId=${OPTARG}
        ;;
        n)
            resourceGroupName=${OPTARG}
        ;;
        l)
            resourceGroupLocation=${OPTARG}
        ;;
        a)
            newStorageAccountName=${OPTARG}
        ;;
        u)
            adminUsername=${OPTARG}
        ;;
        d)
            dnsNameForPublicIP=${OPTARG}
        ;;        
    esac
done
shift $((OPTIND-1))

#Prompt for parameters is some required parameters are missing
if [[ -z "$subscriptionId" ]]; then
    echo "Your subscription ID can be looked up with the CLI using: az account show --out json "
    echo "Enter your subscription ID:"
    read subscriptionId
    [[ "${subscriptionId:?}" ]]
fi

#Prompt for parameters is some required parameters are missing
if [[ -z "$resourceGroupName" ]]; then
    echo "Your Resource Group Name"
    echo "Enter your resource group name:"
    read resourceGroupName
    [[ "${resourceGroupName:?}" ]]
fi

#Prompt for parameters is some required parameters are missing
if [[ -z "$resourceGroupLocation" ]]; then
    echo "Your Resource Group Location"
    echo "Enter your resource group location:"
    read resourceGroupLocation
    [[ "${resourceGroupLocation:?}" ]]
fi

#Prompt for parameters is some required parameters are missing
if [[ -z "$newStorageAccountName" ]]; then
    echo "Your Storage Account"
    echo "Enter your new storage account:"
    read newStorageAccountName
    [[ "${newStorageAccountName:?}" ]]
fi

#Prompt for parameters is some required parameters are missing
if [[ -z "$adminUsername" ]]; then
    echo "Your VM user name"
    echo "Enter your admin user name:"
    read adminUsername
    [[ "${adminUsername:?}" ]]
fi

#Prompt for parameters is some required parameters are missing
if [[ -z "$dnsNameForPublicIP" ]]; then
    echo "Your DNS public IP "
    echo "Enter your new DNS public IP:"
    read dnsNameForPublicIP
    [[ "${dnsNameForPublicIP:?}" ]]
fi

#login to azure using your credentials
az account show 1> /dev/null

if [ $? != 0 ];
then
    az login
fi

#set the default subscription id
echo "Setting subscription to $subscriptionId..."

az account set --subscription $subscriptionId

#Create Resource Group
echo "Creating Resource Group $resourceGroupName in $resourceGroupLocation ..."
az group create --name $resourceGroupName --location $resourceGroupLocation

#Provision VM
echo "Provisioning Proctor VM on $resourceGroupName in [$resourceGroupLocation]..."
az group deployment create --name $resourceGroupName --resource-group $resourceGroupName --parameters "{\"newStorageAccountName\": {\"value\": \"$newStorageAccountName\"},\"adminUsername\": {\"value\": \"$adminUsername\"},\"dnsNameForPublicIP\": {\"value\": \"$dnsNameForPublicIP\"}}" --template-uri https://raw.githubusercontent.com/odaibert/azure-cli-docker-ubuntu-vm/master/azuredeploy.json
