module Index

open Elmish
open Fable.Remoting.Client
open SavingsTracker

type Model =
    { FinancialModels: FinancialModel list
      Input: string
      Priority : int option }

type Msg =
    | GotModels of FinancialModel list
    | SetInput of string
    | SetPriority of int
    | AddModel
    | AddedModel of FinancialModel

let modelsApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IModelApi>

let init(): Model * Cmd<Msg> =
    let model =
        { FinancialModels = []
          Input = ""
          Priority = None }
    let cmd = Cmd.OfAsync.perform modelsApi.GetModels () GotModels
    model, cmd

let update (msg: Msg) (appModel: Model): Model * Cmd<Msg> =
    match msg with
    | GotModels models ->
        { appModel with FinancialModels = models }, Cmd.none
    | SetInput value ->
        { appModel with Input = value }, Cmd.none
    | SetPriority value ->
        { appModel with Priority = Some value }, Cmd.none
    | AddModel ->
        let model = FinancialModel.create appModel.Input appModel.Priority.Value
        //let cmd = Cmd.OfAsync.perform modelsApi.AddModel model AddedModel
        { appModel with Input = ""; Priority = None }, Cmd.none
    | AddedModel model ->
        { appModel with FinancialModels = appModel.FinancialModels @ [ model ] }, Cmd.none

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Content.Ol.ol [ ] [
                for model in model.FinancialModels do
                    li [ ] [ str model.Description ]
            ]
        ]
        Field.div [ Field.IsGrouped ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.Input
                  Input.Placeholder "What needs to be done?"
                  Input.OnChange (fun x -> SetInput x.Value |> dispatch) ]
            ]
            Control.p [ Control.IsExpanded ] [
                Input.number [
                  Input.Value (match model.Priority with | Some x -> string x | None -> "")
                  Input.Placeholder "Priority?"
                  Input.OnChange (fun x -> SetPriority (int x.Value) |> dispatch) ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.Disabled (FinancialModel.isValid model.Input |> not)
                    Button.OnClick (fun _ -> dispatch AddModel)
                ] [
                    str "Add"
                ]
            ]
        ]
    ]

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "safe_template_test" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
