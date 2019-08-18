# DataTableSchema.FieldNameNode Class

Represents a substring delimited by '.' In [Field.name][].

**Syntax**

```csharp
public sealed class FieldNameNode
```

* * *
## Properties

Name | Description
---- | -----------
[name          ](#00) | Gets the name of the node.
[arrayIndex    ](#01) | Gets the index of the array to which the node belongs.
[isArrayElement](#02) | Gets whether the node is an element of an array.

<a name="00"><hr></a>
## name Property

Gets the name of the node.

It does not include parts that represent index of an array.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String

<a name="01"><hr></a>
## arrayIndex Property

Gets the index of the array to which the node belongs.

If not in the array, the value is -1.

**Syntax**

```csharp
public int arrayIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## isArrayElement Property

Gets whether the node is an element of an array.

Same as `arrayIndex >= 0`.

**Syntax**

```csharp
public bool isArrayElement { get; }
```

Property Value<br>
Type: System.Boolean

* * *

[Field.name]: ./DataTableSchema.Field.html#04
