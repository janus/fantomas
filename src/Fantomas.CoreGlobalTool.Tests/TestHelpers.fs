module Fantomas.CoreGlobalTool.Tests.TestHelpers

open System
open System.Diagnostics
open System.IO
open System.Text
open Fantomas.Extras

type TemporaryFileCodeSample
    internal
    (
        codeSnippet: string,
        ?hasByteOrderMark: bool,
        ?fileName: string,
        ?subFolder: string
    ) =
    let hasByteOrderMark = defaultArg hasByteOrderMark false

    let filename =
        let name =
            match fileName with
            | Some fn -> fn
            | None -> Guid.NewGuid().ToString()

        match subFolder with
        | Some sf ->
            let tempFolder = Path.Join(Path.GetTempPath(), sf)

            if not (Directory.Exists(tempFolder)) then
                Directory.CreateDirectory(tempFolder) |> ignore

            Path.Join(tempFolder, sprintf "%s.fs" name)
        | None -> Path.Join(Path.GetTempPath(), sprintf "%s.fs" name)

    do
        (if hasByteOrderMark then
             File.WriteAllText(filename, codeSnippet, Encoding.UTF8)
         else
             File.WriteAllText(filename, codeSnippet))

    member _.Filename: string = filename

    interface IDisposable with
        member this.Dispose() : unit =
            File.Delete(filename)

            subFolder
            |> Option.iter
                (fun sf ->
                    Path.Join(Path.GetTempPath(), sf)
                    |> Directory.Delete)

type OutputFile internal () =
    let filename =
        Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString() + ".fs")

    member _.Filename: string = filename

    interface IDisposable with
        member this.Dispose() : unit =
            if File.Exists(filename) then
                File.Delete(filename)

type ConfigurationFile internal (content: string) =
    let filename =
        Path.Join(Path.GetTempPath(), ".editorconfig")

    do File.WriteAllText(filename, content)
    member _.Filename: string = filename

    interface IDisposable with
        member this.Dispose() : unit =
            if File.Exists(filename) then
                File.Delete(filename)

type FantomasIgnoreFile internal (content: string) =
    let filename =
        Path.Join(Path.GetTempPath(), IgnoreFile.IgnoreFileName)

    do File.WriteAllText(filename, content)
    member _.Filename: string = filename

    interface IDisposable with
        member this.Dispose() : unit =
            if File.Exists(filename) then
                File.Delete(filename)

type FantomasToolResult =
    { ExitCode: int
      Output: string
      Error: string }

let runFantomasTool arguments : FantomasToolResult =
    let pwd =
        Path.GetDirectoryName(typeof<TemporaryFileCodeSample>.Assembly.Location)

    let configuration =
#if DEBUG
        "Debug"
#else
        "Release"
#endif

    let fantomasDll =
        Path.Combine(
            pwd,
            "..",
            "..",
            "..",
            "..",
            "Fantomas.CoreGlobalTool",
            "bin",
            configuration,
            "netcoreapp3.1",
            "fantomless-tool.dll"
        )

    use p = new Process()
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.FileName <- @"dotnet"
    p.StartInfo.Arguments <- sprintf "%s %s" fantomasDll arguments
    p.StartInfo.WorkingDirectory <- Path.GetTempPath()
    p.StartInfo.RedirectStandardOutput <- true
    p.StartInfo.RedirectStandardError <- true
    p.Start() |> ignore
    let output = p.StandardOutput.ReadToEnd()
    let error = p.StandardError.ReadToEnd()
    p.WaitForExit()

    { ExitCode = p.ExitCode
      Output = output
      Error = error }

let checkCode (files: string list) : FantomasToolResult =
    let files =
        files
        |> List.map (fun file -> sprintf "\"%s\"" file)
        |> String.concat " "

    let arguments = sprintf "--check %s" files
    runFantomasTool arguments

let formatCode (files: string list) : FantomasToolResult =
    let arguments =
        files
        |> List.map (fun file -> sprintf "\"%s\"" file)
        |> String.concat " "

    runFantomasTool arguments
