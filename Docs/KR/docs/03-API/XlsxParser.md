# XlsxParser Class

XLSX파일들을 파싱하여 [DataSet][]을 생성해냅니다.



**Syntax**

```csharp
public sealed class XlsxParser : IDisposable
```

* * *
## Constructors

Name | Description
---- | -----------
[XlsxParser(params XlsxRequest[])](#00) | [XlsxRequest][]들로 새 인스턴스를 초기화하고 파싱을 진행합니다.

* * *
## Properties

Name | Description
---- | -----------
[requests      ](#01) | 인스턴스를 생성할 때 사용되었던 인자들을 가져옵니다.
[progress      ](#02) | 파싱의 진행 상황을 가져옵니다.
[isDone        ](#03) | 파싱의 완료 여부를 가져옵니다.
[erroredSchemas](#04) | 파싱중에 에러가 발생하여 [DataTable][]을 생성하지 못한 [DataTableSchema][]들을 가져옵니다.
[errors        ](#05) | 파싱중에 발생한 모든 에러들을 가져옵니다.
[dataSet       ](#06) | 파싱을 통해 생성된 [DataSet][]을 가져옵니다.
[coroutine     ](#07) | 파싱을 진행하는 메인 코루틴을 가져옵니다.
[startTime     ](#08) | 파싱을 시작한 시간을 가져옵니다.
[elapsedTime   ](#09) | 파싱이 진행된 시간을 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[IsDuplicatedSchema(DataTableSchema)](#10) | 지정된 [DataTableSchema][]가 중복되었는지 여부를 반환합니다.
[GetOriginalSchema(DataTableSchema) ](#11) | 지정된 [DataTableSchema][]의 이름으로 가장 먼저 생성된 [DataTableSchema][]를 반환합니다.
[Dispose()                          ](#12) | 파싱 과정 중에 사용된 리소스들은 해제합니다.


<a name="00"><hr></a>
## XlsxParser(params XlsxRequest[]) Constructor

[XlsxRequest][]들로 새 인스턴스를 초기화하고 파싱을 진행합니다.

**Syntax**

```csharp
public XlsxParser(
    params XlsxRequest[] requests
)
```

<a name="01"><hr></a>
## requests Property

인스턴스를 생성할 때 사용되었던 인자들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<XlsxRequest> requests { get; private set; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[XlsxRequest][]&gt;

<a name="02"><hr></a>
## progress Property

파싱의 진행 상황을 가져옵니다.

**Syntax**

```csharp
public float progress { get; private set; }
```

Property Value<br>
Type: System.Single

<a name="03"><hr></a>
## isDone Property

파싱의 완료 여부를 가져옵니다.

`progress == 1`과 같습니다.

**Syntax**

```csharp
public bool isDone { get; }
```

Property Value<br>
Type: System.Boolean

<a name="04"><hr></a>
## erroredSchemas Property

파싱중에 에러가 발생하여 [DataTable][]을 생성하지 못한 [DataTableSchema][]들을 가져옵니다.

**Syntax**

```csharp
public ReadOnlyCollection<DataTableSchema> erroredSchemas { get; }
```

Property Value<br>
Type: System.Collections.ObjectModel.ReadOnlyCollection&lt;[DataTableSchema]&gt;

**Remarks**

중복문제만 있어서 erroredSchemas에 포함된 [DataTableSchema][]는 에러를 가지고 있지 않습니다.

<a name="05"><hr></a>
## errors Property

파싱중에 발생한 모든 에러들을 가져옵니다.

**Syntax**

```csharp
public Errors errors { get; }
```

Property Value<br>
Type: [XlsxParser.Errors][]

<a name="06"><hr></a>
## dataSet Property

파싱을 통해 생성된 [DataSet][]을 가져옵니다.

**Syntax**

```csharp
public DataSet dataSet { get; private set; }
```

Property Value<br>
Type: [DataSet][]

<a name="07"><hr></a>
## coroutine Property

파싱을 진행하는 메인 코루틴을 가져옵니다.

이를 이용해 코루틴 내에서 XlsxParser의 파싱의 완료될 때 까지 기다릴 수 있습니다.

**Syntax**

```csharp
public Coroutine coroutine { get; private set; }
```

Property Value<br>
Type: UnityEngine.Coroutine

**Examples**

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser("a.xlsx");
    yield return parser.coroutine;
}
```

<a name="08"><hr></a>
## startTime Property

파싱을 시작한 시간을 가져옵니다.

게임의 시작된 후의 초단위의 시간입니다.

**Syntax**

```csharp
public float startTime { get; private set; }
```

Property Value<br>
Type: System.Single

<a name="09"><hr></a>
## elapsedTime Property

파싱이 진행된 시간을 가져옵니다.

초단위의 시간입니다.

파싱의 완료된 이후로는 증가되지 않습니다.

**Syntax**

```csharp
public float elapsedTime { get; }
```

Property Value<br>
Type: System.Single

<a name="10"><hr></a>
## IsDuplicatedSchema(DataTableSchema) Method

지정된 [DataTableSchema][]가 중복되었는지 여부를 반환합니다.

**Syntax**

```csharp
public IsDuplicatedSchema(
    DataTableSchema schema
)
```

Parameters

1. *schema*<br>
    Type: [DataTableSchema][]

Return Value<br>
Type: System.Boolean    

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *schema*가 **null**입니다.

**Remarks**

같은 파서에서 만들어진 스키마가 인자로 들어와야 정상 작동 합니다.

<a name="11"><hr></a>
## GetOriginalSchema(DataTableSchema) Method

지정된 [DataTableSchema][]의 이름으로 가장 먼저 생성된 [DataTableSchema][]를 반환합니다.

**Syntax**

```csharp
public DataTableSchema GetOriginalSchema(
    DataTableSchema schema
)
```
Parameters

1. *schema*<br>
    Type: [DataTableSchema][]

Return Value<br>
Type: [DataTableSchema][]    

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *schema*가 **null**입니다.

**Remarks**

같은 파서에서 만들어진 스키마가 인자로 들어와야 정상 작동 합니다.

<a name="12"><hr></a>
## Dispose() Method

파싱 과정 중에 사용된 리소스들은 해제합니다.

파싱 완료 후에 자동 호출되어 using block을 사용할 필요가 없습니다.

파싱 중에는 호출해도 아무일도 하지 않습니다.

**Syntax**

```csharp
public void Dispose()
```


* * *

[DataSet]:           ./DataSet.html
[XlsxRequest]:       ./XlsxRequest.html
[DataTable]:         ./DataTable.html
[DataTableSchema]:   ./DataTableSchema.html
[XlsxParser.Errors]: ./XlsxParser.Errors.html
