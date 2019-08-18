# ExtensionMethods Class

편의성을 위해 만들어진 확장 메소드들을 모아놓은 클래스입니다.

사용하기위해 **InfSprout.XlsxParser.ExtensionMethods** 네임스페이스를 선언해야 합니다.
 
**Syntax**

```csharp
public static class ExtensionMethods
```

* * *
## Methods

Name | Description
---- | -----------
[ToSingleLine(this string)                                                           ](#00) | 한 줄짜리로 변환된 문자열을 반환합니다.
[ToNumber(this string)                                                               ](#01) | 문자열을 double 타입으로 변환합니다.
[ToEnum&lt;T&gt;(this string)                                                        ](#02) | 문자열을 enum 타입으로 변환합니다.
[SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)              ](#03) | XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 UnityWebRequest 생성 함수를 설정합니다.
[SetPassword(this string, string)                                                    ](#04) | XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 암호를 설정합니다.
[SetFieldTypeConverter(this string, IFieldTypeConverter)                             ](#05) | XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 [IFieldTypeConverter][]의 구현체를 설정합니다.
[AddPdtsText(this string, int, string)                                               ](#06) | XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 미리 정의된 [DataTableSchema][] 텍스트를 추가합니다.
[SetDefaultWebRequestCreator(this XlsxRequest[], Func&lt;string, UnityWebRequest&gt;)](#07) | [XlsxRequest][]배열에 기본 UnityWebRequest 생성 함수를 설정합니다.
[SetDefaultPassword(this XlsxRequest[], string)                                      ](#08) | [XlsxRequest][]배열에 기본 암호를 설정합니다. 
[SetDefaultFieldTypeConverter(this XlsxRequest[], IFieldTypeConverter)               ](#09) | [XlsxRequest][]배열에 기본 [IFieldTypeConverter][]의 구현체를 설정합니다.

<a name="00"><hr></a>
## ToSingleLine(this string) Method

한 줄짜리로 변환된 문자열을 반환합니다.

'\r'이 @"\r"로 '\n'이 @"\n"으로 변환됩니다.

**Syntax**

```csharp
public static string ToSingleLine(
    this string src
)
```

Parameters

1. *src*<br>
    Type: System.String<br>
    
Return Value<br>
Type: System.String

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.	

<a name="01"><hr></a>
## ToNumber(this string) Method

문자열을 double 타입으로 변환합니다.

**Syntax**

```csharp
public static double ToNumber(
    this string src
)
```

Parameters

1. *src*<br>
    Type: System.String<br>

Return Value<br>
Type: System.Double

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.	
FormatException	      | *src*가 숫자 양식이 아닙니다.
OverflowException     |	*src*가 double의 표현범위를 넘어서는 경우

<a name="02"><hr></a>
## ToEnum&lt;T&gt;(this string) Method

문자열을 enum 타입으로 변환합니다.

**Syntax**
```csharp
public static T ToEnum<T>(
    this string src
)
```

Parameters

1. *src*<br>
    Type: System.String<br>

Return Value<br>
Type: T

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.
ArgumentException     | *T*의 타입이 enum이 아닙니다.
&nbsp;                | *src*가 빈 문자열입니다.
&nbsp;                | *src*가 공백문자만 포함한 경우
&nbsp;                | *src*가 이름형식인데 *T*에서 정의되지 않은 경우
OverflowException     | *src*가 숫자형식인데 *T*의 범위를 넘어선 경우 

<a name="03"><hr></a>
## SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;) Method

XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 UnityWebRequest 생성 함수를 설정합니다.

**Syntax**

```csharp
public static XlsxRequest SetWebRequestCreator(
    this string uri,
    Func<string, UnityWebRequest> WebRequestCreator
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>

1. *WebRequestCreator*<br>
    Type: System.Func&lt;System.String, UnityEngine.Networking.UnityWebRequest&gt;<br>

Return Value<br>
Type: [XlsxRequest][]<br>

<a name="04"><hr></a>
## SetPassword(this string, string) Method

XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 암호를 설정합니다.

**Syntax**

```csharp
public static XlsxRequest SetPassword(
    this string uri,
    string password
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>

1. *password*<br>
    Type: System.String<br>

Return Value<br>
Type: [XlsxRequest][]<br>

<a name="05"><hr></a>
## SetFieldTypeConverter(this string, IFieldTypeConverter) Method

XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 [IFieldTypeConverter][]의 구현체를 설정합니다.

**Syntax**

```csharp
public static XlsxRequest SetFieldTypeConverter(
    this string uri,
    IFieldTypeConverter ftc
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>

1. *ftc*<br>
    Type: [DataTable.IFieldTypeConverter][IFieldTypeConverter]<br>

Return Value<br>
Type: [XlsxRequest][]<br>

<a name="06"><hr></a>
## AddPdtsText(this string, int, string) Method

XLSX 파일의 URI를 [XlsxRequest][]로 변환하고 미리 정의된 [DataTableSchema][] 텍스트를 추가합니다.<br>

**Syntax**

```csharp
public static XlsxRequest AddPdtsText(
    this string uri,
    int sheetIndex,
    string schemaText
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>

1. *sheetIndex*<br>
    Type: System.Int32<br>
    해당 XLSX 파일의 시트 인덱스 입니다.<br>
    가장 왼 쪽에 있는 시트의 인덱스는 0 입니다.

1. *schemaText*<br>
    Type: System.String<br>
    [DataTableSchema][] 텍스트 입니다.<br>
    시작과 끝부분의 공백문자는 제거됩니다.

Return Value<br>
Type: [XlsxRequest][]<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *sheetIndex*가 0보다 작습니다.
ArgumentNullException       | *schemaText*가 **null**입니다. 

<a name="07"><hr></a>
## SetDefaultWebRequestCreator(this XlsxRequest[], Func&lt;string, UnityWebRequest&gt;) Method

[XlsxRequest][]배열에 기본 UnityWebRequest 생성 함수를 설정합니다.<br>

이미 **null**값이 아닌 UnityWebRequest 생성 함수가 설정되어 있는 [XlsxRequest][]에는 아무일도 하지 않습니다.

**Syntax**

```csharp
public static XlsxRequest[] SetDefaultWebRequestCreator(
    this XlsxRequest[] src,
    Func<string, UnityWebRequest> WebRequestCreator
)
```

Parameters

1. *src*<br>
    Type: [XlsxRequest][][]<br>

1. *WebRequestCreator*<br>
    Type: System.Func&lt;System.String, UnityEngine.UnityWebRequest&gt;<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.

<a name="08"><hr></a>
## SetDefaultPassword(this XlsxRequest[], string) Method

[XlsxRequest][]배열에 기본 암호를 설정합니다.<br>

이미 **null**값이 아닌 암호가 설정되어 있는 [XlsxRequest][]에는 아무일도 하지 않습니다. 

**Syntax**

```csharp
public static XlsxRequest[] SetDefaultPassword(
    this XlsxRequest[] src,
    string password
)

```

Parameters

1. *src*<br>
    Type: [XlsxRequest][][]<br>

1. *password*<br>
    Type: System.String<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.

* * *
## SetDefaultFieldTypeConverter(this XlsxRequest[], IFieldTypeConverter) Method

[XlsxRequest][]배열에 기본 [IFieldTypeConverter][]의 구현체를 설정합니다.<br>

이미 **null**값이 아닌 [IFieldTypeConverter][]가 설정되어 있는 [XlsxRequest][]에는 아무일도 하지 않습니다. 

**Syntax**

```csharp
public static XlsxRequest[] SetDefaultFieldTypeConverter(
    this XlsxRequest[] src,
    IFieldTypeConverter ftc
)
```

Parameters

1. *src*<br>
    Type: [XlsxRequest][][]<br>

1. *ftc*<br>
    Type: [DataTable.IFieldTypeConverter][IFieldTypeConverter]<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentNullException | *src*가 **null**입니다.

* * *

[XlsxRequest]:         ./XlsxRequest.html
[IFieldTypeConverter]: ./DataTable.IFieldTypeConverter.html
[DataTableSchema]:     ./DataTableSchema.html
