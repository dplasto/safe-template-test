namespace Shared

open System

type Todo =
    { Id : Guid
      Description : string
      Priority : int }

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) (priority : int) =
        { Id = Guid.NewGuid()
          Description = description
          Priority = priority }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos : unit -> Async<Todo list>
      addTodo : Todo -> Async<Todo> }