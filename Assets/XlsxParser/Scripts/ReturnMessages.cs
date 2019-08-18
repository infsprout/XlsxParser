using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XlsxParser
{

    public class ReturnMessages : IEnumerable<string>
    {
        private string _source;
        private string[] _lines;

        public int count { get; private set; }

        public string this[int n] {
            get {
                _SplitSource();
                return _lines[n];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            _SplitSource();
            return ((IEnumerable<string>)_lines).GetEnumerator();
        }

        public override string ToString()
        {
            return _source;
        }

        private void _SplitSource()
        {
            if (_lines != null) {
                return;
            }
            _lines = (count == 0) ? new string[0] : _source.Split('\n');
        }

        protected ReturnMessages(StringBuilder sb)
        {
            for (var n = sb.Length - 1; n >= 0; --n) {
                var c = sb[n];
                if (c == '\n') {
                    sb.Remove(n, 1);
                } else {
                    break;
                }
            }
            _source = sb.ToString();
            if (_source.Length > 0) {
                ++count;
            }
            for (var n = 0; n < _source.Length; ++n) {
                if (_source[n] == '\n') {
                    ++count;
                }
            }
        }

    }
    
}