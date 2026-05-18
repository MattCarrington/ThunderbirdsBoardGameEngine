# Pulumi Azure DEV Walkthrough

This folder contains a minimal Pulumi + .NET infrastructure project for learning IaC with Azure.

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

Current config keys:

- azure-native:location
- thunderbirds-azure-dev:baseName
- thunderbirds-azure-dev:containerImage
- thunderbirds-azure-dev:containerPort

Examples:

pulumi config set azure-native:location westeurope
pulumi config set thunderbirds-azure-dev:baseName tbbge2
pulumi config set thunderbirds-azure-dev:containerImage myregistry.azurecr.io/thunderbirds-api:1.0.0
pulumi config set thunderbirds-azure-dev:containerPort 8080

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
