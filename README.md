# Highlight
A relatively simple and extensible syntax highlighter written in C#.

## TL;DR
```powershell
Install-Package Highlight
```

```csharp
var highlighter = new Highlighter(new HtmlEngine());
var highlightedCode = highlighter.Highlight("C#", csharpCode);
```

## Syntax definitions
The following is a list of all the definition names of syntaxes/languages that are supported out of the box;

- ASPX
- C
- C++
- C#
- COBOL
- Eiffel
- Fortran
- Haskell
- HTML
- Java
- JavaScript
- Mercury
- MSIL
- Pascal
- Perl
- PHP
- Python
- Ruby
- SQL
- Visual Basic
- VBScript
- VB.NET
- XML

## Output engines
Highlight supports the notion of an output engine which makes it possible to get the syntax highlighted result output in any format. Out of the box Highlight support HTML, XML and RTF output formats.

The HtmlEngine supports inline styles which can be enabled by setting the **UseCss** property to **true**;

```csharp
var highlighter = new Highlighter(new HtmlEngine { UseCss = true });
var highlightedCode = highlighter.Highlight("C#", csharpCode);
```
