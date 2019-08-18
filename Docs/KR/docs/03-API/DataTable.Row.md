# DataTable.Row Class

[DataTable][]의 행을 나타냅니다.

**Syntax**

```csharp
public sealed class Row : IEnumerable<object>
```

* * *
## Properties

Name | Description
---- | -----------
[startRef   ](#00) | 행이 시작되는 셀주소입니다.
[Item\[int\]](#01) | 셀의 값을 가져옵니다.
[cellCount  ](#02) | 셀의 개수를 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#03) | 셀의 값을 반복하는 열거자를 반환합니다.

<a name="00"><hr></a>
## startRef Property

행이 시작되는 셀주소입니다.

**Syntax**

```csharp
public CellRef startRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="01"><hr></a>
## Item[int] Property

셀의 값을 가져옵니다.<br>

대부분의 경우 [DataTable.GetCellValue&lt;T&gt;(int, int)] 메소드를 사용하는 것이 더 편합니다.

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
ArgumentOutOfRangeException | *index*가 0보다 작습니다.
&nbsp;                      | *index*가 [cellCount][]보다 크거나 같습니다.

**Examples**

셀 추가정보 가져오기

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

셀의 개수를 가져옵니다.

**Syntax**

```csharp
public int cellCount { get; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## GetEnumerator() Method

셀의 값을 반복하는 열거자를 반환합니다.

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
