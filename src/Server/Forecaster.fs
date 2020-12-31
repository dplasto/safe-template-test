namespace SavingsTracker

open System
open SavingsTracker

[<RequireQualifiedAccess>]
module Forecaster =

    let forecast (model : FinancialModel) : (SavingsPot * float * float) list =

        let savings = model.SavingsPots
        let transactions = model.Transactions
        let inflationRate = model.InflationRate

        let applyYear (year : int) (age : int) (savingsPots : (SavingsPot * float) list) : (SavingsPot * float) list =

            let transactions =
                transactions
                |> List.filter (fun transaction -> age >= transaction.StartAge && age <= transaction.EndAge)

            let savingsAtYearStart =
                savingsPots
                |> List.sumBy snd

            // Calculate totals.
            let total transactionType =
                transactions
                |> List.filter (fun transaction -> transaction.TransactionType = transactionType)
                |> List.sumBy (fun transaction ->
                    transaction.Amount
                    |> Amount.toYearly
                    |> Amount.inflate year transaction.InflationRate
                )

            let totalIncome = total Income
            let totalExpense = total Expense
            let totalSavingsContribution = total SavingsContribution
            let totalSavingsWithdrawal = total SavingsWithdrawal

            let applyTransaction (monthOfYear : int) ((pot, amount) : SavingsPot * float) (transaction : Transaction) : SavingsPot * float =

                let inflatedTransactionAmount =

                    let inflate amount =
                        amount
                        |> Amount.inflate year transaction.InflationRate // TODO only inflating transactions yearly!!

                    match transaction.Amount with
                    | Monthly amount ->
                        amount
                        |> inflate
                        |> Some
                    | Yearly  (amount, transactionMonthOfYear)
                    | OnceOff (amount, transactionMonthOfYear) ->
                        if transactionMonthOfYear = monthOfYear then
                            amount
                            |> inflate
                            |> Some
                        else
                            None

                match inflatedTransactionAmount with
                | Some x ->
                    if transaction.From = Some pot then
                        if transaction.TransactionType = SavingsWithdrawal then
                            pot, Math.Max(0.0, amount - x) // ZOMG
                        else
                            pot, amount - x
                    else if transaction.To = Some pot then
                        pot, amount + (x / (1.0 - transaction.Multiplier))
                    else
                        pot, amount
                | None ->
                    pot, amount





            let applyMonth (savingsPots : (SavingsPot * float) list) (transactions : Transaction list) (monthOfYear : int) : (SavingsPot * float) list =
                savingsPots
                |> List.map (fun (pot, amount) -> pot, amount |> Amount.calculateReturn year pot.Growth 0.0) // TODO inflation zero?? // Apply return to savings pots.
                |> List.map (fun pot ->
                    List.fold (applyTransaction monthOfYear) pot transactions
                )

            let months = seq { 0 .. 11 }

            let savingsPots = Seq.fold (fun pots month -> applyMonth pots transactions month) savingsPots months

            let surplusDeficit = totalIncome - totalExpense - totalSavingsContribution + totalSavingsWithdrawal

            let savingsAtYearEnd =
                savingsPots
                |> List.sumBy snd

            printfn "%d, %f, %f, %f, %f, %f, %f, %f" age savingsAtYearStart totalIncome totalExpense totalSavingsContribution totalSavingsWithdrawal surplusDeficit savingsAtYearEnd

            savingsPots

        let rec applyYears (year : int) (age : int) (endAge : int) (savingsPots : (SavingsPot * float) list) : (SavingsPot * float) list =

            if age > endAge then
                savingsPots
            else
                let savingsPots = applyYear year age savingsPots
                applyYears (year+1) (age+1) endAge savingsPots

        printfn "Age, Savings at year start, Total income, Total expenses, Total contributions, Net withdrawals, Surplus/Deficit, Savings at year end"

        let outcome = applyYears 0 model.StartAge model.EndAge savings

        //printfn "%A" outcome

        outcome
        |> List.map (fun (pot, amount) ->
            let inflationAdjusted =
                let fv = amount
                let i = inflationRate
                let n = model.EndAge - model.StartAge + 1 |> float
                fv * ((1.0 + i) ** -n)
            pot, amount, inflationAdjusted
        )



