# Populating data from DataTable

To use table data properly, you need to represent it in object form.

The following example populates data to the collection of objects specified by [DataTable][].

```csharp
IEnumerator Start()
{
    var parser = new XlsxParser("a.xlsx");
    yield return parser.coroutine;
    var table = parser.dataSet["MyTable"];
    var dict = new Dictionary<string, MyClass>();
    var w1 = table.GetObjectMappingWarnings<MyClass>();
    var w2 = table.Populate(dict);
    Debug.Log("dict.Count:" + dict.Count);
}
```

The code is simple but there are a few things you need to know about the data population process.

The first is [DataTable][] not converts data to an object but populates data to an object.
[DataTable][] calls the creation function only when there is no instance when populating the object with data.
This means that the data table can also be part of an object.

The second is that the data population process is not interrupted and warnings are sent as a result.
The variables *w1*, *w2* in the code are warnings that occur during the data population.
The difference is that *w1* are warnings found through static analysis of [PropertyMappingAttribute][] and related objects, and *w2* are warnings that is actually found during object creation and data population. The warning message is detailed, so if a warning occurs, you will easily find the cause.

Finally, it is a way to change the name of the property to be populated with data that using [PropertyMappingAttribute][].

The following is a part of the **Object3D.cs** file in the example.

```csharp
using PM = DataTable.PropertyMappingAttribute;
[PM("shape.t"   , "transform")]
[PM("shape.t.s" , "localScale")]
[PM("shape.t.r" , "localEulerAngles")]
[PM("text"      , "textMesh")]
[PM("text.value", "text")]
[PM("text.t"    , "transform")]
[PM("text.t.p"  , "localPosition")]
public class Object3D : MonoBehaviour
{
                   ...
    
    public string id { get; private set; }
    public ShapeType shapeType { get; private set; }
    public MaterialType materialType { get; private set; }
    public MeshRenderer shape { get; private set; }
    public TextMesh textMesh { get; private set; }
    public float hoveringSeed { get; private set; }

                   ...
```

The schema of the table that populates data to the above object is as follows.
![picture-2](../images/picture-2.png)

Let the first parameter of [PropertyMappingAttribute][] is *source* and the second parameter is *target*.

*source* represents of the name of [FieldNameNode][], including the name of the parent node.

*target* is a member or property of an object with the same depth as *source*. but it is not included the name of the parent member(member declarer).

*source* of the first attribute that associated with **Object3D** is mapped to *target* "transform". Also it is applied to "shape.t.s", "shape.t.r" that are *source* of other attributes.

Actually, when data is populated to **Object3D**, the value of "shape.t.s.x" with the 4th [Field][] will be populated to **Object3D.shape.transform.localScale.x**.

If *source* has the same attributes, only the attributes that are associated at the last are activated and the remaining attributes are ignored. Attributes associated to the parent class are attributes that are associated before attributes in the child.


* * *

Please see the example **XlsxParser_02_ObjectMapping** for more information.

* * *

[DataTable]:                ../03-API/DataTable.html
[PropertyMappingAttribute]: ../03-API/DataTable.PropertyMappingAttribute.html
[FieldNameNode]:            ../03-API/DataTableSchema.FieldNameNode.html
[Field]:                    ../03-API/DataTableSchema.Field.html
