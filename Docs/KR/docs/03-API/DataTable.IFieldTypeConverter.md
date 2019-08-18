# DataTable.IFieldTypeConverter Interface

사용자가 정의한 필드 타입으로 변환해 줍니다.

사용자가 정의한 필드 타입은 반드시 ValueType(enum, struct)이어야 합니다.

**Syntax**

```csharp
public interface IFieldTypeConverter
```

* * *
## Methods

Name | Description
---- | -----------
[FromString(string, string)](#00) | 문자열을 지정된 타입으로 변환합니다.
[ToString(string, object)  ](#01) | 지정된 타입을 문자열로 변환합니다. 

<a name="00"><hr></a>
## FromString(string, string) Method

문자열을 지정된 타입으로 변환합니다.

[XlsxParser][]가 [DataTable][]을 생성 하는 과정에서 호출됩니다.

**Syntax**

```csharp
public object FromString(
    string type,
    string src
)
```

Parameters

1. *type*<br>
    Type: System.String<br>
    사용자가 정의한 타입 이름.

1. *src*<br>
    Type: System.String<br>
    아직 변환되지 않은 문자열 타입의 셀 값

Return Value<br>
Type: String.Object<br>
사용자가 정의한 타입으로 변환되어 있는 셀 값<br>
**null**을 반환하는 경우 [DataTable][]에 [Error][]가 추가됩니다.  

**Remarks**

이 메소드 내에서 예외가 발생해도 [XlsxParser][]의 파싱 과정이 중단되지 않습니다.<br>
예외는 [Error][]로 변환되어 [DataTable][]에 추가됩니다. 

<a name="01"><hr></a>
## ToString(string, object) Method

지정된 타입을 문자열로 변환합니다. 

[DataTable.GetCellValue&lt;T&gt;(int, int)][] 메소드에서 
T가 string일 때 호출됩니다.

**Syntax**

```csharp
public string ToString(
    string type,
    object src
)
```

1. *type*<br>
    Type: System.String<br>
    사용자가 정의한 타입 이름.

1. *src*<br>
    Type: System.Object<br>
    사용자가 정의한 타입으로 변환되어 있는 셀 값

Return Value<br>
Type: System.String<br>

* * *

[XlsxParser]: ./XlsxParser.html
[DataTable]:  ./DataTable.html
[Error]:      ./DataTable.Error.html
[DataTable.GetCellValue&lt;T&gt;(int, int)]: ./DataTable.html#08
