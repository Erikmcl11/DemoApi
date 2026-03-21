#!/bin/bash

# Variables
RESOURCE_GROUP="rg-demo"
LOCATION="brazilsouth"
CONTAINERAPPS_ENV="demo-environment"
APP_NAME="demoapi"
DOCKER_IMAGE="erikmcl/demoapi:latest"

# El resource group ya existe con el SQL Server y la DB
echo "Usando resource group existente: $RESOURCE_GROUP"

# Crea el entorno de Container Apps
az containerapp env create \
  --name $CONTAINERAPPS_ENV \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# Construye la imagen Docker (ejecutar desde la raiz del proyecto)
cd ../src/DemoApi
docker build -t $DOCKER_IMAGE .

# Sube la imagen a Docker Hub
docker push $DOCKER_IMAGE

# Despliega en Azure Container Apps
az containerapp up \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment $CONTAINERAPPS_ENV \
  --image $DOCKER_IMAGE
