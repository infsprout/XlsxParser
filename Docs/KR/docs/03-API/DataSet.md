# DataSet Class

[XlsxParser][]에 의해 생성되는 [DataTable][]의 Collection입니다.

**Syntax**

```csharp
public sealed class DataSet : IEnumerable<DataTable>
```

* * *
## Properties

Name | Description
---- | -----------
[Item\[string\]](#00) | 지정한 이름을 갖는 [DataTable][]을 가져옵니다.
[tableCount    ](#01) | [DataTable][]의 개수를 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#02) | [DataTable][]을 반복하는 열거자를 반환합니다.

<a name="00"><hr></a>
## Item[string] Property

지정한 이름을 갖는 [DataTable][]을 가져옵니다.

**Syntax**

```csharp
public DataTable this[
    string tableName
] { get; }
```

Parameters

1. *tableName*<br>  
    Type: System.String<br>
    가져올 [DataTable][]의 이름입니다.

Property Value<br>
Type: [DataTable][]<br>
지정한 [DataTable][]이 없는 경우 **null**을 가져옵니다.

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *tableName*이 **null**입니다.

<a name="01"><hr></a>
## tableCount Property

[DataTable][]의 개수를 가져옵니다.

**Syntax**

```csharp
public int tableCount { get; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## GetEnumerator() Method

[DataTable][]을 반복하는 열거자를 반환합니다.

**Syntax**

```csharp
public IEnumerator<DataTable> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTable][]&gt;

* * *

[XlsxParser]:         ./XlsxParser.html
[DataTable]:          ./DataTable.html
