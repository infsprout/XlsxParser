# CellRef Structure

시트의 셀 주소를 나타냅니다.

**Syntax**

```csharp
public struct CellRef : IComparable
```

* * *
## Constructors

Name | Description
---- | -----------
[CellRef(string)  ](#00) | A1스타일의 참조값으로 새 인스턴스를 초기화 합니다.
[CellRef(int, int)](#01) | 행과 열 인덱스로 새 인스턴스를 초기화 합니다.

* * *
## Properties

Name | Description
---- | -----------
[row](#02) | 행 인덱스 값을 가져옵니다.
[col](#03) | 열 인덱스 값을 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[ToA1StyleRef()   ](#04) | A1스타일의 참조값을 반환합니다.
[ToString()       ](#05) | A1스타일의 참조값을 반환합니다.
[GetHashCode()    ](#06) | 해쉬 코드값을 반환합니다. 
[Equals(object)   ](#07) | 이 인스턴스와 지정한 개체의 값이 같은지를 확인합니다.
[CompareTo(object)](#08) | 이 인스턴스를 지정된 객체와 비교하고 정렬 순서에서 이 인스턴스의 위치가 지정된 객체보다 앞인지, 뒤인지 또는 동일한지를 나타냅니다.

<a name="00"><hr></a>
## CellRef(string) Constructor

A1스타일의 참조값으로 새 인스턴스를 초기화 합니다.<br>
A1스타일은 열을 문자열로 행을 0보다 큰 숫자로 표현하는 스타일입니다.

**Syntax**

```csharp
public CellRef(
    string a1StyleRef
)
```

Parameters

1. *a1StyleRef*<br>
    Type: System.String<br>
    A1스타일의 참조값, 대소문자 구분은 하지 않습니다. 

**Exceptions**    

Exception | Condition
--------- | ---------
ArgumentNullException | *a1StyleRef*가 **null**입니다.
ArguementException    | *a1StyleRef*가 A1스타일 참조 양식이 아닙니다.

<a name="01"><hr></a>
## CellRef(int, int) Constructor

행과 열 인덱스로 새 인스턴스를 초기화 합니다.

**Syntax**

```csharp
public CellRef(
    int row, 
    int col
)
```

Parameters

1. *row*<br>
    Type: System.Int32<br>

1. *col*<br>
    Type: System.Int32<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentException | *row*가 0보다 작습니다.
&nbsp;            | *col*이 0보다 작습니다.

<a name="02"><hr></a>
## row Property

행 인덱스 값을 가져옵니다.

**Syntax**

```csharp
public int row { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## col Property

열 인덱스 값을 가져옵니다.

**Syntax**

```csharp
public int col { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="04"><hr></a>
## ToA1StyleRef() Method

A1스타일의 참조값을 반환합니다.

**Syntax**

```csharp
public string ToA1StyleRef()
```

Return Value<br>
Type: System.String

<a name="05"><hr></a>
## ToString() Method

A1스타일의 참조값을 반환합니다.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

<a name="06"><hr></a>
## GetHashCode() Method

해쉬 코드값을 반환합니다. 

**Syntax**

```csharp
public override int GetHashCode()    
```

Return Value
Type: System.Int32

<a name="07"><hr></a>
## Equals(object) Method

이 인스턴스와 지정한 개체의 값이 같은지를 확인합니다.

**Syntax**

```csharp
public override bool Equals(
    object other
)
```

Parameters

1. *other*<br>
    Type: System.Object

Return Value<br>
Type: System.Boolean


<a name="08"><hr></a>
## CompareTo(object) Method

이 인스턴스를 지정된 객체와 비교하고 정렬 순서에서 이 인스턴스의 위치가 지정된 객체보다 앞인지, 뒤인지 또는 동일한지를 나타냅니다.

행값을 우선 비교하고 행값이 같으면 열값을 비교합니다.

**Syntax**

```csharp
public override int CompareTo(
    object other
)
```
Return Value<br>
Type: System.Int32

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentException | *other*가 CellRef타입이 아닙니다.

* * *
