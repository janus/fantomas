module Fantomas.Tests.RemoveUnnecessaryParenthesesTests

open NUnit.Framework
open FsUnit
open Fantomas.Tests.TestHelper

[<Test>]
let ``parentheses around single identifiers in if expressions are unnecessary, 684`` () =
    formatSourceString
        false
        """
if (foo) then bar else baz
"""
        config
    |> prepend newline
    |> should
        equal
        """
if foo then bar else baz
"""

[<Test>]
let ``parentheses around single identifiers in if expressions are unnecessary, 684 (multiline)`` () =
    formatSourceString
        false
        """
if (foooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo) then
    bar else baz
"""
        config
    |> prepend newline
    |> should
        equal
        """
if foooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo then
    bar
else
    baz
"""

[<Test>]
let ``parentheses around single identifiers in elif expressions are unnecessary, 684`` () =
    formatSourceString
        false
        """
if foo then bar
elif (baz) then foobar
"""
        config
    |> prepend newline
    |> should
        equal
        """
if foo then bar
elif baz then foobar
"""

[<Test>]
let ``parentheses shouldn't be removed (1)`` () =
    formatSourceString
        false
        """
if foo then (bar)
elif baz then foobar
"""
        config
    |> prepend newline
    |> should
        equal
        """
if foo then (bar)
elif baz then foobar
"""

[<Test>]
let ``parentheses shouldn't be removed (2)`` () =
    formatSourceString
        false
        """
if foo then bar
elif baz then (foobar)
"""
        config
    |> prepend newline
    |> should
        equal
        """
if foo then bar
elif baz then (foobar)
"""

[<Test>]
let ``parentheses around single identifiers in elif expressions are unnecessary, 684 (multiline)`` () =
    formatSourceString
        false
        """
if fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo then bar
elif (bazzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz) then foobar
"""
        config
    |> prepend newline
    |> should
        equal
        """
if fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo then
    bar
elif bazzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz then
    foobar
"""
