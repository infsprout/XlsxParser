# XlsxRequest Class

Used as an argument to the [XlsxParser][] constructor.

By default, it has the URI of the XLSX file and the information needed to parse the XLSX file by [XlsxParser][].

**Syntax**

```csharp
public sealed class XlsxRequest
```

* * *
## Constructors

Name | Description
---- | -----------
[XlsxRequest(string)](#00) | Initializes a new instance with the URI of the XLSX file.

* * *
## Operators
Name | Description
---- | -----------
[XlsxRequest(string)](#01) | Converts the URI of an XLSX file to an instance.

* * *
## Methods

Name | Description
---- | -----------
[ToString()                                               ](#02) | Returns the URI of the XLSX file.
[SetWebRequestCreator(Func&lt;string, UnityWebRequest&gt;)](#03) | Sets the UnityWebRequest creation function.
[SetPassword(string)                                      ](#04) | Sets the password for the XLSX file.
[SetFieldTypeConverter(IFieldTypeConverter)               ](#05) | Sets the implementation of [IFieldTypeConverter][].
[AddPdtsText(int, string)                                 ](#06) | Adds the predefined [DataTableSchema][] text.

* * *
## Extension Methods

Name | Description
---- | -----------
[SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)][&07] | Converts the URI of the XLSX file to an instance and sets the UnityWebRequest creation function.
[SetPassword(this string, string)                                      ][&08] | Converts the URI of the XLSX file to an instance and sets the password.
[SetFieldTypeConverter(this string, IFieldTypeConverter)               ][&09] | Converts the URI of the XLSX file to an instance and sets the implementation of [IFieldTypeConverter][].
[AddPdtsText(this string, int, string)                                 ][&10] | Converts the URI of the XLSX file to an instance and adds the predefined [DataTableSchema][] text.


<a name="00"><hr></a>
## XlsxRequest(string) Constructor
Initializes a new instance with the URI of the XLSX file.

**Syntax**

```csharp
public XlsxRequest(
    string uri
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>
    The URI of the XLSX file. If the value is **null**, *uri* is set to an empty string.

<a name="01"><hr></a>
## XlsxRequest(string) Operator

Converts the URI of an XLSX file to an instance.

Because this operator, you don't need create instances directly through constructor.

**Syntax**

```csharp
public static implicit operator XlsxRequest(
    string uri
)
```

Parameters

1. *uri*<br>
    Type: System.String<br>
    The URI of the XLSX file. If the value is **null**, *uri* is set to an empty string.

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
Returns the URI of the XLSX file.

**Syntax**

```csharp
public override string ToString()
```

Return Value<br>
Type: System.String

<a name="03"><hr></a>
## SetWebRequestCreator(Func&lt;string, UnityWebRequest&gt;) Method

Sets the UnityWebRequest creation function.

Use if **WWWForm** need to use.

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

Using extension method [SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)][&07], you can do the following:

```csharp
...

IEnumerator Start()
{
    var parser = new XlsxParser(
        "http://mysite.com/xlsx/a".SetWWWCreator(CreateWWW)
    );
    yield return parser.coroutine;
}
```

**Remarks**

Returns the instance that newly created and set.

<a name="04"><hr></a>
## SetPassword(string) Method

Sets the password for the XLSX file.

If the XLSX file is encrypted, it will attempt to decrypt it using that password.

Supports both **ECMA-376 Standard Encryption** and **ECMA-376 Agile Encryption**.


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

Returns the instance that newly created and set.


<a name="05"><hr></a>
## SetFieldTypeConverter(IFieldTypeConverter) Method

Sets the implementation of [IFieldTypeConverter][].

[IFieldTypeConverter][] is used when [XlsxParser][] creates [DataTable][].


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

Returns the instance that newly created and set.


<a name="06"><hr></a>
## AddPdtsText(int, string) Method

(pdts: Predefined DataTableSchema)<br>
Adds the predefined [DataTableSchema][] text.

[DataTableSchema.cellCommentRef][] generated by this method starts with cell address 'PREDEF1', and the address of the row is incremented by 1 according to the added order.

The cell address of the schema defined in the code starts from 'PREDEF1' and increase in order of 'PREDEF2', 'PREDEF3'.
The column address 'PREDEF' is well greater the column limit of the spreadsheet tool.
So you can use this to distinguish between the schema defined in code or the schema defined in cell comment.

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
    The sheet index of the specified XLSX file.<br>
    The index of the leftmost sheet is zero.

1. *schemaText*<br>
    Type: System.String<br>
    The [DataTableSchema][] text.<br>
    The leading and trailing whitespace characters are removed.

Return Value<br>
Type: XlsxRequest<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *sheetIndex* is less than 0.
ArgumentNullException       | *schemaText* is **null**. 

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

Returns the instance that newly created and set.


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
