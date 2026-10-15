# Managed Identity Setup Guide

This guide walks you through setting up Azure SQL managed identity authentication for the IncidentIQ backend (production only).

## Overview

The IncidentIQ backend uses:
- **Local development**: SQL Server (localdb) with Trusted_Connection
- **Production**: Azure SQL with managed identity authentication

The `AZURE_SQL_CLIENT_ID` secret will be passed as an environment variable during production deployment.

---

## Step 1: Create GitHub Secret

Navigate to: **Repository > Settings > Secrets and variables > Secrets**

### Create the following secret:

#### `AZURE_SQL_CLIENT_ID`
- **Value**: The Client ID of your production managed identity
- **How to find it**:
  1. Go to Azure Portal → Managed Identities
  2. Select your managed identity (e.g., `identity-incidentiq-backend`)
  3. Copy the **Client ID** value
  4. Add it as a GitHub secret named `AZURE_SQL_CLIENT_ID`

---

## Step 2: Verify GitHub Variables

Navigate to: **Repository > Settings > Secrets and variables > Variables**

These should already be configured. Verify they exist:

- ✅ `AZURE_CLIENT_ID` - OIDC federation client ID (for GitHub Actions authentication)
- ✅ `AZURE_TENANT_ID` - Your Azure tenant ID
- ✅ `AZURE_SUBSCRIPTION_ID` - Your Azure subscription ID
- ✅ `ACR_NAME` - Azure Container Registry name
- ✅ `ACR_RESOURCE_GROUP` - ACR resource group
- ✅ `ACR_LOGIN_SERVER` - ACR login server URL (e.g., `myregistry.azurecr.io`)

---

## Step 3: Configure Production Deployment

The workflow includes a commented-out `deploy-production` job. Choose your infrastructure and uncomment the appropriate steps:

### Option A: Azure App Service Deployment

1. Edit `.github/workflows/CD-Dev.yml` 
2. Uncomment the `deploy-production` job
3. Update these GitHub variables:
   - `APP_SERVICE_RESOURCE_GROUP` - Your resource group name
   - `APP_SERVICE_NAME` - Your App Service name

The workflow will:
- Set `AZURE_CLIENT_ID` environment variable from the `AZURE_SQL_CLIENT_ID` secret
- Deploy the container image to App Service

### Option B: AKS (Kubernetes) Deployment

1. Edit `.github/workflows/CD-Dev.yml`
2. Uncomment the `deploy-production` job
3. Uncomment the **Option B** AKS steps (instead of App Service)
4. Update these GitHub variables:
   - `AKS_RESOURCE_GROUP` - Your AKS resource group
   - `AKS_CLUSTER_NAME` - Your AKS cluster name

Ensure your AKS cluster has:
- Namespace `production` created
- Workload identity enabled (for pod-to-managed identity binding)

### Option C: Manual Deployment

If deploying without CI/CD, ensure `AZURE_CLIENT_ID` is set as an environment variable:

**Docker:**
```bash
docker run -e AZURE_CLIENT_ID="<client-id>" \
  myregistry.azurecr.io/incidentiq-backend:latest
```

**Kubernetes:**
```yaml
env:
- name: AZURE_CLIENT_ID
  valueFrom:
    secretKeyRef:
      name: azure-identity-config
      key: AZURE_CLIENT_ID
```

---

## Step 4: Azure SQL Setup (One-time)

Before deployment, ensure Azure SQL is configured:

### 1. Enable Azure AD on SQL Server
```bash
az sql server ad-admin create \
  --resource-group "<resource-group>" \
  --server-name "<server-name>" \
  --display-name "<admin-name>"
```

### 2. Create Database Login
Connect to Azure SQL as the Azure AD admin and run:

```sql
-- In master database
CREATE LOGIN [identity-incidentiq-backend-dev] FROM EXTERNAL PROVIDER;

-- In application database
CREATE USER [identity-incidentiq-backend-dev] FROM LOGIN [identity-incidentiq-backend-dev];
ALTER ROLE db_datareader ADD MEMBER [identity-incidentiq-backend-dev];
ALTER ROLE db_datawriter ADD MEMBER [identity-incidentiq-backend-dev];
```

If your identity needs to run migrations:
```sql
ALTER ROLE db_ddladmin ADD MEMBER [identity-incidentiq-backend-dev];
```

### 3. Verify Connection
Test that the managed identity can connect:
```bash
sqlcmd -S <server>.database.windows.net -d <database> \
  -G -U "<client-id>" -P "<client-secret>"
```

---

## Step 5: Test the Deployment

1. **Push to `main` branch** to trigger the workflow
2. **Monitor the GitHub Actions** run:
   - Backend build should succeed
   - Frontend build should succeed
   - Deployment job (if enabled) should deploy with managed identity
3. **Check application logs** for successful database connections
4. **Verify API endpoints** return data from the database

---

## Troubleshooting

### `AZURE_CLIENT_ID environment variable is required`
**Problem**: Application fails to start because `AZURE_CLIENT_ID` is not set.

**Solution**:
1. Verify the secret `AZURE_CLIENT_ID_DEV` is created in GitHub
2. Verify deployment is passing the environment variable correctly
3. Check Azure App Service Configuration or Kubernetes secret

### Database authentication failure
**Problem**: Connection fails with "Login failed for user"

**Solution**:
1. Verify managed identity Client ID matches what's configured
2. Verify database user was created: `SELECT * FROM sys.sysusers` (should include the managed identity)
3. Verify roles are assigned: `SELECT * FROM sys.user_permissions`
4. Verify connection string uses `.database.windows.net` (not local server)

### Managed identity not found in database
**Problem**: `CREATE LOGIN [identity-name] FROM EXTERNAL PROVIDER` fails

**Solution**:
1. Ensure the identity exists in Azure
2. Ensure Azure SQL Server has Azure AD admin configured
3. Connect using Azure AD admin credentials to run the CREATE LOGIN command

---

## Local Development vs Production

**Local Development Environment:**
- Uses local SQL Server (localdb)
- `appsettings.Development.json` has `UseManagedIdentity: false`
- No `AZURE_CLIENT_ID` environment variable needed

**Production Environment:**
- Uses Azure SQL with managed identity
- `appsettings.json` has `UseManagedIdentity: true`
- Requires `AZURE_CLIENT_ID` environment variable (set from `AZURE_SQL_CLIENT_ID` secret)

The same Docker image works in both environments because configuration is environment-specific.

---

## Next Steps

1. ✅ Create managed identity in Azure (if not done)
2. ✅ Configure Azure SQL to accept managed identity
3. ⏳ Create GitHub secret `AZURE_SQL_CLIENT_ID` with the managed identity's Client ID
4. ⏳ Uncomment and configure `deploy-production` job in `.github/workflows/CD-Dev.yml`
5. ⏳ Update GitHub variables for your deployment infrastructure
6. ⏳ Push to `main` and verify deployment succeeds
7. ⏳ Test API endpoints to confirm database connectivity

