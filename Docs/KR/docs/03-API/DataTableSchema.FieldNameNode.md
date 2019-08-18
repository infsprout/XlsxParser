# DataTableSchema.FieldNameNode Class

[Field.name][]에서 '.'으로 구분되는 하위 문자열을 나타냅니다.

**Syntax**

```csharp
public sealed class FieldNameNode
```

* * *
## Properties

Name | Description
---- | -----------
[name          ](#00) | 노드의 이름을 가져옵니다.
[arrayIndex    ](#01) | 노드가 속해있는 배열의 인덱스를 가져옵니다.
[isArrayElement](#02) | 노드가 배열의 요소인지 여부를 가져옵니다.

<a name="00"><hr></a>
## name Property

노드의 이름을 가져옵니다.

배열을 표현하는 부분은 포함되지 않습니다.

**Syntax**

```csharp
public string name { get; private set; }
```

Property Value<br>
Type: System.String

<a name="01"><hr></a>
## arrayIndex Property

노드가 속해있는 배열의 인덱스를 가져옵니다.

배열에 속하지 않은 경우 값은 -1입니다.

**Syntax**

```csharp
public int arrayIndex { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="02"><hr></a>
## isArrayElement Property

노드가 배열의 요소인지 여부를 가져옵니다.

`arrayIndex >= 0`과 같습니다.

**Syntax**

```csharp
public bool isArrayElement { get; }
```

Property Value<br>
Type: System.Boolean

* * *

[Field.name]: ./DataTableSchema.Field.html#04
