# DataTable.Warnings Class

Represents warnings that occurred during the process of populating the [DataTable][] with the target object.

**Syntax**

```csharp
public sealed class Warnings : ReturnMessages
```

* * *
## Properties

Name | Description
---- | -----------
[count      ][] | The count of messages.
[Item\[int\]][] | Gets the message at the specified index.

* * *
## Methods

Name | Description
---- | -----------
[GetEnumerator()][] | Returns an enumerator that iterates a message.
[ToString()     ][] | Returns the total message that is merged.

* * *

[count]:           ./ReturnMessages.html#00
[Item\[int\]]:     ./ReturnMessages.html#01
[GetEnumerator()]: ./ReturnMessages.html#02
[ToString()]:      ./ReturnMessages.html#03

[DataTable]: ./DataTable.html
