# DataTableSchema.Field Class

[DataTable][]의 열부분의 정보를 나타냅니다.

**Syntax**

```csharp
public sealed class Field
```

* * *
## Properties

Name | Description
---- | -----------
[index    ](#00) | 필드가 정의된 순서를 가져옵니다.
[line     ](#01) | 필드가 정의된 셀 주석에서의 줄번호를 가져옵니다.
[rowOffset](#02) | [Row.startRef][]와의 행 위치 차이를 가져옵니다.
[colOffset](#03) | [Row.startRef][]와의 열 위치 차이를 가져옵니다. 
[name     ](#04) | 필드의 이름을 가져옵니다.
[type     ](#05) | 필드의 값의 타입을 가져옵니다.
[nameNodes](#06) | 필드의 [FieldNameNode][]들을 가져옵니다.
[depth    ](#07) | 필드의 깊이를 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[ToString()](#08) | 디버깅 편의성을 위해 오버라이딩 되었습니다.

<a name="00"><hr></a>
## index Property

필드가 정의된 순서를 가져옵니다.

일반적으로 사용할 일은 없으나 [DataTableSchema.fieldsForBuilder][] 또는 [DataTableSchema.fieldsForWriter][]같이 목적을 위해 정렬된 필드들을 사용할 때 원래 순서를 알아낼 수 있습니다.

**Syntax**

```csharp
public int index { get; private set; }
```

Property Value<br>
Type: System.Int32


<a name="01"><hr></a>
## line Property

필드가 정의된 셀 주석에서의 줄번호를 가져옵니다.

첫번째 줄번호는 1입니다.

**Syntax**

```csharp
public int line { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## rowOffset Property

[Row.startRef][]와의 행 위치 차이를 가져옵니다.

**Syntax**

```csharp
public int rowOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## colOffset Property

[Row.startRef][]와의 열 위치 차이를 가져옵니다. 

**Syntax**

```csharp
public int colOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="04"><hr></a>
## name Property

필드의 이름을 가져옵니다.

필드의 이름은 공백 문자와 배열 인덱스의 0패딩이 제거되어 있습니다.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String

<a name="05"><hr></a>
## type Property

필드의 값의 타입을 가져옵니다.

**Syntax**

```csharp
public string type { get; private set; }
```

Property Value<br>
Type: System.String

<a name="06"><hr></a>
## nameNodes Property

필드의 [FieldNameNode][]들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<FieldNameNode> nameNodes { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[FieldNameNode][]&gt;

<a name="07"><hr></a>
## depth Property

필드의 깊이를 가져옵니다.

반드시 0보다 크거나 같습니다.

`nameNodes.count - 1`과 같습니다.

**Syntax**

```csharp
public int depth { get; }
```

Property Value<br>
Type: System.Int32

<a name="08"><hr></a>
## ToString() Method

디버깅 편의성을 위해 오버라이딩 되었습니다.

**Syntax**

```csharp
public override string ToString()
```

Property Value<br>
Type: System.String


* * *

[DataTable]:     ./DataTable.html
[Row.startRef]:  ./DataTable.Row.html#00
[FieldNameNode]: ./DataTableSchema.FieldNameNode.html
[DataTableSchema.fieldsForBuilder]: ./DataTableSchema.html#15
[DataTableSchema.fieldsForWriter]:  ./DataTableSchema.html#16
