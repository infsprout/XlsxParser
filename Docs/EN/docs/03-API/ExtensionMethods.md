# ExtensionMethods Class

This class is a collection of extension methods created for convenience.

To use it, you need to declare the **InfSprout.XlsxParser.ExtensionMethods** namespace.
 
**Syntax**

```csharp
public static class ExtensionMethods
```

* * *
## Methods

Name | Description
---- | -----------
[ToSingleLine(this string)                                                             ](#00) | Returns the string that converted to single line.
[ToNumber(this string)                                                                 ](#01) | Converts a string to a double type.
[ToEnum&lt;T&gt;(this string)                                                          ](#02) | Converts a string to an enum type.
[SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;)                ](#03) | Converts the URI of the XLSX file to [XlsxRequest][] and sets the UnityWebRequest creation function.
[SetPassword(this string, string)                                                      ](#04) | Converts the URI of the XLSX file to [XlsxRequest][] and sets the password.
[SetFieldTypeConverter(this string, IFieldTypeConverter)                               ](#05) | Converts the URI of the XLSX file to [XlsxRequest][] and sets the [IFieldTypeConverter][] implementation.
[AddPdtsText(this string, int, string)                                                 ](#06) | Converts the URI of the XLSX file to [XlsxRequest][] and adds predefined [DataTableSchema][] text.
[SetDefaultWebRequestCreator(this XlsxRequest\[\], Func&lt;string, UnityWebRequest&gt;)](#07) | Sets the default UnityWebRequest creation function in the [XlsxRequest][] array.
[SetDefaultPassword(this XlsxRequest\[\], string)                                      ](#08) | Sets the default password in the [XlsxRequest][] array.
[SetDefaultFieldTypeConverter(this XlsxRequest\[\], IFieldTypeConverter)               ](#09) | Sets the default [IFieldTypeConverter][] implementation in the [XlsxRequest][] array.


<a name="00"><hr></a>
## ToSingleLine(this string) Method

Returns the string that converted to single line.

'\r' is converted to @"\r" and '\n' to @"\n".

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
ArgumentNullException | *src* is **null**.	

<a name="01"><hr></a>
## ToNumber(this string) Method

Converts a string to a double type.

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
ArgumentNullException | *src* is **null**.	
FormatException	      | *src* is not number format.
OverflowException     |	*src* exceeds range of double type. 

<a name="02"><hr></a>
## ToEnum&lt;T&gt;(this string) Method

Converts a string to an enum type.

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
ArgumentNullException | *src* is **null**.
ArgumentException     | *T* is not enum.
&nbsp;                | *src* is empty.
&nbsp;                | *src* only include whitespace.
&nbsp;                | *src* is name format but is not defined in *T*
OverflowException     | *src* is number format but exceeds range of *T*. 

<a name="03"><hr></a>
## SetWebRequestCreator(this string, Func&lt;string, UnityWebRequest&gt;) Method

Converts the URI of the XLSX file to [XlsxRequest][] and sets the UnityWebRequest creation function.

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

Converts the URI of the XLSX file to [XlsxRequest][] and sets the password.

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

Converts the URI of the XLSX file to [XlsxRequest][] and sets the [IFieldTypeConverter][] implementation.

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

Converts the URI of the XLSX file to [XlsxRequest][] and adds predefined [DataTableSchema][] text.

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
    The sheet index of the specified XLSX file.<br>
    The index of the leftmost sheet is zero.

1. *schemaText*<br>
    Type: System.String<br>
    The [DataTableSchema][] text.<br>
    The leading and trailing whitespace characters are removed.

Return Value<br>
Type: [XlsxRequest][]<br>

**Exceptions**

Exception | Condition
--------- | ---------
ArgumentOutOfRangeException | *sheetIndex* is less than 0.
ArgumentNullException       | *schemaText* is **null**. 

<a name="07"><hr></a>
## SetDefaultWebRequestCreator(this XlsxRequest[], Func&lt;string, UnityWebRequest&gt;) Method

Sets the default WebRequest creation function in the [XlsxRequest][] array.

Does nothing to [XlsxRequest][] where the WebRequest creation function is already set to a value other than **null**.

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
ArgumentNullException | *src* is **null**.

<a name="08"><hr></a>
## SetDefaultPassword(this XlsxRequest[], string) Method

Sets the default password in the [XlsxRequest][] array.

Does nothing to [XlsxRequest][] where password is already set to a value other than **null**.

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
ArgumentNullException | *src* is **null**.

<a name="09"><hr></a>
## SetDefaultFieldTypeConverter(this XlsxRequest[], IFieldTypeConverter) Method

Sets the default [IFieldTypeConverter][] implementation in the [XlsxRequest][] array.

Does nothing to [XlsxRequest][] where [IFieldTypeConverter][] is already set to a value other than **null**. 

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
ArgumentNullException | *src* is **null**.

* * *

[XlsxRequest]:         ./XlsxRequest.html
[IFieldTypeConverter]: ./DataTable.IFieldTypeConverter.html
[DataTableSchema]:     ./DataTableSchema.html
