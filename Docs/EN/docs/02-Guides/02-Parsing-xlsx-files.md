# Parsing XLSX Files

It is very simple to parse XLSX files using [XlsxParser][].

Usage is very similar to **UnityEngine.WWW**.

Here is the code to parse several XLSX files at once.<br>
**NOTE**: The URI of the XLSX file in the code does not actually exist.

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
[XlsxParser][] constructor parameters are originally [XlsxRequest][], but are implicitly converted when **string** is inserted.

If you parse multiple XLSX files at the same time, the tables defined within the XLSX file are considered to be in the same namespace. So if you have a table with the same name, you will get a duplicate error.

[XlsxParser][] does not stop or raise an exception if an error occurs.
For this reason, the user should check the success of parsing through [XlsxParser.errors][].

In addition to **http**, **https**, **file** in [XlsxParser][], the following URI schemes are provided.

Scheme | Descryption
------ | -----------
res   | Loads the XLSX file using **UnityEngine.Resources.LoadAsync()**. 
data  | Loads the XLSX file using **System.IO.FileStream** based on **UnityEngine.Application.dataPath**.
pdata | Loads the XLSX file using **System.IO.FileStream** based on **UnityEngine.Application.persistentDataPath**.

If there is no URI Scheme, uses **System.IO.FileStream** to load the XLSX file.

[XlsxParser][] provides the decryption feature.<br>
Here's how to load an encrypted XLSX file from the spreadsheet tool.

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser(
        "Assets/1.xlsx".SetPassword("pw1234")
    );
    yield return parser.coroutine;
}
```
Use [XlsxRequest.SetPassword(string)][].

* * *

[XlsxParser]:        ../03-API/XlsxParser.html
[XlsxParser.errors]: ../03-API/XlsxParser.html#05

[XlsxRequest]:                     ../03-API/XlsxRequest.html
[XlsxRequest.SetPassword(string)]: ../03-API/XlsxRequest.html#04
