using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace XlsxParser.Internal
{

    using Schema = DataTableSchema;
    using PropertyMapping = DataTable.PropertyMappingAttribute;
    using PropertyMap = Dictionary<string, string>;
    using Warnings = DataTable.Warnings;

    internal sealed class DataTablePopulator<T>
    {
        private sealed class _MemberAccessor
        {
            public MemberInfo info { get; private set; }
            public System.Type valueType { get; private set; }
            public ConstructorInfo valueConstructor { get; private set; }
            public int arrayIndex { get; private set; }
            public bool canRead { get; private set; }
            public bool canWrite { get; private set; }
            public System.Type listItemType { get; private set; }
            public ConstructorInfo listItemConstructor { get; private set; }
            public MethodInfo listReadOnlyGetter { get; private set; }
            public MethodInfo listItemCountGetter { get; private set; }
            public MethodInfo listItemAdder { get; private set; }
            public MethodInfo listItemGetter { get; private set; }
            public MethodInfo listItemSetter { get; private set; }
            public string sourcePath { get; private set; }
            public string targetPath { get; private set; }
            public MethodInfo valueGetter { get; private set; }
            public MethodInfo valueSetter { get; private set; }
            public int declarerIndex { get; private set; }
            public int mappedFieldIndex { get; private set; }
            public object valueCache;

            public _MemberAccessor(
                MemberInfo mi, int arrayIndex,
                string sourcePath, string targetPath,
                int declarerIndex, int mappedFieldIndex)
            {
                info = mi;
                valueType = _GetMemberValueType(mi);
                valueConstructor = _GetConstructor(valueType);
                this.arrayIndex = arrayIndex;
                canRead = _IsReadableMember(mi);
                canWrite = _IsWritableMember(mi);
                listItemType = typeof(_NonListItem);
                if (arrayIndex >= 0) {
                    listItemType = _GetListItemType(valueType);
                }
                listItemConstructor = _GetConstructor(listItemType);
                listReadOnlyGetter = _GetListReadOnlyGetter(listItemType);
                listItemCountGetter = _GetListItemCountGetter(listItemType);
                listItemAdder = _GetListItemAdder(listItemType);
                listItemGetter = _GetListItemGetter(listItemType);
                listItemSetter = _GetListItemSetter(listItemType);
                this.sourcePath = sourcePath;
                this.targetPath = targetPath;
                valueGetter = _GetMemberValueGetter(valueType, listItemType);
                valueSetter = _GetMemberValueSetter(valueType, listItemType);
                this.declarerIndex = declarerIndex;
                this.mappedFieldIndex = mappedFieldIndex;
            }
        }

        private sealed class _MemberAccessingFailure
        {
            public Schema.Field field { get; private set; }
            public _MemberAccessor memberAccessor { get; private set; }
            public string nextMemberName { get; private set; }
            public string cause { get; private set; }

            public _MemberAccessingFailure(
                Schema.Field field, _MemberAccessor ma,
                string nextMemberName, string cause)
            {
                this.field = field;
                this.memberAccessor = ma;
                this.nextMemberName = nextMemberName;
                this.cause = cause;
            }
        }

        private sealed class _NonListItem { }

        public System.WeakReference tableRef { get; private set; }

        private List<_MemberAccessor> _memberAccessors;

        private List<_MemberAccessingFailure> _memberAccessingFailures;

        private Warnings _objectMappingWarnings;
        public Warnings objectMappingWarnings {
            get {
                if (_objectMappingWarnings == null) {
                    _objectMappingWarnings = _GetObjectMappingWarnings();
                }
                return _objectMappingWarnings;
            }
        }

        public DataTablePopulator(DataTable table)
        {
            this.tableRef = new System.WeakReference(table);
            _InitMemberAccessors();
        }

        public Warnings Populate(IList<T> dst, System.Func<T> creator)
        {
            var src = (DataTable)tableRef.Target;
            if (!src.isValid) {
                throw new System.InvalidOperationException();
            }
            if (dst == null) {
                throw new System.ArgumentNullException("dst");
            }
            if (dst is T[] || dst.IsReadOnly) {
                throw new System.ArgumentException(
                    "'dst' must not be 'FixedSize' or 'ReadOnly'."
                );
            }
            if (creator == null) {
                throw new System.ArgumentNullException("creator");
            }
            while (dst.Count < src.rowCount) {
                dst.Add(creator());
            }
            var warnings = new StringBuilder();
            for (var n = 0; n < src.rowCount; ++n) {
                object obj = dst[n];
                if (obj == null) {
                    obj = creator();
                }
                _Populate(n, ref obj, warnings);
                dst[n] = (T)obj;
            }
            return new Warnings(warnings);
        }

        public Warnings Populate(
            IDictionary<string, T> dst,
            System.Func<T> creator, int keyFieldIndex)
        {
            var src = (DataTable)tableRef.Target;
            if (!src.isValid) {
                throw new System.InvalidOperationException();
            }
            if (dst == null) {
                throw new System.ArgumentNullException("dst");
            }
            if (dst.IsReadOnly) {
                throw new System.ArgumentException(
                    "'dst' must not be 'ReadOnly'."
                );
            }
            if (creator == null) {
                throw new System.ArgumentNullException("creator");
            }
            if (keyFieldIndex >= src.schema.fieldCount) {
                throw new System.ArgumentOutOfRangeException("keyFieldIndex");
            }
            var warnings = new StringBuilder();
            for (var n = 0; n < src.rowCount; ++n) {
                var k = "" + n;
                if (keyFieldIndex >= 0) {
                    k = "" + src[n][keyFieldIndex];
                }
                var v = default(T);
                dst.TryGetValue(k, out v);
                if (v == null) {
                    v = creator();
                }
                object obj = v;
                _Populate(n, ref obj, warnings);
                dst[k] = (T)obj;
            }
            return new Warnings(warnings);
        }

        public Warnings Populate(int rowIndex, ref object dst)
        {
            var src = (DataTable)tableRef.Target;
            if (!src.isValid) {
                throw new System.InvalidOperationException();
            }
            if (rowIndex < 0 || rowIndex >= src.rowCount) {
                throw new System.ArgumentOutOfRangeException("rowIndex");
            }
            if (dst == null) {
                throw new System.ArgumentNullException("dst");
            }
            var warnings = new StringBuilder();
            _Populate(rowIndex, ref dst, warnings);
            return new Warnings(warnings);
        }

        #region private methods        

        private static MemberInfo _GetMemberInfo(System.Type t, string name)
        {
            var flags = BindingFlags.Instance
            | BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.GetField | BindingFlags.GetProperty;
            var mis = t.GetMember(name, flags);
            return (mis.Length == 0) ? null : mis[0];
        }

        private static System.Type _GetMemberValueType(MemberInfo mi)
        {
            var pi = mi as PropertyInfo;
            var fi = mi as FieldInfo;
            return (pi != null) ? pi.PropertyType : fi.FieldType;
        }

        private static bool _IsReadableMember(MemberInfo mi)
        {
            var pi = mi as PropertyInfo;
            return (pi != null) ? pi.CanRead : true;
        }

        private static bool _IsIndexedMember(MemberInfo mi)
        {
            var pi = mi as PropertyInfo;
            return (pi == null) ? false : pi.GetIndexParameters().Length > 0;
        }

        private static bool _IsWritableMember(MemberInfo mi)
        {
            var pi = mi as PropertyInfo;
            var fi = mi as FieldInfo;
            return (pi != null) ? pi.CanWrite : !fi.IsInitOnly;
        }

        private static ConstructorInfo _GetConstructor(System.Type t)
        {
            var flags
            = BindingFlags.Instance
            | BindingFlags.Public | BindingFlags.NonPublic;
            return t.GetConstructor(flags, null, System.Type.EmptyTypes, null);
        }

        private static System.Type _GetListItemType(System.Type t)
        {
            if (t.IsArray) {
                return t.GetElementType();
            }
            if (!t.IsGenericType) {
                return typeof(_NonListItem);
            }
            var itemType = t.GetGenericArguments()[0];
            var listType = typeof(IList<>).MakeGenericType(itemType);
            if (listType.IsAssignableFrom(t)) {
                return itemType;
            }
            return typeof(_NonListItem);
        }

        private static MethodInfo _GetListReadOnlyGetter(System.Type itemType)
        {
            var listType = typeof(ICollection<>).MakeGenericType(itemType);
            return listType.GetMethod("get_IsReadOnly");
        }

        private static MethodInfo _GetListItemCountGetter(System.Type itemType)
        {
            var listType = typeof(ICollection<>).MakeGenericType(itemType);
            return listType.GetMethod("get_Count");
        }

        private static MethodInfo _GetListItemAdder(System.Type itemType)
        {
            var listType = typeof(ICollection<>).MakeGenericType(itemType);
            return listType.GetMethod("Add");
        }

        private static MethodInfo _GetListItemGetter(System.Type itemType)
        {
            var listType = typeof(IList<>).MakeGenericType(itemType);
            return listType.GetMethod("get_Item");
        }

        private static MethodInfo _GetListItemSetter(System.Type itemType)
        {
            var listType = typeof(IList<>).MakeGenericType(itemType);
            return listType.GetMethod("set_Item");
        }

        private static MethodInfo _GetMemberValueGetter(
            System.Type valueType, System.Type listItemType)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var t = typeof(DataTablePopulator<T>);
            return t.GetMethod("_GetMemberValue", flags);
        }

        private static MethodInfo _GetMemberValueSetter(
            System.Type valueType, System.Type listItemType)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var t = typeof(DataTablePopulator<T>);
            return t.GetMethod("_SetMemberValue", flags);
        }

        private static PropertyMap _CreatePropertyMap()
        {
            var at = typeof(PropertyMapping);
            var attrs = typeof(T).GetCustomAttributes(at, true);
            var map = new PropertyMap();
            for (var n = attrs.Length - 1; n >= 0; --n) {
                var attr = (PropertyMapping)attrs[n];
                map[attr.source] = attr.target;
            }
            return map;
        }

        private void _InitMemberAccessors()
        {
            var propMap = _CreatePropertyMap();
            var accessors = new List<_MemberAccessor>();
            var failures = new List<_MemberAccessingFailure>();
            var fields = ((DataTable)tableRef.Target).schema.fieldsForWriter;
            var d = 0;
            var n = -1;
            var declarerStart = 0;
            var lastAccessorCount = 0;
            System.Func<string> getSourcePath = () => {
                var dotCount = 0;
                for (var m = 0; m < fields[n].name.Length; ++m) {
                    if (fields[n].name[m] == '.') {
                        if (++dotCount > d) {
                            return fields[n].name.Substring(0, m);
                        }
                    }
                }
                return fields[n].name;
            };
            System.Func<string, string> getTargetName = (sourcePath) => {
                var sb = new StringBuilder();
                foreach (var token in sourcePath.Split('.', '[')) {
                    if (!char.IsNumber(token[0])) {
                        sb.Append(token + ".");
                    }
                }
                if (sb.Length > 0) {
                    sb.Remove(sb.Length - 1, 1);
                }
                string v = null;
                if (propMap.TryGetValue(sb.ToString(), out v)) {
                    return v;
                }
                return fields[n].nameNodes[d].name;
            };
            System.Func<string, int> getDeclarerIndex = (sourcePath) => {
                for (var m = declarerStart; m < lastAccessorCount; ++m) {
                    if (sourcePath.StartsWith(accessors[m].sourcePath)) {
                        return m;
                    }
                }
                return -1;
            };
            System.Func<int, System.Type>
            getMemberValueType = (declarerIndex) => {
                if (declarerIndex < 0) {
                    return typeof(T);
                }
                var ma = accessors[declarerIndex];
                return (ma.arrayIndex < 0) ? ma.valueType : ma.listItemType;
            };
            System.Func<MemberInfo, string>
            verifyMemberInfo = (mi) => {
                if (mi == null) {
                    return "target '{0}' not found.";
                } else if (!_IsReadableMember(mi)) {
                    return "target '{0}' is not readable.";
                } else if (_IsIndexedMember(mi)) {
                    return "target '{0}' is indexer.";
                }
                var ai = fields[n].nameNodes[d].arrayIndex;
                var vt = _GetMemberValueType(mi);
                var lit = _GetListItemType(vt);
                vt = (ai < 0) ? vt : lit;
                if (ai >= 0 && lit == typeof(_NonListItem)) {
                    return "target '{0}' is not 'IList<>' implementation.";
                }
                if (ai < 0 && !_IsWritableMember(mi)) {
                    if (d == fields[n].depth) {
                        return "target '{0}' must be writable.";
                    } else if (vt.IsValueType) {
                        return ""
                            + "If target '{0}' is 'ValueType', "
                            + "must be writable.";
                    }
                }
                if (d < fields[n].depth) {
                    return null;
                }
                if (!vt.IsValueType && vt != typeof(string)) {
                    return "target '{0}' is not 'ValueType' or 'string'.";
                }
                return null;
            };
            System.Func<int, string, string>
            getTargetPath = (declarerIndex, targetName) => {
                var sb = new StringBuilder();
                if (declarerIndex >= 0) {
                    sb.Append(accessors[declarerIndex].targetPath + ".");
                }
                var nd = fields[n].nameNodes[d];
                sb.Append(targetName);
                if (nd.isArrayElement) {
                    sb.Append("[" + nd.arrayIndex + "]");
                }
                return sb.ToString();
            };
            string lastSourcePath = null;
            while (true) {
                if (++n >= fields.Count) {
                    ++d;
                    n = -1;
                    if (lastAccessorCount == accessors.Count) {
                        break;
                    } else {
                        declarerStart = lastAccessorCount;
                        lastAccessorCount = accessors.Count;
                        continue;
                    }
                }
                if (d > fields[n].depth) {
                    continue;
                }
                var ai = fields[n].nameNodes[d].arrayIndex;
                var sp = getSourcePath();
                if (sp == lastSourcePath) {
                    continue;
                }
                lastSourcePath = sp;
                var tn = getTargetName(sp);
                var di = getDeclarerIndex(sp);
                if (di < 0 && d > 0) {
                    continue;
                }
                var ma = (di < 0) ? null : accessors[di];
                var mvt = getMemberValueType(di);
                var mi = _GetMemberInfo(mvt, tn);
                var err = verifyMemberInfo(mi);
                if (!string.IsNullOrEmpty(err)) {
                    err = string.Format(err, tn);
                    failures.Add(
                        new _MemberAccessingFailure(fields[n], ma, tn, err)
                    );
                    continue;
                }
                var tp = getTargetPath(di, tn);
                var mfi = (d < fields[n].depth) ? -1 : fields[n].index;
                ma = new _MemberAccessor(mi, ai, sp, tp, di, mfi);
                accessors.Add(ma);
            }
            _memberAccessors = accessors;
            _memberAccessingFailures = failures;
        }

        private HashSet<string> _CreatePropertyMappingSourceCandidates()
        {
            var set = new HashSet<string>();
            var schema = ((DataTable)tableRef.Target).schema;
            foreach (var field in schema) {
                var sb = new StringBuilder();
                foreach (var node in field.nameNodes) {
                    sb.Append(((sb.Length == 0) ? "" : ".") + node.name);
                    set.Add(sb.ToString());
                }
            }
            return set;
        }

        private List<PropertyMapping> _CreateNormalizedPropertyMappings()
        {
            var srcSet = new HashSet<string>();
            var mappings = new List<PropertyMapping>();
            var at = typeof(PropertyMapping);
            var attrs = typeof(T).GetCustomAttributes(at, true);
            foreach (PropertyMapping attr in attrs) {
                if (srcSet.Add(attr.source)) {
                    mappings.Add(attr);
                }
            }
            mappings.Sort((a, b) => {
                var aNodes = a.source.Split('.');
                var bNodes = b.source.Split('.');
                var end = System.Math.Min(aNodes.Length, bNodes.Length);
                for (var n = 0; n < end; ++n) {
                    var cmp = aNodes[n].CompareTo(bNodes[n]);
                    if (cmp != 0) {
                        return cmp;
                    }
                }
                return aNodes.Length.CompareTo(bNodes.Length);
            });
            return mappings;
        }

        private Warnings _GetObjectMappingWarnings()
        {
            var warnings = new StringBuilder();
            var srcSet = _CreatePropertyMappingSourceCandidates();
            var mappings = _CreateNormalizedPropertyMappings();
            foreach (var mapping in mappings) {
                if (!srcSet.Contains(mapping.source)) {
                    warnings.AppendLine(_GetFullWarning(
                        "Source '" + mapping.source + "' not exist in table."
                    ));
                }
            }
            for (var n = 1; n < mappings.Count; ++n) {
                var a = mappings[n - 1];
                var b = mappings[n];
                var aNodes = a.source.Split('.');
                var bNodes = b.source.Split('.');
                if (aNodes.Length != bNodes.Length) {
                    continue;
                }
                var m = 0;
                for (; m < aNodes.Length; ++m) {
                    if (aNodes[m] != bNodes[m]) {
                        break;
                    }
                }
                if (m < aNodes.Length - 1 || a.target != b.target) {
                    continue;
                }
                warnings.AppendLine(_GetFullWarning(string.Format(
                    "Source '{0}' and '{1}' have same target '{2}'.",
                    a.source, b.source, a.target
                )));
            }
            foreach (var failure in _memberAccessingFailures) {
                warnings.AppendLine(_GetFullWarning(string.Format(
                    "Field '{0}' not mapped because {1}.",
                    failure.field.name, failure.cause), failure.memberAccessor
                ));
            }
            return new Warnings(warnings);
        }


        private int _FillList(_MemberAccessor ma, object list)
        {
            var isArray = list.GetType().IsArray;
            var isReadOnly = (bool)ma.listReadOnlyGetter.Invoke(list, null);
            var count = (int)ma.listItemCountGetter.Invoke(list, null);
            if (!isArray && isReadOnly) {
                return count;
            }
            if (ma.listItemConstructor != null) {
                for (var n = 0; n < count; ++n) {
                    var item = ma.listItemGetter
                        .Invoke(list, new object[] { n });
                    if (item != null) {
                        continue;
                    }
                    item = ma.listItemConstructor.Invoke(null);
                    ma.listItemSetter.Invoke(list, new object[] { item });
                }
            }
            if (isArray) {
                return count;
            }
            if (ma.listItemConstructor != null) {
                while (count <= ma.arrayIndex) {
                    var item = ma.listItemConstructor.Invoke(null);
                    ma.listItemAdder.Invoke(list, new object[] { item });
                    count = (int)ma.listItemCountGetter.Invoke(list, null);
                }
            } else {
                while (count <= ma.arrayIndex) {
                    var item = default(object);
                    if (ma.listItemType.IsValueType) {
                        item = System.Activator.CreateInstance(ma.listItemType);
                    }
                    ma.listItemAdder.Invoke(list, new object[] { item });
                    count = (int)ma.listItemCountGetter.Invoke(list, null);
                }
            }
            return count;
        }

        private string _GetMemberValue(
            _MemberAccessor ma, object obj, out object value)
        {
            var pi = ma.info as PropertyInfo;
            var fi = ma.info as FieldInfo;
            value = (pi != null) ? pi.GetValue(obj, null) : fi.GetValue(obj);
            if (value == null) {
                if (!ma.canWrite) {
                    return "Member is null but is not writable.";
                }
                if (ma.valueConstructor == null) {
                    return "Default constructor not found.";
                }
                value = ma.valueConstructor.Invoke(null);
                if (pi != null) {
                    pi.SetValue(obj, value, null);
                } else {
                    fi.SetValue(obj, value);
                }
            }
            if (ma.arrayIndex < 0) {
                return null;
            }
            var list = value;
            value = null;
            var count = _FillList(ma, list);
            if (count == 0) {
                return "List is 'ReadOnly' or 'FixedSize' but empty.";
            }
            if (ma.arrayIndex >= count) {
                return "Index is out of range. (IList<>.Count: " + count + ")";
            }
            value = ma.listItemGetter
                .Invoke(list, new object[] { ma.arrayIndex });
            if (value == null) {
                if (ma.listItemConstructor != null) {
                    return "Member must be writable 'IList<>'.";
                }
                return "List item default constructor not found.";
            }
            return null;
        }

        private string _GetTableValue(
            int row, int col, System.Type valueType, out object value)
        {
            value = null;
            var table = (DataTable)tableRef.Target;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var method = table.GetType().GetMethod("_GetCellValue", flags);
            try {
                value = method
                    .Invoke(table, new object[] { row, col, valueType });
            } catch (TargetInvocationException e) {
                if (!(e.InnerException is System.InvalidCastException)) {
                    throw e;
                }
                var t1 = table[row][col].GetType();
                var t2 = method.GetGenericArguments()[0];
                return string.Format(
                    "Can not cast type from '{0}' to '{1}'.",
                    t1.FullName, t2.FullName
                );
            }
            return null;
        }

        private string _SetMemberValue(
            _MemberAccessor ma, object obj, object value)
        {
            var pi = ma.info as PropertyInfo;
            var fi = ma.info as FieldInfo;
            if (ma.arrayIndex < 0) {
                if (pi != null) {
                    pi.SetValue(obj, value, null);
                } else {
                    fi.SetValue(obj, value);
                }
                return null;
            }
            var list = (pi != null) ? pi.GetValue(obj, null) : fi.GetValue(obj);
            if (list == null) {
                if (ma.valueConstructor == null) {
                    return "Default constructor not found.";
                }
                list = ma.valueConstructor.Invoke(null);
            }
            var isReadOnly = (bool)ma.listReadOnlyGetter.Invoke(list, null);
            if (isReadOnly && !list.GetType().IsArray) {
                return "Member must be writable 'IList<>'.";
            }
            var count = _FillList(ma, list);
            if (ma.arrayIndex >= count) {
                return "Index is out of range. (IList<>.Count: " + count + ")";
            }
            ma.listItemSetter
                .Invoke(list, new object[] { ma.arrayIndex, value });
            return null;
        }

        private string _GetFullWarning(
            string message, _MemberAccessor ma = null, int rowIndex = -1)
        {
            var table = (DataTable)tableRef.Target;
            var name = table.name + "=>" + typeof(T).FullName;
            if (ma == null) {
                return string.Format("{0}: {1}", name, message);
            } else if (rowIndex < 0) {
                return string.Format("{0}({2}=>{3}): {1}",
                    name, message, ma.sourcePath, ma.targetPath
                );
            }
            return string.Format("{0}({4}, {2}=>{3}): {1}",
                name, message, ma.sourcePath, ma.targetPath, rowIndex
            );
        }

        private void _Populate(
            int rowIndex, ref object dst, StringBuilder warnings)
        {
            var accessors = _memberAccessors;
            foreach (var ma in accessors) {
                var di = ma.declarerIndex;
                var obj = (di < 0) ? dst : accessors[di].valueCache;
                string warning = null;
                if (ma.mappedFieldIndex < 0) {
                    var parameters = new object[] { ma, obj, null };
                    warning = (string)ma.valueGetter.Invoke(this, parameters);
                    ma.valueCache = parameters[2];
                } else {
                    object v = null;
                    var r = rowIndex;
                    var c = ma.mappedFieldIndex;
                    var t = (ma.arrayIndex < 0)
                          ? ma.valueType : ma.listItemType;
                    warning = _GetTableValue(r, c, t, out v);
                    ma.valueCache = v;
                }
                if (!string.IsNullOrEmpty(warning)) {
                    warnings.AppendLine(_GetFullWarning(warning, ma, rowIndex));
                }
            }
            for (var n = accessors.Count - 1; n >= 0; --n) {
                var ma = accessors[n];
                if (ma.valueCache == null) {
                    continue;
                }
                var di = ma.declarerIndex;
                var obj = (di < 0) ? dst : accessors[di].valueCache;
                if (obj == null || !ma.canWrite) {
                    continue;
                }
                var parameters = new object[] { ma, obj, ma.valueCache };
                var warning = (string)ma.valueSetter.Invoke(this, parameters);
                if (!string.IsNullOrEmpty(warning)) {
                    warnings.AppendLine(_GetFullWarning(warning, ma, rowIndex));
                }
            }
        }

        #endregion
    }

}

