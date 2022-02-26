open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

[<AllowNullLiteral>]
type Todo() =
    member val Id = 0 with get, set
    member val Name = "" with get, set
    member val IsComplete = false with get, set
    member val Secret = "" with get, set

type TodoDto() =
    static member fromTodo (todo: Todo) =
        TodoDto(Id = todo.Id, Name = todo.Name, IsComplete = todo.IsComplete)

    member val Id = 0 with get, set
    member val Name = "" with get, set
    member val IsComplete = false with get, set


type TodoDb(options: DbContextOptions<TodoDb>) =
    inherit DbContext(options)

    member _.Todos = base.Set<Todo>()

let args = Environment.GetCommandLineArgs().[1..]
let builder = WebApplication.CreateBuilder(args)
builder.Services.AddDatabaseDeveloperPageExceptionFilter() |> ignore
builder.Services.AddDbContext<TodoDb>(Action<DbContextOptionsBuilder>(fun opt ->
    opt.UseInMemoryDatabase("TodoList") |> ignore)) |> ignore

let app = builder.Build()

app.MapGet("/todos", Func<TodoDb, TodoDto list>(fun db ->
    db.Todos |> Seq.map TodoDto.fromTodo |> Seq.toList)) |> ignore

app.MapGet("/todos/{id}", Func<int, TodoDb, Task<IResult>>(fun id db ->
task {
    match! db.Todos.FindAsync(id) with
    | null -> return Results.NotFound()
    | todo -> return Results.Ok(TodoDto.fromTodo todo)
})) |> ignore

app.MapPost("/todos", Func<TodoDto, TodoDb, Task<IResult>>(fun todoDto db ->
task {
    let todo = Todo(IsComplete = todoDto.IsComplete, Name = todoDto.Name)

    db.Todos.Add(todo) |> ignore
    let! _ = db.SaveChangesAsync()

    return Results.Created($"/todos/{todo.Id}", TodoDto.fromTodo todo)
})) |> ignore

app.MapPut("/todos/{id}", Func<int, TodoDto, TodoDb, Task<IResult>>(fun id todoDto db ->
task {
    match! db.Todos.FindAsync(id) with
    | null -> return Results.NotFound()
    | todo ->
        todo.Name <- todoDto.Name
        todo.IsComplete <- todoDto.IsComplete
        let! _ = db.SaveChangesAsync()
        return Results.NoContent()
})) |> ignore

app.MapDelete("/todos/{id}", Func<int, TodoDb, Task<IResult>>(fun id db ->
task {
    match! db.Todos.FindAsync(id) with
    | null -> return Results.NotFound()
    | todo ->
        db.Todos.Remove(todo) |> ignore
        let! _ = db.SaveChangesAsync()
        return Results.Ok(todo)
})) |> ignore

app.Run()