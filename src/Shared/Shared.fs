namespace SavingsTracker

open System

type IModelApi =
    { GetModels : unit -> Async<FinancialModel list>
      AddModel : FinancialModel -> Async<FinancialModel> }

[<RequireQualifiedAccess>]
module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName