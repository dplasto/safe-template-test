module Index2

open Feliz
open Feliz.Bulma
open Elmish
open SavingsTracker

type State = {
    FinancialModel : FinancialModel
}

type Msg =
    | AddAccount

let init() = { FinancialModel = FinancialModel.create "test" 0 }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | AddAccount -> state, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        Html.button [
            prop.onClick (fun _ -> dispatch AddAccount)
            prop.text "Add account"
        ]

        Html.textf [
            prop.onKeyUp (fun _ -> dispatch)
        ]

        Feliz.React.

        Html.h1 state.FinancialModel.Description

        Html.div [
            prop.className "content"
            prop.children [
                Chart.chart ()
            ]
        ]

    ]