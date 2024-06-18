open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Thoth.Json.Giraffe
open Thoth.Json.Net
open Rommulbad
open Rommulbad.Store

let configureApp (app: IApplicationBuilder) =
    app.UseGiraffe HttpHandlers.httpHandlers

let configureServices (services: IServiceCollection) =
    services
        .AddGiraffe()
        .AddSingleton<Store>(Store())
        .AddSingleton<Json.ISerializer>(ThothSerializer(skipNullField = false, caseStrategy = CaseStrategy.CamelCase))
    |> ignore

[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder.Configure(configureApp).ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    0
