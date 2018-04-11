# Proctor VM

## Description

## Pre-requisites

- Access to [openhack-team-cli](https://github.com/Azure-Samples/openhack-team-cli.git)

## Usage

    `./setup.sh -i <subscriptionId> -g <resourceGroupName> -r <registryName> -c <clusterName> -l <resourceGroupLocation> -n <teamName>`

**NOTE:** You will be asked to login to your subscription if you have not already done so using the azure cli.

### Parameters

- SubscriptionId - id of the subscription to deploy the team infrastructure to
- resourceGroupName -  The name of the resource group to create and deploy to.
- regisgtryName - Name for the Azure Container Registry **_(MUST BE UNIQUE)_**
- clusterName - Name of the Azure Kubernetes Service (k8s) instance **_(MUST BE UNIQUE)_**
- resourceGroupLocation - Azure region to deploy to.  **_Must be a region that supports ACR and AKS._**
- teamName - name of the team.  Containers and apps will use this value in provisioning.
