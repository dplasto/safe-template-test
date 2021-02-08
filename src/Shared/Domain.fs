namespace SavingsTracker

open System

type GrowthModel =
    | EffectiveAnnualReturn of float seq
    | AnnualInterestRateCompoundingMonthly of float seq

[<RequireQualifiedAccess>]
module GrowthModel =

    let zero =
        Seq.initInfinite (fun _ -> 0.0)
        |> AnnualInterestRateCompoundingMonthly

    let fix x =
        Seq.initInfinite (fun _ -> x)

type SavingsPot =
    {
        Name    : string
        Growth  : GrowthModel
        TaxRate : float
    }

type Amount =
    | Monthly of amount : float
    | Yearly of amount : float * monthOfYear : int
    | OnceOff of amount : float * monthOfYear : int

[<RequireQualifiedAccess>]
module Amount =

    let toYearly (amount : Amount) : float =
        match amount with
        | Yearly (x,_) -> x
        | Monthly x -> x * 12.0
        | OnceOff (x,_) -> x
//
//    let toMonthly (amount : Amount) : float =
//        match amount with
//        | Yearly x -> x / 12.0
//        | Monthly x -> x
//        | OnceOff x -> x / 12.0 // TODO failwith "aaagsagr ! " // if startAge = age then x else 0.0
//
//    let mustBeMonthly (amount : Amount) : float =
//        match amount with
//        | Yearly _ -> failwith "Expecting monthly transaction."
//        | Monthly x -> x
//        | OnceOff _ -> failwith "Expecting monthly transaction. TODO" // TODO

    let inflateMonthly (year : int) (rate : float) (amount : float) : float =
        let p = amount
        let r = rate
        let n = 12.0
        let t = float year
        p * (1.0 + (r/n)) ** (t * n)

    let inflate (year : int) (inflationRate : float) (amount : float) : float =
        let p = amount
        let r = inflationRate
        let n = 1.0
        let t = float year
        p * (1.0 + (r/n)) ** (t * n)

    let calculateReturn (year : int) (growthModel : GrowthModel) (inflationRate : float) (amount : float) : float =

        match growthModel with
        | EffectiveAnnualReturn rates ->
            // TODO test this
            let rate =
                rates
                |> Seq.item year
            let r = rate - inflationRate
            (1.0 + (r/12.0))**12.0 - 1.0 // https://global.oup.com/us/companion.websites/9780190296902/sr/interactive/formulas/nominal/
        | AnnualInterestRateCompoundingMonthly rates ->
            let rate =
                rates
                |> Seq.item year
            amount * (1.0 + ((rate - inflationRate) / 12.0))



type TransactionType =
    | Income
    | Expense
    | SavingsContribution
    | SavingsWithdrawal

type Transaction =
    {
        Description : string
        TransactionType : TransactionType
        StartAge : int
        EndAge : int
        Amount : Amount
        Multiplier : float
        From : SavingsPot option
        To : SavingsPot option
        InflationRate : float
    }

type FinancialModel =
    {
        Description : string
        SavingsPots : (SavingsPot * float) list
        Transactions : Transaction list
        InflationRate : float
        StartAge : int
        EndAge : int
    }

[<RequireQualifiedAccess>]
module FinancialModel =

    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) (priority : int) =
        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| GrowthModel.fix 0.1
                TaxRate = 0.0
            }

        let model =
            {
                Description = description
                SavingsPots = [ account, 100.0 ]
                Transactions = []
                InflationRate = 0.0
                StartAge = 0
                EndAge = 0
            }

        model