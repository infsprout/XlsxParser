using UnityEngine;
using System.Text.RegularExpressions;

namespace XlsxParser
{

    public struct CellRef : System.IComparable
    {
        private static Regex _pattern = new Regex(
            @"[A-Za-z]+[1-9][0-9]*\z"
        );

        private const int _A = (int)'A' - 1;
        private const int _D = (int)'Z' - _A;

        public int row { get; private set; }
        public int col { get; private set; }

        public CellRef(string a1StyleRef)
        {
            if (a1StyleRef == null) {
                throw new System.ArgumentNullException("a1StyleRef");
            }
            var v = a1StyleRef.Trim().ToUpper();
            if (!_pattern.IsMatch(v)) {
                throw new System.ArgumentException("a1StyleRef");
            }
            var i = v.Length - 1;
            while (char.IsDigit(v[--i])) ;
            ++i;
            var r = v.Substring(i);
            row = int.Parse(r) - 1;
            var c = v.Substring(0, i);
            var l = c.Length;
            var p = 1;
            col = (int)c[l - 1] - _A;
            for (var n = 1; n < l; ++n) {
                p *= _D;
                col += p * ((int)c[l - 1 - n] - _A);
            }
            col -= 1;
        }

        public CellRef(int row, int col)
        {
            if (row < 0 || col < 0) {
                throw new System.ArgumentException();
            }
            this.row = row;
            this.col = col;
        }

        public string ToA1StyleRef()
        {
            var i = col + 1;
            var c = string.Empty;
            do {
                var r = (i % _D);
                i /= _D;
                if (r != 0) {
                    c = (char)(r + _A) + c;
                } else {
                    i -= 1;
                    c = 'Z' + c;
                }
            } while (i > 0);
            return c + (row + 1);
        }

        public override string ToString()
        {
            return ToA1StyleRef();
        }

        public override int GetHashCode()
        {
            return (row << 16) | col;
        }

        public override bool Equals(object other)
        {
            if (!(other is CellRef)) {
                return false;
            }
            return CompareTo(other) == 0;
        }

        public int CompareTo(object other)
        {
            if (other == null) {
                return 1;
            }
            if (!(other is CellRef)) {
                throw new System.ArgumentException("other");
            }
            var a = this;
            var b = (CellRef)other;
            if (a.row < b.row) { return -1; }
            if (a.row > b.row) { return  1; }
            if (a.col < b.col) { return -1; }
            if (a.col > b.col) { return  1; }
            return 0;
        }

        public static bool operator ==(CellRef a, CellRef b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(CellRef a, CellRef b)
        {
            return a.CompareTo(b) != 0;
        }

        public static bool operator <(CellRef a, CellRef b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(CellRef a, CellRef b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(CellRef a, CellRef b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(CellRef a, CellRef b)
        {
            return a.CompareTo(b) >= 0;
        }
   
    }

}
