using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using WebRequest = UnityEngine.Networking.UnityWebRequest;

namespace XlsxParser
{
    using WebRequestCreator = System.Func<string, WebRequest>;
    using IFieldTypeConverter = DataTable.IFieldTypeConverter;

    public sealed class XlsxRequest
    {
        // Predefined Data Table Schema Text
        internal sealed class PdtsText
        {
            public int sheetIndex { get; private set; }
            public CellRef cellRef { get; private set; }
            public string text { get; private set; }

            internal PdtsText(
                int sheetIndex, CellRef cellRef, string text)
            {
                this.sheetIndex = sheetIndex;
                this.cellRef = cellRef;
                this.text = text;
            }
        }

        internal string uri { get; private set; }
        internal WebRequestCreator webRequestCreator { get; private set; }
        internal string password { get; private set; }
        internal IFieldTypeConverter fieldTypeConverter { get; private set; }
        internal ReadOnlyCollection<PdtsText> pdtsTexts { get; private set; }

        public XlsxRequest(string uri)
        {
            this.uri = uri ?? "";
            this.pdtsTexts = new List<PdtsText>().AsReadOnly();
        }

        internal XlsxRequest(XlsxRequest src)
        {
            this.uri = src.uri;
            this.webRequestCreator = src.webRequestCreator;
            this.password = src.password;
            this.fieldTypeConverter = src.fieldTypeConverter;
            this.pdtsTexts = src.pdtsTexts;
        }

        public static implicit operator XlsxRequest(string uri)
        {
            return new XlsxRequest(uri);
        }

        public override string ToString()
        {
            return uri;
        }

        public XlsxRequest SetWebRequestCreator(
            WebRequestCreator webRequestCreator)
        {
            var clone = new XlsxRequest(this);
            clone.webRequestCreator = webRequestCreator;
            return clone;
        }

        public XlsxRequest SetPassword(string password)
        {
            var clone = new XlsxRequest(this);
            clone.password = password;
            return clone;
        }

        public XlsxRequest SetFieldTypeConverter(IFieldTypeConverter ftc)
        {
            var clone = new XlsxRequest(this);
            clone.fieldTypeConverter = ftc;
            return clone;
        }

        public XlsxRequest AddPdtsText(int sheetIndex, string schemaText)
        {
            if (sheetIndex < 0) {
                throw new System.ArgumentOutOfRangeException("sheetIndex");
            }
            if (schemaText == null) {
                throw new System.ArgumentNullException("schemaText");
            }
            schemaText = schemaText.Trim();
            var clone = new XlsxRequest(this);
            var schemas = new List<PdtsText>(clone.pdtsTexts);
            var cellRef = new CellRef("PREDEF1");
            cellRef = new CellRef(cellRef.row + schemas.Count, cellRef.col);
            schemas.Add(new PdtsText(sheetIndex, cellRef, schemaText));
            clone.pdtsTexts = schemas.AsReadOnly();
            return clone;
        }
    
    }

}
