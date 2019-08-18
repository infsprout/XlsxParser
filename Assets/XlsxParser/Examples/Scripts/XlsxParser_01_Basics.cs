using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UI = UnityEngine.UI;

namespace XlsxParser.Example
{

    public class XlsxParser_01_Basics : MonoBehaviour
    {
        public UI.InputField uriField;
        public GameObject loadingPanel;
        public GameObject toggleGroup;
        public UI.Toggle togglePrototype;
        public GameObject content;
        public UI.Text textPrototype;

        private Dictionary<string, List<UI.Text>> _results;

        public enum MyEnum { Hello, World }

        public struct MyStruct { public int num; }

        public class MyClass { public int num; }

        void Start()
        {
            // password: pw1234
            var uris = new string[] {
                "res://XlsxParser_01",
                "res://XlsxParser_01_SE", // Encrypted File by ECMA-376 Standard Encryption.
                "res://XlsxParser_01_AE", // Encrypted File by ECMA-376 Agile Encryption.
            };
            uriField.text = uris[Random.Range(0, uris.Length)];
        }

        void Update()
        {
            _ClipContent();
        }

        /**
        *   All numeric types should be parsed as 'double' type.
        *   In Google Sheets, Sometimes a decimal point is attached to 
        *   number that looks like integer in sheet.
        */
        public class FieldTypeConverter : DataTable.IFieldTypeConverter
        {
            /**
            Throwing an exception does not stop the parsing, 
            and the exception message is stored to DataTable as an error message.
            */
            public object FromString(string type, string src)
            {
                switch (type) {
                case "MyEnum":
                    return src.ToEnum<MyEnum>();
                case "MyStruct":
                    return new MyStruct { num = (int)src.ToNumber() };
                case "MyClass":
                    return new MyClass { num = (int)src.ToNumber() };
                }
                return null;
            }

            public string ToString(string type, object src)
            {
                switch (type) {
                case "MyEnum":
                    return src.ToString();
                case "MyStruct":
                    return ((MyStruct)src).num.ToString();
                case "MyClass":
                    return ((MyClass)src).num.ToString();
                }
                return src.ToString();
            }
        }

        public void LoadUri()
        {
            if (string.IsNullOrEmpty(uriField.text)) {
                return;
            }
            loadingPanel.SetActive(true);
            var uri = uriField.text;
            var ftc = new FieldTypeConverter();
            StartCoroutine(_LoadXlsxFiles(
                uri.SetPassword("pw1234").SetFieldTypeConverter(ftc)
            ));
        }

        public void OnToggleValueChanged(bool isOn)
        {
            var toggles = toggleGroup.GetComponentsInChildren<UI.Toggle>();
            var selected = System.Array.Find(toggles, x => x.isOn);
            if (!selected) {
                _ShowResult("");
            } else {
                _ShowResult(selected.name);
            }
        }

        private IEnumerator _LoadXlsxFiles(params XlsxRequest[] requests)
        {
            yield return null;
            // You do not need to use the 'using' block.
            // because parser calls 'Dispose()' when completes work.
            var parser = new XlsxParser(requests);
            // yield return parser.coroutine;
            var loadingText = loadingPanel.GetComponentInChildren<UI.Text>();
            var loadingIcon = @"|/-\";
            var m = 0;
            while (!parser.isDone) {
                loadingText.text = "" + loadingIcon[m];
                m = (m + 1) % loadingIcon.Length;
                loadingText.text += "\n" + (int)(parser.progress * 100) + "%";
                yield return new WaitForSeconds(0.25F);
            }
            _results = _CreateResults(parser);
            _ShowResult("");
            _RefreshToggles(parser.dataSet);
            loadingPanel.SetActive(false);
        }

        private void _ShowResult(string name)
        {
            foreach (Transform child in content.transform) {
                child.gameObject.SetActive(false);
            }
            content.transform.DetachChildren();
            var children = _results[name];
            foreach (var child in children) {
                child.gameObject.SetActive(true);
                child.transform.SetParent(content.transform);
            }
        }

        private void _ClipContent()
        {
            var panel = content.transform.parent as RectTransform;
            var panelRect = _GetScreenRect(panel);
            panelRect.yMin -= 200;
            panelRect.yMax += 200;
            foreach (RectTransform child in content.transform) {
                var rect = _GetScreenRect(child);
                var isOverlapped = rect.Overlaps(panelRect);
                var text = child.GetComponent<UI.Text>();
                text.enabled = isOverlapped;
            }
        }

        private Rect _GetScreenRect(RectTransform t)
        {
            var corners = new Vector3[4];
            t.GetWorldCorners(corners);
            var lt = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            var rb = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
            lt.y = Screen.height - lt.y;
            rb.y = Screen.height - rb.y;
            return new Rect(lt.x, lt.y, (rb.x - lt.x), (rb.y - lt.y));
        }

        private Dictionary<string, List<UI.Text>> _CreateResults(
            XlsxParser parser)
        {
            var results = new Dictionary<string, List<UI.Text>>();
            results[""] = _CreateResult(""
                + "\nElapsedTime: " + parser.elapsedTime + "\n\n"
                + _GetColoredParserErrors(parser)
            );
            var colorTags = new string[] {
                "<color=olive>", "<color=teal>"
            };
            foreach (var table in parser.dataSet) {
                var sb = new StringBuilder();
                sb.AppendLine(_GetFieldNames(table));
                sb.AppendLine(_GetFieldNamesForWriter(table));
                var padding = ("" + table.rowCount).Length;
                for (var r = 0; r < table.rowCount; ++r) {
                    sb.Append("[" + ("" + r).PadLeft(padding) + "]: ");
                    sb.Append(colorTags[r % colorTags.Length]);
                    for (var c = 0; c < table.schema.fieldCount; ++c) {
                        if (table[r][c] == null) {
                            sb.Append("null, ");
                        } else {
                            sb.Append(table.GetCellValue<string>(r, c) + ", ");
                        }
                    }
                    sb.AppendLine("</color>");
                }
                results[table.name] = _CreateResult(sb.ToString());
            }
            return results;
        }

        private List<UI.Text> _CreateResult(string text)
        {
            var MaxCharCount = 2000;
            var result = new List<UI.Text>();
            var lines = text.Split('\n');
            var sb = new StringBuilder();
            for (var n = 0; n < lines.Length; ++n) {
                var line = lines[n];
                sb.AppendLine(line);
                if (n < lines.Length - 1 && sb.Length < MaxCharCount) {
                    continue;
                }
                var go = (GameObject)Instantiate(textPrototype.gameObject);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.SetActive(false);
                go.transform.SetParent(null);
                var ui = go.GetComponent<UI.Text>();
                if (sb.Length > 0) {
                    sb.Remove(sb.Length - 1, 1);
                }
                ui.text = sb.ToString();
                result.Add(ui);
                sb = new StringBuilder();
            }
            return result;
        }

        private string _GetColoredParserErrors(XlsxParser parser)
        {
            if (parser.errors.count == 0) {
                return parser.errors.ToString();
            }
            var sb = new StringBuilder();
            var colorTags = new string[] {
                "<color=red>", "<color=magenta>"
            };
            var errors = parser.errors;
            for (var n = 0; n < errors.count; ++n) {
                sb.Append(colorTags[n % colorTags.Length]);
                sb.AppendLine(errors[n] + "</color>");
            }
            return sb.ToString();
        }

        private string _GetFieldNames(DataTable table)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<Fields>");
            foreach (var field in table.schema) {
                sb.AppendLine("    " + field.name);
            }
            return sb.ToString();
        }

        private string _GetFieldNamesForWriter(DataTable table)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<Fields for writer>");
            foreach (var field in table.schema.fieldsForWriter) {
                sb.AppendLine("    " + field.name);
            }
            return sb.ToString();
        }

        private void _RefreshToggles(DataSet dataSet)
        {
            _ClearToggleGroup();
            foreach (var table in dataSet) {
                var go = (GameObject)Instantiate(togglePrototype.gameObject);
                var text = go.GetComponentInChildren<UI.Text>();
                go.hideFlags = HideFlags.DontSave;
                go.name = table.name;
                if (table.isValid) {
                    text.text = table.name;
                } else {
                    text.text = "<color=red>" + table.name + "</color>";
                }
                go.transform.SetParent(toggleGroup.transform);
            }
        }

        private void _ClearToggleGroup()
        {
            var children = new List<Transform>();
            foreach (Transform child in toggleGroup.transform) {
                children.Add(child);
            }
            toggleGroup.transform.DetachChildren();
            children.ForEach(x => Destroy(x.gameObject));
        }

    }

}
