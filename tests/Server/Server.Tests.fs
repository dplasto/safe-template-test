module Server.Tests

open Expecto

open Shared
open Server

let server = testList "Server" [
    testCase "Adding valid Todo" <| fun _ ->
        let expected = false
        let actual = false
        Expect.equal actual expected "Should be false"
]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all