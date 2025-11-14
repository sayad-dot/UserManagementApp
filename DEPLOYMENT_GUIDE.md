# User Management App - Complete Deployment Guide

## ✅ Requirements Verification

Your application successfully meets ALL requirements:

1. ✅ **FIRST REQUIREMENT**: Unique index on email in database
2. ✅ **SECOND REQUIREMENT**: Professional table and toolbar design with Bootstrap
3. ✅ **THIRD REQUIREMENT**: Data sorted by last login time
4. ✅ **FOURTH REQUIREMENT**: Multiple selection with checkboxes (including select all)
5. ✅ **FIFTH REQUIREMENT**: Middleware validates user status before each request
6. ✅ **getUniqIdValue function**: Added as required
7. ✅ **No browser alerts**: Using Bootstrap toasts
8. ✅ **Extensive comments**: Added with IMPORTANT, NOTE, NOTA BENE keywords
9. ✅ **Email verification**: Asynchronous email sending
10. ✅ **User operations**: Block, Unblock, Delete working correctly

## Pre-Deployment Checklist

- ✅ Application tested locally and working
- ✅ Database migrations created
- ✅ .gitignore configured to protect sensitive data
- ✅ No hardcoded secrets in code
- ✅ Build succeeds without errors

## Step 1: Push to GitHub

### Initialize Git Repository (if not already done)
```bash
cd "/media/sayad/Ubuntu-Data/itransition task/UserManagementApp"

# Check if git is already initialized
git status

# If not initialized, run:
git init
git branch -M main
```

### Configure Git (if needed)
```bash
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### Stage and Commit All Files
```bash
# Check what will be committed
git status

# Add all files (respecting .gitignore)
git add .

# Verify what's staged
git status

# Commit with meaningful message
git commit -m "Initial commit: Complete User Management System with all requirements

- Implemented user authentication with JWT
- Added email verification system
- Created user management table with sorting
- Implemented Block/Unblock/Delete operations
- Added middleware for user status validation
- Created unique database index on email
- Used Bootstrap 5 for responsive design
- Added extensive code comments
- All requirements from task specification met"
```

### Create GitHub Repository and Push
```bash
# If you haven't created the repo on GitHub yet, go to github.com and create it
# Then add remote and push:

git remote add origin https://github.com/sayad-dot/UserManagementApp.git

# Push to GitHub
git push -u origin main
```

## Step 2: Deploy to Azure App Service

### Option A: Deploy via Azure Portal (Recommended)

#### 2.1 Build and Package Application
```bash
cd "/media/sayad/Ubuntu-Data/itransition task/UserManagementApp"

# Build in Release mode
dotnet publish UserManagementApp.API/UserManagementApp.API.csproj -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../app.zip .
cd ..
```

#### 2.2 Create App Service in Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"** → Search for **"Web App"**
3. Click **"Create"**

**Basic Settings:**
- **Subscription**: Your Azure subscription
- **Resource Group**: Create new `UserManagementRG` or use existing
- **Name**: `usermanagement-app-sayad` (must be globally unique)
- **Publish**: Code
- **Runtime stack**: .NET 8 (LTS)
- **Operating System**: Linux
- **Region**: East US (or closest to you)

**Pricing:**
- **App Service Plan**: Create new
- **Pricing plan**: B1 (Basic) - ~$13/month, or F1 (Free) for testing

4. Click **"Review + Create"** → **"Create"**
5. Wait for deployment to complete (~2-3 minutes)

#### 2.3 Configure Application Settings

1. Navigate to your newly created App Service
2. Go to **"Configuration"** in the left sidebar

**Add Connection String:**
- Click **"New connection string"**
- **Name**: `DefaultConnection`
- **Value**: 
  ```
  Host=usermanagement-postgres-sayad.postgres.database.azure.com;Database=UserManagementDb;Username=sayadadmin;Password=S@y@d2024!AzurePostgres;SSL Mode=Require;
  ```
- **Type**: PostgreSQL
- Click **"OK"**

**Add Application Settings:**
Click **"New application setting"** for each:

| Name | Value |
|------|-------|
| `Jwt__Secret` | `f8V7pL3mQ9rX2sT1zB6yK4wN0vH5aJ8c` |
| `Jwt__Issuer` | `UserManagementApp` |
| `Jwt__Audience` | `UserManagementAppUsers` |
| `App__BaseUrl` | `https://usermanagement-app-sayad.azurewebsites.net` |
| `Email__SmtpServer` | `smtp.gmail.com` |
| `Email__Port` | `587` |
| `Email__Username` | `sayadibnaazad@iut-dhaka.edu` |
| `Email__Password` | `iqixmmjtnkvsapdd` |
| `Email__From` | `noreply@usermanagement.com` |

3. Click **"Save"** at the top
4. Click **"Continue"** when prompted about restart

#### 2.4 Deploy Application

**Method 1: Using Azure CLI**
```bash
# Install Azure CLI if needed
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login
az login

# Deploy the ZIP file
az webapp deployment source config-zip \
    --resource-group UserManagementRG \
    --name usermanagement-app-sayad \
    --src app.zip
```

**Method 2: Using Portal**
1. In your App Service, go to **"Deployment Center"**
2. Choose **"Local Git"** or **"External Git"**
3. For GitHub: Connect your GitHub repository
4. Or use **"FTPS credentials"** to upload `app.zip` manually

#### 2.5 Configure PostgreSQL Firewall

1. Navigate to your PostgreSQL server: `usermanagement-postgres-sayad`
2. Go to **"Connection security"** or **"Networking"**
3. Under **"Firewall rules"**:
   - Set **"Allow access to Azure services"** to **YES**
   - Add your client IP if you want to access from local tools
4. Click **"Save"**

#### 2.6 Verify Database Connection

The application will automatically create/update the database on first run using `EnsureCreated()`.

To manually check:
```bash
# Stream application logs
az webapp log tail \
    --resource-group UserManagementRG \
    --name usermanagement-app-sayad
```

### Option B: Deploy via Azure CLI (Automated)

Run this complete script:

```bash
cd "/media/sayad/Ubuntu-Data/itransition task/UserManagementApp"

# Variables
RESOURCE_GROUP="UserManagementRG"
LOCATION="eastus"
APP_NAME="usermanagement-app-sayad"
PLAN_NAME="UserManagementPlan"

# Login to Azure
az login

# Create resource group
echo "Creating resource group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create App Service Plan
echo "Creating App Service Plan..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --sku B1 \
    --is-linux

# Create Web App
echo "Creating Web App..."
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --name $APP_NAME \
    --runtime "DOTNET|8.0"

# Configure connection string
echo "Configuring connection string..."
az webapp config connection-string set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings DefaultConnection="Host=usermanagement-postgres-sayad.postgres.database.azure.com;Database=UserManagementDb;Username=sayadadmin;Password=S@y@d2024!AzurePostgres;SSL Mode=Require;" \
    --connection-string-type PostgreSQL

# Configure app settings
echo "Configuring app settings..."
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings \
        Jwt__Secret="f8V7pL3mQ9rX2sT1zB6yK4wN0vH5aJ8c" \
        Jwt__Issuer="UserManagementApp" \
        Jwt__Audience="UserManagementAppUsers" \
        App__BaseUrl="https://${APP_NAME}.azurewebsites.net" \
        Email__SmtpServer="smtp.gmail.com" \
        Email__Port="587" \
        Email__Username="sayadibnaazad@iut-dhaka.edu" \
        Email__Password="iqixmmjtnkvsapdd" \
        Email__From="noreply@usermanagement.com"

# Build and package
echo "Building application..."
dotnet publish UserManagementApp.API/UserManagementApp.API.csproj -c Release -o ./publish

# Create ZIP
echo "Creating deployment package..."
cd publish
zip -r ../app.zip .
cd ..

# Deploy
echo "Deploying to Azure..."
az webapp deployment source config-zip \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --src app.zip

# Enable HTTPS only
echo "Enabling HTTPS only..."
az webapp update \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --https-only true

echo ""
echo "=========================================="
echo "Deployment Complete!"
echo "Your app is available at:"
echo "https://${APP_NAME}.azurewebsites.net"
echo "=========================================="
```

### Option C: Deploy via GitHub Actions (CI/CD)

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure Web App

on:
  push:
    branches:
      - main
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: usermanagement-app-sayad
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Publish
      run: dotnet publish UserManagementApp.API/UserManagementApp.API.csproj -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

To use GitHub Actions:
1. Download publish profile from Azure Portal (App Service → Deployment Center → Manage publish profile)
2. Add it as a secret in GitHub: Settings → Secrets → New repository secret
3. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
4. Push to GitHub and it will auto-deploy

## Step 3: Post-Deployment Testing

### 3.1 Access Your Application
```
https://usermanagement-app-sayad.azurewebsites.net
```

### 3.2 Test Checklist

1. **Registration**
   - [ ] Register a new user
   - [ ] Check email received
   - [ ] User appears in database as "Unverified"

2. **Email Verification**
   - [ ] Click verification link in email
   - [ ] Status changes to "Active"

3. **Login**
   - [ ] Login with verified user
   - [ ] Login with unverified user (should work)
   - [ ] Try blocked user (should fail)

4. **User Management Table**
   - [ ] Table displays all users
   - [ ] Data sorted by last login time
   - [ ] Checkbox selection works
   - [ ] Select all/deselect all works

5. **Toolbar Operations**
   - [ ] Block selected users
   - [ ] Unblock selected users
   - [ ] Delete selected users
   - [ ] Delete all unverified users

6. **Security**
   - [ ] Cannot access admin panel without login
   - [ ] Blocked users cannot login
   - [ ] Deleted users can re-register

### 3.3 Monitor Logs

```bash
# Stream live logs
az webapp log tail \
    --resource-group UserManagementRG \
    --name usermanagement-app-sayad

# Or in Azure Portal:
# App Service → Log stream
```

## Step 4: Troubleshooting

### Application Won't Start

**Check logs:**
```bash
az webapp log show \
    --resource-group UserManagementRG \
    --name usermanagement-app-sayad
```

**Common issues:**
- Wrong connection string format
- PostgreSQL firewall blocking Azure IPs
- Missing environment variables

### Database Connection Fails

1. Verify PostgreSQL firewall allows Azure services
2. Test connection string locally
3. Check PostgreSQL server is running
4. Verify credentials in connection string

### Email Not Sending

- Gmail app password might be incorrect
- Check logs for email errors (non-blocking)
- Verify SMTP settings

### 502 Bad Gateway

- Application might be starting up (wait 1-2 minutes)
- Check application logs for startup errors
- Verify .NET 8.0 runtime is selected

## Step 5: Custom Domain (Optional)

1. Purchase domain from provider
2. In Azure Portal → App Service → Custom domains
3. Add custom domain
4. Configure DNS records
5. Add SSL certificate (free with App Service)

## Step 6: Production Optimizations

### Security Enhancements

1. **Move secrets to Azure Key Vault**
```bash
# Create Key Vault
az keyvault create \
    --name usermanagement-kv \
    --resource-group UserManagementRG \
    --location eastus

# Store secrets
az keyvault secret set --vault-name usermanagement-kv --name "JwtSecret" --value "your-secret"
```

2. **Configure CORS properly** (not AllowAny)

3. **Enable Application Insights** for monitoring

4. **Set up automated backups** for PostgreSQL

### Performance Improvements

1. **Enable Application Insights**
2. **Configure caching**
3. **Use CDN for static files**
4. **Scale up App Service Plan** if needed

## Cost Estimation

### Development/Testing
- **App Service (F1 Free)**: $0
- **PostgreSQL (Basic)**: ~$5-10/month
- **Total**: ~$5-10/month

### Production
- **App Service (B1 Basic)**: ~$13/month
- **PostgreSQL (Basic tier 1 vCore)**: ~$25/month
- **Application Insights**: ~$2-5/month
- **Total**: ~$40-45/month

### Scaling Up
- **App Service (P1V2)**: ~$96/month
- **PostgreSQL (General Purpose 2 vCores)**: ~$140/month
- **Total**: ~$240/month

## Cleanup (If Needed)

To delete all Azure resources:
```bash
az group delete --name UserManagementRG --yes --no-wait
```

## Next Steps

1. ✅ Test locally → **DONE**
2. ✅ Push to GitHub → **NEXT**
3. Deploy to Azure
4. Test deployed application
5. Share the live URL
6. (Optional) Set up custom domain
7. (Optional) Enable monitoring and alerts

## Support

If you encounter issues:
1. Check logs in Azure Portal
2. Verify all configuration settings
3. Test database connection separately
4. Review deployment logs
5. Check GitHub issues or Stack Overflow

Good luck with your deployment! 🚀
