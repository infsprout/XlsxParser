# Changelog

* * *
## 1.2.0 (2019-08-19)

### Changes
- Released source code under MIT license.

- Changed namespace 'InfSprout.XlsxParser' to 'XlsxParser'.

- Changed folder 'InfSprout/XlsxParser' to 'XlsxParser'.

- Moved 'ExtensionMethods' class to 'XlsxParser' namespace.

- Changed HTTP API to use UnityWebRequest instead WWW.

* * *
## 1.1.3 (2017-05-08)

### Improvements
- Supported all XLSX file decryption algorithms except RC2, RC4, DES, DESX that are not recommended by the standard.

* * *
## 1.1.2 (2017-03-31)

### Changes
- Changed name of **DataTable.PropertyMapping** class to **DataTable.PropertyMappingAttribute**.

- 'Compression' folder moved to 'Internal/Compression'.<br>
Also, the access modifier for all classes in the folder changed to **internal**.

- **public** classes and methods that do not need to be used directly by users changed to **internal**.

* * *
## 1.1.1 (2017-03-15)

### Features
- The concept and functionality of **PdtsText** (Predefined DataTableSchema Text) added for define the schema within the code.<br> 


* * *
## 1.1.0 (2017-03-12)

### Features
- Added XLSX file decryption feature.<br>
Supports Both **ECMA-376 Standard Encryption** and **ECMA-376 Agile Encryption**.

### Ease-of-use Improvements

- Added extension methods for **XlsxRequest** class.

- **IFieldTypeConverter.FromString()** method changed to pass the error to **DataTable** class via an exception rather than a string.
