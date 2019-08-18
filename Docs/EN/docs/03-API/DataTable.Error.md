# DataTable.Error

[DataTable][] represents an error occurred during creation.

**Syntax**

```csharp
public sealed class Error
```

* * *
## Properties

Name | Description
---- | -----------
[cellRef](#00) | Gets the address of the cell where the error occurred.
[message](#01) | Gets the error message.

<a name="00"><hr></a>
## cellRef Property

Gets the address of the cell where the error occurred.

**Syntax**

```csharp
public CellRef cellRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="01"><hr></a>
## message Property

Gets the error message.

It is guaranteed to be single line.

**Syntax**

```csharp
public string message { get; private set; }
```

Property Value<br>
Type: System.String

* * *

[DataTable]: ./DataTable.html
[CellRef]:   ./CellRef.html
