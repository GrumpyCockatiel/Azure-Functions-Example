# API Example

This is a boilerplate template for setting-up an Azure Function App using .NET Core 6 isolated functions.

Modify the `Program.cs` to inject different backend Gateways and Loggers

## WARNING

This is setup by default to log to Azure Tables. You will need either an Azure Storage Account or Azurite running locally.
Otherwise, in `Program.cs` just disable the logging by setting it all to NullLogger.

Also, when using Kestrel server locally the default max request size is about 100 MB for uploads. This would be larger in Azure itself.

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
GET http://localhost:7071/api/v1/ping&msg={optional message}
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

### Log

This POST will pass through to the ILogger inerface and log a message directly

```
POST http://localhost:7071/api/v1/log
{
  "message": "Cry me a river as big as the {river}",
	"level": "Error",
  "args":
  {
    "river": "Mississippi"
  }
}
```
the `category` body property is optional and if included will override the default.

### Get Forecast

An example of pulling data from the backend where that could be a 3rd Pary API, a physical data store, a file in a cloud drive... whatever.

```
GET http://localhost:7071/api/v1/forecast
```

The Mock Service generates random data while the NWS live service will pull data for a hard coded location **HGX/65,97**
Of course, you can make the location a parameter itself but then just call the NWS API directly. You could however, create a mappaing between codes and certain locations and pass that as a parameter.

### Insert File

Upload a file to Azure Blob Storage

```
POST
http://localhost:7071/api/v1/file/{fileName?}
```

The body of the post request is the raw file bytes and `filename` is the original file name or at least the file name you want to store in the metadata for when it is later downloaded. You can of course use just the orginal or generate a new one. This method is going to generate a random code for the actual blob name.

### Login

If you setup your own Auth0 Tenant and supply the four Auth0 params in settings, calling Login will just return a login URL pre-rolled and sent back in the response as a plain text string. The client can then follow that string to login. You can modify the login to accept several state parameters that can be passed along the entire login lifecycle. For example, on a mobile app you may include the App Installation ID.

For now there are two URL params

```
GET http://localhost:7071/api/login/{id?}?m=<polling|redirect>&p=<port#>
```

M is the method to use in the login response. Redirect is the preferable way to do this but requires the client machine to be able to make HTTP request to the local host. Using redirect, then P is the port number the client will go look for the login tokin. If you use polling, you will need to store the token someplace in the backend and the client will then need to go fetch it on its own - hence polling, though a web socket would work here as well.

The ID is optional and usually some client/app instance ID. It is added to the OAuth State param and carried though the login lifecycle.

The returned token would sent back in subsequent headers:

```
Authorization: Bearer <myAuth0token>
```

```
{
    "resultType":"Success",
    "result":
    {
        "token":"ey...",
        "userID":"joebob@google.com",
        "refresh":"_wFXUOymzCuC8PTwUPUe00ZWqsrxmgdt4DtqwYanV5fl6",
        "expires":"2023-01-31T23:20:57.737936+00:00"
    },
    "isSuccess":true
}
```

### Token

The token method is the callback location of Auth0 and not meant to be used by an end client

### Logout

Again, just a pre-rolled passed through to Auth0 to do an explicit logout

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

Azurite is very useful for local debugging. I'll add how to set it up here but the VS Code extension has all the directions.

Figuring out the connection string for local Azurite can be found [near the bottom of the MSDN](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio).