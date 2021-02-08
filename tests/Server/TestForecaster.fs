module SavingsTracker.Test

open Expecto

[<RequireQualifiedAccess>]
module TestForecaster =

    let ``Simple test`` () =

        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| GrowthModel.fix 0.1
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

        Expect.floatClose Accuracy.low result 110.47


    // https://www.calculatorsoup.com/calculators/financial/investment-inflation-calculator.php

    let ``Simple compound interest`` () =

        let account =
            {
                Name = "Test"
                Growth = AnnualInterestRateCompoundingMonthly <| GrowthModel.fix 0.1
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

        // model
        // |> Forecaster.forecast
        // |> List.exactlyOne
        // |> (fun (_, _, x) -> x)
        // |> should (equalWithin 0.1) 9389.02


    // // https://www.calculatorsoup.com/calculators/financial/investment-inflation-calculator.php
    // [<Test>]
    // let ``Compound interest inflation adjusted`` () =

    //     // Seems like they calculate inflation at the end of each year...?
    //     let account =
    //         {
    //             Name = "Test"
    //             Growth = AnnualInterestRateCompoundingMonthly <| GrowthModel.fix 0.1
    //             TaxRate = 0.0
    //         }

    //     let deposit =
    //         {
    //             Description = "Deposit"
    //             TransactionType = TransactionType.SavingsContribution
    //             StartAge = 0
    //             EndAge = 99
    //             Amount = Monthly 100.0
    //             Multiplier = 0.0
    //             From = None
    //             To = Some account
    //             InflationRate = 0.0
    //         }

    //     let model =
    //         {
    //             Description = "Test"
    //             SavingsPots = [ account, 1000.0 ]
    //             Transactions = [ deposit ]
    //             InflationRate = 0.05
    //             StartAge = 0
    //             EndAge = 4
    //         }

    //     model
    //     |> Forecaster.forecast
    //     |> List.exactlyOne
    //     |> (fun (_, _, x) -> x)
    //     |> should (equalWithin 0.1) 7356.54

    // [<Test>]
    // let ``Inflation of a single value`` () =

    //     let account =
    //         {
    //             Name = "Test"
    //             Growth = GrowthModel.zero
    //             TaxRate = 0.0
    //         }

    //     let model =
    //         {
    //             Description = "Test"
    //             SavingsPots = [ account, 100.0 ]
    //             Transactions = [ ]
    //             InflationRate = 0.05
    //             StartAge = 0
    //             EndAge = 0
    //         }

    //     model
    //     |> Forecaster.forecast
    //     |> List.exactlyOne
    //     |> (fun (_, _, x) -> x)
    //     |> should (equalWithin 0.1) 95.24