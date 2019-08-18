# DataTable.PropertyMappingAttribute

Matches the [FieldNameNode][] to a member of the actual instance so that the [DataTable][] can populate the data correctly.

The how-to-use is in the **XlsxParser_02.xlsx** file and **Object3D.cs** file in the example.

**Syntax**

```csharp
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = true,
    Inherited = true
)]
public sealed class PropertyMappingAttribute : Attribute
```

* * *
## Constructors

Name | Description
---- | -----------
[PropertyMappingAttribute(string, string)](#00) | Initializes a new instance with [source][] and [target][].

* * *
## Properties

Name | Description
---- | -----------
[source](#01) | Gets the full name, including all of the names of the ancestors of [FieldNameNode][].
[target](#02) | Gets the name of the member that is mapped with [Field NameNode][].


<a name="00"><hr></a>
## PropertyMappingAttribute(string, string) Constructor

Initializes a new instance with [source][] and [target][].

This attribute allows you to specify the member of the instance to which the actual field value is assigned.

**Syntax**

```csharp
public PropertyMappingAttribute(
    string source,
    string target
)
```

Parameters

1. *source*<br>
    Type: System.String

1. *target*<br>
    Type: System.String    


**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *source* is **null**.
&nbsp;                | *target* is **null**.
ArgumentException     | *source* is invalid pattern.
&nbsp;                | *target* is invalid pattern.


<a name="01"><hr></a>
## source Property

Gets the full name, including all of the names of the ancestors of [FieldNameNode][].

**Syntax**

```csharp
public string source { get; private set; }
```

Property Value<br>
Type: System.String

<a name="02"><hr></a>
## target Property

Gets the name of the member that is mapped with [FieldNameNode][].

Unlike [source][], it does not include the name of the parent member.

It always has the same depth and array index as [source][].

**Syntax**

```csharp
public string target { get; private set; }
```

Property Value<br>
Type: System.String

* * *

[source]: #01
[target]: #02

[DataTable]:     ./DataTable.html
[FieldNameNode]: ./DataTableSchema.FieldNameNode.html
[Field]:         ./DataTableSchema.Field.html
