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
        "connStr": "my data connection string"
    }
}
```

Normally, you'd want to exclude that from most repos as it might contain database connection strings or other private keys.

This will allow you to run the function app on your local machine.

## Ping

Your APIs should always include a Ping or Echo or Signature method, preferably set to Anonymous using GET with no required input.
This helps in troubleshooting the API and just make sure it is up and responding.
Of course, you can make the method fancier and return more details about the environment state.

This example includes a single Ping with the endpoint
```
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

## Program.cs

Using the isolated architecture liberates your APIs from a specific runtime. Basically, they just become like an exe.

If you open the `Program.cs` file you will see the Dependency Injection. This works much like the other .NET environments.

A .NET Core Function app will run on Windows, Mac or Linux.

## Deployment to Azure

The easiest way to deploy is just create a new Azure Function App and Publish from Visual Studio directly. If you save your project to GitHub, you can accept the boilerplate YML script created in Deployment to automatically redeploy after each checkin.

## EnvironmentSettings

Environment Settings will load the Configuration values by key name into an instance of this class. These values should rarely if ever change and only vary by deloyment environment. For more configurable Settings - use a data store.

In the Azure Function resource go to Configuration and add any environment vaiables you need. Then modify EnvironmentSettings.cs to match you settings.