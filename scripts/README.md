# Deploying the shared backend using the VSTS release pipeline
VSTS release pipeline allows you to deploy the entire backend on your Azure subscription.  After running the deployment scripts, follow the manual configuration instructions to complete service configurations.

## Prerequisites

### If you use PowerShell

* [Visual Studio Team Services (VSTS) Account](http://aka.ms/webpi-azps)
* [An active Azure subscription](https://azure.microsoft.com) with at least 24 available cores (for on-demand HDInsight cluster)

## Create a Key Vault
You need to have a Key Vault to keep the secrets that are used in the VSTS pipeline and PowerShell scripts. If you already have a Key Vault, you can use that. Make sure to follow the steps below for setting up your Key Vault so VSTS can access it.

If you don't already have a Key Vault or want to create a new one, use the createKeyVault.ps1 script under the **/scripts/PowerShell/scripts** folder.

Usage:

.\createKeyVault.ps1 <resource group name> <resource group location> <key vault name> <service principal ID used by your VSTS account> <your azure subscription ID> <key vault admin user ID>

Resource group name: Resource group to be created
Resource group location: Location of the resource group that will be created
Key Vault Name: Choose a unique name for you Key Vault
Service principal ID used by your VSTS account: You need to authorize the SPN created by VSTS to access your Key Vault. To find that SPN, go to your VSTS account, then S


## Create a VSTS Release pipeline 
Create a new project in your VSTS account. Then import the Release Management pipeline file named OpenHack-RM.json under the **/scripts/PowerShell/tools** directory and made the necessary changes (Azure Subscription etc.)

Run the release and follow the following steps when the release is completed successfully.


## Manual Configuration

### Modify MyDrivingDB
From the portal, access the MyDrivingDB Sql database.  Login to the query editor and run:

```
ALTER TABLE IOTHubDatas ADD DEFAULT (0) FOR Deleted
ALTER TABLE POIs ADD DEFAULT (0) FOR Deleted
```

### Configure Azure Streaming Analytics Power BI Outputs 

1. In the [Azure classic portal](https://manage.windowsazure.com/), go to **Stream Analytics** and select the **mydriving-hourlypbi** job.

1. Click the **STOP** button at the bottom of the page. You need to stop the job in order to add a new output.

1. Click **OUTPUTS** at the top of the page, and then click **Add Output**.

1. In the **Add an Output** dialog box, select **Power BI** and then click next.

1. In the **Add a Microsoft Power BI output**, supply a work or school account for the Stream Analytics job output. If you already have a Power BI account, select **Authorize Now**. If not, choose **Sign up now**.

	![Adding a Power BI output](Images/adding-powerbi-output.png?raw=true "Adding a Power BI output")

	_Adding a Power BI output_

1. Next, set the following values and click the checkmark:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: ASA-HourlyData
	- **Table Name**: HourlyTripData
	- **Workspace**: You can use the default

	![Power BI output settings](Images/asa-powerbi-output-settings.png?raw=true "Power BI output settings")

	_Power BI output settings_

1. Click the **START** button to restart the job.

1. Repeat the same steps to configure the **mydriving-sqlpbi** job using the following values:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: MyDriving-ASAdataset
	- **Table Name**: TripPointData
	- **Workspace**: You can use the default

1. Make sure that **mydriving-archive**, **mydriving-sqlpbi**, **mydriving-hourlypbi**, and **mydriving-vinlookup** jobs are all running.

### Machine Learning Configuration
1. Before you can proceed, you need to obtain the credentials for the storage account and SQL databases in the solution. Go 
   to the [Azure portal](https://portal.azure.com), click **Resource Groups**, select the solution's resource group, then **All Settings**, and then **Resources**. 
    
1. In the list of resources, click the storage account whose name is prefixed with **"mydrivingstr"**, then **All Settings**, and then **Access Keys**. Make a note of the **Storage Account Name** and **Key1**.

   ![Storage Account Credentials](Images/storage-account-credentials.png?raw=true "Storage Account Credentials")
   
   _Storage Account Credentials_

1. Next, locate the **myDrivingAnalyticsDB** SQL Database in the solution, open its **All Settings** blade, and then its **Properties** blade. Make a note of the, database name, **Server Name** and **Server Admin Login** properties.
  
   ![SQL Database Credentials](Images/sql-database-credentials.png?raw=true "SQL Database Credentials")
   
   _SQL Database Credentials_
 
1. Repeat the previous step to obtain the database name, **Server Name** and **Server Admin Login** properties of the **myDrivingDB** SQL Database.

1. Now, go to the [Azure classic portal](https://manage.windowsazure.com/), select the **Machine Learning** service and then the **mydriving** workspace. Open the workspace by selecting **Sign-in to ML Studio**.

1. Click the **Reader** module at the top of the experiment diagram to select it and in the **Properties** pane, set the **Account Name** and **Account Key** properties to the storage account values obtained previously.

   ![Configuring the ML Reader](Images/ml-configure-reader.png?raw=true "Configuring the ML Reader")
   
   _Configuring the ML Reader_
   
1. Click **Run** at the bottom of the page to run the **MyDriving** experiment.

1. Once the run is complete, select the **Train Model** module in the diagram, click **Setup Web Service**, and then **Deploy Web Service**. Reply **yes** when prompted for confirmation.
   
   ![Deploying an ML Web Service](Images/ml-deploy-web-service.png?raw=true "Deploying an ML Web Service")
   
   _Deploying an ML Web Service_
   
1. Switch to the **Predictive Experiment** tab and configure the **Reader** module property by updating the **Account Name** and **Account Key** properties with the same storage account information that you used previously to configure the **Training Experiment**.
 
1. Select one of the two **Writer** modules in the diagram and in the **Properties** pane, update the **Database server name**, **Server user account name**, and **Server user account password** properties with the values obtained previously. Use the values corresponding to the database shown in the **Database name** property. For the **Server user account name** set the value as &lt;user name&gt;@&lt;server name&gt;. Use the password that you specified when you ran the deployment script.
 
  ![Configuring the ML Writer](Images/ml-configure-writer.png?raw=true "Configuring the ML Writer")
   
  _Configuring the ML Writer_
 
1. Repeat the previous step to configure the **Writer** module for the other database.
 
1. Now, click **Run**
 
1. After the run completes, click **Deploy Web Service**.

1. Go back to the [Azure classic portal](https://manage.windowsazure.com/), select the **Machine Learning** service and then the **mydriving** workspace. Now switch to the **Web Services** tab and select the **MyDriving [Predictive Exp.]** web service.

   ![Configuring ML Web Services](Images/ml-web-services.png?raw=true "Configuring ML Web Services")
   
   _Configuring ML Web Services_

1. Click **Add Endpoint**, enter **_retrain_** as the name of the new endpoint, and then click the checkmark.

   ![Adding an ML Web Service Endpoint](Images/ml-adding-endpoint.png?raw=true "Adding an ML Web Service Endpoint")
   
   _Adding an ML Web Service Endpoint_

1. Click **retrain** in the list of endpoints to shown its **Dashboard** and then copy the API key, under the **Quick Glance** section.

1. Click **BATCH EXECUTION** to open the API documentation page and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Return to the **retrain** endpoint dashboard, click **UPDATE RESOURCE** and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Keep a record of the API Key and the batch execution and update resource request URIs of the **retrain** endpoint. You'll need these values later to configure the Data Factory's **AzureMLScoringandUpdateLinkedService** linked service. 

1. Return to the **Web Services** list and select the **MyDriving** web service, then select the **default** endpoint to show its **Dashboard**, and then copy the API key, under the **Quick Glance** section.

1. Click **BATCH EXECUTION** to open the API documentation page and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Keep a record of the API Key and the batch execution request URI of the **default** endpoint. You'll need these values later to configure the Data Factory's **TrainingEndpoint-AMLLinkedService** linked service.

### Azure Data Factory configuration

1. In the [Azure portal](https://portal.azure.com), select the resource group where the solution is deployed and under **Resources**, select the Data Factory resource.

   ![Data Factory](Images/data-factory.png?raw=true "Data Factory")
   
   _Data Factory_

1. In the data factory blade, select the **Author and deploy** action.

1. In the authoring blade, expand **Linked Services** and then select **AzureMLScoringandUpdateLinkedService**.
   
   ![Configuring the Data Factory](Images/configuring-data-factory.png?raw=true "Configuring the Data Factory")
   
   _Configuring the Data Factory_
   
1. Update the linked service definition by entering the information that you obtained previously from the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service. 

  - **mlEndpoint**: request URI of the batch execution operation for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service
  - **apiKey**: API key for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service
  - **updateResourceEndpoint**: request URI of the update resource operation for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service

   ![Configuring a Linked Service](Images/configuring-linked-service.png?raw=true "Configuring a Linked Service")

    _Configuring a Linked Service_

1. Click **Deploy**.

1. Next, under **Linked Services**, select **TrainingEndpoint-AMLLinkedService** and update its definition by entering the information that you obtained previously from the **default** endpoint of the **MyDriving** web service.

  - **mlEndpoint**: request URI of the batch execution operation of the **default** endpoint of the **MyDriving** web service
  - **apiKey**: API key for the **default** endpoint of the **MyDriving** web service

1. Click **Deploy**.