using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XlsxParser
{

    public sealed class DataSet : IEnumerable<DataTable>
    {
        private Dictionary<string, DataTable> _tables;

        public DataTable this[string name] {
            get {
                DataTable table = null;
                _tables.TryGetValue(name, out table);
                return table;
            }
        }

        public int tableCount {
            get {
                return _tables.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<DataTable> GetEnumerator()
        {
            return ((IEnumerable<DataTable>)_tables.Values).GetEnumerator();
        }

        internal DataSet(Dictionary<string, DataTable> tables)
        {
            _tables = tables;
        }

    }

}
