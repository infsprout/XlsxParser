# DataTable Class

[XlsxParser][]를 통해 파싱된 테이블 데이터를 나타냅니다.<br>
그리고 원하는 객체에 데이터를 채울 수 있습니다.

**Syntax**

```csharp
public sealed class DataTable : IEnumerable<DataTable.Row>
```

* * *
## Properties

Name | Descryption
---- | -----------
[schema            ](#00) | 테이블을 생성하는 데 사용된 [DataTableSchema][]를 가져옵니다.
[name              ](#01) | 테이블의 이름을 가져옵니다.
[Item\[int\]       ](#02) | 테이블의 행을 가져옵니다.
[rowCount          ](#03) | 테이블의 행의 개수를 가져옵니다.
[fieldTypeConverter](#04) | 테이블을 생성하는 데 사용된 [IFieldTypeConverter][] 구현체를 가져옵니다.
[errors            ](#05) | 테이블을 생성하는 과정에서 발생한 에러들을 가져옵니다.
[isValid           ](#06) | 테이블의 유효한지의 여부를 가져옵니다.

* * *
## Methods

Name | Descryption
---- | -----------
[GetEnumerator()                                                    ](#07) | 테이블의 행을 반복하는 열거자를 반환합니다.
[GetCellValue&lt;T&gt;(int, int)                                    ](#08) | 테이블의 셀의 값을 변환하여 반환합니다.
[GetObjectMappingWarnings&lt;T&gt;()                                ](#09) | 테이블에서 지정한 타입으로 데이터를 채울 때 발생할 수 있는 문제들을 반환합니다.
[Populate&lt;T&gt;(IList&lt;T&gt;)                                  ](#10) | 테이블의 데이터를 지정한 컬렉션에 채웁니다.
[Populate&lt;T&gt;(IList&lt;T&gt;, Func&lt;T&gt;)                   ](#11) | 테이블의 데이터를 지정한 컬렉션에 채웁니다.
[Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, int)               ](#12) | 테이블의 데이터를 지정한 컬렉션에 채웁니다.
[Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, Func&lt;T&gt;, int)](#13) | 테이블의 데이터를 지정한 컬렉션에 채웁니다.
[Populate&lt;T&gt;(int, ref T)                                      ](#14) | 테이블의 행을 지정한 인스턴스에 채웁니다.
[ClearPopulatorCache()                                              ](#15) | 테이블의 데이터를 채우는 과정에서 생긴 캐시 데이터를 모두 지웁니다.

<a name="00"><hr></a>
## schema Property

테이블을 생성하는 데 사용된 [DataTableSchema][]를 가져옵니다.

**Syntax**

```csharp
public DataTableSchema schema { get; private set; }
```

Property Value<br>
Type: [DataTableSchema][]


<a name="01"><hr></a>
## name Property

테이블의 이름을 가져옵니다.

`schema.name`과 같습니다.

**Syntax**

```csharp
public string name { get; }
```

Property Value<br>
Type: System.String
  

<a name="02"><hr></a>
## Item[int] Property

테이블의 행을 가져옵니다.

**Syntax**

```csharp
public Row this[
    int index
] { get; }
```

Property Value<br>
Type: [DataTable.Row][]

<a name="03"><hr></a>
## rowCount Property

테이블의 행의 개수를 가져옵니다.

**Syntax**

```csharp
public int rowCount { get; }
```
    
Property Value<br>
Type: System.Int32    


<a name="04"><hr></a>
## fieldTypeConverter Property

테이블을 생성하는 데 사용된 [IFieldTypeConverter][] 구현체를 가져옵니다.

**Syntax**

```csharp
public IFieldTypeConverter fieldTypeConverter { get; private set; }
```

Property Value<br>
Type: [DataTable.IFieldTypeConverter][IFieldTypeConverter]<br>
사용자가 지정하지 않은 경우에는 내부적으로 기본 구현체가 지정됩니다.<br>
그래서 값이 **null**인 경우는 없습니다.

  
<a name="05"><hr></a>
## errors Property

테이블을 생성하는 과정에서 발생한 에러들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<Error> errors { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTable.Error][]&gt;
  
<a name="06"><hr></a>
## isValid Property

테이블의 유효한지의 여부를 가져옵니다.

`errors.Count == 0` 과 같습니다.

**Syntax**

```csharp
public bool isValid { get; }
```

Property Value<br>
Type: System.Boolean
  
<a name="07"><hr></a>
## GetEnumerator() Method

테이블의 행을 반복하는 열거자를 반환합니다.

**Syntax**

```csharp
public IEnumerator<Row> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTable.Row][]&gt;


<a name="08"><hr></a>
## GetCellValue&lt;T&gt;(int, int) Method

테이블의 셀의 값을 변환하여 반환합니다.

**Syntax**

```csharp
public T GetCellVelue<T>(
    int row,
    int col
)
```

Paramaters

1. *row*<br>
    Type: System.Int32

1. *col*<br>
    Type: System.Int32

Return Value<br>
Type: T<br>
지정된 셀의 변환된 값.    

**Exceptions**

[IFieldTypeConverter.ToString(string, object)][]에서 던지는 예외도 포함됩니다.

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *row*가 0보다 작습니다.
&nbsp;                      | *row*가 [rowCount][]보다 크거나 같습니다.
&nbsp;                      | *col*이 0보다 작습니다.
&nbsp;                      | *col*이 [schema.fieldCount][]보다 크거나 같습니다.
InvalidCastException	    | 셀의 값이 *T*으로 변환될 수 없습니다.
FormatException             | 셀의 값이 *T*에서 인식되는 형식이 아닙니다.
OverflowException           | 셀의 값이 *T*의 범위를 벗어나는 숫자를 나타냅니다.


<a name="09"><hr></a>
## GetObjectMappingWarnings&lt;T&gt;() Method

테이블에서 지정한 타입으로 데이터를 채울 때 발생할 수 있는 문제들을 경고들로 반환합니다.

**Syntax**

```csharp
Warnings GetObjectMappingWarnings<T>()
```

Return Value<br>
Type: [DataTable.Warnings][]
  
<a name="10"><hr></a>
## Populate&lt;T&gt;(IList&lt;T&gt;) Method

테이블의 데이터를 지정한 컬렉션에 채웁니다.

**Syntax**

```csharp
Warnings Populate<T>(
    IList<T> dst
) where T : new()
```

Paramaters

1. *dst*<br>
    Type: System.Collections.Generic.IList&lt;T&gt;<br>
    테이블의 데이터가 채워질 컬렉션입니다.

Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException | 테이블이 유효하지 않습니다.
ArgumentNullException     | *dst*가 **null**입니다.
ArgumentException         | *dst*가 배열입니다.
&nbsp;                    | *dst*가 읽기전용입니다.

<a name="11"><hr></a>
## Populate&lt;T&gt;(IList&lt;T&gt;, Func&lt;T&gt;) Method

테이블의 데이터를 지정한 컬렉션에 채웁니다.

**Syntax**

```csharp
Warnings Populate<T>(
    IList<T> dst,
    Func<T> creator
)
```

Paramaters

1. *dst*<br>
    Type: System.Collections.Generic.IList&lt;T&gt;<br>
    테이블의 데이터가 채워질 컬렉션입니다.

1. *creator*<br>
    Type: System.Func&lt;T&gt;<br>
    테이블의 행을 채울 인스턴스가 없을 때 생성하는 함수입니다.<br>
    이미 인스턴스가 있다면 호출되지 않습니다.    

Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException | 테이블이 유효하지 않습니다.
ArgumentNullException     | *dst*가 **null**입니다.
&nbsp;                    | *creator*가 **null**입니다.
ArgumentException         | *dst*가 배열입니다.
&nbsp;                    | *dst*가 읽기전용입니다.


<a name="12"><hr></a>
## Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, int) Method

테이블의 데이터를 지정한 컬렉션에 채웁니다.

**Syntax**

```csharp
public Warnings Populate(
    IDictionary<string, T> dst,
    int keyFieldIndex
) where T : new()
```

Paramaters

1. *dst*<br>
    Type: System.Collections.Generic.IList&lt;T&gt;<br>
    테이블의 데이터가 채워질 컬렉션입니다.

1. *keyFieldIndex*<br>
    Type: System.Int32<br>
    *dst*의 키로 사용되는 [DataTableSchema.Field][]의 인덱스입니다.<br>
    [DataTable.Row][]\[keyFieldIndex\]가 *dst*의 키가 됩니다.<br>
    만약 값이 음수라면 행의 인덱스가 키가 됩니다.


Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | 테이블이 유효하지 않습니다.
ArgumentNullException       | *dst*가 **null**입니다.
ArgumentException           | *dst*가 배열입니다.
&nbsp;                      | *dst*가 읽기전용입니다.
ArgumentOutOfRangeException | *keyFieldIndex*가 [schema.fieldCount][]보다 크거나 같습니다.

**Remarks**

키가 중복되는 경우 값을 아무런 경고없이 값을 덮어씁니다.

스프레드 시트 툴에 있는 **데이터 유효성 검사**기능을 사용하여 키의 유일성을 보장하십시오.

<a name="13"><hr></a>
## Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, Func&lt;T&gt;, int) Method

테이블의 데이터를 지정한 컬렉션에 채웁니다.

**Syntax**

```csharp
public Warnings Populate(
    IDictionary<string, T> dst,
    Func<T> creator,
    int keyFieldIndex
)
```

Paramaters

1. *dst*<br>
    Type: System.Collections.Generic.IList&lt;T&gt;<br>
    테이블의 데이터가 채워질 컬렉션입니다.

1. *creator*<br>
    Type: System.Func&lt;T&gt;<br>
    테이블의 행을 채울 인스턴스가 없을 때 생성하는 함수입니다.<br>
    이미 인스턴스가 있다면 호출되지 않습니다.   

1. *keyFieldIndex*<br>
    Type: System.Int32<br>
    *dst*의 키로 사용되는 [DataTableSchema.Field][]의 인덱스입니다.<br>
    [DataTable.Row][]\[keyFieldIndex\]가 *dst*의 키가 됩니다.<br>
    만약 값이 음수라면 행의 인덱스가 키가 됩니다.


Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | 테이블이 유효하지 않습니다.
ArgumentNullException       | *dst*가 **null**입니다.
&nbsp;                      | *creator*가 **null**입니다.
ArgumentException           | *dst*가 배열입니다.
&nbsp;                      | *dst*가 읽기전용입니다.
ArgumentOutOfRangeException | *keyFieldIndex*가 [schema.fieldCount][]보다 크거나 같습니다.

**Remarks**

키가 중복되는 경우 값을 아무런 경고없이 값을 덮어씁니다.

스프레드 시트 툴에 있는 **데이터 유효성 검사**기능을 사용하여 키의 유일성을 보장하십시오.


<a name="14"><hr></a>
## Populate&lt;T&gt;(int, ref T) Method

테이블의 행을 지정한 인스턴스에 채웁니다.

**Syntax**

```csharp
public Warnings Populate<T>(
    int rowIndex,
    ref T dst
)
```

Parameters

1. *rowIndex*<br>
    Type: System.Int32

1. *dst*<br>
    Type: T<<br>
    데이터 행을 채울 인스턴스입니다.    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | 테이블이 유효하지 않습니다.
ArgumentOutOfRangeException | *rowIndex*가 0보다 작습니다. 
&nbsp;                      | *rowIndex*가 [rowCount][]보다 크거나 같습니다.
ArgumentNullException       | *dst*가 **null**입니다.


<a name="15"><hr></a>
## ClearPopulatorCache() Method

테이블의 데이터를 채우는 과정에서 생긴 캐시 데이터를 모두 지웁니다.

일반적으로 사용할 필요가 없습니다.

**Syntax**

```csharp
public void ClearPopulatorCache()
```
  

* * *

[XlsxParser]:          ./XlsxParser.html
[DataTableSchema]:     ./DataTableSchema.html
[IFieldTypeConverter]: ./DataTable.IFieldTypeConverter.html
[DataTable.Row]:       ./DataTable.Row.html
[schema.fieldCount]:   ./DataTableSchema.html#14

[IFieldTypeConverter.ToString(string, object)]: ./DataTable.IFieldTypeConverter.html#01

[DataTable.Error]:       ./DataTable.Error.html
[DataTable.Warnings]:    ./DataTable.Warnings.html
[DataTableSchema.Field]: ./DataTableSchema.Field.html
