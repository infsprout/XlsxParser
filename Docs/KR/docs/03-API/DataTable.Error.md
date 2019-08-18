# DataTable.Error

[DataTable][] 생성 중에 생긴 에러를 나타냅니다.

**Syntax**

```csharp
public sealed class Error
```

* * *
## Properties

Name | Description
---- | -----------
[cellRef](#00) | 에러가 발생한 셀주소를 가져옵니다.
[message](#01) | 에러 메시지를 가져옵니다.

<a name="00"><hr></a>
## cellRef Property

에러가 발생한 셀주소를 가져옵니다.

**Syntax**

```csharp
public CellRef cellRef { get; private set; }
```

Property Value<br>
Type: [CellRef][]


<a name="01"><hr></a>
## message Property

에러 메시지를 가져옵니다.

반드시 1줄인 것을 보장합니다.

**Syntax**

```csharp
public string message { get; private set; }
```

Property Value<br>
Type: System.String

* * *

[DataTable]: ./DataTable.html
[CellRef]:   ./CellRef.html
