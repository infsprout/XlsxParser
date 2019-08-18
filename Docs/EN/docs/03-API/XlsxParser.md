# XlsxParser Class

Creates [DataSet][] by XLSX file parsing.



**Syntax**

```csharp
public sealed class XlsxParser : IDisposable
```

* * *
## Constructors

Name | Description
---- | -----------
[XlsxParser(params XlsxRequest\[\])](#00) | Initializes a new instance with [XlsxRequest][] and proceed with parsing.

* * *
## Properties

Name | Description
---- | -----------
[requests      ](#01) | Gets the arguments that were used when creating the instance.
[progress      ](#02) | Gets the progress of the parsing.
[isDone        ](#03) | Gets whether parsing is complete.
[erroredSchemas](#04) | Gets [DataTableSchema][]s that failed to creates a [DataTable][] due to an error during parsing.
[errors        ](#05) | Gets all errors that occurred during parsing.
[dataSet       ](#06) | Gets the [DataSet][] created by parsing.
[coroutine     ](#07) | Gets the main coroutine for parsing.
[startTime     ](#08) | Gets the time at which parsing began.
[elapsedTime   ](#09) | Gets the elapsed time since parsing has began.

* * *
## Methods

Name | Description
---- | -----------
[IsDuplicatedSchema(DataTableSchema)](#10) | Returns whether the specified [DataTableSchema][] is duplicated.
[GetOriginalSchema(DataTableSchema) ](#11) | Returns the first [DataTableSchema][] that was created with the name of the specified [DataTableSchema][].
[Dispose()                          ](#12) | Releases the resources used during the parsing process.


<a name="00"><hr></a>
## XlsxParser(params XlsxRequest[]) Constructor

Initializes a new instance with [XlsxRequest][] and proceed with parsing.

**Syntax**

```csharp
public XlsxParser(
    params XlsxRequest[] requests
)
```

<a name="01"><hr></a>
## requests Property

Gets the arguments that were used when creating the instance.

**Syntax**

```csharp
public ReadOnlyCollection<XlsxRequest> requests { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[XlsxRequest][]&gt;

<a name="02"><hr></a>
## progress Property

Gets the progress of the parsing.

**Syntax**

```csharp
public float progress { get; private set; }
```

Property Value<br>
Type: System.Single

<a name="03"><hr></a>
## isDone Property

Gets whether parsing is complete.

Same as `progress == 1`.

**Syntax**

```csharp
public bool isDone { get; }
```

Property Value<br>
Type: System.Boolean

<a name="04"><hr></a>
## erroredSchemas Property

Gets [DataTableSchema][]s that failed to creates a [DataTable][] due to an error during parsing.

**Syntax**

```csharp
public ReadOnlyCollection<DataTableSchema> erroredSchemas { get; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema]&gt;

**Remarks**

It has no error that [DataTableSchema][] included to erroredschemas because has duplication problem only.

<a name="05"><hr></a>
## errors Property

Gets all errors that occurred during parsing.

**Syntax**

```csharp
public Errors errors { get; }
```

Property Value<br>
Type: [XlsxParser.Errors][]

<a name="06"><hr></a>
## dataSet Property

Gets the [DataSet][] created by parsing.

**Syntax**

```csharp
public DataSet dataSet { get; private set; }
```

Property Value<br>
Type: [DataSet][]

<a name="07"><hr></a>
## coroutine Property

Gets the main coroutine for parsing.

You can use this to wait until the parsing of XlsxParser is completed within the coroutine.

**Syntax**

```csharp
public Coroutine coroutine { get; private set; }
```

Property Value<br>
Type: UnityEngine.Coroutine

**Examples**

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser("a.xlsx");
    yield return parser.coroutine;
}
```

<a name="08"><hr></a>
## startTime Property

Gets the time at which parsing began.

The time in seconds since the game started.

**Syntax**

```csharp
public float startTime { get; private set; }
```

Property Value<br>
Type: System.Single

<a name="09"><hr></a>
## elapsedTime Property

Gets the elapsed time since parsing has began.

The time in seconds.

It is not incremented since the parsing has been completed.

**Syntax**

```csharp
public float elapsedTime { get; }
```

Property Value<br>
Type: System.Single

<a name="10"><hr></a>
## IsDuplicatedSchema(DataTableSchema) Method

Returns whether the specified [DataTableSchema][] is duplicated.

**Syntax**

```csharp
public IsDuplicatedSchema(
    DataTableSchema schema
)
```

Parameters

1. *schema*<br>
    Type: [DataTableSchema][]

Return Value<br>
Type: System.Boolean    

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *schema* is **null**.

**Remarks**

The argument must be a schema created from the same parser.

<a name="11"><hr></a>
## GetOriginalSchema(DataTableSchema) Method

Returns the first [DataTableSchema][] that was created with the name of the specified [DataTableSchema][].

**Syntax**

```csharp
public DataTableSchema GetOriginalSchema(
    DataTableSchema schema
)
```
Parameters

1. *schema*<br>
    Type: [DataTableSchema][]

Return Value<br>
Type: [DataTableSchema][]    

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *schema* is **null**.

**Remarks**

The argument must be a schema created from the same parser.

<a name="12"><hr></a>
## Dispose() Method

Releases the resources used during the parsing process.

It is automatically called after parsing is complete, so you do not need to use a using block.

Calling during parsing will not do anything.

**Syntax**

```csharp
public void Dispose()
```


* * *

[DataSet]:           ./DataSet.html
[XlsxRequest]:       ./XlsxRequest.html
[DataTable]:         ./DataTable.html
[DataTableSchema]:   ./DataTableSchema.html
[XlsxParser.Errors]: ./XlsxParser.Errors.html
