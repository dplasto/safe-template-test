namespace TestForecaster

open Expecto
open SavingsTracker

[<RequireQualifiedAccess>]
module TestForecaster =

    let simpleTest = "Simple test", fun () ->

        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| 0.1
                TaxRate = 0.0
            }

        let model =
            {
                Description = "Test"
                SavingsPots = [ account, 100.0 ]
                Transactions = []
                InflationRate = 0.0
                StartAge = 0
                EndAge = 0
            }

        let result =
            model
            |> Forecaster.forecast
            |> List.exactlyOne
            |> (fun (_, _, x) -> x)

        Expect.floatClose Accuracy.low result 110.47 "Floats should match"


    // https://www.calculatorsoup.com/calculators/financial/investment-inflation-calculator.php
    let simpleCompoundInterest = "Simple compound interest", fun () ->

        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| 0.1
                TaxRate = 0.0
            }

        let deposit =
            {
                Description = "Deposit"
                TransactionType = TransactionType.SavingsContribution
                StartAge = 0
                EndAge = 99
                Amount = Monthly 100.0
                Multiplier = 0.0
                From = None
                To = Some account
                InflationRate = 0.0
            }

        let model =
            {
                Description = "Test"
                SavingsPots = [ account, 1000.0 ]
                Transactions = [ deposit ]
                InflationRate = 0.0
                StartAge = 0
                EndAge = 4
            }

        ()

        let result =
            model
            |> Forecaster.forecast
            |> List.exactlyOne
            |> (fun (_, _, x) -> x)

        Expect.floatClose Accuracy.low result 9389.02 "Floats should match"

    // https://www.calculatorsoup.com/calculators/financial/investment-inflation-calculator.php
    let compondInterestInflationAdjusted = "Compound interest inflation adjusted", fun () ->

        // Seems like they calculate inflation at the end of each year...?
        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| 0.1
                TaxRate = 0.0
            }

        let deposit =
            {
                Description = "Deposit"
                TransactionType = TransactionType.SavingsContribution
                StartAge = 0
                EndAge = 99
                Amount = Monthly 100.0
                Multiplier = 0.0
                From = None
                To = Some account
                InflationRate = 0.0
            }

        let model =
            {
                Description = "Test"
                SavingsPots = [ account, 1000.0 ]
                Transactions = [ deposit ]
                InflationRate = 0.05
                StartAge = 0
                EndAge = 4
            }

        let result =
            model
            |> Forecaster.forecast
            |> List.exactlyOne
            |> (fun (_, _, x) -> x)

        Expect.floatClose Accuracy.low result 7356.5 "Floats should match"

    let inflationOfASingleValue = "Inflation of a single value", fun () ->

        let account =
            {
                Name = "Test"
                Growth = GrowthModel.zero
                TaxRate = 0.0
            }

        let model =
            {
                Description = "Test"
                SavingsPots = [ account, 100.0 ]
                Transactions = [ ]
                InflationRate = 0.05
                StartAge = 0
                EndAge = 0
            }

        let result =
            model
            |> Forecaster.forecast
            |> List.exactlyOne
            |> (fun (_, _, x) -> x)

        Expect.floatClose Accuracy.low result 95.24 "Floats should match"

    let testCase (test : string * (unit -> unit)) : Test =
        testCase (fst test) (snd test)

    let forecasterTests = testList "Forecaster" [
        testCase simpleTest
        testCase simpleCompoundInterest
        testCase compondInterestInflationAdjusted
        testCase inflationOfASingleValue
    ]