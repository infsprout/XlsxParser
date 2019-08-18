# XlsxRequest Class

[XlsxParser][] 생성자의 인자로 사용됩니다.<br>  
기본적으로 XLSX 파일의 URI를 가지고 있고 [XlsxParser][]가 XLSX 파일을 파싱할 때  
필요한 정보들을 가지고 있습니다.

**Syntax**

```csharp
public sealed class XlsxRequest
```

* * *
## Constructors

Name | Description
---- | -----------
[XlsxRequest(string)](#00) | XLSX 파일의 URI로 새 인스턴스를 초기화합니다.

* * *
## Operators
Name | Description
---- | -----------
[XlsxRequest(string)](#01) | XLSX 파일의 URI를 인스턴스로 변환합니다.

* * *
## Methods

Name | Description
---- | -----------
[ToString()                                               ](#02) | XLSX 파일의 URI를 반환합니다.
[SetWebRequestCreator(Func&lt;string, UnityWebRequest&gt;)](#03) | UnityWebRequest 생성 함수를 설정합니다.
[SetPassword(string)                                      ](#04) | XLSX 파일의 암호를 설정합니다.
[SetFieldTypeConverter(IFieldTypeConverter)               ](#05) | [IFieldTypeConverter][]의 구현체를 설정합니다.
[AddPdtsText(int, string)                                 ](#06) | 미리 정의된 [DataTableSchema][] 텍스트를 추가합니다.

* * *
## Extension Methods

Name | Description
---- | -----------
[SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)][&07] | XLSX 파일의 URI를 인스턴스로 변환하고 UnityWebRequest 생성 함수를 설정합니다.
[SetPassword(this string, string)                                      ][&08] | XLSX 파일의 URI를 인스턴스로 변환하고 암호를 설정합니다.
[SetFieldTypeConverter(this string, IFieldTypeConverter)               ][&09] | XLSX 파일의 URI를 인스턴스로 변환하고 [IFieldTypeConverter][]의 구현체를 설정합니다.
[AddPdtsText(this string, int, string)                                 ][&10] | XLSX 파일의 URI를 인스턴스로 변환하고 미리 정의된 [DataTableSchema][] 텍스트를 추가합니다.


<a name="00"><hr></a>
## XlsxRequest(string) Constructor
XLSX 파일의 URI로 새 인스턴스를 초기화합니다.

**Syntax**

```csharp
public XlsxRequest(
    string uri
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>
    XLSX 파일의 URI입니다. 만약 **null**값이면 *uri*는 빈 문자열로 설정됩니다.

<a name="01"><hr></a>
## XlsxRequest(string) Operator
XLSX 파일의 URI를 인스턴스로 변환합니다.

이 연산자로 인해 생성자로 직접 인스턴스를 생성할 일은 거의 없습니다.

**Syntax**

```csharp
public static implicit operator XlsxRequest(
    string uri
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>
    XLSX 파일의 URI입니다. 만약 **null**값이면 *uri*는 빈 문자열로 설정됩니다.

Return Value<br>
Type: XlsxRequest<br>


**Examples**

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser(
        "a.xlsx" // == new XlsxRequest("a.xlsx")
    );
    yield return parser.coroutine;
}
```    

<a name="02"><hr></a>
## ToString() Method
인스턴스를 생성할 때 사용한 URI를 반환합니다.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

<a name="03"><hr></a>
## SetWebRequestCreator(Func&lt;string, UnityWebRequest&gt;) Method
UnityWebRequest 생성 함수를 설정합니다.<br>
WWWForm을 사용할 일이 있으면 사용합니다.

**Syntax**

```csharp
public XlsxRequest SetWebRequestCreator(
    Func<string, UnityWebRequest> webRequestCreator
)
```

Parameters

1. *webRequestCreator*<br>
    Type: System.Func&lt;System.String, UnityEngine.Networking.UnityWebRequest&gt;<br>

Return Value<br>
Type: XlsxRequest<br>

**Examples**

```csharp
UnityWebRequest CreateWebRequest(string uri)
{
    var wwwForm = new WWWForm();
    // TODO your code
    return new UnityWebRequest(uri, wwwForm);
}

IEnumerator Start()
{
    var parser = new XlsxParser(
        new XlsxRequest("http://mysite.com/xlsx/a")
        .SetWebRequestCreator(CreateWebRequest)
    );
    yield return parser.coroutine;
}
```

확장 메서드 [SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)][&07]를 사용하면 다음과 같이 할 수 있습니다.

```csharp
...

IEnumerator Start()
{
    var parser = new XlsxParser(
        "http://mysite.com/xlsx/a".SetWebRequestCreator(CreateWebRequest)
    );
    yield return parser.coroutine;
}
```

**Remarks**

새로 생성되어 설정된 인스턴스를 반환합니다.

<a name="04"><hr></a>
## SetPassword(string) Method
XLSX 파일의 암호를 설정합니다.<br>
만약 XLSX 파일이 암호화 되어 있다면 해당 암호를 이용해 복호화를 시도합니다. 

**ECMA-376 Standard Encryption**, **ECMA-376 Agile Encryption** 둘 다 지원합니다. <br>


**Syntax**

```csharp
public XlsxRequest SetPassword(
    string password
)
```

Parameters

1. *password*<br>
    Type: System.String

Return Value<br>
Type: XlsxRequest<br>

**Remarks**

새로 생성되어 설정된 인스턴스를 반환합니다.


<a name="05"><hr></a>
## SetFieldTypeConverter(IFieldTypeConverter) Method

[XlsxParser][]가 [DataTable][] 을 생성할 때 사용할 [IFieldTypeConverter][]의 구현체를 설정합니다.

**Syntax**

```csharp
public XlsxRequest SetFieldTypeConverter(
    IFieldTypeConverter ftc
)
```

Parameters

1. *ftc*<br>
    Type: [DataTable.IFieldTypeConverter][IFieldTypeConverter]<br>

Return Value<br>
Type: XlsxRequest<br>


**Remarks**

새로 생성되어 설정된 인스턴스를 반환합니다.


<a name="06"><hr></a>
## AddPdtsText(int, string) Method

(pdts: Predefined DataTableSchema)<br>
미리 정의된 [DataTableSchema][] 텍스트를 추가합니다.

이 메서드에 의해 생성 된 [DataTableSchema.cellCommentRef][]는 셀 주소 'PREDEF1'로 시작하고 추가 된 순서에 따라 행의 주소가 1 씩 증가합니다.

코드에 정의 된 스키마의 셀 주소는 'PREDEF1'에서 시작하여 'PREDEF2', 'PREDEF3'순으로 증가합니다. 열 주소 'PREDEF'는 스프레드시트 툴의 열 제한보다 훨씬 큽니다. 따라서 이것을 사용하여 코드에 정의 된 스키마 또는 셀 주석에 정의 된 스키마를 구별 할 수 있습니다.

**Syntax**

```csharp
public XlsxRequest AddPdtsText(
    int sheetIndex,
    string schemaText,
)
```

Parameters

1. *sheetIndex*<br>
    Type: System.Int32<br>
    해당 XLSX 파일의 시트 인덱스 입니다.<br>
    가장 왼 쪽에 있는 시트의 인덱스는 0 입니다.

1. *schemaText*<br>
    Type: System.String<br>
    [DataTableSchema][] 텍스트 입니다.<br>
    시작과 끝부분의 공백문자는 제거됩니다.

Return Value<br>
Type: XlsxRequest<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *sheetIndex*가 0보다 작습니다.
ArgumentNullException       | *schemaText*가 **null**입니다. 

**Examples**

```csharp
IEnumerator Start()
{
    var sheetIndex = 0;
    var schemaText = @"
        [[ MyTable
            myField : string
        ]] D1
    ";
    var parser = new XlsxParser(
        "a.xlsx".AddPdtsText(sheetIndex, schemaText)
    );
    yield return parser.coroutine;
}
```

**Remarks**

새로 생성되어 설정된 인스턴스를 반환합니다.


* * *

[&07]: ./ExtensionMethods.html#03
[&08]: ./ExtensionMethods.html#04
[&09]: ./ExtensionMethods.html#05
[&10]: ./ExtensionMethods.html#06

[XlsxParser]:          ./XlsxParser.html
[IFieldTypeConverter]: ./DataTable.IFieldTypeConverter.html
[DataTableSchema]:     ./DataTableSchema.html
[DataTable]:           ./DataTable.html

[DataTableSchema.cellCommentRef]: ./DataTableSchema.html#04
