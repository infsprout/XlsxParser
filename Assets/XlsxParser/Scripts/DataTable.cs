using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace XlsxParser
{
    using Internal;

    using Schema = DataTableSchema;
    using AttributeTargets = System.AttributeTargets;

    public sealed class DataTable : IEnumerable<DataTable.Row>
    {
        public sealed class Warnings : ReturnMessages
        {
            internal Warnings(StringBuilder sb) : base(sb) { }
        }

        public interface IFieldTypeConverter
        {
            object FromString(string type, string src);
            string ToString(string type, object src);
        }

        private sealed class _FieldTypeConverter : IFieldTypeConverter
        {
            public object FromString(string type, string src) { return null; }
            public string ToString(string type, object src) { return null; }
        }

        public sealed class Row : IEnumerable<object>
        {
            public CellRef startRef { get; private set; }
            private object[] _cells;

            internal Row(CellRef startRef, object[] cells)
            {
                this.startRef = startRef;
                this._cells = cells;
            }

            public object this[int n] {
                get {
                    return _cells[n];
                }
            }

            public int cellCount {
                get {
                    return _cells.Length;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }

            public IEnumerator<object> GetEnumerator()
            {
                return ((IEnumerable<object>)_cells).GetEnumerator();
            }
        }

        public sealed class Error
        {
            public CellRef cellRef { get; private set; }
            public string message { get; private set; }

            internal Error(CellRef cellRef, string message)
            {
                this.cellRef = cellRef;
                this.message = message;
            }
        }

        [System.AttributeUsage(
            AttributeTargets.Class | AttributeTargets.Struct,
            AllowMultiple = true,
            Inherited = true
        )]
        public sealed class PropertyMappingAttribute : System.Attribute
        {
            private static Regex _sourcePattern = new Regex(
                @"^[_A-Za-z]\w*(\.[_A-Za-z]\w*)*"
            );
            private static Regex _targetPattern = new Regex(
                @"^[_A-Za-z]\w*"
            );

            public string source { get; private set; }
            public string target { get; private set; }

            public PropertyMappingAttribute(string source, string target)
            {
                if (source == null) {
                    throw new System.ArgumentNullException("source");
                } else if (!_sourcePattern.IsMatch(source)) {
                    throw new System.ArgumentException(""
                        + "'source' pattern is invalid. (pattern: "
                        + _sourcePattern + ")"
                    );
                }
                if (target == null) {
                    throw new System.ArgumentNullException("target");
                } else if (!_targetPattern.IsMatch(target)) {
                    throw new System.ArgumentException(""
                        + "'target' pattern is invalid. (pattern: "
                        + _targetPattern + ")"
                    );
                }
                this.source = source;
                this.target = target;
            }
        }

        public Schema schema { get; private set; }

        public string name {
            get {
                return schema.name;
            }
        }

        private List<Row> _rows;
        public Row this[int n] {
            get {
                return _rows[n];
            }
        }

        public int rowCount {
            get {
                return _rows.Count;
            }
        }

        public IFieldTypeConverter fieldTypeConverter { get; private set; }

        public ReadOnlyCollection<Error> errors { get; private set; }

        public bool isValid {
            get {
                return errors.Count == 0;
            }
        }

        private Dictionary<string, object> _populatorCache;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<Row> GetEnumerator()
        {
            return ((IEnumerable<Row>)_rows).GetEnumerator();
        }

        internal DataTable(
            Schema schema, List<Row> rows, IFieldTypeConverter ftc)
        {
            var errors = new List<Error>();
            this.schema = schema;
            this._rows = rows;
            if (ftc != null) {
                this.fieldTypeConverter = ftc;
            } else {
                this.fieldTypeConverter = new _FieldTypeConverter();
            }
            this._rows = _ParseCellValues(errors);
            this.errors = errors.AsReadOnly();
            this._populatorCache = new Dictionary<string, object>();
        }

        public T GetCellValue<T>(int row, int col)
        {
            if (row < 0 || row >= rowCount) {
                throw new System.ArgumentOutOfRangeException("row");
            }
            if (col < 0 || col >= schema.fieldCount) {
                throw new System.ArgumentOutOfRangeException("col");
            }
            return (T)_GetCellValue(row, col, typeof(T));
        }

        public Warnings GetObjectMappingWarnings<T>()
        {
            var populator = _GetPopulator<T>();
            return populator.objectMappingWarnings;
        }

        public Warnings Populate<T>(IList<T> dst) where T : new()
        {
            return Populate(dst, () => new T());
        }

        public Warnings Populate<T>(IList<T> dst, System.Func<T> creator)
        {
            var populator = _GetPopulator<T>();
            return populator.Populate(dst, creator);
        }

        public Warnings Populate<T>(
            IDictionary<string, T> dst, int keyFieldIndex = 0)
            where T : new()
        {
            return Populate<T>(dst, () => new T(), keyFieldIndex);
        }

        public Warnings Populate<T>(
            IDictionary<string, T> dst,
            System.Func<T> creator, int keyFieldIndex = 0)
        {
            var populator = _GetPopulator<T>();
            return populator.Populate(dst, creator, keyFieldIndex);
        }

        public Warnings Populate<T>(int rowIndex, ref T dst)
        {
            var populator = _GetPopulator<T>();
            object tmp = dst;
            var warnings = populator.Populate(rowIndex, ref tmp);
            dst = (T)tmp;
            return warnings;
        }

        public void ClearPopulatorCache()
        {
            _populatorCache.Clear();
        }

        #region private methods

        private string _ToFieldTypeConvertorError(System.Exception e)
        {
            var trace = new System.Diagnostics.StackTrace(e, true);
            var frame = trace.GetFrame(0);
            var cause = "";
            for (var n = 1; n < trace.FrameCount; ++n) {
                var tmp = trace.GetFrame(n);
                var mb = tmp.GetMethod();
                var mfn = mb.DeclaringType + "." + mb.Name;
                if (mfn == "InfSprout.XlsxParser.DataTable._ParseCellValues") {
                    frame = trace.GetFrame(n - 1);
                    if (n > 1) {
                        cause = string.Format(", {0}()",
                            trace.GetFrame(n - 2).GetMethod().Name
                        );
                    }
                    break;
                }
            }
            var error = string.Format("{0}(\"{1}\") at {2}:{3}{4}",
                e.GetType(), e.Message, frame.GetMethod().DeclaringType,
                frame.GetFileLineNumber(), cause
            );
            return error.ToSingleLine();
        }

        private List<Row> _ParseCellValues(List<Error> errors)
        {
            var src = _rows;
            var dst = new List<Row>(src.Count);
            foreach (var row in src) {
                var cells = new object[row.cellCount];
                for (var n = 0; n < cells.Length; ++n) {
                    var error = _ParseCellValue(
                        schema[n].type, (string)row[n], out cells[n]
                    );
                    if (string.IsNullOrEmpty(error)) {
                        continue;
                    }
                    string customError = null;
                    object v = null;
                    try {
                        v = fieldTypeConverter.FromString(
                            schema[n].type, (string)row[n]
                        );
                    } catch (System.Exception e) {
                        customError = _ToFieldTypeConvertorError(e);
                    }
                    if (v == null) {
                        if (!string.IsNullOrEmpty(customError)) {
                            error = customError;
                        } else if (row[n] != null) {
                            error = "Cell value cannot convert to null.";
                        }
                    } else {
                        error = null;
                        var t = v.GetType();
                        if (!t.IsValueType && t != typeof(string)) {
                            error = ""
                            + "Cell value type must be "
                            + "'ValueType' or 'string'.";
                        }
                    }
                    cells[n] = v;
                    if (string.IsNullOrEmpty(error)) {
                        continue;
                    }
                    cells[n] = null;
                    var cellRef = new CellRef(
                        row.startRef.row + schema[n].rowOffset,
                        row.startRef.col + schema[n].colOffset
                    );
                    errors.Add(new Error(cellRef, error));
                }
                dst.Add(new Row(row.startRef, cells));
            }
            return dst;
        }

        private static string _ParseCellValue(
            string type, string src, out object dst)
        {
            dst = src;
            if (string.IsNullOrEmpty(src)) {
                return "Cell must not be empty.";
            }
            if (type == "string") {
                return null;
            }
            if (type == "boolean") {
                var bValue = false;
                var nValue = 0.0;
                if (bool.TryParse(src, out bValue)) {
                    dst = bValue;
                } else if (double.TryParse(src, out nValue)) {
                    dst = (nValue > 0);
                } else {
                    return "Cell value is must be 'boolean' type.";
                }
                return null;
            }
            if (type == "number") {
                var result = 0.0;
                if (double.TryParse(src, out result)) {
                    dst = result;
                    return null;
                }
                return "Cell value is must be 'number' type.";
            }
            return "Cell value is unknown type.";
        }

        private object _GetCellValue(int row, int col, System.Type t)
        {
            var v = _rows[row][col];
            if (v == null) {
                return (!t.IsValueType)
                ? null : System.Activator.CreateInstance(t);
            }
            if (v.GetType() == t) {
                return v;
            }
            var cvt = schema[col].type;
            if (t == typeof(string)) {
                if (cvt == "string" || cvt == "number") {
                    return v.ToString();
                } else if (cvt == "boolean") {
                    return v.ToString().ToLower();
                }
                return fieldTypeConverter.ToString(cvt, v);
            }
            return System.Convert.ChangeType(v, t);
        }

        private DataTablePopulator<T> _GetPopulator<T>()
        {
            var k = typeof(T).FullName;
            object v = null;
            if (_populatorCache.TryGetValue(k, out v)) {
                return (DataTablePopulator<T>)v;
            }
            v = new DataTablePopulator<T>(this);
            _populatorCache[k] = v;
            return (DataTablePopulator<T>)v;
        }

        #endregion

    }

}
