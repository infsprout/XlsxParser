# DataTableSchema.Error

시트의 셀 주석을 파싱하여
[DataTableSchema][]를 생성하는 과정에서 발생한 에러를 나타냅니다.

**Syntax**

```csharp
public sealed class Error
```

* * *
## Properties

Name | Description
---- | -----------
[line   ](#00) | 셀 주석에서 에러가 발생한 줄 번호.
[message](#01) | 에러 설명 메시지.

<a name="00"><hr></a>
## line Property

셀 주석에서 에러가 발생한 줄 번호.<br>

첫번째 줄 번호는 1입니다.

**Syntax**

```csharp
public int line { get; private set; }
```

Property Value<br>
Type: System.Int32

<a name="01"><hr></a>
## message Property

에러 설명 메시지입니다.<br>

반드시 1줄인 것을 보장합니다. 

**Syntax**

```csharp
public string message { get; private set; }
```

Property Value<br>
Type: System.String

* * *

[DataTableSchema]: ./DataTableSchema.html
