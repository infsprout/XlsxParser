# ReturnMessages Class

특정 메소드가 반환하는 여러개의 문자열 메시지들을 나타냅니다.

기본적으로 여러개의 메시지들이 하나의 문자열로 합쳐져있는 상태입니다.

각각의 메시지는 줄바꿈 문자로 구분되어 있고 한 줄인 것을 보장합니다.

직접 사용되지 않습니다.

**Syntax**

```csharp
public class ReturnMessages : IEnumerable<string>
```

* * *
## Properties

Name | Description
---- | -----------
[count      ](#00) | 메시지의 개수 입니다.
[Item\[int\]](#01) | 지정한 인덱스에 있는 메시지를 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()](#02) | 메시지를 반복하는 열거자를 반환합니다.
[ToString()     ](#03) | 합쳐져 있는 전체 메시지를 반환합니다.

<a name="00"><hr></a>
## count Property

메시지의 개수 입니다.

**Syntax**

```csharp
public int count { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## Item[int] Property

지정한 인덱스에 있는 메시지를 가져옵니다.

**Syntax**

```csharp
public string this[
    int index
] { get; }
```

Parameters

1. *index*<br>
    Type: System.Int32

Property Value
Type: System.String

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *index*가 0보다 작습니다.
&nbsp;                      | *index*가 [count][]보다 크거나 같습니다.  

<a name="02"><hr></a>
## GetEnumerator() Method

메시지를 반복하는 열거자를 반환합니다.

**Syntax**

```csharp
public IEnumerator<string> GetEnumerator()
```

Return Value<br>
Type: System.Collections.Generic.IEnumerator&lt;System.String&gt;


<a name="03"><hr></a>
## ToString() Method

합쳐져 있는 전체 메시지를 반환합니다.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

* * *
