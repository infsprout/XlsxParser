# DataTable.IFieldTypeConverter Interface

Converts to user-defined field type.

User-defined field types must be ValueType(enum, struct).

**Syntax**

```csharp
public interface IFieldTypeConverter
```

* * *
## Methods

Name | Description
---- | -----------
[FromString(string, string)](#00) | Converts a string to a specified type.
[ToString(string, object)  ](#01) | Converts a specified type to a string.

<a name="00"><hr></a>
## FromString(string, string) Method

Converts a string to a specified type.

It is called by [XlsxParser][] in the process of creating [DataTable][].

**Syntax**

```csharp
public object FromString(
    string type,
    string src
)
```

Parameters

1. *type*<br>
    Type: System.String<br>
    The user-defined type name.

1. *src*<br>
    Type: System.String<br>
    The cell value of string type that has not yet been converted.

Return Value<br>
Type: String.Object<br>
Cell value that has been converted to a user-defined type.<br>
If you return **null**, [Error][] is added to [DataTable][]. 

**Remarks**

An exception within this method will not interrupt the parsing of [XlsxParser][].<br>
The exception is converted to [Error][] and added to [DataTable][].

<a name="01"><hr></a>
## ToString(string, object) Method

Converts a specified type to a string.

It is called by [DataTable.GetCellValue&lt;T&gt;(int, int)][] when T is a string.

**Syntax**

```csharp
public string ToString(
    string type,
    object src
)
```

1. *type*<br>
    Type: System.String<br>
    The user-defined type name.

1. *src*<br>
    Type: System.Object<br>
    The cell value that has been converted to the user-defined type.

Return Value<br>
Type: System.String<br>

* * *

[XlsxParser]: ./XlsxParser.html
[DataTable]:  ./DataTable.html
[Error]:      ./DataTable.Error.html
[DataTable.GetCellValue&lt;T&gt;(int, int)]: ./DataTable.html#08
