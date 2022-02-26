<div align="center">
<img src="Docs/Images/Logo.jpg" alt="Logo" width="80%" height="80%">
</div>

[![TAO3](https://img.shields.io/nuget/dt/TAO3?style=flat-square&label=TAO3)](https://www.nuget.org/packages/TAO3/)

TAO3 is a [.NET Interactive](https://github.com/dotnet/interactive) extension for writting interactively adhoc data transformation quickly in C#.

For more information on .NET interactive:
-  [Introduction to .NET Interactive on youtube (30 minutes)](https://www.youtube.com/watch?v=DMYtIJT1OeU)
-  [.NET Interactive Github repository](https://github.com/dotnet/interactive)
-  [.NET Interactive documentation](https://github.com/dotnet/interactive/tree/main/docs)
-  [.NET Interactive samples](https://github.com/dotnet/interactive/tree/main/samples)

<details open>
  <summary>Table of contents:</summary>

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
- [Magic Commands](#magic-commands)
  - [`#!input` Magic Command](#input-magic-command)
  - [`#!output` Magic Command](#output-magic-command)
  - [`#!macro` Magic Command](#macro-magic-command)
  - [`#!run` Magic Command](#run-magic-command)
  - [`#!connectMSSQL` Magic Command](#connectmssql-magic-command)
  - [`#!generateHttpClient` Magic Command](#generatehttpclient-magic-command)
- [Sources and destinations](#sources-and-destinations)
  - [`clipboard` Source And Destination](#clipboard-source-and-destination)
  - [`notepad++` Source And Destination](#notepad-source-and-destination)
  - [`file` Source And Destination](#file-source-and-destination)
  - [`clipboardFile` Source](#clipboardfile-source)
  - [`http` Source And Destination](#http-source-and-destination)
  - [`cell`](#cell)
- [Converters](#converters)
  - [`text` Converter](#text-converter)
  - [`line` Converter](#line-converter)
  - [`json` Converter](#json-converter)
  - [`csv` Converter](#csv-converter)
  - [`csvh` Converter](#csvh-converter)
  - [`xml` Converter](#xml-converter)
  - [`html` Converter](#html-converter)
  - [`sql` Converter](#sql-converter)
  - [`csharp` Converter](#csharp-converter)
- [Kernels](#kernels)
  - [`#!razor` Kernel](#razor-kernel)
  - [`#!translate` Kernel](#translate-kernel)
- [Prelude](#prelude)
  - [Excel](#excel)
  - [Notepad++](#notepad)
  - [Keyboard](#keyboard)
  - [Clipboard](#clipboard)
  - [Toast](#toast)
  - [Converter](#converter)
  - [SourceService](#sourceservice)
  - [OutputDestination](#outputdestination)
  - [DestinationService](#destinationservice)
  - [Cells](#cells)
  - [Translation](#translation)
  - [TypeProviders](#typeproviders)
- [Extending TAO3](#extending-tao3)
- [Roadmap](#roadmap)

</details>

## Getting Started

### Prerequisites
- [.NET Interactive](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) in VS Code
- [Notepad++](https://notepad-plus-plus.org/downloads/) (optional)
- Excel (optional)

```c#
//Import the TAO3 nuget package
#r "nuget:TAO3"

//Import the TAO3 prelude
using static TAO3.Prelude;
```

```c#
//Creates a macro that is trigger when CTRL+SHIFT+1 is pressed, that removes the empty lines from the clipboard
#!macro CTRL+SHIFT+1
#!input clipboard line lines
#!out clipboard line

return lines
    .Where(x => x != "");
```

## Magic Commands
Magic commands are special directives provided by .NET interactive and extension developer that enables the user to send special command to the .NET interactive engine. See the documentation from the [dotnet interactive repo](https://github.com/dotnet/interactive/blob/main/docs/magic-commands.md) for a detailed explanation of how magic commands works.

### `#!input` Magic Command

The `#!input` command enables you to load a textual value from a source into a type safe C# class.

**Syntax:**
```
#!input <source> [source arguments] <converter> [converter arguments] <name> [--verbose]
```

**Aliases:** `#!in`

<details open>
  <summary>Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`source`|true|Describe where to get the data to import. The most common source are `clipboard`, `file`, `http` and `file`. See [Source and destinations](#sources-and-destinations) for more details.|
|`source arguments`|-|Parameterise the source. See [Source and destinations](#sources-and-destinations) for more details.|
|`converter`|true|Describe how the source text will be transformed to a C# object. The most common converters are `json`, `text`, `csv`, `xml` and `line`. See [Converters](#converters) for more details.|
|`converter arguments`|-|Parameterise the converter. See [Converters](#converters) for more details.|
|`name`|true|Name of the variable containing the converted text from the source.
|`--verbose` (or `-v`)|false|If enabled, the generated C# classe will be printed in the cell output

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
//Copy the clipboard textual value into a variable named "str" of type string
#!input clipboard text str

return str;
```

**Exemple 2:**
```c#
//Copy every lines of the active tab in Notepad++ into a variable named "lines" of type List<string>
#!input notepad line lines

return lines;
```

**Exemple 3:**
```c#
//1. Takes the text in the clipbard
//2. Infer a C# class named "MyJsonObject"
//3. Deserialize the text from the clipboard using the infered class
//4. Copie the deseralized value into a variable named "myJsonObject"
#!input clipboard json myJsonObject

return myJsonObject;
```

**Exemple 4:**
```c#
//Shorter version of the previous command
#!in cb json myJsonObject

return myJsonObject;
```

**Exemple 5:**

Input:
```c#
//Loads C:\Artists.xml into a variable named artists.
//Print the generated C# class into the output of the cell
#!input file C:\Artists.xml xml artists --verbose

return artists;
```

<details>
  <summary>Output:</summary>

```c#
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TAO3.Converters.Sql;
using TAO3.Converters.Xml;

[TableName("artists")]
public class Artists
{
    [JsonProperty("Artist")]
    [Index(0)]
    [Name("Artist")]
    public Artist Artist { get; set; } = null!;
}

[TableName("Artist")]
public class Artist
{
    [JsonProperty("Name")]
    [Index(0)]
    [Name("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("Age")]
    [Index(1)]
    [Name("Age")]
    public string Age { get; set; } = null!;

    [JsonProperty("Track")]
    [Index(2)]
    [Name("Track")]
    [JsonConverter(typeof(ValueToList<Track>))]
    public List<Track> Track { get; set; } = null!;
}

[TableName("Track")]
public class Track
{
    [JsonProperty("Name")]
    [Index(0)]
    [Name("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("Genre")]
    [Index(1)]
    [Name("Genre")]
    public string Genre { get; set; } = null!;
}

Artists artists = __internal_010ff204dcb24d529c03e502ffbbf60d.Deserialize<Artists>(__internal_9149ba43941a420c913b54b6095ea283, __internal_c7cbd2804ad449b4838956977a80e2d2);
```
</details>

</details>



---

### `#!output` Magic Command

**Syntax:**
```
#!output <destination> [destination arguments] <converter> [converter arguments] [variable] [--verbose]
```

**Aliases:** `#!out`

<details open>
  <summary>Arguments:</summary>

|Name|Required|Default value|Description|
|--|--|--|-- |
|`destination`|false|`clipboard`|Describe where to export the data. The most common destination are `clipboard`, `file`, `http` and `file`. See [Source and destinations](#sources-and-destinations) for more details.|
|`destination arguments`|-|-|Parameterise the source. See [Source and destinations](#sources-and-destinations) for more details.|
|`converter`|false|`text` if output is string, `line` if output is `IEnumerable<string>`, else `json`|Describe how the c# object will be serialised. The most common converters are `json`, `text`, `csv`, `xml` and `line`. See [Converters](#converters) for more details.|
|`converter arguments`|-|-|Parameterise the converter. See [Converters](#converters) for more details.|
|`variable`|false|return value of the cell|Name of the variable containing the converted text from the source.
|`--verbose` (or `-v`)|false|false|If enabled, the generated C# class will be printed in the cell output

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
//Copy "Hello world" into the clipboard
#!output clipboard text
return "Hello world!";
```

**Exemple 2:**
```c#
//Copy "Hello world" into the clipboard
#!output clipboard text str
string str = "Hello world!";
```

**Exemple 3:**
```c#
//Copy the following value into the active tab in notepad++:
//{
//    "A": 1,
//    "B": true
//}
#!output notepad++ json
return new
{
    A = 1,
    B = true
};
```

```c#
//Create a file named "D:\myScript.sql" and with the following content:
//INSERT INTO [MyTable] ([Id], [Value]) VALUES(1, 'Hello');
//INSERT INTO [MyTable] ([Id], [Value]) VALUES(2, 'World');
#!output file D:\myScript.sql sql

record MyTable(int Id, string Value);

return new List<MyTable>
{
    new MyTable(1, "Hello"),
    new MyTable(2, "World")
};
```

</details>

---

### `#!macro` Magic Command
**Syntax:**
```
#!macro <shortcut> [--silent]
```

**Aliases:** none

<details open>
  <summary>Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`shortcut`|true|Key combination that will trigger the macro.|
|`--silent` (or `-s`)|false|Removes the toast notification when the key combination is pressed|

</details>

**Shortcut syntax**
```
[CTRL+] [SHIFT+] [ALT+] <key>
<key>: A, B, ..., Y, Z, 0, 1, ..., 8, 9, F1, F2, ..., F11, F12
```

**Examples of shortcut**
```
CTRL+SHIFT+1
ALT+B
P
CTRL+SHIFT+ALT+F12
```

<details open>
  <summary>Examples:</summary>

```c#
//Creates a macro with the sortcut "CTRL + SHIFT + 1" that transform the content of the clipboard from JSON to XML.
#!macro CTRL+SHIFT+1
#!in cb json myJsonValue
#!out cb xml
return myJsonValue;
```

</details>

---

### `#!run` Magic Command
Run a script from a file.

**Syntax:**
```
#!run file <path>
```

**Aliases:** none

Depending on the file extension, a different kernel will be used to execute the script:
|File extension|Kernel|
|--|--|
|`.dib`|`Composite`|
|`.ipynb`|`Composite`|
|`.cs`|`C#`|
|`.csx`|`C#`|
|`.fs`|`F#`|
|`.fsi`|`F#`|
|`.fsx`|`F#`|
|`.fsscript`|`F#`|
|`.js`|`javascript`|
|`.html`|`HTML`|
|`.ps1`|`PowerShell`|
|other|`C#`|

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
#!run D:\myClass.cs
```

</details>

---

### `#!connectMSSQL` Magic Command
Wrapper around `#!connect mssql` provided by [Microsoft.DotNet.Interactive.SqlServer](https://www.nuget.org/packages/Microsoft.DotNet.Interactive.SqlServer/). This wrapper adds parameter for each part of the connection string, to help the user to write the connection string.

<details open>
  <summary>Examples:</summary>
  
```c#
#!connectMSSQL --server . --database Northwind --integratedSecurity
//Equivalent to:
#r "nuget: Microsoft.DotNet.Interactive.SqlServer, *-*"
#!connect mssql "server=.;database=Northwind;Integrated Security=true" --kernel-name Northwind --create-dbcontext
```
</details>

---

### `#!generateHttpClient` Magic Command
Wrapper of `SvcUtil.exe` that generates HTTP clients for svc and asmx endpoints.

**Syntax:**
```
#!generateHttpClient <uri> [--clientName] [--verbose]
```

## Sources and destinations

Sources are locations where the text is readed for the [#!input](#input) magic command.

Destinations are where the result of the [#!output](#output) magic command is written to. 

Most sources are also destinations and vice versa.

### `clipboard` Source And Destination

Uses the text in the clipboard as a source or a destination.

**Aliases:** `cb`

**`#!input` Arguments:** none

**`#!output` Arguments:** none

---

### `notepad++` Source And Destination

Uses the active tab in notepad++ as a source or a destination. 

**Aliases:** `npp`, `notepad`, `n++`

**`#!input` Arguments:** none

<details open>
  <summary><code>#!output</code> Arguments:</summary>
  
|Name|Required|Description|
|--|--|--|
|`--language`|false|Name of the language used for the syntax highlighting in the current tab of notepad++. If ommited, the default language defined in the converter will be used. See [NppLanguage.cs](/TAO3/Services/Notepad/NppLanguage.cs) for all legal values.

</details>

---

### `file` Source And Destination

Uses a file as a source or a destination

**Aliases:** none

<details open>
  <summary><code>#!input</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`path`|true|Path to the file used as a source|
|`--encoding`|false|Encoding used to read the file. Legal values are: `iso-8859-1`, `us-ascii` `utf-16`, `utf-16BE`, `utf-32`, `utf-32BE`, `utf-8`

</details>

<details open>
  <summary><code>#!output</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`path`|true|Path to the file used as a destination|
|`--encoding`|false|Encoding used to read the file. Legal values are: `iso-8859-1`, `us-ascii` `utf-16`, `utf-16BE`, `utf-32`, `utf-32BE`, `utf-8`

</details>

---

### `clipboardFile` Source

Uses the currently copied files as a source. `clipboardFile` cannot be used in the `#!output` magic command.

**Aliases:** `cbf`

<details open>
  <summary><code>#!input</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`--encoding`|false|Encoding used to read the files. Legal values are: `iso-8859-1`, `us-ascii` `utf-16`, `utf-16BE`, `utf-32`, `utf-32BE`, `utf-8`

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**

Files copied in clipboard
```
D:\Blogs.json
D:\Posts.json
D:\Archives\ArchivedPosts.json
```

Cell:
```
#!input clipboardFile json copiedFiles -v
```

Result:
```c#
//...
public class CopiedFiles
{
    //...
    public Blogs Blogs { get; set; }
    //...
    public Posts Posts { get; set; }
    //...
    public ArchivedPosts ArchivedPosts { get; set; }
}

//Infered classes for D:\Blogs.json
public class Blogs
{
    //...
}

//Infered classes for D:\Posts.json
public class Posts
{
    //...
}

//Infered classes for D:\Archives\ArchivedPosts.json
public class ArchivedPosts
{
    //...
}

```

</details>

---

### `http` Source And Destination
Uses a HTTP request as source or a destination.

**Aliases:** none

<details open>
  <summary><code>#!input</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`uri`|true`|URI used to do the request|
|`--verb`|false|HTTP verb used to do request. Legal values are `Delete`, `Get`, `Head`, `Options`, `Patch`, `Post`, `Put` and `Trace`. The default value is `Get`

</details>

<details open>
  <summary><code>#!output</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`uri`|true`|URI used to do the request|
|`--verb`|false|HTTP verb used to do request. Legal values are `Delete`, `Get`, `Head`, `Options`, `Patch`, `Post`, `Put` and `Trace`. The default value is `Post`|
|`mediaType`|false|Media type used for the content of the request. If ommited, the `MimeType` defined in the [converter](#converter) will be used as the `mediaType`|

</details>

### `cell`
Taked the current cell as the source, without the `#!input` magic command. `cell` cannot be used in the `#!output` magic command.

**Aliases:** none

<details open>
  <summary>Examples:</summary>

**Exemple 1:**

Cell 1 (HTML cell):
```html
#!input cell xml code
<div>
    hello world
</div>
```

Cell 2 (C# cell):
```c#
return code;
```

output:
|div|
|--|
|hello world|

</details>

## Converters

### `text` Converter

---

### `line` Converter

---

### `json` Converter

<details open>
  <summary><code>#!input</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`--type` (or `-t`)|false|Type to use to deserialise the input text. It can be `dynamic`. If omited, the type will be infered|
|`--settings`|false|Converter settings of type `Newtonsoft.Json.JsonSerializerSettings`|

</details>

<details open>
  <summary><code>#!output</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`--settings`|false|Converter settings of type `Newtonsoft.Json.JsonSerializerSettings`|

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
#!input clipboard json myJson
return myJson;
```

**Exemple 2:**
```c#
var bob = new
{
    Name = "Bob",
    Age = 50
};
#!output file D:\file.json json bob
```

**Exemple 3:**
```c#
#!input clipboard json --type dynamic myJson
return myJson;
```

**Exemple 4:**
```c#
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

#!input clipboard json --type Person person
return person;
```

**Exemple 5:**
```c#
Newtonsoft.Json.JsonSerializerSettings mySettings = new Newtonsoft.Json.JsonSerializerSettings();

#!in cb json --settings mySettings myJson
return myJson;
```

</details>

---

### `csv` Converter

<details open>
  <summary><code>#!input</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`--separator` (or `-s`)|false|Separator used between the values in the CSV
|`--type` (or `-t`)|false|Type to use to deserialise the input text. It can be `dynamic`. If omited, the type will be infered|
|`--settings`|false|Converter settings of type `CsvHelper.Configuration.CsvConfiguration`|

</details>

<details open>
  <summary><code>#!output</code> Arguments:</summary>

|Name|Required|Description|
|--|--|--|
|`--separator` (or `-s`)|false|Separator used between the values in the CSV
|`--settings`|false|Converter settings of type `CsvHelper.Configuration.CsvConfiguration`|

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
#!input clipboard csv myCSV
return myCSV;
```

**Exemple 2:**
```c#
var bob = new
{
    Name = "Bob",
    Age = 50
};
#!output file D:\file.csv csv bob
//Writes "Bob,50" in D:\file.csv
```

**Exemple 3:**
```c#
#!input clipboard csv --type dynamic rows
return rows;
```

**Exemple 4:**
```c#
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

#!input clipboard csv --type Person persons
return persons;
```


**Exemple 5:**
```c#
CsvHelper.Configuration.CsvConfiguration mySettings = new CsvHelper.Configuration.CsvConfiguration();

#!in cb csv --settings mySettings rows
return rows;
```

</details>

---

### `csvh` Converter
Exactly the same as [csv](#csvh-converter), except the CSV has an header.

---

### `xml` Converter

---

### `html` Converter

---

### `sql` Converter

---

### `csharp` Converter

## Kernels
In .NET Interactive, [kernels](https://github.com/dotnet/interactive/blob/main/docs/kernels-overview.md) are a way to add custom language to a notebook. Kernels provide a way to execute code of a specific language and language services like code completion and diagnostics. 

TAO3 provides 2 kernels. The first one is the [#!razor](#razor) kernel that enables you to generate textual output using the [razor template engine](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-6.0). The second one is the [#!translate](#translate) kernel that enables you to translate text from one language to another.

### `#!razor` Kernel

**Syntax:**
```
#!razor [--mimeType <mimetype>] [--name <name>] [--suppress] [--verbose]
```

<details open>
  <summary>Arguments:</summary>

|Name|Description|
|--|--|
|`--mimeType`|Mime type used to display the resulting document|
|`--name`|Name of the variable containing the resulting document|
|`--supress`|Suppress the displaying of the resulting document|
|`--verbose`|Print the generated C# class representing the razor template into to cell output|

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
var persons = new[]{
    new
    {
        Name = "Bob",
        Age = 45
    },
    new
    {
        Name = "George",
        Age = 32
    }
};

#!razor
<table>
    <thead>
        <tr>
            <th>Name</th>
            <th>Age</th>
        </tr>
    </thead>
    <tbody>
    @foreach (var person in persons)
    {
        <tr>
            <td>@person.Name</td>
            <td>@person.Age</td>
        </tr>
    }
    </tbody>
</table>
```

**Exemple 2:**
```c#
#!output clipboard text generatedSql

var rows = new[]{
    new
    {
        Id = 1,
        Col = "Col1"
    },
    new
    {
        Id = 2,
        Col = "Col2"
    }
};

#!razor --name generatedSql --suppress
@foreach (var row in rows)
{
    <text>UPDATE [TableName] ([Id], [Col]) SET [Col] = </text>
    @row.Col

    <text> WHERE [Id] = </text>
    @row.Id;

    <text>
</text>
}
```

</details>

---

### `#!translate` Kernel
The `#!translate` kernel is wrapper around the [LibreTranslate](https://github.com/LibreTranslate/LibreTranslate) translation API.

```
#!translate <source> <target>
```

<details open>
  <summary>Arguments:</summary>

|Name|Description|
|--|--|
|`source`|Language of the input text|
|`target`|Language to translate the input text to|

</details>

<details open>
  <summary>Language list:</summary>

|Language|Description|
|--|--|
|en|English|
|ar|Arabic|
|az|Azerbaijani|
|zh|Chinese|
|cs|Czech|
|nl|Dutch|
|fi|Finnish|
|fr|French|
|de|German|
|hi|Hindi|
|hu|Hungarian|
|id|Indonesian|
|ga|Irish|
|it|Italian|
|ja|Japanese|
|ko|Korean|
|pl|Polish|
|pt|Portuguese|
|ru|Russian|
|es|Spanish|
|sv|Swedish|
|tr|Turkish|
|uk|Ukranian|
|vi|Vietnamese|
|auto|Auto Detect|

</details>

<details open>
  <summary>Examples:</summary>

**Exemple 1:**
```c#
//Set the URI used for the translation. This is required. 
//If you want to self host, or choose another URI, consult the LibreTranslate Github page:
//https://github.com/LibreTranslate/LibreTranslate
using static TAO3.Prelude;
ConfigureTranslator("https://libretranslate.de/");

//Writes "Bonjour !" in the cell output
#!translate en fr
Hello world!
```

## Prelude

The prelude ([TAO3.Prelude](/TAO3/Prelude.cs)) provides a convinient way to access most of the APIs provided by `TAO3` using C#. 

It is recommanded to import the prelude statically, so that you can have access to its method directly:
```csharp
#r "nuget:TAO3"
using static TAO3.Prelude;
```

</details>

### Excel

---

### Notepad++

---

### Keyboard

---

### Clipboard

---

### Toast

---

### Converter

---

### SourceService

---

### OutputDestination

---

### DestinationService

---

### Cells

---

### Translation

---

### TypeProviders

## Extending TAO3
- Custom converters
  - source converter
    - type inference (TypeProvider)
  - destination converter
- Custom sources
- Custom destinations

## Roadmap
[TodoList.dib](TAO3/TodoList.dib)