# Pulumi Azure DEV Walkthrough

This folder contains a minimal Pulumi + .NET infrastructure project for learning IaC with Azure.

## Architecture

The deployed application is **co-hosted**: both the UI (Blazor WebAssembly) and API (ASP.NET Core) run in the same container.

- The API is the main application (port 8080)
- The UI is published and served as static files from the API's `wwwroot` folder
- The UI endpoint selection is explicit via `RulesClient:EndpointMode`
- When deployed, both components share the same public URL

This means:
- No CORS complications (same origin)
- Single container to manage
- Co-hosted mode uses runtime browser origin for API calls at deployment URL

Rules client endpoint modes:
- `CoHosted`: Uses browser origin at runtime (`builder.HostEnvironment.BaseAddress`). Best for same-container deployments.
- `External`: Uses configured `RulesClient:BaseAddress`. Best for local development with separate API host.

## What it provisions

- Azure Resource Group
- Azure Container App Environment
- Azure Container App
- External HTTPS ingress for the Container App

The stack exports an application URL so you can quickly verify deployment success.

## Prerequisites

1. Install .NET 8 SDK.
2. Install Pulumi CLI.
3. Install Azure CLI.
4. Sign in and select subscription:

	az login
	az account set --subscription "<your-subscription-name-or-id>"

## Local deployment flow

From this folder:

1. Initialize or select the stack:

	pulumi stack init dev

	If it already exists:

	pulumi stack select dev

2. Restore and preview:

	dotnet restore
	pulumi preview

3. Deploy:

	pulumi up

4. Check outputs:

	pulumi stack output resourceGroupName
	pulumi stack output containerAppEnvironmentName
	pulumi stack output containerAppName
	pulumi stack output containerImage
	pulumi stack output applicationUrl

## Configuration

### Container Image

The **containerImage** is the core of your deployment. It's a pre-built Docker image containing both the API and UI (as static files).

Build and push your image before deploying:

```bash
docker build -t ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc .
docker push ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc
```

Then configure Pulumi to use it:

```bash
pulumi config set thunderbirds-azure-dev:containerImage ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc
```

### All Config Keys

- azure-native:location
- thunderbirds-azure-dev:baseName
- thunderbirds-azure-dev:containerImage (default: ghcr.io/mattcarrington/thunderbirds-app:latest)
- thunderbirds-azure-dev:containerPort (default: 8080)
- thunderbirds-azure-dev:registryUsername (optional, for private registries)
- thunderbirds-azure-dev:registryPassword (optional, for private registries; must be set as a Pulumi secret)

When using registry credentials, set both registryUsername and registryPassword together, and ensure containerImage includes an explicit registry server (for example `ghcr.io/...` or `myregistry.azurecr.io/...`). The Pulumi program derives the registry server from containerImage.

### Examples

pulumi config set azure-native:location westeurope
pulumi config set thunderbirds-azure-dev:baseName tbbge2
pulumi config set thunderbirds-azure-dev:containerImage myregistry.azurecr.io/thunderbirds-api:1.0.0
pulumi config set thunderbirds-azure-dev:containerPort 8080

# For private container registries:
pulumi config set thunderbirds-azure-dev:registryUsername <username>
pulumi config set --secret thunderbirds-azure-dev:registryPassword <password>

If you are using a public image, you do not need to set registryUsername or registryPassword.

All CLI commands in this guide work in PowerShell as well as Bash.

## How the Co-Hosted Deployment Works

When the app is deployed to Azure:

1. The container starts and the API runs on port 8080
2. The browser loads the UI from the API's `wwwroot` folder
3. `RulesClient:EndpointMode` is `CoHosted`, so the UI resolves API base URL from the **browser's current origin**
   - In Azure: `https://tbbge-dev-app96f32a3e--0000002.victorioussand-830bb169.uksouth.azurecontainerapps.io`
4. The UI makes API calls to that same origin (no CORS needed)

For local development with separately hosted API:

- Set `RulesClient:EndpointMode` to `External`
- Set `RulesClient:BaseAddress` to your local API URL (for example `https://localhost:8080/`)

This endpoint policy is handled in `src/Ui/ThunderbirdsBoardGameEngine.UI/Program.cs` and makes deployment behavior explicit.

## CI/CD readiness

This is ready to check in and use in CI/CD for DEV.

Recommended approach:

1. Use Pulumi Service as backend.
2. Authenticate to Azure in GitHub Actions with OpenID Connect (OIDC) using azure/login.
3. Run Pulumi non-interactively:

	pulumi stack select dev
	pulumi preview --non-interactive
	pulumi up --yes --non-interactive

4. Store sensitive values as secrets (do not hard-code in source). Use:

	pulumi config set --secret <key> <value>

## Cleanup

Destroy all resources created by this stack:

pulumi destroy

Optionally remove local stack metadata:

pulumi stack rm dev
