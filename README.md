[![build and test](https://github.com/Matl3us/LuaSharp/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/Matl3us/LuaSharp/actions/workflows/build-and-test.yml)

# Lua Language Interpreter

> Lua interpreter built in .NET for parsing and analyzing Lua code. This project provides an interactive REPL (Read-Eval-Print Loop) environment that allows users to interpret Lua files or analyze individual expressions, performing lexical analysis, tokenization, and partial Abstract Syntax Tree (AST) parsing.

## Features

- **REPL Environment:** Analyze individual expressions interactively or load and evaluate full Lua files.
- **Lexical Analysis:** Transforms Lua source code into tokens, breaking down the code for further analysis.
- **Tokenization:** Converts code into a sequence of tokens representing Lua syntax elements.
- **Abstract Syntax Tree (AST) Generation:** Parses tokens and generate an AST for selected Lua language constructs.
- **Expression Parsing:** Parses and evaluates selected Lua language expressions, with support for operator precedence and grouped statements.
- **Syntax Support**
  - _Data Types:_ Parse integers, floating-point numbers, hexadecimals, booleans, and other Lua literals.
  - _Lua Statements:_ Support for prefix and infix operations, grouped statements with parentheses, and operator precedence.
  - _Control Structures:_ Supports parsing common Lua structures like `if`, `for`, `while`, and `repeat`.
  - _Function Handling:_ Parses function declarations and calls.

### Project Structure

- **Lexer:** Performs lexical analysis and tokenization of Lua code.
- **Parser:** Converts tokens into an AST, following Lua’s operator precedence rules and statement grouping.
- **Tests:** Checks the if the lexer and parser works correctly
