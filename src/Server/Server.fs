module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open SavingsTracker

type Storage () =
    let models =
        printfn "Create models"
        ResizeArray<FinancialModel>()

    member __.GetModels () =
        printfn "GetModels"
        List.ofSeq models

    member __.AddModel (model: FinancialModel) =
        models.Add model
        Ok ()

let storage = Storage()

storage.AddModel(FinancialModel.create "Create new SAFE project" 1) |> ignore
storage.AddModel(FinancialModel.create "Write your app" 2) |> ignore
storage.AddModel(FinancialModel.create "Ship it !!!" 3) |> ignore

let modelsApi =
    { GetModels = fun () -> async { return storage.GetModels() }
      AddModel =
        fun model -> async {
            match storage.AddModel model with
            | Ok () -> return model
            | Error e -> return failwith e
        } }

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue modelsApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
