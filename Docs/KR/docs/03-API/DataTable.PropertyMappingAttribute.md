# DataTable.PropertyMappingAttribute

[DataTable][]이 올바르게 데이터 채우기를 할 수 있도록<br>
[FieldNameNode][]를 실제 인스턴스의 멤버에 매칭 시킵니다.

실제 사용법은 예제에 있는 XlsxParser_02.xlsx 파일과 Object3D.cs에 있습니다.

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
[PropertyMappingAttribute(string, string)](#00) | [source][]와 [target][]으로 새 인스턴스를 초기화합니다.

* * *
## Properties

Name | Description
---- | -----------
[source](#01) | [FieldNameNode][]의 상위 노드의 이름들을 모두 포함하는 전체 이름을 가져옵니다.
[target](#02) | [FieldNameNode][]와 매핑되는 맴버의 이름을 가져옵니다.


<a name="00"><hr></a>
## PropertyMappingAttribute(string, string) Constructor

[source][]와 [target][]으로 새 인스턴스를 초기화합니다.

이 속성의 이용해 실제 필드값이 지정되는 인스턴스의 멤버를 지정할 수 있습니다.

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
ArgumentNullException | *source*가 **null**입니다.
&nbsp;                | *target*이 **null**입니다.
ArgumentException     | *source*의 패턴이 잘못되었습니다.
&nbsp;                | *target*의 패턴이 잘못되었습니다.


<a name="01"><hr></a>
## source Property

[FieldNameNode][]의 상위 노드의 이름들을 모두 포함하는 전체 이름을 가져옵니다.

**Syntax**

```csharp
public string source { get; private set; }
```

Property Value<br>
Type: System.String

<a name="02"><hr></a>
## target Property

[FieldNameNode][]와 매핑되는 멤버의 이름을 가져옵니다.

[source][]와 달리 상위 멤버의 이름은 포함하지 않습니다.

[source][]와 항상 같은 깊이와 배열 인덱스를 가지고 있습니다.

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
