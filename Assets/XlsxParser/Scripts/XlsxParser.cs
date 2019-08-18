using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Networking; 
using WebRequest = UnityEngine.Networking.UnityWebRequest;

namespace XlsxParser
{
    using Internal;

    using Schema = DataTableSchema;
    using IFieldTypeConverter = DataTable.IFieldTypeConverter;
    using PdtsText = XlsxRequest.PdtsText;
    using WebRequestCreator = System.Func<string, WebRequest>;

    public sealed class XlsxParser : System.IDisposable
    {
        public sealed class Errors : ReturnMessages
        {
            internal Errors(StringBuilder sb) : base(sb) { }
        }

        private class _CoroutineRunner : MonoBehaviour { }

        private class _LoadingResult
        {
            public float progress;
            public Stream stream;
            public string error;
        }

        private delegate IEnumerator _Loader(
            XlsxRequest request, _LoadingResult result
        );

        private static Dictionary<string, _Loader>
        _loaderMap = new Dictionary<string, _Loader> {
            {""     , _LoadFromFile},
            {"data" , _LoadFromFile},
            {"pdata", _LoadFromFile},
            {"res"  , _LoadFromResources},
            {"file" , _LoadFromWebRequest},
            {"http" , _LoadFromWebRequest},
            {"https", _LoadFromWebRequest},
        };

        public ReadOnlyCollection<XlsxRequest> requests { get; private set; }

        public float progress { get; private set; }

        public bool isDone {
            get {
                return progress >= 1 && dataSet != null;
            }
        }

        public ReadOnlyCollection<Schema> erroredSchemas {
            get {
                if (!isDone) {
                    return new List<Schema>().AsReadOnly();
                }
                return _erroredSchemas.AsReadOnly();
            }
        }

        public Errors errors {
            get {
                if (!isDone) {
                    return new Errors(new StringBuilder());
                }
                if (_errors == null) {
                    _errors = _GetAllErrorMessages();
                }
                return _errors;
            }
        }

        public DataSet dataSet { get; private set; }

        public Coroutine coroutine { get; private set; }

        public float startTime { get; private set; }

        private float _elapsedTime;
        public float elapsedTime {
            get {
                if (isDone) {
                    return _elapsedTime;
                } else {
                    return Time.realtimeSinceStartup - startTime;
                }
            }
        }

        private bool _isDisposed;

        private _CoroutineRunner _coroutineRunner;

        private _LoadingResult[] _loadingResults;

        private XlsxReader[] _readers;

        private List<Schema> _erroredSchemas;

        private Errors _errors;

        private Dictionary<string, DataTable> _tables;

        public XlsxParser(params XlsxRequest[] requests)
        {
            _VerifyRequests(requests);
            this.requests = new ReadOnlyCollection<XlsxRequest>(requests);
            _isDisposed = false;
            _InitCoroutineRunner();
            _RunLoaders();
            _erroredSchemas = new List<Schema>();
            _tables = new Dictionary<string, DataTable>();
            coroutine = _coroutineRunner.StartCoroutine(_Parse());
            startTime = Time.realtimeSinceStartup;
        }

        public bool IsDuplicatedSchema(Schema schema)
        {
            var orig = GetOriginalSchema(schema);
            if (orig == null) {
                return false;
            }
            return ReferenceEquals(orig, schema);
        }

        public Schema GetOriginalSchema(Schema schema)
        {
            if (schema == null) {
                throw new System.ArgumentNullException("schema");
            }
            DataTable table = null;
            if (!_tables.TryGetValue(schema.name, out table)) {
                return null;
            }
            return table.schema;
        }

        public void Dispose()
        {
            if (!isDone || _isDisposed) {
                return;
            }
            _isDisposed = true;
            foreach (var result in _loadingResults) {
                result.stream = null;
            }
            foreach (var reader in _readers) {
                if (reader != null) {
                    reader.Close();
                }
            }
            _readers = null;
            var go = _coroutineRunner.gameObject;
            if (Application.isEditor) {
                Object.DestroyImmediate(go);
            } else {
                Object.Destroy(go);
            }
        }

        #region private methods

        private static string _GetUriScheme(string uri)
        {
            var n = uri.IndexOf("://");
            if (n < 0) {
                return string.Empty;
            }
            return uri.Substring(0, n).ToLower();
        }

        private static void _VerifyRequests(XlsxRequest[] requests)
        {
            var error = new StringBuilder();
            for (var n = 0; n < requests.Length; ++n) {
                var uri = (requests[n] == null) ? null : requests[n].uri;
                if (string.IsNullOrEmpty(uri)) {
                    error.AppendFormat(
                        "requests[{0}]: uri must not be empty.\n", n
                    );
                    continue;
                }
                var scheme = _GetUriScheme(uri);
                if (_loaderMap.ContainsKey(scheme)) {
                    continue;
                }
                error.AppendFormat(
                    "requests[{0}]: '{1}' is unsupported scheme.\n", n, scheme
                );
            }
            if (error.Length > 0) {
                throw new System.ArgumentException(error.ToString());
            }
        }

        private void _InitCoroutineRunner()
        {
            var go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(go);
            _coroutineRunner = go.AddComponent<_CoroutineRunner>();
        }

        private void _RunLoaders()
        {
            var count = requests.Count;
            _loadingResults = new _LoadingResult[count];
            _readers = new XlsxReader[count];
            for (var n = 0; n < count; ++n) {
                _loadingResults[n] = new _LoadingResult();
                var loader = _loaderMap[_GetUriScheme(requests[n].uri)];
                _coroutineRunner.StartCoroutine(
                    loader(requests[n], _loadingResults[n])
                );
            }
        }

        private static IEnumerator _LoadFromFile(
            XlsxRequest request, _LoadingResult result)
        {
            var scheme = _GetUriScheme(request.uri);
            var si = 0;
            if (!string.IsNullOrEmpty(scheme)) {
                si = (scheme + "://").Length;
            }
            var path = request.uri;
            switch (scheme) {
            case "data": path = Application.dataPath; break;
            case "pdata": path = Application.persistentDataPath; break;
            }
            if (si > 0) {
                path += "/" + request.uri.Substring(si);
            }
            if (File.Exists(path)) {
                result.stream = new FileStream(path,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite
                );
            } else {
                result.stream = null;
                result.error = "File not found.";
            }
            result.progress = 1;
            yield break;
        }

        private static IEnumerator _LoadFromResources(
            XlsxRequest request, _LoadingResult result)
        {
            var scheme = _GetUriScheme(request.uri) + "://";
            var path = request.uri.Substring(scheme.Length);
            ResourceRequest req = null;
            TextAsset ta = null;
            req = Resources.LoadAsync<TextAsset>(path);
            if (req != null) {
                while (!req.isDone) {
                    result.progress = Mathf.Clamp(
                        req.progress, result.progress, 0.99F
                    );
                    yield return null;
                }
                result.progress = 1;
                ta = req.asset as TextAsset;
            }
            if (!ta) {
                result.error = "Asset not found.";
            } else {
                result.stream = new MemoryStream(ta.bytes);
            }
            result.progress = 1;
        }

        private static IEnumerator _LoadFromWebRequest(
            XlsxRequest request, _LoadingResult result)
        {
            WebRequestCreator createWebRequest = (uri) => {
                WebRequest webReq = null;
                if (request.webRequestCreator != null) {
                    webReq = request.webRequestCreator(uri);
                }
                if (webReq == null) {
                    webReq = new WebRequest(uri);
                }
                if (webReq.downloadHandler == null) {
                    webReq.downloadHandler = new DownloadHandlerBuffer();
                }
                return webReq;
            };
            using (var webReq = createWebRequest(request.uri)) {
                webReq.SendWebRequest();
                while (!webReq.isDone) {
                    result.progress = Mathf.Clamp(
                        webReq.downloadProgress, result.progress, 0.99F
                    );
                    yield return null;
                }
                if (webReq.isNetworkError || webReq.isHttpError) {
                    result.error = webReq.error;
                } else {
                    var bytes = webReq.downloadHandler.data;
                    result.stream = new MemoryStream(bytes);
                }
            }
            result.progress = 1;
        }

        private float _GetLoadProgress()
        {
            var p = 0F;
            foreach (var result in _loadingResults) {
                p += Mathf.Clamp(result.progress, 0, 1);
            }
            if (p >= _loadingResults.Length) {
                return 1;
            }
            return p / _loadingResults.Length;
        }

        private IEnumerator _Parse()
        {
            while (progress < 0.3F) {
                progress = 0.3F * _GetLoadProgress();
                yield return null;
            }
            progress = 0.3F;
            for (var n = 0; n < requests.Count; ++n) {
                var result = _loadingResults[n];
                if (string.IsNullOrEmpty(result.error)) {
                    var req = requests[n];
                    var entry = ZipEntry.Create(result.stream);
                    var hasPassword = !string.IsNullOrEmpty(req.password);
                    if (entry == null && hasPassword) {
                        var dr = new XlsxDecryption.Result();
                        yield return _coroutineRunner.StartCoroutine(
                            XlsxDecryption
                                .Decrypt(result.stream, req.password, dr)
                        );
                        if (!string.IsNullOrEmpty(dr.error)) {
                            result.error = dr.error;
                        } else if (dr.bytes != null) {
                            result.stream.Dispose();
                            result.stream = new MemoryStream(dr.bytes);
                            entry = ZipEntry.Create(result.stream);
                        }
                    } else if (entry == null && !hasPassword) {
                        if (XlsxDecryption.IsEncryptedFile(result.stream)) {
                            result.error = "Password required.";
                        }
                    }
                    _readers[n] = XlsxReader.Create(entry);
                    if (_readers[n] == null
                    && string.IsNullOrEmpty(result.error)) {
                        result.error = "Invalid xlsx stream.";
                    }
                }
                progress = 0.3F + (0.2F * ((float)n / requests.Count));
                yield return null;
            }
            progress = 0.5F;
            XlsxReader lastReader = null;
            for (var n = 0; n < requests.Count;) {
                yield return null;
                var reader = _readers[n];
                if (reader == null) {
                    ++n; continue;
                }
                System.Action updateProgress = () => {
                    var c = 0.5F * (1.0F / requests.Count);
                    progress = 0.5F + (c * (n + reader.progress));
                    progress = Mathf.Min(0.99F, progress);
                };
                updateProgress();
                if (!ReferenceEquals(reader, lastReader)) {
                    reader.Read();
                    lastReader = reader;
                }
                var sheetIndex = reader.sheetIndex;
                if (sheetIndex == reader.sheetCount) {
                    ++n; continue;
                }
                var schemas = _CreateSchemas(requests[n], n);
                if (schemas.Count == 0) {
                    reader.MoveToNextSheet();
                    continue;
                }
                var builders = _CreateBuilders(schemas);
                var appendingCount = 0;
                do {
                    builders.ForEach(x => x.tryAppendCell(reader));
                    appendingCount += builders.Count;
                    if (appendingCount >= 5000) {
                        appendingCount = 0;
                        updateProgress();
                        yield return null;
                    }
                    reader.Read();
                } while (sheetIndex == reader.sheetIndex);
                updateProgress();
                foreach (var builder in builders) {
                    var ftc = requests[n].fieldTypeConverter;
                    var table = builder.Create(ftc);
                    _tables[table.name] = table;
                    yield return null;
                }
            }
            progress = 1;
            dataSet = new DataSet(_tables);
            _elapsedTime = Time.realtimeSinceStartup - startTime;
            Dispose();
        }

        private List<KeyValuePair<CellRef, string>> _MakeDtsTexts(
            XlsxRequest request, XlsxReader reader)
        {
            var texts = new List<KeyValuePair<CellRef, string>>();
            foreach (var pdtsText in request.pdtsTexts) {
                if (pdtsText.sheetIndex == reader.sheetIndex) {
                    texts.Add(new KeyValuePair<CellRef, string>(
                        pdtsText.cellRef, pdtsText.text
                    ));
                }
            }
            texts.AddRange(reader.GetCellComments());
            return texts;
        }

        private List<Schema> _CreateSchemas(
            XlsxRequest request, int readerIndex)
        {
            var reader = _readers[readerIndex];
            var schemas = new List<Schema>();
            var dtsTexts = _MakeDtsTexts(request, reader);
            foreach (var text in dtsTexts) {
                var schema = Schema.Parse(
                    readerIndex, requests[readerIndex].uri,
                    reader.sheetIndex, reader.sheetName,
                    text.Key, text.Value
                );
                if (schema == null) {
                    continue;
                }
                if (!schema.isValid || _tables.ContainsKey(schema.name)) {
                    _erroredSchemas.Add(schema);
                } else {
                    _tables.Add(schema.name, null);
                    schemas.Add(schema);
                }
            }
            return schemas;
        }

        private static List<DataTableBuilder> _CreateBuilders(
            List<Schema> schemas)
        {
            var builders = new List<DataTableBuilder>(schemas.Count);
            foreach (var schema in schemas) {
                builders.Add(new DataTableBuilder(schema));
            }
            return builders;
        }

        private Errors _GetAllErrorMessages()
        {
            if (dataSet == null) {
                throw new System.InvalidOperationException();
            }
            var sb = new StringBuilder();
            for (var n = 0; n < _loadingResults.Length; ++n) {
                var uri = requests[n].uri;
                var err = _loadingResults[n].error;
                if (string.IsNullOrEmpty(err)) {
                    continue;
                }
                err = err.ToSingleLine();
                sb.AppendFormat("{0}(requests[{1}]): {2}\n", uri, n, err);
            }
            foreach (var schema in erroredSchemas) {
                var pos = string.Format("{0}({1},{2}",
                    schema.workbookName, schema.sheetName, schema.cellCommentRef
                );
                var orig = GetOriginalSchema(schema);
                if (orig != null) {
                    sb.AppendFormat(pos + "): "
                        + "Table name '{0}' is already declared in "
                        + "'{1}, {2}, {3}'\n", schema.name,
                        orig.workbookName, orig.sheetName, orig.cellCommentRef
                    );
                }
                foreach (var error in schema.errors) {
                    sb.AppendFormat(
                        pos + ",{0}): {1}\n", error.line, error.message
                    );
                }
            }
            foreach (var table in dataSet) {
                var pos = string.Format("{0}({1}",
                    table.schema.workbookName, table.schema.sheetName
                );
                foreach (var error in table.errors) {
                    sb.AppendFormat(pos + ",{0},{1}): {2}\n",
                        error.cellRef, table.name, error.message
                    );
                }
            }
            return new Errors(sb);
        }

        #endregion

    }

}