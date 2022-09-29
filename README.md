# GloboTicket Demo Application

This application is intended to demonstrate the basics of deploying an asp.net core application to kubernetes

## Architecture Overview

- The **frontend** microservice is a simple ASP.NET Core 6 website. It allows visitors to browse the catalog of concerts, and place an order for tickets
- The **catalog** microservice provides the list of concerts that tickets can be purchased for. To keep this demo as simple as possible, the catalog microservice returns a hard-coded in-memory list. Created with `dotnet new webapi -o catalog --no-https` (no https because we're going to rely on dapr for securing communication between microservices). A dapr cron job calls a scheduled endpoint on this.
- The **ordering** microservice takes new orders. It receives the order via pub-sub messaging. It sends an email to thank the user for purchasing. A dapr output

## Deploying to Kubernetes (AKS) on Azure
The [`aks-deploy.ps1`](aks-deploy.ps1) PowerShell script shows the steps needed to deploy this to Azure. Don't run this directly. You'll need the Azure CLI installed, and you'll also need to pick unique resource names that are available. The script includes example commands you can use to check it's all working as expected.

You need to change the name of the subscription you will use plus create a unique name for storage account and service bus. Also you need to change these names in the component configurations to match the choosen names. Otherwise the application will fail at runtime with messages it can not find the resources specified.
