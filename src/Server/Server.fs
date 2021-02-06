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
        printfn "GetModels start"
        let result = List.ofSeq models
        printfn "GetModels end"
        result

    member __.AddModel (model: FinancialModel) =
        printfn "AddModel start"
        models.Add model
        printfn "AddModel end"
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
            | Error e -> return model
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
