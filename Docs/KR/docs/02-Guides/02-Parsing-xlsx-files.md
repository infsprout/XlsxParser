# Parsing XLSX Files

[XlsxParser][]를 사용해 XLSX 파일들을 파싱하는 것은 매우 간단합니다.

**UnityEngine.WWW**와 사용법이 매우 유사합니다.

다음은 여러개의 XLSX파일을 한번에 파싱하는 코드입니다.<br>
**주의**: 코드안에 있는 XLSX파일의 URI는 실제로 존재하지 않습니다.

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser(
        "Assets/1.xlsx",
        "http://mysite.com/xlsx/2",
        "https://mysite.com/xlsx/3",
        "res://4",
        "data://5.xlsx",
        "pdata://6.xlsx" 
    );
    yield return parser.coroutine;
    foreach (var error in parser.errors) {
        Debug.LogWarning(error);
    }
    Debug.Log("table count:" + parser.dataSet.tableCount);
}
```
[XlsxParser][]의 생성자의 파라미터는 원래[XlsxRequest][]이지만 **string**을 넣으면 암묵적으로 변환됩니다.

여러개의 XLSX파일을 동시에 파싱하는 경우 XLSX파일 내에 정의된 테이블들은 같은 네임스페이스에 있는것으로 간주합니다. 그래서 이름이 같은 테이블이 있는 경우 중복 에러가 발생하게 됩니다.

[XlsxParser][]는 에러가 발생해도 중단되거나 예외가 발생하지 않습니다.
그렇기 때문에 사용자는 파싱의 성공 여부를 [XlsxParser.errors][]에서 꼭 확인 하셔야 합니다.

[XlsxParser][]에서 **http**, **https**, **file**외에 추가적으로 제공하는 URI Scheme은 다음과 같습니다.


Scheme | Descryption
------ | -----------
res   | **UnityEngine.Resources.LoadAsync()**<b></b>를 이용해 XLSX파일을 불러옵니다. 
data  | **UnityEngine.Application.dataPath**를 기준으로 **System.IO.FileStream**을 이용해 XLSX파일을 불러옵니다.
pdata | **UnityEngine.Application.persistentDataPath**를 기준으로 **System.IO.FileStream**을 이용해 XLSX파일을 불러옵니다.

URI Scheme이 없는 경우 **System.IO.FileStream**를 이용해 XLSX파일을 불러옵니다.

[XlsxParser][]는 복호화 기능을 제공합니다.<br>
스프레드시트 툴에서 암호화해서 저장한 XLSX파일을 불러오는 방법은 다음과 같습니다.

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser(
        "Assets/1.xlsx".SetPassword("pw1234")
    );
    yield return parser.coroutine;
}
```
[XlsxRequest.SetPassword(string)][]를 이용합니다.

* * *

[XlsxParser]:        ../03-API/XlsxParser.html
[XlsxParser.errors]: ../03-API/XlsxParser.html#05

[XlsxRequest]:                     ../03-API/XlsxRequest.html
[XlsxRequest.SetPassword(string)]: ../03-API/XlsxRequest.html#04
