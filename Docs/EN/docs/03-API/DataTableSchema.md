# DataTableSchema Class

Represents the information needed to create a [DataTable][].

In the schema, there is a concept of **block**, **block** means the minimum range that includes all [Field][]s in the schema.

The **block** is a rectangle drawn with a red dotted line in the first picture in Guide [Writing DataTableSchema][].

**Syntax**

```csharp
public sealed class DataTableSchema : IEnumerable<DataTableSchema.Field>
```

* * *
## Properties

Name | Descryption
---- | -----------
[workbookIndex   ](#00) | Gets the order of the XLSX files to be parsed from [XlsxParser][].
[workbookName    ](#01) | Gets the URI of the XLSX file that schema is created.
[sheetIndex      ](#02) | Gets the index of the sheet that schema is created.
[sheetName       ](#03) | Gets the name of the sheet that schema is created.
[cellCommentRef  ](#04) | Gets the [CellRef][] of the cell comment that schema is created.
[name            ](#05) | Gets the name of the table that schema defined.
[rangeStartRef   ](#06) | Gets the [CellRef][] of the first **block** of the table.
[isRotated       ](#07) | Gets whether the table is in vertical or horizontal format.
[nextBlockOffset ](#08) | Gets the count of cells in the table that need to be moved to read the next **block**. 
[blockHeight     ](#09) | Get the height of the **block**.
[blockWidth      ](#10) | Get the width of the **block**.
[errors          ](#11) | Gets errors that occurred during schema creation.
[isValid         ](#12) | Gets whether the schema is valid.
[Item\[int\]     ](#13) | Gets the specified [Field][].
[fieldCount      ](#14) | Gets the count of [Field][].
[fieldsForBuilder](#15) | Get sorted [Field][]s for table creation.
[fieldsForWriter ](#16) | Get sorted [Field][]s to write table data.

* * *
## Methods

Name | Descryption
---- | -----------
[GetEnumerator()](#17) | Returns an enumerator that iterates [Field][].
[ToString()     ](#18) | Overridden for debugging convenience.

<a name="00"><hr></a>
## workbookIndex Property

Gets the order of the XLSX files to be parsed from [XlsxParser][].

**Syntax**

```csharp
public int workbookIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## workbookName Property

Gets the URI of the XLSX file that schema is created.

**Syntax**

```csharp
public string workbookName { get; private set; }
```

Property Value<br>
Type: System.String


<a name="02"><hr></a>
## sheetIndex Property

Gets the index of the sheet that schema is created.

**Syntax**

```csharp
public int sheetIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## sheetName Property

Gets the name of the sheet that schema is created.

**Syntax**

```csharp
public string sheetName { get; private set; }
```

Property Value<br>
Type: System.String

<a name="04"><hr></a>
## cellCommentRef Property

Gets the [CellRef][] of the cell comment that schema is created.

**Syntax**

```csharp
public CellRef cellCommentRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]

<a name="05"><hr></a>
## name Property

Gets the name of the table that schema defined.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String


<a name="06"><hr></a>
## rangeStartRef Property

Gets the [CellRef][] of the first **block** of the table.

**Syntax**

```csharp
public CellRef rangeStartRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="07"><hr></a>
## isRotated Property

Gets whether the table is in vertical or horizontal format.

If the value is **true**, it is in horizontal format.

**Syntax**

```csharp
public bool isRotated { get; private set; }
```

Property Value<br>
Type: System.Boolean


<a name="08"><hr></a>
## nextBlockOffset Property

Gets the count of cells in the table that need to be moved to read the next **block**.

**Syntax**

```csharp
public int nextBlockOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="09"><hr></a>
## blockHeight Property

Get the height of the **block**.

**Syntax**

```csharp
public int blockHeight { get; private set; }
```

Property Value<br>
Type: System.Int32


<a name="10"><hr></a>
## blockWidth Property

Get the width of the **block**.

**Syntax**

```csharp
public int blockWidth { get; private set; }
```

Property Value<br>
Type: System.Int32


<a name="11"><hr></a>
## errors Property

Gets errors that occurred during schema creation.

**Syntax**

```csharp
public ReadOnlyCollecction<Error> errors { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectMode.ReadOnlyCollection&lt;[DataTableSchema.Error][]&gt;


<a name="12"><hr></a>
## isValid Property

Gets whether the schema is valid.

Same as `errors == 0`.

**Syntax**

```csharp
public bool isValid { get; }
```

Property Value<br>
Type: System.Boolean


<a name="13"><hr></a>
## Item[int] Property

Gets the specified [Field][].

**Syntax**

```csharp
public Field this[
    int index
] { get; }
```

Parameters

1. *index*<br>
    Type: System.Int32

Property Value<br>
Type: [DataTableSchema.Field][Field]

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *index* is less than 0.
&nbsp;                      | *index* is greater than or equal to [fieldCount][].


<a name="14"><hr></a>
## fieldCount Property

Gets the count of [Field][].

**Syntax**

```csharp
public int fieldCount { get; }
```

Property Value<br>
Type: System.Int32


<a name="15"><hr></a>
## fieldsForBuilder Property

Get sorted [Field][]s for table creation.

**Syntax**

```csharp
public ReadOnlyCollection<Field> fieldsForBuilder { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema.Field][Field]&gt;


<a name="16"><hr></a>
## fieldsForWriter Property

Get sorted [Field][]s to write table data.

**Syntax**

```csharp
public ReadOnlyCollection<Field> fieldsForWriter { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema.Field][Field]&gt;


<a name="17"><hr></a>
## GetEnumerator() Method

Returns an enumerator that iterates [Field][].

**Syntax**

```csharp
public IEnumerator<Field> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTableSchema.Field][Field]&gt;


<a name="18"><hr></a>
## ToString() Method

Overridden for debugging convenience.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String
    
* * *

[Writing DataTableSchema]: ../02-Guides/01-Writing-DataTableSchema.html
[DataTable]:               ./DataTable.html
[XlsxParser]:              ./XlsxParser.html
[Field]:                   ./DataTableSchema.Field.html
[DataTableSchema.Error]:   ./DataTableSchema.Error.html
[CellRef]:                 ./CellRef.html
