# DataTable.Row Class

Represents a row in [DataTable][].

**Syntax**

```csharp
public sealed class Row : IEnumerable<object>
```

* * *
## Properties

Name | Description
---- | -----------
[startRef   ](#00) | The cell address at which the row begins.
[Item\[int\]](#01) | Gets the value of the cell.
[cellCount  ](#02) | Gets the count of cells.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#03) | Returns an enumerator that iterates a cell value.

<a name="00"><hr></a>
## startRef Property

The cell address at which the row begins.

**Syntax**

```csharp
public CellRef startRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="01"><hr></a>
## Item[int] Property

Gets the value of the cell.

In most cases, using the [DataTable.GetCellValue&lt;T&gt;(int, int)][] is more convenient.

**Syntax**

```csharp
public object this[
    int index
] { get; }
```

Parameters

1. *index*<br>
    Type: System.Object<br>

Property Value<br>
Type: System.Object

**Exception**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *index* is less than 0.
&nbsp;                      | *index* is greater than or equal to [cellCount][].

**Examples**

How to get cell details.

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser("a.xlsx");
    yield return parser;
    var table = parser.dataSet["MyTable"];
    var rowIndex = 0;
    var colIndex = 0;
    var row       = table[rowIndex];
    var cellValue = row[colIndex];
    var field     = table.schema[col];
    var cellRef   = new CellRef(
        row.startRef.row + field.rowOffset,
        row.startRef.col + field.colOffset
    );
    Debug.Log("cellRef: "  + cellRef);
    Debug.Log("cellType: " + field.type);
}
```                            

<a name="02"><hr></a>
## cellCount Property

Gets the count of cells.

**Syntax**

```csharp
public int cellCount { get; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## GetEnumerator() Method

Returns an enumerator that iterates a cell value.

**Syntax**

```csharp
public IEnumerator<object> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;System.Object&gt;

* * *

[cellCount]: #02

[CellRef]:   ./CellRef.html
[DataTable.GetCellValue&lt;T&gt;(int, int)]: ./DataTable.html#08
[DataTable]: ./DataTable.html
