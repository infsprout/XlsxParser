using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace XlsxParser
{
    using Internal;

    using XmlReaderMap = Dictionary<string, XmlReader>;
    using CellComments = SortedDictionary<CellRef, string>;
    using CellCommentsKvp = KeyValuePair<CellRef, string>;

    internal sealed class XlsxReader : System.IDisposable
    {
        private static Regex _sheetPathPattern = new Regex(
            @"xl\/worksheets\/sheet([0-9]+)\.xml"
        );

        private static Regex _sheetRelsPathPattern = new Regex(
            @"xl\/worksheets\/_rels\/sheet([0-9]+)\.xml\.rels"
        );

        private static Regex _commentsPathPattern = new Regex(
            @"xl\/comments([0-9]+)\.xml"
        );

        private static Regex _commentsRelsTargetPattern = new Regex(
            @"\.\.\/comments([0-9]+)\.xml"
        );

        private List<string> _sheetRelsIds;
        private List<string> _sheetNames;
        private Dictionary<string, string> _sheetPaths;
        private XmlReader _sharedStringsReader;
        private Dictionary<string, Stream> _sheetStreams;
        private XmlReaderMap _sheetReaders;
        private XmlReaderMap _sheetRelsReaders;
        private XmlReaderMap _commentsReaders;
        private List<string> _sharedStrings;
        private CellComments _cellComments;
        private XmlReader _sheetReader;

        public int sheetCount { get; private set; }
        public string sheetName { get; private set; }
        public int sheetIndex { get; private set; }
        public CellRef cellRef { get; private set; }

        private string _cellValue;
        public string cellValue {
            get {
                if (_cellValue == null) {
                    _cellValue = _ReadCellValue();
                }
                return _cellValue;
            }
        }

        private float _progress;
        public float progress {
            get {
                if (isClosed) {
                    return _progress;
                } else {
                    _progress = _GetProgress();
                    return _progress;
                }
            }
        }

        public bool isClosed { get; private set; }

        private XlsxReader()
        {
            _sheetRelsIds = new List<string>();
            _sheetNames = new List<string>();
            _sheetPaths = new Dictionary<string, string>();
            _sheetStreams = new Dictionary<string, Stream>();
            _sheetReaders = new XmlReaderMap();
            _sheetRelsReaders = new XmlReaderMap();
            _commentsReaders = new XmlReaderMap();
            _sharedStrings = new List<string>();
        }

        public static XlsxReader Create(Stream stream)
        {
            return Create(ZipEntry.Create(stream));
        }

        internal static XlsxReader Create(ZipEntry entry)
        {
            if (entry == null) {
                return null;
            }
            var inst = new XlsxReader();
            do {
                if (inst._TryReadWorkbookXml(entry)
                || inst._TryReadWorkbookRels(entry)
                || inst._TryLoadSharedStringsXml(entry)
                || inst._TryLoadSheetXml(entry)
                || inst._TryLoadSheetRels(entry)
                || inst._TryLoadCommentsXml(entry)
                ) { }
            } while ((entry = entry.Next()) != null);
            if (inst._sheetNames.Count == 0) {
                inst.Dispose();
                return null;
            }
            inst.sheetCount = inst._sheetRelsIds.Count;
            inst.sheetIndex = -1;
            inst.MoveToNextSheet();
            return inst;
        }

        public override string ToString()
        {
            _ReadCommentsXml();
            return "XlsxReader (" + sheetName + ", " + cellRef + ")"
            + "\n  sheetCount: " + sheetCount
            + "\n  sheetName: " + sheetName
            + "\n  sheetIndex: " + sheetIndex
            + "\n  cellCommentCount: " + _cellComments.Count
            + "\n  cellRef: " + cellRef
            + "\n  cellValue: " + cellValue
            + "\n  isClosed: " + isClosed
            + "\n";
        }

        //public virtual void Close()
        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        //protected virtual void Dispose(bool disposing)
        private void Dispose(bool disposing)
        {
            if (disposing) {
                var readers = new List<XmlReader>();
                readers.Add(_sharedStringsReader);
                readers.AddRange(_sheetReaders.Values);
                readers.AddRange(_sheetRelsReaders.Values);
                readers.AddRange(_commentsReaders.Values);
                foreach (var reader in readers) {
                    if (reader != null) {
                        reader.Close();
                    }
                }
            }
            _sheetRelsIds = null;
            _sheetNames = null;
            _sheetPaths = null;
            _sharedStringsReader = null;
            _sheetReaders = null;
            _commentsReaders = null;
            _sharedStrings = null;
            _sheetReader = null;
            isClosed = true;
        }

        public string GetCellComment(CellRef k)
        {
            string v;
            TryGetCellComment(k, out v);
            return v;
        }

        public bool TryGetCellComment(CellRef k, out string v)
        {
            _ReadCommentsXml();
            return _cellComments.TryGetValue(k, out v);
        }

        public ReadOnlyCellComments GetCellComments()
        {
            _ReadCommentsXml();
            return new ReadOnlyCellComments(_cellComments);
        }

        public class ReadOnlyCellComments : IEnumerable<CellCommentsKvp>
        {
            private CellComments _cellComments;
            public ReadOnlyCellComments(CellComments cellComments)
            {
                _cellComments = cellComments;
            }

            public IEnumerator<CellCommentsKvp> GetEnumerator()
            {
                return _cellComments.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public bool Read()
        {
            if (isClosed || sheetIndex >= sheetCount) {
                return false;
            }
            var needNextSheet = !_TryReadSheetXml();
            if (needNextSheet) {
                return (MoveToNextSheet()) ? Read() : false;
            }
            var sr = _sheetReader;
            cellRef = new CellRef(sr.GetAttribute("r"));
            _cellValue = null;
            return true;
        }

        public bool MoveToNextSheet()
        {
            if (isClosed || ++sheetIndex >= sheetCount) {
                return false;
            }
            var sheetPath = _sheetPaths[_sheetRelsIds[sheetIndex]];
            sheetName = _sheetNames[sheetIndex];
            _sheetReader = _sheetReaders[sheetPath];
            _cellComments = null;
            cellRef = new CellRef(0, 0);
            _cellValue = null;
            return true;
        }

        #region private methods   

        private static XmlReader _CreateXmlReader(ZipEntry entry)
        {
            var stream = entry.GetUncompressedStream();
            return _CreateXmlReader(stream);
        }

        private static XmlReader _CreateXmlReader(Stream stream)
        {
            var settings = new XmlReaderSettings {
                CloseInput = true
            };
            return XmlTextReader.Create(stream, settings);
        }

        private static void _ExtendList<T>(
            List<T> list, int targetCount)
        {
            while (list.Count < targetCount) {
                list.Add(default(T));
            }
        }

        private bool _TryReadWorkbookXml(ZipEntry entry)
        {
            if (entry.fileName != "xl/workbook.xml") {
                return false;
            }
            using (var reader = _CreateXmlReader(entry)) {
                while (reader.Read()) {
                    if (!reader.IsStartElement("sheet")) {
                        continue;
                    }
                    _sheetRelsIds.Add(reader.GetAttribute("r:id"));
                    _sheetNames.Add(reader.GetAttribute("name"));
                }
            }
            return true;
        }

        private bool _TryReadWorkbookRels(ZipEntry entry)
        {
            if (entry.fileName != "xl/_rels/workbook.xml.rels") {
                return false;
            }
            using (var reader = _CreateXmlReader(entry)) {
                while (reader.Read()) {
                    if (!reader.IsStartElement("Relationship")) {
                        continue;
                    }
                    var relsId = reader.GetAttribute("Id");
                    var sheetPath = "xl/" + reader.GetAttribute("Target");
                    var match = _sheetPathPattern.Match(sheetPath);
                    if (!match.Success) {
                        continue;
                    }
                    _sheetPaths.Add(relsId, sheetPath);
                }
            }
            return true;
        }

        private bool _TryLoadSharedStringsXml(ZipEntry entry)
        {
            if (entry.fileName != "xl/sharedStrings.xml") {
                return false;
            }
            if (_sharedStringsReader == null) {
                _sharedStringsReader = _CreateXmlReader(entry);
            }
            return true;
        }

        private bool _TryLoadSheetXml(ZipEntry entry)
        {
            var match = _sheetPathPattern.Match(entry.fileName);
            if (!match.Success) {
                return false;
            }
            var path = entry.fileName;
            var stream = new MemoryStream(entry.GetUncompressedBytes());
            var reader = _CreateXmlReader(stream);
            _sheetStreams.Add(path, stream);
            _sheetReaders.Add(path, reader);
            return true;
        }

        private bool _TryLoadSheetRels(ZipEntry entry)
        {
            var match = _sheetRelsPathPattern.Match(entry.fileName);
            if (!match.Success) {
                return false;
            }
            var fileName = Path.GetFileNameWithoutExtension(entry.fileName);
            var sheetPath = "xl/worksheets/" + fileName;
            var reader = _CreateXmlReader(entry);
            _sheetRelsReaders.Add(sheetPath, reader);
            return true;
        }

        private bool _TryLoadCommentsXml(ZipEntry entry)
        {
            var match = _commentsPathPattern.Match(entry.fileName);
            if (!match.Success) {
                return false;
            }
            var path = entry.fileName;
            var reader = _CreateXmlReader(entry);
            _commentsReaders.Add(path, reader);
            return true;
        }

        private string _GetSharedString(int index)
        {
            var reader = _sharedStringsReader;
            while (index >= _sharedStrings.Count) {
                var sb = new StringBuilder();
                reader.ReadToFollowing("si");
                var d = reader.Depth;
                while (reader.Read()) {
                    if (reader.Depth == d) {
                        break;
                    }
                    if (reader.IsStartElement("t")) {
                        reader.Read();
                        sb.Append(reader.Value);
                    }
                }
                _sharedStrings.Add(sb.ToString());
            }
            return _sharedStrings[index];
        }

        private bool _TryReadSheetXml()
        {
            var reader = _sheetReader;
            return reader.ReadToFollowing("c");
        }

        private XmlReader _GetCommentsReader()
        {
            var sheetPath = _sheetPaths[_sheetRelsIds[sheetIndex]];
            XmlReader rels = null;
            if (!_sheetRelsReaders.TryGetValue(sheetPath, out rels)) {
                return null;
            }
            while (rels.Read()) {
                if (!rels.IsStartElement("Relationship")) {
                    continue;
                }
                var target = rels.GetAttribute("Target");
                var match = _commentsRelsTargetPattern.Match(target);
                if (!match.Success) {
                    continue;
                }
                var path = "xl/" + Path.GetFileName(target);
                XmlReader reader = null;
                _commentsReaders.TryGetValue(path, out reader);
                return reader;
            }
            return null;
        }

        private void _ReadCommentsXml()
        {
            if (_cellComments != null) {
                return;
            }
            _cellComments = new CellComments();
            if (isClosed || sheetIndex >= sheetCount) {
                return;
            }
            var reader = _GetCommentsReader();
            if (reader == null) {
                return;
            }
            CellRef? k = null;
            StringBuilder v = null;
            while (reader.Read()) {
                if (reader.Name == "comment") {
                    var nt = reader.NodeType;
                    if (nt == XmlNodeType.Element) {
                        k = new CellRef(reader.GetAttribute("ref"));
                    } else if (nt == XmlNodeType.EndElement) {
                        _cellComments.Add(k.Value, v.ToString());
                        k = null;
                        v = null;
                    }
                    continue;
                }
                if (k.HasValue && reader.IsStartElement("t")) {
                    reader.Read();
                    if (v == null) {
                        v = new StringBuilder();
                    }
                    v.Append(reader.Value);
                    continue;
                }
            }
        }

        private string _ReadCellValue()
        {
            if (isClosed || sheetIndex >= sheetCount) {
                return null;
            }
            var reader = _sheetReader;
            if (!reader.IsStartElement("c")) {
                return null;
            }
            var t = reader.GetAttribute("t");
            if (!reader.ReadToDescendant("v")) {
                return null;
            }
            reader.Read();
            if (t != "s") {
                return reader.Value;
            }
            var n = int.Parse(reader.Value);
            return _GetSharedString(n);
        }

        private float _GetProgress()
        {
            if (sheetIndex >= sheetCount) {
                return 1;
            }
            var progress = (float)sheetIndex / sheetCount;
            var sheetPath = _sheetPaths[_sheetRelsIds[sheetIndex]];
            var stream = _sheetStreams[sheetPath];
            progress += (float)stream.Position / (float)stream.Length;
            return System.Math.Min(1F, progress);
        }

        #endregion

    }

}
