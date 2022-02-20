![Logo](/Docs/Images/Logo.jpg)

TAO3 is a [.NET Interactive](https://github.com/dotnet/interactive) extension for writting interactively adhoc data transformation quickly in C#.


**Table of contents**
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
- [Magic commands](#magic-commands)
  - [#!input](#input)
    - [Exemples:](#exemples)
  - [#!output](#output)
    - [Exemples:](#exemples-1)
  - [#!macro](#macro)
    - [Exemples:](#exemples-2)
  - [#!run](#run)
    - [Exemples:](#exemples-3)
  - [#!connectMSSQL](#connectmssql)
  - [#!generateHttpClient](#generatehttpclient)
- [Sources and destinations](#sources-and-destinations)
- [Converters](#converters)
  - [`text` converter](#text-converter)
  - [`line` converter](#line-converter)
  - [`json` converter](#json-converter)
    - [Exemples:](#exemples-4)
  - [`csv` converter](#csv-converter)
    - [Exemples:](#exemples-5)
  - [`csvh` converter](#csvh-converter)
  - [`xml` converter](#xml-converter)
  - [`html` converter](#html-converter)
  - [`sql` converter](#sql-converter)
  - [`csharp` converter](#csharp-converter)
- [Kernels](#kernels)
  - [#!razor](#razor)
    - [Exemples:](#exemples-6)
  - [#!translate](#translate)
    - [Exemples:](#exemples-7)
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

## Magic commands
Magic commands are special directives provided by dotnet interactive and extension developer that. See the documentation from the [dotnet interactive repo]((https://github.com/dotnet/interactive/blob/main/docs/magic-commands.md)) for a detailed explanation of how magic commands works.

### #!input
The `#!input` command enables you to load a textual value from a source into a type safe C# class.

**Syntax:**
```
#!input <source> [source arguments] <converter> [converter arguments] <name> [--verbose]
```

**Aliases:** `#!in`

**Arguments:**
|Name|Required|Description|
|--|--|--|
|`source`|true|Describe where to get the data to import. The most common source are `clipboard`, `file`, `http` and `file`. See [Source and destinations](#sources-and-destinations) for more details.|
|`source arguments`|-|Parameterise the source. See [Source and destinations](#sources-and-destinations) for more details.|
|`converter`|true|Describe how the source text will be transformed to a C# object. The most common converters are `json`, `text`, `csv`, `xml` and `line`. See [Converters](#converters) for more details.|
|`converter arguments`|-|Parameterise the converter. See [Converters](#converters) for more details.|
|`name`|true|Name of the variable containing the converted text from the source.
|`--verbose` (or `-v`)|false|If enabled, the generated C# classe will be printed in the cell output

#### Exemples:

**Exemple 1:**
```c#
//Copy the clipboard textual value into a variable named "str" of type string
#!input clipboard text str

return str;
```

**Exemple 2:**
```c#
//Copy every lines of the active tab in Notepad++ into a variable named "lines" of type List<string>
#!input npp line lines

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

Output:
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

### #!output

**Syntax:**
```
#!output <destination> [destination arguments] <converter> [converter arguments] [variable] [--verbose]
```

**Aliases:** `#!out`

**Arguments:**
|Name|Required|Default value|Description|
|--|--|--|-- |
|`destination`|false|`clipboard`|Describe where to export the data. The most common destination are `clipboard`, `file`, `http` and `file`. See [Source and destinations](#sources-and-destinations) for more details.|
|`destination arguments`|-|-|Parameterise the source. See [Source and destinations](#sources-and-destinations) for more details.|
|`converter`|false|`text` if output is string, `line` if output is `IEnumerable<string>`, else `json`|Describe how the c# object will be serialised. The most common converters are `json`, `text`, `csv`, `xml` and `line`. See [Converters](#converters) for more details.|
|`converter arguments`|-|-|Parameterise the converter. See [Converters](#converters) for more details.|
|`variable`|false|return value of the cell|Name of the variable containing the converted text from the source.
|`--verbose` (or `-v`)|false|false|If enabled, the generated C# class will be printed in the cell output

#### Exemples:

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

### #!macro
**Syntax:**
```
#!macro <shortcut> [--silent]
```

**Aliases:** none

**Arguments:**
|Name|Required|Description|
|--|--|--|
|`shortcut`|true|Key combination that will trigger the macro.|
|`--silent` (or `-s`)|false|Removes the toast notification when the key combination is pressed|


**Shortcut syntax**
```
[CTRL+] [SHIFT+] [ALT+] <key>
<key>: A, B, ..., Y, Z, 0, 1, ..., 8, 9, F1, F2, ..., F11, F12
```

**Exemples of shortcut**
```
CTRL+SHIFT+1
ALT+B
P
CTRL+SHIFT+ALT+F12
```


#### Exemples:

**Exemple 1:**
```c#
//Creates a macro with the sortcut "CTRL + SHIFT + 1" that transform the content of the clipboard from JSON to XML.
#!macro CTRL+SHIFT+1
#!in cb json myJsonValue
#!out cb xml
return myJsonValue;
```

### #!run
Run a script. The supported file types are `.dib`, `

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

#### Exemples:

**Exemple 1:**
```c#
#!run D:\myClass.cs
```


### #!connectMSSQL
Wrapper around `#!connect mssql` provided by [Microsoft.DotNet.Interactive.SqlServer](https://www.nuget.org/packages/Microsoft.DotNet.Interactive.SqlServer/). This wrapper adds parameter for each part of the connection string, to help the user to write the connection string.

**Exemples:**
```c#
#!connectMSSQL --server . --database Northwind --integratedSecurity
//Equivalent to:
#r "nuget: Microsoft.DotNet.Interactive.SqlServer, *-*"
#!connect mssql "server=.;database=Northwind;Integrated Security=true" --kernel-name Northwind --create-dbcontext
```

### #!generateHttpClient
Wrapper of `SvcUtil.exe` that generates HTTP clients for svc and asmx endpoints.

**Syntax:**
```
#!generateHttpClient <uri> [--clientName] [--verbose]
```

## Sources and destinations


## Converters

### `text` converter
### `line` converter
### `json` converter

**`#!input` arguments:**
|Name|Required|Description|
|--|--|--|
|`--type` (or `-t`)|false|Type to use to deserialise the input text. It can be `dynamic`. If omited, the type will be infered|
|`--settings`|false|Converter settings of type `Newtonsoft.Json.JsonSerializerSettings`|

**`#!output` arguments:**
|Name|Required|Description|
|--|--|--|
|`--settings`|false|Converter settings of type `Newtonsoft.Json.JsonSerializerSettings`|

#### Exemples:

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

### `csv` converter

**`#!input` arguments:**
|Name|Required|Description|
|--|--|--|
|`--separator` (or `-s`)|false|Separator used between the values in the CSV
|`--type` (or `-t`)|false|Type to use to deserialise the input text. It can be `dynamic`. If omited, the type will be infered|
|`--settings`|false|Converter settings of type `CsvHelper.Configuration.CsvConfiguration`|

**`#!output` arguments:**
|Name|Required|Description|
|--|--|--|
|`--separator` (or `-s`)|false|Separator used between the values in the CSV
|`--settings`|false|Converter settings of type `CsvHelper.Configuration.CsvConfiguration`|

#### Exemples:

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

### `csvh` converter
Exactly the same as [csv](#csvh-converter), except the CSV has an header.

### `xml` converter
### `html` converter
### `sql` converter
### `csharp` converter

## Kernels
In .NET Interactive, [kernels](https://github.com/dotnet/interactive/blob/main/docs/kernels-overview.md) are a way to add custom language to a notebook. Kernels provide a way to execute code of a specific language and language services like code completion and diagnostics. 

TAO3 provides 2 kernels. The first one is the [#!razor](#razor) kernel that enables you to generate textual output using the [razor template engine](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-6.0). The second one is the [#!translate](#translate) kernel that enables you to translate text from one language to another.

### #!razor

**Syntax:**
```
#!razor [--mimeType <mimetype>] [--name <name>] [--suppress] [--verbose]
```

**Arguments:**
|Name|Description|
|--|--|
|`--mimeType`|Mime type used to display the resulting document|
|`--name`|Name of the variable containing the resulting document|
|`--supress`|Suppress the displaying of the resulting document|
|`--verbose`|Print the generated C# class representing the razor template into to cell output|

#### Exemples:

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

### #!translate
The `#!translate` kernel is wrapper around the [LibreTranslate](https://github.com/LibreTranslate/LibreTranslate) translation API.

```
#!translate <source> <target>
```

**Arguments:**
|Name|Description|
|--|--|
|`source`|Language of the input text|
|`target`|Language to translate the input text to|

**Language list:**
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

#### Exemples:

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

### Excel
### Notepad++
### Keyboard
### Clipboard
### Toast
### Converter
### SourceService
### OutputDestination
### DestinationService
### Cells
### Translation
### TypeProviders
