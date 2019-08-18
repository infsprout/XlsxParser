# ReturnMessages Class

Represents multiple string messages returned by a particular method.

By default, multiple messages are merged into single string.

Each message is separated by a newline character and is guaranteed to be a single line.

It is not used directly.

**Syntax**

```csharp
public class ReturnMessages : IEnumerable<string>
```

* * *
## Properties

Name | Description
---- | -----------
[count      ](#00) | The count of messages.
[Item\[int\]](#01) | Gets the message at the specified index.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#02) | Returns an enumerator that iterates a message.
[ToString()     ](#03) | Returns the total message that is merged.

<a name="00"><hr></a>
## count Property

The count of messages.

**Syntax**

```csharp
public int count { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## Item[int] Property

Gets the message at the specified index.

**Syntax**

```csharp
public string this[
    int index
] { get; }
```

Parameters

1. *index*<br>
    Type: System.Int32

Property Value<br>
Type: System.String

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *index* is less than 0.
&nbsp;                      | *index* is greater than or equal to [count][].  

<a name="02"><hr></a>
## GetEnumerator() Method

Returns an enumerator that iterates a message.

**Syntax**

```csharp
public IEnumerator<string> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;System.String&gt;


<a name="03"><hr></a>
## ToString() Method

Returns the total message that is merged.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

* * *
