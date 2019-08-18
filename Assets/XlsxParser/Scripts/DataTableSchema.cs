using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace XlsxParser
{

    public sealed class DataTableSchema : IEnumerable<DataTableSchema.Field>
    {
        private static Regex _startPattern = _CreatePattern(
            "^", @"\[\[", @"([A-Za-z]\w*)", "$"
        );

        private static Regex _commentPattern = _CreatePattern("^", "#");

        private static Regex _fieldPattern = _CreatePattern(
            "^", @"\[", "(?<cref>[A-Za-z]+[1-9][0-9]*)", @"\]",
            "(?<name>",
                @"[_A-Za-z]\w*", @"(\[", "[0-9]+", @"\])?",
                "(",
                    @"\.", @"[_A-Za-z]\w*", @"(\[", "[0-9]+", @"\])?",
                ")*",
            ")",
            ":", @"(?<type>[_A-Za-z]\w*)",
            "$"
        );

        private static Regex _endPattern = _CreatePattern(
            "^", @"\]\]", "([DdRr][1-9][0-9]*)", "$"
        );

        private interface _IField
        {
            void SetRowOffset(int offset);
            void SetColOffset(int offset);
        }

        public sealed class Field : _IField
        {
            public int index { get; private set; }
            public int line { get; private set; }
            public int rowOffset { get; private set; }
            public int colOffset { get; private set; }
            public string name { get; private set; }
            public string type { get; private set; }

            public
            ReadOnlyCollection<FieldNameNode> nameNodes { get; private set; }

            public int depth {
                get { return nameNodes.Count - 1; }
            }

            internal Field(
                int index, int line,
                int rowOffset, int colOffset, string name, string type)
            {
                this.index = index;
                this.line = line;
                this.rowOffset = rowOffset;
                this.colOffset = colOffset;
                this.name = _NormalizeFieldName(name);
                this.type = type;
                this.nameNodes = _GetFieldNameNodes(this.name);
            }

            void _IField.SetRowOffset(int offset)
            {
                rowOffset = offset;
            }

            void _IField.SetColOffset(int offset)
            {
                colOffset = offset;
            }

            public override string ToString()
            {
                return string.Format(
                    "({0,2}) Line[{1,2}] R[{2,2}]C[{3,2}] {4} : {5}",
                    index, line, rowOffset, colOffset, name, type
                );
            }
        }

        public sealed class FieldNameNode
        {
            public string name { get; private set; }
            public int arrayIndex { get; private set; }
            public bool isArrayElement {
                get { return arrayIndex >= 0; }
            }

            internal FieldNameNode(string name, int arrayIndex)
            {
                this.name = name;
                this.arrayIndex = System.Math.Max(-1, arrayIndex);
            }
        }

        public sealed class Error
        {
            public int line { get; private set; }
            public string message { get; private set; }

            internal Error(int line, string message)
            {
                this.line = line;
                this.message = message;
            }
        }

        public int workbookIndex { get; private set; }
        public string workbookName { get; private set; }
        public int sheetIndex { get; private set; }
        public string sheetName { get; private set; }
        public CellRef cellCommentRef { get; private set; }

        public string name { get; private set; }
        public CellRef rangeStartRef { get; private set; }
        public bool isRotated { get; private set; }
        public int nextBlockOffset { get; private set; }
        public int blockHeight { get; private set; }
        public int blockWidth { get; private set; }

        public ReadOnlyCollection<Error> errors { get; private set; }
        public bool isValid {
            get {
                return errors.Count == 0;
            }
        }

        private List<Field> _fields;
        public Field this[int n] {
            get {
                return _fields[n];
            }
        }

        public int fieldCount {
            get {
                return _fields.Count;
            }
        }

        public ReadOnlyCollection<Field> fieldsForBuilder { get; private set; }
        public ReadOnlyCollection<Field> fieldsForWriter { get; private set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return ((IEnumerable<Field>)_fields).GetEnumerator();
        }

        private DataTableSchema() { }

        internal static DataTableSchema Parse(
            int workbookIndex, string workbookName,
            int sheetIndex, string sheetName,
            CellRef cellCommentRef, string cellComment)
        {
            if (!cellComment.StartsWith("[[")) {
                return null;
            }
            var lines = _SplitCellComment(cellComment);
            var errors = new List<Error>();
            var inst = new DataTableSchema();
            inst.workbookIndex = workbookIndex;
            inst.workbookName = workbookName;
            inst.sheetIndex = sheetIndex;
            inst.sheetName = sheetName;
            inst.cellCommentRef = cellCommentRef;
            inst._fields = new List<Field>();
            if (!inst._TryParseStartPattern(lines, 0, errors)) {
                inst.name = string.Format("ERROR_{0}_{1}_{2}",
                    workbookIndex, sheetIndex, cellCommentRef
                );
            }
            var endLineNum = 0;
            for (var n = 1; n < lines.Length; ++n) {
                if (lines[n].Trim().Length == 0) {
                    continue;
                }
                if (_commentPattern.IsMatch(lines[n])) {
                    continue;
                }
                if (inst._TryParseFieldPattern(lines, n, errors)) {
                    continue;
                }
                if (!lines[n].Trim().StartsWith("]]")) {
                    continue;
                }
                if (inst._TryParseEndPattern(lines, n, errors)) {
                    errors.RemoveAt(errors.Count - 1);
                    endLineNum = n + 1;
                    break;
                }
            }
            if (endLineNum <= 1) {
                errors.Add(new Error(lines.Length,
                    "End pattern not found."
                ));
            } else if (inst._fields.Count == 0) {
                errors.Add(new Error(endLineNum,
                    "Table must have one field at least."
                ));
            }
            inst._InitBlockInfo();
            inst._VerifyNextBlockOffset(endLineNum, errors);
            inst._InitFieldsForBuilder();
            inst._InitFieldsForWriter();
            inst._VerifyCellRefDuplication(errors);
            inst._VerifyFieldType(errors);
            inst._VerifyFieldDuplication(errors);
            inst.errors = errors.AsReadOnly();
            return inst;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("workbookIndex: " + workbookIndex);
            sb.AppendLine("workbookName: " + workbookName);
            sb.AppendLine("sheetIndex: " + sheetIndex);
            sb.AppendLine("sheetName: " + sheetName);
            sb.AppendLine("cellCommentRef: " + cellCommentRef);
            sb.AppendLine("name: " + name);
            sb.AppendLine("rangeStartRef: " + rangeStartRef);
            sb.AppendLine("isRotated: " + isRotated);
            sb.AppendLine("nextBlockOffset: " + nextBlockOffset);
            sb.AppendLine("blockHeight: " + blockHeight);
            sb.AppendLine("blockWidth: " + blockWidth);
            sb.AppendLine("isValid:" + isValid);
            sb.AppendLine("_fields:");
            foreach (var field in _fields) {
                sb.AppendLine("    " + field);
            }
            return sb.ToString();
        }

        #region private methods

        private static Regex _CreatePattern(params string[] tokens)
        {
            var sb = new StringBuilder();
            sb.Append(tokens[0]);
            for (var n = 1; n < tokens.Length; ++n) {
                sb.Append(@"[ \t]*");
                sb.Append(tokens[n]);
            }
            return new Regex(sb.ToString());
        }

        private static string _NormalizeFieldName(string fieldName)
        {
            var sb = new StringBuilder();
            var isFirstZero = false;
            for (var n = 0; n < fieldName.Length; ++n) {
                var c = fieldName[n];
                if (char.IsWhiteSpace(c)) {
                    continue;
                }
                if (c == '[') {
                    isFirstZero = true;
                } else if (c == '0' && isFirstZero) {
                    if (char.IsDigit(fieldName[n + 1])) {
                        continue;
                    }
                } else {
                    isFirstZero = false;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static
        ReadOnlyCollection<FieldNameNode>
        _GetFieldNameNodes(string normalizedFieldName)
        {
            var nodes = new List<FieldNameNode>();
            var words = normalizedFieldName.Split('.');
            foreach (var word in words) {
                var i = word.IndexOf("[");
                if (i < 0) {
                    nodes.Add(new FieldNameNode(word, -1));
                } else {
                    var name = word.Substring(0, i);
                    var arrayIndex = int.Parse(
                        word.Substring(++i, word.Length - 1 - i)
                    );
                    nodes.Add(new FieldNameNode(name, arrayIndex));
                }
            }
            return nodes.AsReadOnly();
        }

        private static string[] _SplitCellComment(string cellComment)
        {
            var lines = cellComment.Split(
                new string[] { "\r\n", "\n" }, System.StringSplitOptions.None
            );
            for (var n = lines.Length - 1; n > 0; --n) {
                if (lines[n].Trim().Length != 0) {
                    System.Array.Resize(ref lines, n + 1);
                    break;
                }
            }
            return lines;
        }

        private bool _TryParseStartPattern(
            string[] lines, int n, List<Error> errors)
        {
            var match = _startPattern.Match(lines[n]);
            if (!match.Success) {
                errors.Add(new Error(n + 1,
                    "Start pattern is incorrect."
                ));
                return false;
            }
            name = match.Groups[1].ToString();
            return true;
        }

        private bool _TryParseFieldPattern(
            string[] lines, int n, List<Error> errors)
        {
            var match = _fieldPattern.Match(lines[n]);
            if (!match.Success) {
                errors.Add(new Error(n + 1,
                    "Field pattern is incorrect."
                ));
                return false;
            }
            var cr = new CellRef(match.Groups["cref"].ToString());
            var field = new Field(
                _fields.Count, n + 1, cr.row, cr.col,
                match.Groups["name"].ToString(),
                match.Groups["type"].ToString()
            );
            if (_fields.Count == 0) {
                rangeStartRef = cr;
            } else {
                rangeStartRef = new CellRef(
                    System.Math.Min(rangeStartRef.row, cr.row),
                    System.Math.Min(rangeStartRef.col, cr.col)
                );
            }
            _fields.Add(field);
            return true;
        }

        private bool _TryParseEndPattern(
            string[] lines, int n, List<Error> errors)
        {
            var match = _endPattern.Match(lines[n]);
            if (!match.Success) {
                errors.Add(new Error(n + 1,
                    "End pattern is incorrect."
                ));
                return false;
            }
            var g = match.Groups[1].ToString().ToUpper();
            if (g[0] == 'R') {
                isRotated = true;
            }
            nextBlockOffset = int.Parse(g.Substring(1));
            return true;
        }

        private void _InitBlockInfo()
        {
            var startRow = rangeStartRef.row;
            var startCol = rangeStartRef.col;
            var bH = 0;
            var bW = 0;
            foreach (var field in _fields) {
                ((_IField)field).SetRowOffset(field.rowOffset - startRow);
                ((_IField)field).SetColOffset(field.colOffset - startCol);
                bH = System.Math.Max(bH, field.rowOffset);
                bW = System.Math.Max(bW, field.colOffset);
            }
            blockHeight = bH + 1;
            blockWidth = bW + 1;
        }

        private void _VerifyNextBlockOffset(int endLineNum, List<Error> errors)
        {
            var minBlockOffset = (isRotated) ? blockWidth : blockHeight;
            if (endLineNum > 1 && nextBlockOffset < minBlockOffset) {
                errors.Add(new Error(endLineNum, ""
                    + "Next block offset must be greater than or equal to "
                    + "block " + ((isRotated) ? "width" : "height")
                    + " (" + minBlockOffset + ")."
                ));
            }
        }

        private void _InitFieldsForBuilder()
        {
            var fields = new List<Field>(_fields);
            fields.Sort((a, b) => {
                if (a.rowOffset < b.rowOffset) { return -1; }
                if (a.rowOffset > b.rowOffset) { return  1; }
                if (a.colOffset < b.colOffset) { return -1; }
                if (a.colOffset > b.colOffset) { return  1; }
                return a.line.CompareTo(b.line);
            });
            fieldsForBuilder = fields.AsReadOnly();
        }

        private void _InitFieldsForWriter()
        {
            var fields = new List<Field>(_fields);
            var recordOrders = new Dictionary<string, int>();
            for (var n = 0; n < _fields.Count; ++n) {
                var key = _fields[n].nameNodes[0].name;
                if (!recordOrders.ContainsKey(key)) {
                    recordOrders.Add(key, n + 1);
                }
            }
            System.Func<FieldNameNode, FieldNameNode, int>
            compareByRecordOrder = (a, b) => {
                var aR = 0;
                var bR = 0;
                recordOrders.TryGetValue(a.name, out aR);
                recordOrders.TryGetValue(b.name, out bR);
                var cR = aR.CompareTo(bR);
                return (cR != 0) ? cR : a.name.CompareTo(b.name);
            };
            fields.Sort((a, b) => {
                var count = System.Math.Min(
                    a.nameNodes.Count, b.nameNodes.Count
                );
                for (var n = 0; n < count; ++n) {
                    var aN = a.nameNodes[n];
                    var bN = b.nameNodes[n];
                    var cN = aN.name.CompareTo(bN.name);
                    if (cN != 0) {
                        return (n != 0) ? cN : compareByRecordOrder(aN, bN);
                    }
                    if (aN.arrayIndex == bN.arrayIndex) {
                        continue;
                    } else {
                        return aN.arrayIndex.CompareTo(bN.arrayIndex);
                    }
                }
                var cmp = a.depth.CompareTo(b.depth);
                if (cmp != 0) {
                    return cmp;
                }
                return a.line.CompareTo(b.line);
            });
            fieldsForWriter = fields.AsReadOnly();
        }

        private void _VerifyCellRefDuplication(List<Error> errors)
        {
            var fields = fieldsForBuilder;
            for (var n = 1; n < fieldCount; ++n) {
                var aF = fields[n - 1];
                var bF = fields[n];
                if (aF.rowOffset != bF.rowOffset
                || aF.colOffset != bF.colOffset) {
                    continue;
                }
                var oF = aF;
                for (var m = n - 2; m >= 0; --m) {
                    oF = fields[m];
                    if (oF.rowOffset != aF.rowOffset
                    || oF.colOffset != aF.colOffset) {
                        oF = fields[m + 1];
                        break;
                    }
                }
                var cr = new CellRef(
                    rangeStartRef.row + bF.rowOffset,
                    rangeStartRef.col + bF.colOffset
                );
                var msg = string.Format(
                    "CellRef '{0}' is already used at {1} line.", cr, oF.line
                );
                errors.Add(new Error(bF.line, msg));
            }
        }

        private void _VerifyFieldType(List<Error> errors)
        {
            foreach (var field in _fields) {
                var t = field.type;
                if (t != "array" && t != "object") {
                    continue;
                }
                errors.Add(new Error(field.line, ""
                    + "Field type '" + t + "' is implicit type "
                    + "which cannot declare explicitly."
                ));
            }

            for (var n = 1; n < fieldCount; ++n) {
                var aF = fieldsForWriter[n - 1];
                var bF = fieldsForWriter[n];
                var end = System.Math.Min(aF.depth, bF.depth);
                for (var m = 0; m <= end; ++m) {
                    var aN = aF.nameNodes[m];
                    var bN = bF.nameNodes[m];
                    if (aN.name != bN.name) {
                        break;
                    }
                    if (m < end && aN.isArrayElement == bN.isArrayElement) {
                        continue;
                    }
                    var aT = (aN.isArrayElement) ? "array" : "object";
                    var bT = (bN.isArrayElement) ? "array" : "object";
                    aT = (aF.depth == m) ? aF.type : aT;
                    bT = (bF.depth == m) ? bF.type : bT;
                    if (aT == bT) {
                        continue;
                    }
                    System.Func<string, int, int> findDotIndex
                    = (str, cnt) => {
                        var i = -1;
                        for (var k = 0; k < cnt; ++k) {
                            i = str.IndexOf(".", i + 1);
                            if (i < 0) { break; }
                        }
                        return i;
                    };

                    var sb = new StringBuilder();
                    sb.Append("Field type is inconsistent. ");
                    if (aF.depth == m) {
                        sb.Append(aF.name);
                    } else {
                        var i = findDotIndex(aF.name, m + 1);
                        sb.Append(aF.name.Substring(0, i));
                    }
                    sb.AppendFormat("(at {0} line) is '{1}' but ", aF.line, aT);
                    if (bF.depth == m) {
                        sb.Append(bF.name);
                    } else {
                        var i = findDotIndex(bF.name, m + 1);
                        sb.Append(bF.name.Substring(0, i));
                    }
                    sb.AppendFormat("(at {0} line) is '{1}'.", bF.line, bT);
                    errors.Add(new Error(bF.line, sb.ToString()));
                }
            }
        }

        private void _VerifyFieldDuplication(List<Error> errors)
        {
            var fields = fieldsForWriter;
            for (var n = 1; n < fieldCount; ++n) {
                var aF = fields[n - 1];
                var bF = fields[n];
                if (aF.name != bF.name) {
                    continue;
                }
                var oF = aF;
                for (var m = n - 2; m >= 0; --m) {
                    oF = fields[m];
                    if (oF.name != aF.name) {
                        oF = fields[m + 1];
                        break;
                    }
                }
                var msg = string.Format(
                    "Field name '{0}' is already declared at {1} line.",
                    bF.name, oF.line
                );
                errors.Add(new Error(bF.line, msg));
            }
        }

        #endregion

    }

}
