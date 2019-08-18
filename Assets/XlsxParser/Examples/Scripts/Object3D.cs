using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XlsxParser.Example
{

    using PM = DataTable.PropertyMappingAttribute;

    // NOTE: You cannot change object hierarchy or array index.
    [PM("shape.t", "transform")]
    [PM("shape.t.s", "localScale")]
    [PM("shape.t.r", "localEulerAngles")]
    [PM("text", "textMesh")]
    [PM("text.value", "text")]
    [PM("text.t", "transform")]
    [PM("text.t.p", "localPosition")]
    public class Object3D : MonoBehaviour
    {
        public enum ShapeType
        {
            Sphere = PrimitiveType.Sphere,
            Capsule = PrimitiveType.Capsule,
            Cylinder = PrimitiveType.Cylinder,
            Cube = PrimitiveType.Cube,
        }

        public enum MaterialType
        {
            White, Red, Green, Blue, Pink,
        }

        public string id { get; private set; }

        public ShapeType shapeType { get; private set; }
        public MaterialType materialType { get; private set; }
        public MeshRenderer shape { get; private set; }
        public TextMesh textMesh { get; private set; }
        public float hoveringSeed { get; private set; }

        void Update()
        {
            if (!string.IsNullOrEmpty(id) && id != name) {
                name = id;
            }
            _UpdateShape();
            _UpdateMaterial();
            var lookAtPoint = textMesh.transform.position;
            lookAtPoint += Camera.main.transform.forward;
            textMesh.transform.LookAt(lookAtPoint);
            var hovering = (Time.realtimeSinceStartup - hoveringSeed) % 2;
            var pos = shape.transform.position;
            pos.y = 0.5F * Mathf.Sin(Mathf.PI * hovering);
            shape.transform.position = pos;
        }

        public static Object3D Create()
        {
            var go = new GameObject();
            var cp = go.AddComponent<Object3D>();
            cp._AddTextMesh();
            cp._UpdateShape();
            cp._UpdateMaterial();
            cp.hoveringSeed = Random.Range(0.0F, 2.0F);
            return cp;
        }

        private void _AddTextMesh()
        {
            if (textMesh) {
                return;
            }
            var go = new GameObject();
            go.name = "_textMesh";
            go.transform.position = new Vector3(0, 1, 0);
            var mr = go.AddComponent<MeshRenderer>();
            var tm = go.AddComponent<TextMesh>();
            tm.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            mr.material = tm.font.material;
            tm.fontSize = 72;
            tm.characterSize = 0.05F;
            tm.transform.SetParent(this.transform);
            tm.text = tm.transform.parent.name;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            textMesh = tm;
        }

        private void _UpdateShape()
        {
            if (shape && shape.name == shapeType.ToString()) {
                return;
            }
            var p = Vector3.zero;
            var r = Vector3.zero;
            var s = Vector3.one;
            if (shape) {
                p = shape.transform.localPosition;
                r = shape.transform.localEulerAngles;
                s = shape.transform.localScale;
                shape.transform.SetParent(null);
                Destroy(shape.gameObject);
                shape = null;
            }
            var pt = (PrimitiveType)shapeType;
            var go = GameObject.CreatePrimitive(pt);
            go.name = shapeType.ToString();
            var mr = go.GetComponent<MeshRenderer>();
            mr.transform.SetParent(this.transform);
            mr.transform.localPosition = p;
            mr.transform.localEulerAngles = r;
            mr.transform.localScale = s;
            shape = mr;
        }

        private void _UpdateMaterial()
        {
            if (!shape || shape.material.name == materialType.ToString()) {
                return;
            }
            var mat = new Material(Shader.Find("VertexLit"));
            Color color = Color.black;
            switch (materialType) {
            case MaterialType.White: color = Color.white; break;
            case MaterialType.Red: color = Color.red; break;
            case MaterialType.Green: color = Color.green; break;
            case MaterialType.Blue: color = Color.blue; break;
            case MaterialType.Pink:
                color = new Color(0.737255F, 0.560784F, 0.560784F);
                break;
            }
            color.r *= 0.6F;
            color.g *= 0.6F;
            color.b *= 0.6F;
            mat.color = Color.black;
            mat.SetColor("_Emission", color);
            shape.material = mat;
            shape.material.name = materialType.ToString();
        }
    
    }

}
