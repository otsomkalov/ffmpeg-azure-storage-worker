﻿module Infrastructure.Startup

#nowarn "20"

open Azure.Storage.Queues
open Infrastructure.Settings
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Extensions.DependencyInjection

let addIntegrationsCore (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<StorageSettings>(cfg.GetSection(StorageSettings.SectionName))

  services.BuildSingleton<StorageSettings, IOptions<StorageSettings>>(_.Value)

  services
    .BuildSingleton<QueueServiceClient, StorageSettings>(fun cfg -> QueueServiceClient(cfg.ConnectionString))

  services
    .BuildSingleton<RemoteStorage.DownloadFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.downloadFile)
    .BuildSingleton<RemoteStorage.UploadFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.uploadFile)
    .BuildSingleton<RemoteStorage.DeleteFile, BlobServiceClient, StorageSettings, ILoggerFactory>(RemoteStorage.deleteFile)

    .BuildSingleton<Queue.GetMessage, QueueServiceClient, StorageSettings>(Queue.getMessage)
    .BuildSingleton<Queue.DeleteMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.deleteMessageFactory)
    .BuildSingleton<Queue.SendSuccessMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.sendSuccessMessageFactory)
    .BuildSingleton<Queue.SendFailureMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.sendFailureMessageFactory)