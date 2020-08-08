# AzureLoginSample

A minimal sample using azure login with dotnet core so I can refer to it and test this later.
It attempts to be minimal in

* Dependencies - only using aspnet's `Microsoft.AspNetCore.Authentication.OpenIdConnect` package.
* Configuration code - as minimal as possible anyway.
* Final saved cookie size - around 1kb.

## Azure side

An app registered with [Azure](https://portal.azure.com/) is needed. This is "App Registrations" blade.

* On *API permissions* view add the scopes requested.
* On *Authentication* view check both **ID tokens** and **Access tokens** if using `idtoken` response (no client secret). 
If using `code` response can uncheck **Access tokens** (and use client secret).
* If app roles are needed see [this doc](https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps).

