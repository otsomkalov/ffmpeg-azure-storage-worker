namespace Worker

open Azure.Storage.Queues
open Infrastructure
open Infrastructure.Settings
open Integrations.Azure.Storage.Blobs
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Microsoft.FSharp.Core
open Worker.Settings
open otsom.fs.Extensions.DependencyInjection
open Domain.Workflows

#nowarn "20"

module Program =
  open Microsoft.Extensions.Hosting
  open Microsoft.Extensions.DependencyInjection

  let private configureAppConfig _ (builder: IConfigurationBuilder) =
    builder.AddUserSecrets(true)

    ()

  let private configureQueueServiceClient (options: IOptions<StorageSettings>) =
    let settings = options.Value

    QueueServiceClient(settings.ConnectionString)

  let private configureServices (ctx: HostBuilderContext) (services: IServiceCollection) =
    services
      .BuildSingleton<AppSettings, IConfiguration>(fun cfg -> cfg.Get<AppSettings>())
      .BuildSingleton<FFMpegSettings, IConfiguration>(fun cfg -> cfg.GetSection(FFMpegSettings.SectionName).Get<FFMpegSettings>())

    services
    |> Startup.addIntegrationsCore ctx.Configuration
    |> Startup.addAzureStorageIntegration

    services.AddHostedService<Worker.Worker>() |> ignore

    services
      .BuildSingleton<Converter.Convert, FFMpegSettings, ILoggerFactory>(FFMpegConverter.convert)

    services.AddApplicationInsightsTelemetryWorkerService()

    ()

  let private createHostBuilder args =
    Host
      .CreateDefaultBuilder(args)
      .ConfigureAppConfiguration(configureAppConfig)
      .ConfigureServices(configureServices)

  [<EntryPoint>]
  let main args =
    createHostBuilder(args).Build().Run()

    0 // exit code
