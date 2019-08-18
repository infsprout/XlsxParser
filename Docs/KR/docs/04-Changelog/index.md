# Changelog

* * *
## 1.2.0 (2019-08-19)

### Changes
- MIT 라이센스하에 소스 코드를 공개했습니다.

- 네임 스페이스 'InfSprout.XlsxParser'가 'XlsxParser'로 변경되었습니다.

- 'InfSprout / XlsxParser'폴더를 'XlsxParser'로 변경했습니다.

- 'ExtensionMethods'클래스를 'XlsxParser'네임 스페이스로 이동했습니다.

- WWW 대신 UnityWebRequest를 사용하도록 HTTP API를 변경했습니다.

* * *
## 1.1.3 (2017-05-08)

### Improvements
- 표준에서 권장하지 않는 RC2, RC4, DES, DESX을 제외한 모든 XLSX파일 복호화 관련 알고리즘들을 지원합니다.

* * *
## 1.1.2 (2017-03-31)

### Changes
- **DataTable.PropertyMapping** 클래스의 이름이 **DataTable.PropertyMappingAttribute**로 바뀌었습니다.

- 'Compression' 폴더가 'Internal/Compression'로 이동하였습니다.<br>
또한 폴더안에 있는 모든 클래스의 접근 한정자가 **internal**로 변경되었습니다.

- 사용자가 직접 사용할 필요가 없는 **public** 클래스들과 메소드들의 접근 한정자가 **internal**로 변경되었습니다.

* * *
## 1.1.1 (2017-03-15)

### Features
- 코드내에서 스키마를 정의하기 위해 **PdtsText** (Predefined DataTableSchema Text) 개념과 기능이 추가되었습니다.<br> 


* * *
## 1.1.0 (2017-03-12)

### Features
- XLSX 파일의 복호화 기능이 추가되었습니다.<br>
**ECMA-376 Standard Encryption**과 **ECMA-376 Agile Encryption** 둘 다 지원합니다.  

### Ease-of-use Improvements

- **XlsxRequest** 클래스를 위한 확장 메소드들을 추가하였습니다.

- **IFieldTypeConverter.FromString()** 메소드가 에러를 문자열이 아니라 예외를 통해 **DataTable** 클래스에 전달하도록 변경되었습니다.
