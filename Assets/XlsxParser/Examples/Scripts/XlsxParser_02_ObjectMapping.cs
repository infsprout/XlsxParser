using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XlsxParser.Example
{

    public class XlsxParser_02_ObjectMapping : MonoBehaviour
    {
        public class FieldTypeConverter : DataTable.IFieldTypeConverter
        {
            public object FromString(string type, string src)
            {
                switch (type) {
                case "ShapeType": return src.ToEnum<Object3D.ShapeType>();
                case "MaterialType": return src.ToEnum<Object3D.MaterialType>();
                case "TextAnchor": return src.ToEnum<TextAnchor>();
                }
                return null;
            }

            public string ToString(string type, object src)
            {
                return src.ToString();
            }
        }

        private Dictionary<string, Object3D> _myObjects;

        IEnumerator Start()
        {
            var ftc = new FieldTypeConverter();
            var parser = new XlsxParser(
                "res://XlsxParser_02".SetFieldTypeConverter(ftc)
            );
            yield return parser.coroutine;
            if (parser.errors.count > 0) {
                Debug.LogWarning(parser.errors);
                yield break;
            }
            var table = parser.dataSet["Object3D"];
            DataTable.Warnings warnings = null;
            warnings = table.GetObjectMappingWarnings<Object3D>();
            if (warnings.count > 0) {
                Debug.LogWarning(warnings);
            }
            _myObjects = new Dictionary<string, Object3D>();
            var n = 0;
            // If key field is duplicated, will be overwritten.
            // To ensure key unique, use data validation in xlsx editor.
            // If keyFieldIndex is negative, key is set row order.
            warnings = table.Populate(_myObjects, () => {
                var inst = Object3D.Create();
                var x = 3.0F * (n % 3);
                var z = 3.0F * (n / 3);
                ++n;
                inst.transform.position = new Vector3(x, 3, z);
                return inst;
            }, 0);
            if (warnings.count > 0) {
                Debug.LogWarning(warnings);
            }
        }
    
    }

}