# DataTableSchema.Field Class

Represents information for a column in [DataTable][].

**Syntax**

```csharp
public sealed class Field
```

* * *
## Properties

Name | Description
---- | -----------
[index    ](#00) | Gets the order in which the fields are defined.
[line     ](#01) | Gets the line number in the cell comment where the field is defined.
[rowOffset](#02) | Gets the row position difference that from [Row.startRef][].
[colOffset](#03) | Gets the column position difference that from [Row.startRef][]. 
[name     ](#04) | Gets the name of the field.
[type     ](#05) | Gets the type of the field's value.
[nameNodes](#06) | Gets the [FieldNameNode][]s of the field.
[depth    ](#07) | Gets the depth of the field.

* * *
## Methods

Name | Description
---- | -----------
[ToString()](#08) | Overridden for debugging convenience.

<a name="00"><hr></a>
## index Property

Gets the order in which the fields are defined.

It is not normally used, but you can get the original order when you use fields that are sorted for purposes like [DataTableSchema.fieldsForBuilder][] or [DataTableSchema.fieldsForWriter][].

**Syntax**

```csharp
public int index { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## line Property

Gets the line number in the cell comment where the field is defined.

The first line number is 1.

**Syntax**

```csharp
public int line { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## rowOffset Property

Gets the row position difference that from [Row.startRef][].

**Syntax**

```csharp
public int rowOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="03"><hr></a>
## colOffset Property

Gets the column position difference that from [Row.startRef][].

**Syntax**

```csharp
public int colOffset { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="04"><hr></a>
## name Property

Gets the name of the field.

Whitespace and zero padding removed in the name of field.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String

<a name="05"><hr></a>
## type Property

Gets the type of the field's value.

**Syntax**

```csharp
public string type { get; private set; }
```

Property Value<br>
Type: System.String

<a name="06"><hr></a>
## nameNodes Property

Gets the [FieldNameNode][]s of the field.

**Syntax**

```csharp
public ReadOnlyCollection<FieldNameNode> nameNodes { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[FieldNameNode][]&gt;

<a name="07"><hr></a>
## depth Property

Gets the depth of the field.

Must be greater than or equal to 0.

Same as`nameNodes.count - 1`

**Syntax**

```csharp
public int depth { get; }
```

Property Value<br>
Type: System.Int32

<a name="08"><hr></a>
## ToString() Method

Overridden for debugging convenience.

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
