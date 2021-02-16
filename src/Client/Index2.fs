module Index2

open Feliz
open Feliz.Bulma
open Elmish
open SavingsTracker

type State = {
    FinancialModel : FinancialModel
    NewAccountName : string
    NewAccountCash : int option
}

type Msg =
    | AddAccount
    | UpdateNewAccountName of newAccountName : string

let init() =
    {
        FinancialModel = FinancialModel.create "test" 0
        NewAccountName = ""
        NewAccountCash = None
    }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | AddAccount -> state, Cmd.none
    | UpdateNewAccountName newAccountName -> { state with NewAccountName = newAccountName }, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        Html.button [
            prop.onClick (fun _ -> dispatch AddAccount)
            prop.text "Add account"
        ]

        Bulma.input.text [
            prop.placeholder "Account name"
            prop.value state.NewAccountName
            prop.onChange (UpdateNewAccountName  >> dispatch)
        ]

        Bulma.input.number [
            prop.placeholder "Account cash"
            prop.value (Option.defaultValue 0 state.NewAccountCash)
        ]


        Html.h1 state.FinancialModel.Description

        Html.div [
            prop.className "content"
            prop.children [
                Chart.chart ()
            ]
        ]

    ]