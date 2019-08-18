# XlsxParser.Errors Class

[XlsxParser][]가 파싱 과정중에 발생한 에러들을 나타냅니다.

**Syntax**

```csharp
public sealed class Errors : ReturnMessages
```

* * *
## Properties

Name | Description
---- | -----------
[count      ][] | 메시지의 개수 입니다.
[Item\[int\]][] | 지정한 인덱스에 있는 메시지를 가져옵니다.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()][] | 메시지를 반복하는 열거자를 반환합니다.
[ToString()     ][] | 합쳐져 있는 전체 메시지를 반환합니다.

* * *

[count]:           ./ReturnMessages.html#00
[Item\[int\]]:     ./ReturnMessages.html#01
[GetEnumerator()]: ./ReturnMessages.html#02
[ToString()]:      ./ReturnMessages.html#03

[XlsxParser]: ./XlsxParser.html
