module Server.Tests

open Expecto

open Shared
open Server

let all =
    testList "All"
        [
            Shared.Tests.shared
            TestForecaster.TestForecaster.forecasterTests
        ]

[<EntryPoint>]
let main _ =
    runTests defaultConfig all
