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

## Program.cs

Using the isolated architecture liberates your APIs from a specific runtime. Basically, they just become like an exe.

If you open the `Program.cs` file you will see the Dependency Injection. This works much like the other .NET environmentds.

## Deployment to Azure

## EnvironmentSettings

Environment Settings will load the Configuration values by key name into an instance of this class. These values should rarely if ever change and only vary by deloyment environment. For more configurable Settings - use a data store.