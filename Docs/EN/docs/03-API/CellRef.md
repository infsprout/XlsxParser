# CellRef Structure

Represents a cell address of a sheet.

**Syntax**

```csharp
public struct CellRef : IComparable
```

* * *
## Constructors

Name | Description
---- | -----------
[CellRef(string)  ](#00) | Initializes a new instance with an A1 style reference.
[CellRef(int, int)](#01) | Initializes a new instance with row index and column index.

* * *
## Properties

Name | Description
---- | -----------
[row](#02) | Gets the row index value.
[col](#03) | Gets the column index value.

* * *
## Methods

Name | Description
---- | -----------
[ToA1StyleRef()   ](#04) | Returns the A1 style reference.
[ToString()       ](#05) | Returns the A1 style reference.
[GetHashCode()    ](#06) | Returns the hash code value.
[Equals(object)   ](#07) | Returns a value indicating whether this instance is equal to a specified object.
[CompareTo(object)](#08) | Compares this instance to the specified object and indicates whether the position of this instance is before, behind, or after the specified object in the sort order.

<a name="00"><hr></a>
## CellRef(string) Constructor

Initializes a new instance with an A1 style reference.<br>
A1 style is a style that expresses columns as alphabetic characters and rows as numbers greater than 0.

**Syntax**

```csharp
public CellRef(
    string a1StyleRef
)
```

Parameters

1. *a1StyleRef*<br>
    Type: System.String<br>
    The a1 style reference value, case-insensitive.

**Exceptions**    

Exception | Condition
--------- | ---------
ArgumentNullException | *a1StyleRef* is **null**.
ArguementException    | *a1StyleRef* is not an A1 style reference format.

<a name="01"><hr></a>
## CellRef(int, int) Constructor

Initializes a new instance with row index and column index.

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
ArgumentException | *row* is less than 0.
&nbsp;            | *col* is less than 0.

<a name="02"><hr></a>
## row Property

Gets the row index value.

**Syntax**

```csharp
public int row { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## col Property

Gets the column index value.

**Syntax**

```csharp
public int col { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="04"><hr></a>
## ToA1StyleRef() Method

Returns the A1 style reference.

**Syntax**

```csharp
public string ToA1StyleRef()
```

Return Value<br>
Type: System.String

<a name="05"><hr></a>
## ToString() Method

Returns the A1 style reference.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

<a name="06"><hr></a>
## GetHashCode() Method

Returns the hash code value.

**Syntax**

```csharp
public override int GetHashCode()    
```

Return Value<br>
Type: System.Int32

<a name="07"><hr></a>
## Equals(object) Method

Returns a value indicating whether this instance is equal to a specified object.

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

Compares this instance to the specified object and indicates whether the position of this instance is before, behind, or after the specified object in the sort order.

The row values are compared first, and if the row values are the same, the column values are compared.

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
ArgumentException | *other* is not a CellRef type.

* * *
