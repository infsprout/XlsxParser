# DataTableSchema.Error

Represents an occured error when parsing a cell comment in a sheet to create a [DataTableSchema][].

**Syntax**

```csharp
public sealed class Error
```

* * *
## Properties

Name | Description
---- | -----------
[line   ](#00) | The line number where the error occurred in the cell comment.
[message](#01) | The error message.

<a name="00"><hr></a>
## line Property

The line number where the error occurred in the cell comment.

The first line number is 1.

**Syntax**

```csharp
public int line { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## message Property

The error message.<br>

It is guaranteed to be single line.

**Syntax**

```csharp
public string message { get; private set; }
```

Property Value<br>
Type: System.String

* * *

[DataTableSchema]: ./DataTableSchema.html
