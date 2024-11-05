module Integrations.Azure.Storage.Queue.Startup

#nowarn "20"

open Azure.Storage.Queues
open Infrastructure.Settings
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open otsom.fs.Extensions.DependencyInjection

let addAzureStorageQueueIntegration (services: IServiceCollection) =
  services
    .BuildSingleton<QueueServiceClient, StorageSettings>(fun cfg -> QueueServiceClient(cfg.ConnectionString))

    .BuildSingleton<Queue.GetMessage, QueueServiceClient, StorageSettings>(Queue.getMessage)
    .BuildSingleton<Queue.DeleteMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.deleteMessageFactory)
    .BuildSingleton<Queue.SendSuccessMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.sendSuccessMessageFactory)
    .BuildSingleton<Queue.SendFailureMessageFactory, QueueServiceClient, StorageSettings, ILoggerFactory>(Queue.sendFailureMessageFactory)
