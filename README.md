# API Example

This is a boilerplate template for setting-up an Azure Function App using .NET Core 6 isolated functions.

Modify the `Program.cs` to inject different backend Gateways and Loggers

## Getting started

You need to add a `local.settings.json` file to your project that looks like the below

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "ASPNETCORE_ENVIRONMENT": "Local",
        "connStr": "my data connection string",
        "callback": "http://localhost:50002/api/token",
        "clientID": "<My Auth0 App Client ID>",
        "clientSecret": "<My Auth0 App Client Secret>",
        "tenant": "<My Auth0 Tenant>",
        "port": "50002",
        "fileStore": "Azure Storage Connection String",
        "defaultContainer": "myBlobs"
    }
}
```

Normally, you'd want to exclude that from most repos as it might contain database connection strings or other private keys.

**NEVER PUT Secrets or Private Keys in an end user client** even hardcoded and BASE64 encoded.

This will allow you to run the function app on your local machine. But you need to make sure it is copied to the output folder, so check is that your `.csproj` is modified to include the below:

```
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <AzureFunctionsVersion>v4</AzureFunctionsVersion>
    <OutputType>Exe</OutputType>
    ...
  </PropertyGroup>
    <ItemGroup>
    <None Update="host.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="local.settings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

Yes, like I said above, the OutputType is `exe`.

You can run the functions locally and then test them with Postman. If you install Azurite, you can test uploading files to Azure Blobs.

## Example API Methods

### Ping

Your APIs should always include a Ping or Echo or Signature method, preferably set to Anonymous using GET with no required input.
This helps in troubleshooting the API and just make sure it is up and responding.
Of course, you can make the method fancier and return more details about the environment state.

This example includes a single Ping with the endpoint
```
GET
http://localhost:7071/api/v1/ping/{optional message}
```
Of course, your local port may vary depening on the settings in the project.

A common questions is can you remove or change the default base path of the APIs. Yes by adding to the host.json file:

```
"extensions": {
    "http": {
        "routePrefix": ""
    }
},
```

This will remove the `api` part of the route from all your calls.

### Get Forecast

An example of pulling data from the backend where that could be a 3rd Pary API, a physical data store, a file in a cloud drive... whatever.

```
GET
http://localhost:7071/api/v1/forecast
```

The Mock Service generates random data while the NWS live service will pull data for a hard coded location **HGX/65,97**
Of course, you can make the location a parameter itself.

### Insert File

Upload a file to Azure Blob Storage

```
POST
http://localhost:7071/api/v1/file/{fileName?}
```

The body of the post request is the raw file bytes and `filename` is the original file name or at least the file name you want to store in the metadata for when it is later downloaded. You can of course use just the orginal or generate a new one. This method is going to generate a random code for the actual blob name.

## Program.cs

Using the isolated architecture liberates your APIs from a specific runtime. Basically, they just become like an exe.

If you open the `Program.cs` file you will see the Dependency Injection. This works much like the other .NET environments.

A .NET Core Function app will run on Windows, Mac or Linux.

## Identity Management

Integration with Auth0 is demoed using a Login API call. You will need to setup your own Auth0 App.

Once a user has a token, that can be returned to the client (stored locally) and sent in the Authorization Header of each subsequent call.

## Deployment to Azure

The easiest way to deploy is just create a new Azure Function App and Publish from Visual Studio directly. If you save your project to GitHub, you can accept the boilerplate YML script created in Deployment to automatically redeploy after each checkin.

## EnvironmentSettings

Environment Settings will load the Configuration values by key name into an instance of this class. These values should rarely if ever change and only vary by deloyment environment. For more configurable Settings - use a data store.

In the Azure Function resource go to Configuration and add any environment vaiables you need. Then modify EnvironmentSettings.cs to match you settings.

## Azurite

Azurite is very useful to do local debugging. I'll add how to set it up here but the VS Code extension has all the directions.