namespace Integrations.Azure.Storage.Blobs

open System.IO
open Azure.Storage.Blobs
open Domain.Workflows
open FSharp
open Infrastructure.Settings
open Microsoft.Extensions.Logging
open otsom.fs.Extensions.Operators
open Infrastructure.Core

module RemoteStorage =
  let downloadFile
    (client: BlobServiceClient)
    (settings: StorageSettings)
    (loggerFactory: ILoggerFactory)
    : RemoteStorage.DownloadFile =
    let logger = loggerFactory.CreateLogger(nameof RemoteStorage.DownloadFile)
    let inputContainerClient = client.GetBlobContainerClient(settings.Input.Container)

    fun inputFileName ->
      task {
        let blobClient = inputContainerClient.GetBlobClient(inputFileName)

        let downloadedFileExtension = Path.GetExtension inputFileName
        let downloadedFile = File.create downloadedFileExtension

        do! blobClient.DownloadToAsync(downloadedFile.Path) &|> ignore

        Logf.logfi
          logger
          "Remote input file %s{InputFileName} downloaded to local %s{DownloadedFileName}"
          inputFileName
          downloadedFile.FullName

        return downloadedFile
      }

  let uploadFile
    (client: BlobServiceClient)
    (settings: StorageSettings)
    (loggerFactory: ILoggerFactory)
    : RemoteStorage.UploadFile =
    let logger = loggerFactory.CreateLogger(nameof RemoteStorage.UploadFile)
    let outputContainerClient = client.GetBlobContainerClient(settings.Output.Container)

    fun file ->
      task {
        let outputBlobClient = outputContainerClient.GetBlobClient(file.FullName)

        do! outputBlobClient.UploadAsync(file.Path, true) &|> ignore

        Logf.logfi logger "Converted file %s{ConvertedFileName} uploaded" file.FullName
      }

  let deleteFile
    (client: BlobServiceClient)
    (settings: StorageSettings)
    (loggerFactory: ILoggerFactory)
    : RemoteStorage.DeleteFile =
    let logger = loggerFactory.CreateLogger(nameof RemoteStorage.DeleteFile)
    let inputContainerClient = client.GetBlobContainerClient(settings.Input.Container)

    fun name ->
      task {
        let inputBlobContainer = inputContainerClient.GetBlobClient(name)

        do! inputBlobContainer.DeleteIfExistsAsync() &|> ignore

        Logf.logfi logger "Remote input file %s{RemoteInputFileName} deleted" name
      }