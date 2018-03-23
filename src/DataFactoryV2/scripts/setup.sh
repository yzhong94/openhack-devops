#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

# -e: immediately exit if any command has a non-zero exit status
# -o: prevents errors in a pipeline from being masked
# IFS new value is less likely to cause confusing bugs when looping arrays or arguments (e.g. $@)

usage() { echo "Usage: $0 -i <subscriptionId> -g <resourceGroupName> -n <deploymentName> -l <resourceGroupLocation>" 1>&2; exit 1; }

declare subscriptionId=""
declare resourceGroupName=""
declare deploymentName=""
declare resourceGroupLocation=""

# Initialize parameters specified from command line
while getopts ":i:g:n:l:" arg; do
	case "${arg}" in
		i)
			subscriptionId=${OPTARG}
			;;
		g)
			resourceGroupName=${OPTARG}
			;;
		n)
			deploymentName=${OPTARG}
			;;
		l)
			resourceGroupLocation=${OPTARG}
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

if [[ -z "$resourceGroupName" ]]; then
	echo "This script will look for an existing resource group, otherwise a new one will be created "
	echo "You can create new resource groups with the CLI using: az group create "
	echo "Enter a resource group name"
	read resourceGroupName
	[[ "${resourceGroupName:?}" ]]
fi

if [[ -z "$deploymentName" ]]; then
	echo "Enter a name for this deployment:"
	read deploymentName
fi

if [[ -z "$resourceGroupLocation" ]]; then
	echo "If creating a *new* resource group, you need to set a location "
	echo "You can lookup locations with the CLI using: az account list-locations "

	echo "Enter resource group location:"
	read resourceGroupLocation
fi

#templateFile Paths
dataFactoryTemplateFilePath="../ARM/datafactory_template.json"

if [ ! -f "$dataFactoryTemplateFilePath" ]; then
	echo "$dataFactoryTemplateFilePath not found"
	exit 1
fi

linkedServicesTemplateFilePath="../ARM/linkedservices_template.json"

if [ ! -f "$linkedServicesTemplateFilePath" ]; then
	echo "$linkedServicesTemplateFilePath not found"
	exit 1
fi

datasetsTemplateFilePath="../ARM/datasets_template.json"

if [ ! -f "$datasetsTemplateFilePath" ]; then
	echo "$datasetsTemplateFilePath not found"
	exit 1
fi

pipelinesTemplateFilePath="../ARM/pipelines_template.json"

if [ ! -f "$pipelinesTemplateFilePath" ]; then
	echo "$pipelinesTemplateFilePath not found"
	exit 1
fi

#parameter file paths
dataFactoryParametersFilePath="../ARM/datafactory_parameters.json"

if [ ! -f "$dataFactoryParametersFilePath" ]; then
	echo "$dataFactoryParametersFilePath not found"
	exit 1
fi

linkedServicesParametersFilePath="../ARM/linkedservices_parameters.json"

if [ ! -f "$linkedServicesParametersFilePath" ]; then
	echo "$linkedServicesParametersFilePath not found"
	exit 1
fi

datasetsParametersFilePath="../ARM/datasets_parameters.json"

if [ ! -f "$datasetsParametersFilePath" ]; then
	echo "$datasetsParametersFilePath not found"
	exit 1
fi

pipelinesParametersFilePath="../ARM/pipelines_parameters.json"

if [ ! -f "$pipelinesParametersFilePath" ]; then
	echo "$pipelinesParametersFilePath not found"
	exit 1
fi

if [ -z "$subscriptionId" ] || [ -z "$resourceGroupName" ] || [ -z "$deploymentName" ]; then
	echo "Either one of subscriptionId, resourceGroupName, deploymentName is empty"
	usage
fi

#login to azure using your credentials
az account show 1> /dev/null

if [ $? != 0 ];
then
	az login
fi

#set the default subscription id
az account set --subscription $subscriptionId

set +e

#Check for existing RG
az group show -n $resourceGroupName 1> /dev/null

if [ $? != 0 ]; then
	echo "Resource group with name" $resourceGroupName "could not be found. Creating new resource group.."
	set -e
	(
		set -x
		az group create --name $resourceGroupName --location $resourceGroupLocation 1> /dev/null
	)
	else
	echo "Using existing resource group..."
fi

#Start data factory deployment
echo "Starting data factory deployment..."
(
	set -x
	az group deployment create --name "$deploymentName-df" --resource-group "$resourceGroupName" --template-file "$dataFactoryTemplateFilePath" --parameters "@${dataFactoryParametersFilePath}"
)

if [ $?  == 0 ];
 then
	echo "Data Factory template has been successfully deployed"
fi

#Start linked services deployment
echo "Starting linked services deployment..."
(
	set -x
	az group deployment create --name "$deploymentName-ls" --resource-group "$resourceGroupName" --template-file "$linkedServicesTemplateFilePath" --parameters "@${linkedServicesParametersFilePath}"
)

if [ $?  == 0 ];
 then
	echo "Linked Services template has been successfully deployed"
fi

#Start datasets deployment
echo "Starting datasets deployment..."
(
	set -x
	az group deployment create --name "$deploymentName-ds" --resource-group "$resourceGroupName" --template-file "$datasetsTemplateFilePath" --parameters "@${datasetsParametersFilePath}"
)

if [ $?  == 0 ];
 then
	echo "Datasets template has been successfully deployed"
fi

#Start pipelines deployment
echo "Starting datasets deployment..."
(
	set -x
	az group deployment create --name "$deploymentName-pl" --resource-group "$resourceGroupName" --template-file "$pipelinesTemplateFilePath" --parameters "@${pipelinesParametersFilePath}"
)

if [ $?  == 0 ];
 then
	echo "Pipelines template has been successfully deployed"
fi