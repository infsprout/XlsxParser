# DataTableSchema Class

[DataTable][] 생성하기 위해 필요한 정보들을 가지고 있습니다. 

스키마에서 **블럭**이란 개념이 있는데 **블럭**이란 스키마 내의 모든 [Field][]들을 모두 포함하는 최소한의 범위를 의미합니다.

가이드 [Writing DataTableSchema][]에 있는 첫번째 그림에서 빨간 점선으로 그려진 
사각형 입니다.

**Syntax**

```csharp
public sealed class DataTableSchema : IEnumerable<DataTableSchema.Field>
```

* * *
## Properties

Name | Descryption
---- | -----------
[workbookIndex   ](#00) | [XlsxParser][]에서 파싱되는 XLSX파일의 순서를 가져옵니다.
[workbookName    ](#01) | 스키마가 생성된 XLSX파일의 URI를 가져옵니다.
[sheetIndex      ](#02) | 스키마가 생성된 시트의 순서를 가져옵니다.
[sheetName       ](#03) | 스키마가 생성된 시트의 이름을 가져옵니다.
[cellCommentRef  ](#04) | 스키마가 생성된 [CellRef][]를 가져옵니다.
[name            ](#05) | 스키마가 정의한 테이블의 이름을 가져옵니다.
[rangeStartRef   ](#06) | 테이블의 첫번째 **블럭**의 [CellRef][]를 가져옵니다.
[isRotated       ](#07) | 테이블이 세로형식인지 가로형식인지 여부를 가져옵니다.
[nextBlockOffset ](#08) | 테이블을 다음 **블럭**을 읽기 위해 이동해야 하는 셀의 칸 수를 가져옵니다. 
[blockHeight     ](#09) | **블럭**의 높이를 가져옵니다.
[blockWidth      ](#10) | **블럭**의 너비를 가져옵니다.
[errors          ](#11) | 스키마 생성 중에 발생한 에러들을 가져옵니다.
[isValid         ](#12) | 스키마의 유효성 여부를 가져옵니다.
[Item\[int\]     ](#13) | 지정한 [Field][]를 가져옵니다.
[fieldCount      ](#14) | [Field][]들의 개수를 가져옵니다.
[fieldsForBuilder](#15) | 테이블 생성을 위해 정렬된 [Field][]들을 가져옵니다.
[fieldsForWriter ](#16) | 테이블 데이터를 쓰기 위해 정렬된 [Field][]들을 가져옵니다.

* * *
## Methods

Name | Descryption
---- | -----------
[GetEnumerator()](#17) | [Field][]를 반복하는 열거자를 반환합니다.
[ToString()     ](#18) | 디버깅 편의성을 위해 오버라이딩 되었습니다.

<a name="00"><hr></a>
## workbookIndex Property

[XlsxParser][]에서 파싱되는 XLSX파일의 순서를 가져옵니다.

**Syntax**

```csharp
public int workbookIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## workbookName Property

스키마가 생성된 XLSX파일의 URI를 가져옵니다.

**Syntax**

```csharp
public string workbookName { get; private set; }
```

Property Value<br>
Type: System.String


<a name="02"><hr></a>
## sheetIndex Property

스키마가 생성된 시트의 순서를 가져옵니다.

**Syntax**

```csharp
public int sheetIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## sheetName Property

스키마가 생성된 시트의 이름을 가져옵니다.

**Syntax**

```csharp
public string sheetName { get; private set; }
```

Property Value<br>
Type: System.String

<a name="04"><hr></a>
## cellCommentRef Property

스키마가 생성된 [CellRef][]를 가져옵니다.

**Syntax**

```csharp
public CellRef cellCommentRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]

<a name="05"><hr></a>
## name Property

스키마가 정의한 테이블의 이름을 가져옵니다.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String


<a name="06"><hr></a>
## rangeStartRef Property

테이블의 첫번째 **블럭**의 [CellRef][]를 가져옵니다.

**Syntax**

```csharp
public CellRef rangeStartRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="07"><hr></a>
## isRotated Property

테이블이 세로형식인지 가로형식인지 여부를 가져옵니다.

값이 **true**이면 가로형식입니다. 

**Syntax**

```csharp
public bool isRotated { get; private set; }
```

Property Value<br>
Type: System.Boolean


<a name="08"><hr></a>
## nextBlockOffset Property

테이블을 다음 **블럭**을 읽기 위해 이동해야 하는 셀의 칸 수를 가져옵니다. 

**Syntax**

```csharp
public int nextBlockOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="09"><hr></a>
## blockHeight Property

**블럭**의 높이를 가져옵니다.

**Syntax**

```csharp
public int blockHeight { get; private set; }
```

Property Value<br>
Type: System.Int32


<a name="10"><hr></a>
## blockWidth Property

**블럭**의 너비를 가져옵니다.

**Syntax**

```csharp
public int blockWidth { get; private set; }
```

Property Value<br>
Type: System.Int32


<a name="11"><hr></a>
## errors Property

스키마 생성 중에 발생한 에러들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollecction<Error> errors { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectMode.ReadOnlyCollection&lt;[DataTableSchema.Error][]&gt;


<a name="12"><hr></a>
## isValid Property

스키마의 유효성 여부를 가져옵니다.

`errors == 0`과 같습니다.

**Syntax**

```csharp
public bool isValid { get; }
```

Property Value<br>
Type: System.Boolean


<a name="13"><hr></a>
## Item[int] Property

지정한 [Field][]를 가져옵니다.

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
ArgumentOutOfRangeException | *index*가 0보다 작습니다.
&nbsp;                      | *index*가 [fieldCount][]보다 크거나 같습니다.


<a name="14"><hr></a>
## fieldCount Property

[Field][]들의 개수를 가져옵니다.

**Syntax**

```csharp
public int fieldCount { get; }
```

Property Value<br>
Type: System.Int32


<a name="15"><hr></a>
## fieldsForBuilder Property

테이블 생성을 위해 정렬된 [Field][]들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<Field> fieldsForBuilder { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema.Field][Field]&gt;


<a name="16"><hr></a>
## fieldsForWriter Property

테이블 데이터를 쓰기 위해 정렬된 [Field][]들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<Field> fieldsForWriter { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema.Field][Field]&gt;


<a name="17"><hr></a>
## GetEnumerator() Method

[Field][]를 반복하는 열거자를 반환합니다.

**Syntax**

```csharp
public IEnumerator<Field> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTableSchema.Field][Field]&gt;


<a name="18"><hr></a>
## ToString() Method

디버깅 편의성을 위해 오버라이딩 되었습니다.

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
