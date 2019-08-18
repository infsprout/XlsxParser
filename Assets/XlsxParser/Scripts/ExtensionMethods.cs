using UnityEngine;
using System.Collections;

using WebRequest = UnityEngine.Networking.UnityWebRequest;

namespace XlsxParser
{
    using WebRequestCreator = System.Func<string, WebRequest>;
    public static class ExtensionMethods
    {
        public static string ToSingleLine(this string src)
        {
            if (src == null) {
                throw new System.ArgumentNullException("src");
            }
            return src.Replace("\r", @"\r").Replace("\n", @"\n");
        }

        public static double ToNumber(this string src)
        {
            return double.Parse(src);
        }

        public static T ToEnum<T>(this string src)
        {
            var t = typeof(T);
            if (!t.IsEnum) {
                throw new System.ArgumentException(
                    "Type '" + t + "' is not enum."
                );
            }
            return (T)System.Enum.Parse(typeof(T), src);
        }

        public static XlsxRequest SetWebRequestCreator(
            this string uri, WebRequestCreator webRequestCreator)
        {
            return new XlsxRequest(uri).SetWebRequestCreator(webRequestCreator);
        }

        public static XlsxRequest SetPassword(this string uri, string password)
        {
            return new XlsxRequest(uri).SetPassword(password);
        }

        public static XlsxRequest SetFieldTypeConverter(
            this string uri, DataTable.IFieldTypeConverter ftc)
        {
            return new XlsxRequest(uri).SetFieldTypeConverter(ftc);
        }

        public static XlsxRequest AddPdtsText(
            this string uri, int sheetIndex, string schemaText)
        {
            return new XlsxRequest(uri).AddPdtsText(sheetIndex, schemaText);
        }

        public static XlsxRequest[] SetDefaultWebRequestCreator(
            this XlsxRequest[] src, WebRequestCreator webRequestCreator)
        {
            if (src == null) {
                throw new System.ArgumentNullException("src");
            }
            var dst = new XlsxRequest[src.Length];
            for (var n = 0; n < dst.Length; ++n) {
                dst[n] = src[n];
                if (dst[n].webRequestCreator == null) {
                    dst[n] = dst[n].SetWebRequestCreator(webRequestCreator);
                }
            }
            return dst;
        }

        public static XlsxRequest[] SetDefaultPassword(
            this XlsxRequest[] src, string password)
        {
            if (src == null) {
                throw new System.ArgumentNullException("src");
            }
            var dst = new XlsxRequest[src.Length];
            for (var n = 0; n < dst.Length; ++n) {
                dst[n] = src[n];
                if (string.IsNullOrEmpty(dst[n].password)) {
                    dst[n] = dst[n].SetPassword(password);
                }
            }
            return dst;
        }

        public static XlsxRequest[] SetDefaultFieldTypeConverter(
            this XlsxRequest[] src, DataTable.IFieldTypeConverter ftc)
        {
            if (src == null) {
                throw new System.ArgumentNullException("src");
            }
            var dst = new XlsxRequest[src.Length];
            for (var n = 0; n < dst.Length; ++n) {
                dst[n] = src[n];
                if (dst[n].fieldTypeConverter == null) {
                    dst[n] = dst[n].SetFieldTypeConverter(ftc);
                }
            }
            return dst;
        }
        
    }

}
