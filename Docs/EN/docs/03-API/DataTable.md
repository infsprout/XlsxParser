# DataTable Class

Represents table data parsed through [XlsxParser][].<br>
And you can populate the data to object that you want.

**Syntax**

```csharp
public sealed class DataTable : IEnumerable<DataTable.Row>
```

* * *
## Properties

Name | Descryption
---- | -----------
[schema            ](#00) | Gets the [DataTableSchema][] used to create the table.
[name              ](#01) | Gets the name of the table.
[Item\[int\]       ](#02) | Gets the row of the table.
[rowCount          ](#03) | Gets the count of rows in the table.
[fieldTypeConverter](#04) | Gets the [IFieldTypeConverter][] implementation used to create the table.
[errors            ](#05) | Gets errors generated during table creation.
[isValid           ](#06) | Gets whether the table is valid.

* * *
## Methods

Name | Descryption
---- | -----------
[GetEnumerator()                                                    ](#07) | Returns an enumerator that iterates through the rows in the table.
[GetCellValue&lt;T&gt;(int, int)                                    ](#08) | Converts and returns the value of a cell in the table.
[GetObjectMappingWarnings&lt;T&gt;()                                ](#09) | Returns problems that can occur when populating the data with the type specified in the table.
[Populate&lt;T&gt;(IList&lt;T&gt;)                                  ](#10) | Populates the data in the table into the specified collection.
[Populate&lt;T&gt;(IList&lt;T&gt;, Func&lt;T&gt;)                   ](#11) | Populates the data in the table into the specified collection.
[Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, int)               ](#12) | Populates the data in the table into the specified collection.
[Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, Func&lt;T&gt;, int)](#13) | Populates the data in the table into the specified collection.
[Populate&lt;T&gt;(int, ref T)                                      ](#14) | Populates a row in the table with the specified instance.
[ClearPopulatorCache()                                              ](#15) | Clears all cache data that was created during the process of populating the table.

<a name="00"><hr></a>
## schema Property

Gets the [DataTableSchema][] used to create the table.

**Syntax**

```csharp
public DataTableSchema schema { get; private set; }
```

Property Value<br>
Type: [DataTableSchema][]


<a name="01"><hr></a>
## name Property

Gets the name of the table.

Same as `schema.name`.

**Syntax**

```csharp
public string name { get; }
```

Property Value<br>
Type: System.String
  

<a name="02"><hr></a>
## Item[int] Property

Gets the row of the table.

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

Gets the count of rows in the table.

**Syntax**

```csharp
public int rowCount { get; }
```
    
Property Value<br>
Type: System.Int32    


<a name="04"><hr></a>
## fieldTypeConverter Property

Gets the [IFieldTypeConverter][] implementation used to create the table.

**Syntax**

```csharp
public IFieldTypeConverter fieldTypeConverter { get; private set; }
```

Property Value<br>
Type: [DataTable.IFieldTypeConverter][IFieldTypeConverter]<br>
If not specified by the user, a default implementation is specified internally.<br>
So the value will not be **null**.

  
<a name="05"><hr></a>
## errors Property

Gets errors generated during table creation.

**Syntax**

```csharp
public ReadOnlyCollection<Error> errors { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTable.Error][]&gt;
  
<a name="06"><hr></a>
## isValid Property

Gets whether the table is valid.<br>

Same as `errors.Count == 0`.

**Syntax**

```csharp
public bool isValid { get; }
```

Property Value<br>
Type: System.Boolean
  
<a name="07"><hr></a>
## GetEnumerator() Method

Returns an enumerator that iterates through the rows in the table.

**Syntax**

```csharp
public IEnumerator<Row> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;[DataTable.Row][]&gt;


<a name="08"><hr></a>
## GetCellValue&lt;T&gt;(int, int) Method

Converts and returns the value of a cell in the table.

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
The converted value of the specified cell.  

**Exceptions**

Includes exceptions thrown from [IFieldTypeConverter.ToString(string, object)][].

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *row* is less than 0.
&nbsp;                      | *row* is greater than or equal to [rowCount][].
&nbsp;                      | *col* is less than 0.
&nbsp;                      | *col* is greater than or equal to [schema.fieldCount][].
InvalidCastException	    | The value of the cell can not be converted to *T*.
FormatException             | The value of the cell is not in a format recognized by *T*.
OverflowException           | The value of the cell represents a number that outside the range of *T*.


<a name="09"><hr></a>
## GetObjectMappingWarnings&lt;T&gt;() Method

Returns problems that can occur when populating the data with the type specified in the table.

**Syntax**

```csharp
Warnings GetObjectMappingWarnings<T>()
```

Return Value<br>
Type: [DataTable.Warnings][]
  
<a name="10"><hr></a>
## Populate&lt;T&gt;(IList&lt;T&gt;) Method

Populates the data in the table into the specified collection.

**Syntax**

```csharp
Warnings Populate<T>(
    IList<T> dst
) where T : new()
```

Paramaters

1. *dst*<br>
    Type: System.Collections.Generic.IList&lt;T&gt;<br>
    The collection in which the table's data will be populated.

Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException | The table is invalid.
ArgumentNullException     | *dst* is **null**.
ArgumentException         | *dst* is an array.
&nbsp;                    | *dst* is readonly.

<a name="11"><hr></a>
## Populate&lt;T&gt;(IList&lt;T&gt;, Func&lt;T&gt;) Method

Populates the data in the table into the specified collection.

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
    The collection in which the table's data will be populated.

1. *creator*<br>
    Type: System.Func&lt;T&gt;<br>
    This function creates a instance when there are no instances to populate the table's rows.<br>
    If there is already an instance, it will not be called.  

Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException | The table is invalid.
ArgumentNullException     | *dst* is **null**.
&nbsp;                    | *creator* is **null**.
ArgumentException         | *dst* is an array.
&nbsp;                    | *dst* is readonly.


<a name="12"><hr></a>
## Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, int) Method

Populates the data in the table into the specified collection.

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
    The collection in which the table's data will be populated.

1. *keyFieldIndex*<br>
    Type: System.Int32<br>
    The index of [DataTableSchema.Field][] to be used as the key of *dst*.<br>
    The cell value [DataTable.Row][]\[keyFieldIndex\] becomes the key of *dst*.<br>
    If the value is negative, the row index is the key.


Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | The table is invalid.
ArgumentNullException       | *dst* is **null**.
ArgumentException           | *dst* is an array.
&nbsp;                      | *dst* is readonly.
ArgumentOutOfRangeException | *keyFieldIndex* is greater than or equal to [schema.fieldCount][].

**Remarks**

If the key is duplicated, the value is overwritten with no warning.

Use the **Data Validation** feature in the spreadsheet tool to ensure uniqueness of your key.

<a name="13"><hr></a>
## Populate&lt;T&gt;(IDictionary&lt;string, T&gt;, Func&lt;T&gt;, int) Method

Populates the data in the table into the specified collection.

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
    The collection in which the table's data will be populated.

1. *creator*<br>
    Type: System.Func&lt;T&gt;<br>
    This function creates a instance when there are no instances to populate the table's rows.<br>
    If there is already an instance, it will not be called.   

1. *keyFieldIndex*<br>
    Type: System.Int32<br>
    The index of [DataTableSchema.Field][] to be used as the key of *dst*.<br>
    The cell value [DataTable.Row][]\[keyFieldIndex\] becomes the key of *dst*.<br>
    If the value is negative, the row index is the key.


Return Value<br>
Type: [DataTable.Warnings][]    

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | The table is invalid.
ArgumentNullException       | *dst* is **null**.
&nbsp;                      | *creator* is **null**.
ArgumentException           | *dst* is an array.
&nbsp;                      | *dst* is readonly.
ArgumentOutOfRangeException | *keyFieldIndex* is greater than or equal to [schema.fieldCount][].

**Remarks**

If the key is duplicated, the value is overwritten with no warning.

Use the **Data Validation** feature in the spreadsheet tool to ensure uniqueness of your key.


<a name="14"><hr></a>
## Populate&lt;T&gt;(int, ref T) Method

Populates a row in the table with the specified instance.

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
    Type: T<br>
    The instance to populate the row's data.   

**Exceptions**

Exception | Condition
--------- | ---------
InvalidOperationException   | The table is invalid.
ArgumentOutOfRangeException | *rowIndex* is less than 0.
&nbsp;                      | *rowIndex* is greater than or equal to [rowCount][].
ArgumentNullException       | *dst* is **null**.


<a name="15"><hr></a>
## ClearPopulatorCache() Method

Clears all cache data that was created during the process of populating the table.

Generally, you don't need use it.

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
