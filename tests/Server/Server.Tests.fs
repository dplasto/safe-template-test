module Server.Tests

open Expecto

open Shared
open Server

let server = testList "Server" [
    testCase "Simple test" TestForecaster.TestForecaster.simpleTest
]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let main _ =
    runTests defaultConfig all
