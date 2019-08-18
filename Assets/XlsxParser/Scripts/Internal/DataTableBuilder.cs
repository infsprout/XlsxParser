using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XlsxParser.Internal
{

    using Schema = DataTableSchema;
    using FieldMap = Dictionary<string, DataTableSchema.Field>;

    internal sealed class DataTableBuilder
    {
        private Schema _schema;
        private FieldMap _fieldMap;
        private int _fieldOrder;
        private int _rowIndex;
        private int _rowCountLimit;
        private CellRef _rowRef;
        private List<string[]> _rows;

        public System.Func<XlsxReader, bool> tryAppendCell { get; private set; }

        public DataTableBuilder(Schema schema)
        {
            _schema = schema;
            _fieldMap = null;
            _fieldOrder = 0;
            _rowIndex = 0;
            _rowCountLimit = int.MaxValue;
            _rowRef = _schema.rangeStartRef;
            _rows = new List<string[]>();
            _rows.Add(new string[schema.fieldCount]);
            if (_schema.isRotated) {
                tryAppendCell = _TryAppendCellToRight;
            } else {
                tryAppendCell = _TryAppendCellToDown;
            }
        }

        public DataTable Create(DataTable.IFieldTypeConverter ftc)
        {
            var rows = new List<DataTable.Row>(_rows.Count);
            var startRef = _schema.rangeStartRef;
            var rowOffset = ( _schema.isRotated) ? 0 : _schema.nextBlockOffset;
            var colOffset = (!_schema.isRotated) ? 0 : _schema.nextBlockOffset;

            var count = System.Math.Min(_rows.Count, _rowCountLimit);
            var isEmptyRow = System.Array.TrueForAll(
                _rows[count - 1], x => x == null
            );
            if (isEmptyRow) {
                --count;
            }
            for (var n = 0; n < count; ++n) {
                var cr = new CellRef(
                    startRef.row + (n * rowOffset),
                    startRef.col + (n * colOffset)
                );
                rows.Add(new DataTable.Row(cr, _rows[n]));
            }

            return new DataTable(_schema, rows, ftc);
        }

        #region private methods

        private Schema.Field _GetField(int rowOffset, int colOffset)
        {
            if (_fieldMap == null) {
                _fieldMap = new FieldMap();
                foreach (var field in _schema) {
                    _fieldMap.Add(
                        field.rowOffset + "," + field.colOffset, field
                    );
                }
            }
            var k = rowOffset + "," + colOffset;
            Schema.Field v = null;
            _fieldMap.TryGetValue(k, out v);
            return v;
        }

        private bool _DoNotAppendCell(XlsxReader reader)
        {
            return false;
        }

        private bool _TryAppendCellToRight(XlsxReader reader)
        {
            var bH = _schema.blockHeight;
            var r = reader.cellRef.row - _rowRef.row;
            var c = reader.cellRef.col - _rowRef.col;

            if (r < 0 || c < 0 || r >= bH) {
                return false;
            }
            var rowIndex = c / _schema.nextBlockOffset;
            if (rowIndex >= _rowCountLimit) {
                return false;
            }
            c %= _schema.nextBlockOffset;
            while (rowIndex >= _rows.Count) {
                _rows.Add(new string[_schema.fieldCount]);
            }
            var field = _GetField(r, c);
            if (field == null) {
                return false;
            }

            var fi = field.index;
            var v = reader.cellValue;
            _rows[rowIndex][fi] = v;

            if (rowIndex > 0 && _rows[rowIndex - 1][fi] == null) {
                _rowCountLimit = rowIndex;
            }
            return true;
        }

        private bool _TryAppendCellToDown(XlsxReader reader)
        {
            var fields = _schema.fieldsForBuilder;
            var bW = _schema.blockWidth;
            var r = reader.cellRef.row - _rowRef.row;
            var c = reader.cellRef.col - _rowRef.col;
            if (r < 0 || c < 0 || c >= bW) {
                return false;
            }
            while (r >= _schema.nextBlockOffset) {
                var row = _rowRef.row + _schema.nextBlockOffset;
                _rowRef = new CellRef(row, _rowRef.col);
                _fieldOrder = 0;
                var isFullRow = System.Array.TrueForAll(
                    _rows[_rowIndex], x => x != null
                );
                if (!isFullRow) {
                    tryAppendCell = _DoNotAppendCell;
                    return false;
                }
                _rows.Add(new string[_schema.fieldCount]);
                _rowIndex = _rows.Count - 1;
                r -= _schema.nextBlockOffset;
            }
            do {
                if (_fieldOrder >= fields.Count) {
                    return false;
                }
                var field = fields[_fieldOrder];
                if (field.rowOffset > r) { return false; }
                if (field.rowOffset < r) { ++_fieldOrder; continue; }
                if (field.colOffset > c) { return false; }
                if (field.colOffset < c) { ++_fieldOrder; continue; }
                var fi = fields[_fieldOrder].index;
                var v = reader.cellValue;
                _rows[_rowIndex][fi] = v;
                ++_fieldOrder;
            } while (false);
            return true;
        }

        #endregion

    }

}