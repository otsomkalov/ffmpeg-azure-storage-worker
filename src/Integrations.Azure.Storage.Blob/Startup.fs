module Integrations.Azure.Storage.Blob.Startup

#nowarn "20"

open Azure.Storage.Blobs
open Infrastructure.Settings
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open otsom.fs.Extensions.DependencyInjection
open Domain.Workflows

let addAzureStorageBlobIntegration (services:IServiceCollection) =
  services
    .BuildSingleton<BlobServiceClient, StorageSettings>(fun cfg -> BlobServiceClient(cfg.ConnectionString))

  services
    .BuildSingleton<RemoteStorage.DownloadFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.downloadFile)
    .BuildSingleton<RemoteStorage.UploadFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.uploadFile)
    .BuildSingleton<RemoteStorage.DeleteFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.deleteFile)
