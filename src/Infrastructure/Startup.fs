module Infrastructure.Startup

#nowarn "20"

open Infrastructure.Settings
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Extensions.DependencyInjection

let addIntegrationsCore (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<StorageSettings>(cfg.GetSection(StorageSettings.SectionName))

  services.BuildSingleton<StorageSettings, IOptions<StorageSettings>>(_.Value)

