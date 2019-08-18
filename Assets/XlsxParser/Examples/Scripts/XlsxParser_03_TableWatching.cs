using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XlsxParser.Example
{

    using UI = UnityEngine.UI;
    using FieldTypeConverter = XlsxParser_02_ObjectMapping.FieldTypeConverter;

    public class XlsxParser_03_TableWatching : MonoBehaviour
    {
        private static string _home {
            get {
                return Application.dataPath + "/InfSprout/XlsxParser/Examples";
            }
        }

        public UI.ScrollRect console;

        private Dictionary<string, Object3D> _myObjects;

        private Queue<string> _logQueue = new Queue<string>();

        private DataTable _oldTable;

        void Start()
        {
            Application.logMessageReceived += _HandleLog;
            Debug.LogWarning("This example works in editor and standalone.");
            _RefreshConsole();
            var fileName = "XlsxParser_03";
            _CopyXlsxFromResources(fileName);
            var path = _home + "/" + fileName + ".xlsx";
            _myObjects = new Dictionary<string, Object3D>();
            StartCoroutine(Watch(path, _OnDataSetUpdated));
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= _HandleLog;
        }

        private static void _CopyXlsxFromResources(string fileName)
        {
#if (!UNITY_WEBPLAYER)
            var ta = Resources.Load<TextAsset>(fileName);
            if (!Directory.Exists(_home)) {
                Directory.CreateDirectory(_home);
            }
            var path = _home + "/" + fileName + ".xlsx";
            if (!File.Exists(path)) {
                File.WriteAllBytes(path, ta.bytes);
            }
#endif
        }

        public static IEnumerator Watch(
            string path,
            System.Action<DataSet, XlsxParser.Errors> onDataSetUpdated)
        {
            var lastWriteTime = new System.DateTime(
                1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc
            );
            var ftc = new FieldTypeConverter();
            while (File.Exists(path)) {
                yield return null;
                var wt = File.GetLastWriteTimeUtc(path);
                if (wt <= lastWriteTime) {
                    continue;
                }
                lastWriteTime = wt;
                var parser = new XlsxParser(path.SetFieldTypeConverter(ftc));
                while (!parser.isDone) {
                    yield return null;
                }
                if (onDataSetUpdated != null) {
                    onDataSetUpdated(parser.dataSet, parser.errors);
                }
            }
        }

        public static string GetTableChanges(
            DataTable newTable, DataTable oldTable)
        {
            var changes = new StringBuilder();
            if (newTable == null) {
                throw new System.ArgumentNullException("newTable");
            }
            if (oldTable == null) {
                changes.AppendFormat("Table '{0}' added.", newTable.name);
                return changes.ToString();
            }
            if (!_SchemaEquals(newTable, oldTable)) {
                if (newTable.name == oldTable.name) {
                    changes.AppendFormat(
                        "Table '{0}' schema changed.\n", newTable.name
                    );
                } else {
                    changes.AppendFormat(
                        "Table '{0}' removed.\n", oldTable.name
                    );
                    changes.AppendFormat(
                        "Table '{0}' added.\n", newTable.name
                    );
                }
            }
            var dRowCount = newTable.rowCount - oldTable.rowCount;
            if (dRowCount > 0) {
                changes.AppendFormat("{0}: {1} {2} added.\n",
                    newTable.name, dRowCount, (dRowCount == 1) ? "row" : "rows"
                );
            } else if (dRowCount < 0) {
                changes.AppendFormat("{0}: {1} {2} removed.\n",
                    newTable.name, dRowCount, (dRowCount == 1) ? "row" : "rows"
                );
            }
            var newFields = newTable.schema.fieldsForBuilder;
            var oldFields = oldTable.schema.fieldsForBuilder;
            for (var r = 0; r < newTable.rowCount; ++r) {
                if (r >= oldTable.rowCount) {
                    break;
                }
                for (var n = 0; n < newFields.Count; ++n) {
                    var nv = newTable
                        .GetCellValue<string>(r, newFields[n].index);
                    var ov = oldTable
                        .GetCellValue<string>(r, oldFields[n].index);
                    if (nv == ov) {
                        continue;
                    }
                    var c = newFields[n].index;
                    var cr = new CellRef(
                        newTable[r].startRef.row + newFields[n].rowOffset,
                        newTable[r].startRef.col + newFields[n].colOffset
                    );
                    changes.AppendFormat("{0}({1},{2},{3},{4}): {5} -> {6}\n",
                        newTable.name, r, c, cr, newFields[n].name, ov, nv
                    );
                }
            }
            if (changes.Length > 0) {
                changes.Remove(changes.Length - 1, 1);
            }
            return changes.ToString();
        }

        private static bool _SchemaEquals(DataTable a, DataTable b)
        {
            if (a.schema.fieldCount != b.schema.fieldCount) {
                return false;
            }
            for (var n = 0; n < a.schema.fieldCount; ++n) {
                var aF = a.schema.fieldsForBuilder[n];
                var bF = b.schema.fieldsForBuilder[n];
                if (aF.rowOffset != bF.rowOffset 
                ||  aF.colOffset != bF.colOffset) {
                    return false;
                }
                if (aF.name != bF.name || aF.type != bF.type) {
                    return false;
                }
            }
            return true;
        }

        private void _HandleLog(string log, string stackTrace, LogType lt)
        {
            var colorTag = "";
            switch (lt) {
            case LogType.Error: colorTag = "<color=red>"; break;
            case LogType.Warning: colorTag = "<color=yellow>"; break;
            }
            if (string.IsNullOrEmpty(colorTag)) {
                _logQueue.Enqueue(log);
            } else {
                _logQueue.Enqueue(colorTag + log + "</color>");
            }
        }

        private void _RefreshConsole()
        {
            while (_logQueue.Count > 500) {
                _logQueue.Dequeue();
            }
            var sb = new StringBuilder();
            foreach (var log in _logQueue) {
                sb.AppendLine(log);
            }
            if (sb.Length > 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            var content = console.content.GetComponent<UI.Text>();
            content.text = sb.ToString();
            Canvas.ForceUpdateCanvases();
            console.verticalNormalizedPosition = 0;
        }

        private void _OnDataSetUpdated(
            DataSet dataSet, XlsxParser.Errors errors)
        {
            if (errors.count > 0) {
                foreach (var error in errors) {
                    Debug.LogError(error);
                }
                _RefreshConsole();
                return;
            }
            var table = dataSet["Object3D"];
            var changes = GetTableChanges(table, _oldTable);
            if (changes.Length > 0) {
                foreach (var change in changes.Split('\n')) {
                    Debug.Log(change);
                }
            }
            _oldTable = table;
            DataTable.Warnings warnings = null;
            warnings = table.GetObjectMappingWarnings<Object3D>();
            foreach (var warning in warnings) {
                Debug.LogWarning(warning);
            }
            var n = 0;
            warnings = table.Populate(_myObjects, () => {
                var inst = Object3D.Create();
                var x = 3.0F * (n % 3);
                var z = 3.0F * (n / 3);
                ++n;
                inst.transform.position = new Vector3(x, 3, z);
                return inst;
            }, 0);
            foreach (var warning in warnings) {
                Debug.LogWarning(warning);
            }
            _RefreshConsole();
        }

    }

}
