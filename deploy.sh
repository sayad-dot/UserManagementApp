#!/bin/bash

# Azure Deployment Script for User Management App
# This script automates the deployment process to Azure

echo "=========================================="
echo "User Management App - Azure Deployment"
echo "=========================================="

# Configuration
RESOURCE_GROUP="UserManagementRG"
LOCATION="eastus"
APP_NAME="usermanagement-app-sayad"
PLAN_NAME="UserManagementPlan"
POSTGRES_SERVER="usermanagement-postgres-sayad"
DB_NAME="UserManagementDb"
DB_ADMIN="sayadadmin"

echo ""
echo "Step 1: Logging into Azure..."
az login

echo ""
echo "Step 2: Creating Resource Group (if not exists)..."
az group create --name $RESOURCE_GROUP --location $LOCATION

echo ""
echo "Step 3: Creating App Service Plan (if not exists)..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --sku B1 \
    --is-linux

echo ""
echo "Step 4: Creating Web App (if not exists)..."
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --name $APP_NAME \
    --runtime "DOTNET|8.0"

echo ""
echo "Step 5: Configuring App Settings..."
# Note: Update these with your actual values
read -sp "Enter PostgreSQL Password: " DB_PASSWORD
echo ""

az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings \
        ConnectionStrings__DefaultConnection="Host=${POSTGRES_SERVER}.postgres.database.azure.com;Database=${DB_NAME};Username=${DB_ADMIN};Password=${DB_PASSWORD};SSL Mode=Require;" \
        Jwt__Secret="f8V7pL3mQ9rX2sT1zB6yK4wN0vH5aJ8c" \
        Jwt__Issuer="UserManagementApp" \
        Jwt__Audience="UserManagementAppUsers"

echo ""
echo "Step 6: Deploying Application..."
dotnet publish UserManagementApp.API/UserManagementApp.API.csproj -c Release -o ./publish

cd publish
zip -r ../app.zip .
cd ..

az webapp deployment source config-zip \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --src app.zip

echo ""
echo "=========================================="
echo "Deployment Complete!"
echo "Your app should be available at:"
echo "https://${APP_NAME}.azurewebsites.net"
echo "=========================================="
