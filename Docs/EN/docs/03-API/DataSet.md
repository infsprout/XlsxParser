# DataSet Class

Collection of [DataTable][] created by [XlsxParser][].

**Syntax**

```csharp
public sealed class DataSet : IEnumerable<DataTable>
```

* * *
## Properties

Name | Description
---- | -----------
[Item\[string\]](#00) | Gets the [DataTable][] with the specified name.
[tableCount    ](#01) | Gets the count of [DataTable][].

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#02) | Returns an enumerator that iterates [DataTable][].

<a name="00"><hr></a>
## Item[string] Property

Gets the [DataTable][] with the specified name.

**Syntax**

```csharp
public DataTable this[
    string tableName
] { get; }
```

Parameters

1. *tableName*<br> 
    Type: System.String<br>
    The name of the [DataTable][] to get.

Property Value<br>
Type: [DataTable][]<br>
Gets **null** if the specified [DataTable][] does not exist.

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *tableName* is **null**.

<a name="01"><hr></a>
## tableCount Property

Gets the count of [DataTable][].

**Syntax**

```csharp
public int tableCount { get; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## GetEnumerator() Method

Returns an enumerator that iterates [DataTable][].

**Syntax**

```csharp
public IEnumerator<DataTable> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTable][]&gt;

* * *

[XlsxParser]:         ./XlsxParser.html
[DataTable]:          ./DataTable.html
