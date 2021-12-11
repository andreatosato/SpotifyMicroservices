$RESOURCE_GROUP="SpotifyXmas"
$LOCATION="northeurope"
$CONTAINERAPPS_ENVIRONMENT="santaclausenv"
$LOG_ANALYTICS_WORKSPACE="spotifyxmas-logs"
$STORAGE_ACCOUNT="spotifyxmas"
$ACR="spotifyxmas.azurecr.io"
$ACR_Login="spotifyxmas"
$ACR_Password="$(az acr credential show -n $ACR --query "passwords[0].value" -o tsv)"
$SB_CONNECTIONSTRING="Endpoint=sb://spotifyxmas.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JYE+RRwsWEznzAtVpJxczpmUyj9OUq+WIaVCluxhxJM="
$AzureKeyVaultTenantId="3ee9c677-f8bd-4043-a37f-ebd59c2c9e48"
$AzureKeyVaultClientId="19a59e37-45f3-4e42-8138-98123eaaa865"
$AzureKeyVaultClientSecret="lYD7Q~ownzJLAS3hsZvxAxoxlu3hxEG.lGDTd"


az login
az account set -s "Microsoft Azure Sponsorship"
# az upgrade
# az extension add --source https://workerappscliextension.blob.core.windows.net/azure-cli-extension/containerapp-0.2.0-py2.py3-none-any.whl
# az provider register --namespace Microsoft.Web

# # az group create --name $RESOURCE_GROUP --location "$LOCATION"

# # Creo workspace
# az monitor log-analytics workspace create --resource-group $RESOURCE_GROUP --workspace-name $LOG_ANALYTICS_WORKSPACE
# $LOG_ANALYTICS_WORKSPACE_CLIENT_ID=(az monitor log-analytics workspace show --query customerId -g $RESOURCE_GROUP -n $LOG_ANALYTICS_WORKSPACE --out tsv)
# $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET=(az monitor log-analytics workspace get-shared-keys --query primarySharedKey -g $RESOURCE_GROUP -n $LOG_ANALYTICS_WORKSPACE --out tsv)

# # Creo Environment
# az containerapp env create `
#   --name $CONTAINERAPPS_ENVIRONMENT `
#   --resource-group $RESOURCE_GROUP `
#   --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID `
#   --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET `
#   --location "$LOCATION"

# Lo storage account è già stato creato a mano
$STORAGE_ACCOUNT_KEY=(az storage account keys list --resource-group $RESOURCE_GROUP --account-name $STORAGE_ACCOUNT --query '[0].value' --out tsv)

# Deploy app:
# az containerapp create `
#   --name frontend `
#   --resource-group $RESOURCE_GROUP `
#   --environment $CONTAINERAPPS_ENVIRONMENT `
#   --secrets "accountkey=$STORAGE_ACCOUNT_KEY,connectionstring=$SB_CONNECTIONSTRING,azuretenantid=$AzureKeyVaultTenantId,azureclientid=$AzureKeyVaultClientId,azureclientsecret=$AzureKeyVaultClientSecret" `
#   --registry-login-server $ACR `
#   --registry-username $ACR_Login `
#   --registry-password $ACR_Password `
#   --image $ACR/frontend:xmas `
#   --target-port 80 `
#   --ingress 'external' `
#   --min-replicas 1 `
#   --max-replicas 1 `
#   --enable-dapr `
#   --dapr-app-port 80 `
#   --dapr-app-id spotifyfrontend `
#   --dapr-components ./components.yaml `
#   --verbose

  az containerapp create `
  --name backend `
  --resource-group $RESOURCE_GROUP `
  --environment $CONTAINERAPPS_ENVIRONMENT `
  --secrets "accountkey=$STORAGE_ACCOUNT_KEY,connectionstring=$SB_CONNECTIONSTRING,azuretenantid=$AzureKeyVaultTenantId,azureclientid=$AzureKeyVaultClientId,azureclientsecret=$AzureKeyVaultClientSecret" `
  --registry-login-server $ACR `
  --registry-username $ACR_Login `
  --registry-password $ACR_Password `
  --image $ACR/backend:xmas `
  --min-replicas 1 `
  --max-replicas 1 `
  --enable-dapr `
  --dapr-app-port 80 `
  --dapr-app-id spotifybackend `
  --dapr-components ./components.yaml `
  --verbose

  # az containerapp create `
  # --name albums `
  # --resource-group $RESOURCE_GROUP `
  # --environment $CONTAINERAPPS_ENVIRONMENT `
  # --secrets "accountkey=$STORAGE_ACCOUNT_KEY,connectionstring=$SB_CONNECTIONSTRING,azuretenantid=$AzureKeyVaultTenantId,azureclientid=$AzureKeyVaultClientId,azureclientsecret=$AzureKeyVaultClientSecret" `
  # --registry-login-server $ACR `
  # --registry-username $ACR_Login `
  # --registry-password $ACR_Password `
  # --image $ACR/albums:xmas `
  # --min-replicas 1 `
  # --max-replicas 1 `
  # --enable-dapr `
  # --dapr-app-port 80 `
  # --dapr-app-id albums `
  # --dapr-components ./components.yaml `
  # --verbose

  # az containerapp create `
  # --name artists `
  # --resource-group $RESOURCE_GROUP `
  # --environment $CONTAINERAPPS_ENVIRONMENT `
  # --secrets "accountkey=$STORAGE_ACCOUNT_KEY,connectionstring=$SB_CONNECTIONSTRING,azuretenantid=$AzureKeyVaultTenantId,azureclientid=$AzureKeyVaultClientId,azureclientsecret=$AzureKeyVaultClientSecret" `
  # --registry-login-server $ACR `
  # --registry-username $ACR_Login `
  # --registry-password $ACR_Password `
  # --image $ACR/artists:xmas `
  # --min-replicas 1 `
  # --max-replicas 1 `
  # --enable-dapr `
  # --dapr-app-port 80 `
  # --dapr-app-id artists `
  # --dapr-components ./components.yaml `
  # --verbose
  
  # az containerapp create `
  # --name songs `
  # --resource-group $RESOURCE_GROUP `
  # --environment $CONTAINERAPPS_ENVIRONMENT `
  # --secrets "accountkey=$STORAGE_ACCOUNT_KEY,connectionstring=$SB_CONNECTIONSTRING,azuretenantid=$AzureKeyVaultTenantId,azureclientid=$AzureKeyVaultClientId,azureclientsecret=$AzureKeyVaultClientSecret" `
  # --registry-login-server $ACR `
  # --registry-username $ACR_Login `
  # --registry-password $ACR_Password `
  # --image $ACR/songs:xmas `
  # --min-replicas 1 `
  # --max-replicas 1 `
  # --enable-dapr `
  # --dapr-app-port 80 `
  # --dapr-app-id songs `
  # --dapr-components ./components.yaml `
  # --verbose