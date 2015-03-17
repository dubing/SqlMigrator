using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;

namespace SqlMigratorWinform.Utility
{
    public static class SerializeHelper
    {
        private static Dictionary<string, Type> _KnownTypes = null;
        private static Dictionary<string, Type> _SimpleTypes = null;

        public static object BinaryDeserialize(byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        public static byte[] BinarySerialize(object source)
        {
            if (source == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, source);
                return ms.ToArray();
            }
        }

        private static bool ContainsKnownType(string name, Type type)
        {
            Type refType;
            if (!KnownTypes.TryGetValue(name, out refType))
            {
                return false;
            }
            return (refType == type);
        }

        private static bool ContainsSimpleType(string name, Type type)
        {
            Type refType;
            if (!SimpleTypes.TryGetValue(name, out refType))
            {
                return false;
            }
            return (refType == type);
        }

        private static object CreateInstance(Type type)
        {
            ConstructorInfo ci = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (ci != null)
            {
                return ci.Invoke(null);
            }
            return null;
        }

        private static string GetCleanName(string name)
        {
            if (name.EndsWith("[]"))
            {
                name = "ArrayOf" + name.Substring(0, name.Length - 2);
            }
            return name.Replace('`', '_');
        }

        private static string GetTypeName(Type type)
        {
            if (type == Type.GetType(type.FullName))
            {
                return type.FullName;
            }
            return type.AssemblyQualifiedName;
        }

        private static bool HasEmptyConstructor(Type type)
        {
            return (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) != null);
        }

        private static bool IsKnownType(Type type)
        {
            if (type.IsArray)
            {
                return KnownTypes.ContainsValue(TypeHelper.GetArrayItemType(type));
            }
            if (type.IsGenericType)
            {
                return false;
            }
            return KnownTypes.ContainsValue(type);
        }

        private static bool IsKnownType(string name, Type type)
        {
            if (type.FullName == "System.RuntimeType")
            {
                type = type.BaseType;
            }
            if (type.IsArray && name.StartsWith("ArrayOf"))
            {
                bool isKnownType = false;
                Type elmType = TypeHelper.GetArrayItemType(type);
                string elmName = name.Substring("ArrayOf".Length);
                isKnownType = ContainsKnownType(elmName, elmType);
                if (!isKnownType)
                {
                    char Param0002 = elmName[0];
                    isKnownType = ContainsSimpleType(Param0002.ToString().ToLower() + elmName.Substring(1), elmType);
                }
                return isKnownType;
            }
            if (type.IsGenericType)
            {
                return false;
            }
            return ContainsKnownType(name, type);
        }

        private static bool IsSimpleType(Type type)
        {
            if (type.IsArray)
            {
                return SimpleTypes.ContainsValue(TypeHelper.GetArrayItemType(type));
            }
            if (type.IsGenericType)
            {
                return false;
            }
            return SimpleTypes.ContainsValue(type);
        }

        public static object StringDeserialize(string objString)
        {
            ObjectStateFormatter osf = new ObjectStateFormatter();
            return osf.Deserialize(objString);
        }

        public static string StringSerialize(object source)
        {
            ObjectStateFormatter osf = new ObjectStateFormatter();
            return osf.Serialize(source);
        }

        private static Type TryGetObjectType(XmlReader reader, Type defauleType)
        {
            string typeName = reader["objectType"];
            Type ret = null;
            if (!string.IsNullOrEmpty(typeName))
            {
                ret = TypeHelper.TryGetType(typeName, null);
            }
            if (ret == null)
            {
                KnownTypes.TryGetValue(reader.Name, out ret);
            }
            if ((ret == null) && reader.Name.StartsWith("ArrayOf"))
            {
                string elmName = reader.Name.Substring("ArrayOf".Length);
                KnownTypes.TryGetValue(elmName, out ret);
                if (ret == null)
                {
                    char Param0002 = elmName[0];
                    elmName = Param0002.ToString().ToLower() + elmName.Substring(1);
                    KnownTypes.TryGetValue(elmName, out ret);
                }
                if (ret != null)
                {
                    ret = Array.CreateInstance(ret, 0).GetType();
                }
            }
            return ((ret == null) ? defauleType : ret);
        }

        private static Type TryGetType(XmlReader reader, string attributeName, Type defaultType)
        {
            string typeName = reader[attributeName];
            if (string.IsNullOrEmpty(typeName))
            {
                return defaultType;
            }
            return TypeHelper.TryGetType(typeName, defaultType);
        }

        public static object XmlDeserialize(XmlNode objNode)
        {
            return XmlDeserialize(objNode, null);
        }

        public static object XmlDeserialize(XmlNode objNode, Type objType)
        {
            if (objNode == null)
            {
                return null;
            }
            XmlAttribute xsiAttr = XmlHelper.SetAttribute(objNode, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance", "http://www.w3.org/2000/xmlns/");
            XmlAttribute xsdAttr = XmlHelper.SetAttribute(objNode, "xmlns:xsd", "http://www.w3.org/2001/XMLSchema", "http://www.w3.org/2000/xmlns/");
            XmlNodeReader reader = new XmlNodeReader(objNode);
            if (!reader.Read())
            {
                return null;
            }
            object obj = XmlReaderDeserialize(reader, objType);
            XmlHelper.DeleteAttribute(xsiAttr);
            XmlHelper.DeleteAttribute(xsdAttr);
            return obj;
        }

        public static void XmlDeserializeChildElements(XmlReader reader, ReadChildElementHandler readElements, params object[] args)
        {
            int startDepth = reader.Depth;
            string startName = reader.Name;
            bool isContinue = reader.Read();
            while (isContinue)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    readElements(reader, args);
                }
                else if (((reader.NodeType == XmlNodeType.EndElement) && (reader.Depth == startDepth)) && (reader.Name == startName))
                {
                    reader.Read();
                    isContinue = false;
                }
                else
                {
                    isContinue = reader.Read();
                }
            }
        }

        private static object XmlDeserializeIDictionary(XmlReader reader, Type objType, Type keyType, Type valueType)
        {
            object key;
            object value;
            Type itemKeyType;
            Type itemValueType;
            IDictionary obj = Activator.CreateInstance(objType) as IDictionary;
            XmlDeserializeChildElements(reader, delegate(XmlReader _reader, object[] args)
            {
                itemKeyType = TryGetType(reader, "keyType", keyType);
                itemValueType = TryGetType(reader, "valueType", valueType);
                if (reader.Read())
                {
                    key = XmlReaderDeserialize(reader, itemKeyType);
                    value = XmlReaderDeserialize(reader, itemValueType);
                    obj[key] = value;
                }
            }, new object[0]);
            return obj;
        }

        private static object XmlDeserializeIList(XmlReader reader, Type objType, Type itemType)
        {
            object item;
            ArrayList list = new ArrayList();
            if (objType.IsArray)
            {
                itemType = TypeHelper.GetArrayItemType(objType);
            }
            XmlDeserializeChildElements(reader, delegate(XmlReader _reader, object[] args)
            {
                item = XmlReaderDeserialize(reader, itemType);
                list.Add(item);
            }, new object[0]);
            if (objType.IsArray)
            {
                Array array = Array.CreateInstance(itemType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i], i);
                }
                return array;
            }
            IList obj = Activator.CreateInstance(objType) as IList;
            foreach (object o in list)
            {
                obj.Add(o);
            }
            return obj;
        }

        private static object XmlDeserializeNameObjects(XmlReader reader, Type objType, Type valueType)
        {
            string key;
            object value;
            Type itemValueType;
            NameObjectCollectionBase obj = Activator.CreateInstance(objType) as NameObjectCollectionBase;
            MethodInfo baseSet = objType.GetMethod("BaseSet", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { Generic<string>.Type, Generic<object>.Type }, null);
            XmlDeserializeChildElements(reader, delegate(XmlReader _reader, object[] args)
            {
                itemValueType = TryGetType(reader, "valueType", valueType);
                key = reader["name"];
                if (reader.Read())
                {
                    value = XmlReaderDeserialize(reader, itemValueType);
                    baseSet.Invoke(obj, new object[] { key, value });
                }
            }, new object[0]);
            return obj;
        }

        public static object XmlReaderDeserialize(XmlReader reader)
        {
            return XmlReaderDeserialize(reader, null);
        }

        public static object XmlReaderDeserialize(XmlReader reader, Type objType)
        {
            Type valueType;
            object Param0003;
            if (reader == null)
            {
                return null;
            }
            objType = TryGetObjectType(reader, objType);
            if (objType == null)
            {
                objType = Generic<object>.Type;
            }
            if (IsSimpleType(objType))
            {
                XmlSerializer serializer = new XmlSerializer(objType);
                return serializer.Deserialize(reader);
            }
            if (objType == typeof(Type))
            {
                return Type.GetType(reader.ReadElementString());
            }
            if (objType == typeof(Color))
            {
                bool isNamedColor = false;
                if (!bool.TryParse(reader["IsNamedColor"], out isNamedColor))
                {
                    isNamedColor = false;
                }
                string color = reader.ReadElementString();
                if (isNamedColor)
                {
                    return Color.FromName(color);
                }
                return Color.FromArgb(int.Parse(color, NumberStyles.HexNumber));
            }
            if (TypeHelper.IsInheritBase(objType, typeof(NameObjectCollectionBase)) && HasEmptyConstructor(objType))
            {
                valueType = TryGetType(reader, "valueType", null);
                return XmlDeserializeNameObjects(reader, objType, valueType);
            }
            if ((objType.GetInterface("IDictionary") != null) && HasEmptyConstructor(objType))
            {
                Type keyType = TryGetType(reader, "keyType", null);
                valueType = TryGetType(reader, "valueType", null);
                return XmlDeserializeIDictionary(reader, objType, keyType, valueType);
            }
            if ((objType.GetInterface("IList") != null) && (objType.IsArray || HasEmptyConstructor(objType)))
            {
                Type itemType = TryGetType(reader, "itemType", null);
                return XmlDeserializeIList(reader, objType, itemType);
            }
            try
            {
                Param0003 = new XmlSerializer(objType).Deserialize(reader);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(string.Format("The {0} is not supported by XmlDeserialize.", objType.FullName), ex);
            }
            return Param0003;
        }

        public static XmlNode XmlSerialize(object source)
        {
            return XmlSerialize(source, true);
        }

        public static XmlNode XmlSerialize(object source, bool writeObjectType)
        {
            XmlDocument document = new XmlDocument();
            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
                writer.WriteStartElement("XmlSerialize");
                writer.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
                XmlWriterSerialize(writer, source);
                writer.WriteEndElement();
                writer.Flush();
                ms.Position = 0L;
                document.Load(ms);
            }
            XmlNode result = document.DocumentElement.ChildNodes[0];
            Type objType = (source == null) ? Generic<object>.Type : source.GetType();
            if (!((!writeObjectType || (result.Attributes["objectType"] != null)) || IsKnownType(result.Name, objType)))
            {
                XmlHelper.SetAttribute(result, "objectType", GetTypeName(objType));
            }
            return result;
        }

        private static void XmlSerializeIDictionary(XmlWriter writer, IDictionary obj, Type keyType, Type valueType)
        {
            Type objType = obj.GetType();
            writer.WriteStartElement(GetCleanName(objType.Name));
            IDictionaryEnumerator de = obj.GetEnumerator();
            if (objType.IsGenericType)
            {
                if (keyType == null)
                {
                    string keyTypeName = Arithmetic.GetInlineItem(objType.FullName, 1, '[', ']');
                    if (!string.IsNullOrEmpty(keyTypeName))
                    {
                        keyType = Type.GetType(keyTypeName);
                    }
                }
                if (valueType == null)
                {
                    string valueTypeName = Arithmetic.GetInlineItem(objType.FullName, 2, '[', ']');
                    if (!string.IsNullOrEmpty(valueTypeName))
                    {
                        valueType = Type.GetType(valueTypeName);
                    }
                }
            }
            bool writeKeyType = (keyType == null) || keyType.Equals(Generic<object>.Type);
            if (!(writeKeyType || SimpleTypes.ContainsValue(keyType)))
            {
                writer.WriteAttributeString("keyType", GetTypeName(keyType));
            }
            bool writeValueType = (valueType == null) || valueType.Equals(Generic<object>.Type);
            if (!(writeValueType || SimpleTypes.ContainsValue(valueType)))
            {
                writer.WriteAttributeString("valueType", GetTypeName(valueType));
            }
            while (de.MoveNext())
            {
                Type type;
                writer.WriteStartElement("Item");
                if (de.Key != null)
                {
                    type = de.Key.GetType();
                    if (!((!writeKeyType && (type == keyType)) || IsKnownType(type)))
                    {
                        writer.WriteAttributeString("keyType", GetTypeName(type));
                    }
                }
                if (de.Value != null)
                {
                    type = de.Value.GetType();
                    if (!((!writeValueType && (type == valueType)) || IsKnownType(type)))
                    {
                        writer.WriteAttributeString("valueType", GetTypeName(type));
                    }
                }
                XmlWriterSerialize(writer, de.Key, keyType);
                XmlWriterSerialize(writer, de.Value, valueType);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void XmlSerializeIList(XmlWriter writer, IList obj, Type itemType)
        {
            Type objType = obj.GetType();
            writer.WriteStartElement(GetCleanName(objType.Name));
            IEnumerator etor = obj.GetEnumerator();
            if (itemType == null)
            {
                if (objType.IsArray)
                {
                    itemType = TypeHelper.GetArrayItemType(objType);
                }
                else if (objType.IsGenericType)
                {
                    string itemTypeName = Arithmetic.GetInlineItem(objType.AssemblyQualifiedName, 1, '[', ']');
                    if (!string.IsNullOrEmpty(itemTypeName))
                    {
                        itemType = Type.GetType(itemTypeName);
                    }
                }
            }
            bool writeItemType = (itemType == null) || itemType.Equals(Generic<object>.Type);
            if (!(writeItemType || SimpleTypes.ContainsValue(itemType)))
            {
                writer.WriteAttributeString("itemType", GetTypeName(itemType));
            }
            while (etor.MoveNext())
            {
                object item = etor.Current;
                if (item != null)
                {
                    Type type = item.GetType();
                    if (!((!writeItemType && (type == itemType)) || IsKnownType(type)))
                    {
                        XmlNode element = XmlSerialize(item, true);
                        writer.WriteNode(new XmlNodeReader(element), true);
                    }
                    else
                    {
                        XmlWriterSerialize(writer, item, itemType);
                    }
                }
                else if (itemType == null)
                {
                    XmlWriterSerialize(writer, null, Generic<object>.Type);
                }
                else
                {
                    XmlWriterSerialize(writer, null, itemType);
                }
            }
            writer.WriteEndElement();
        }

        private static void XmlSerializeNameObjects(XmlWriter writer, NameObjectCollectionBase obj, Type valueType)
        {
            Type objType = obj.GetType();
            writer.WriteStartElement(GetCleanName(objType.Name));
            bool writeValueType = (valueType == null) || valueType.Equals(Generic<object>.Type);
            if (!writeValueType)
            {
                writer.WriteAttributeString("valueType", GetTypeName(valueType));
            }
            object[] values = objType.GetMethod("BaseGetAllValues", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(obj, null) as object[];
            for (int i = 0; i < values.Length; i++)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("name", obj.Keys[i]);
                if (values[i] != null)
                {
                    Type type = values[i].GetType();
                    if (!((!writeValueType && (type == valueType)) || IsKnownType(type)))
                    {
                        writer.WriteAttributeString("valueType", GetTypeName(type));
                    }
                }
                XmlWriterSerialize(writer, values[i], valueType);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public static void XmlWriterSerialize(XmlWriter writer, object source)
        {
            XmlWriterSerialize(writer, source, null);
        }

        public static void XmlWriterSerialize(XmlWriter writer, object source, Type refType)
        {
            if (writer != null)
            {
                Type objType = (source == null) ? Generic<object>.Type : source.GetType();
                if (((source == null) && (refType != null)) && KnownTypes.ContainsValue(refType))
                {
                    objType = refType;
                }
                if ((source == null) || IsSimpleType(objType))
                {
                    XmlSerializer serializer = new XmlSerializer(objType);
                    serializer.Serialize(writer, source);
                }
                else if (source is Type)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteString(GetTypeName(source as Type));
                    writer.WriteEndElement();
                }
                else if (source is Color)
                {
                    Color color = (Color)source;
                    writer.WriteStartElement("Color");
                    if (color.IsNamedColor)
                    {
                        writer.WriteAttributeString("IsNamedColor", bool.TrueString);
                        writer.WriteString(color.Name);
                    }
                    else
                    {
                        writer.WriteString(color.ToArgb().ToString("x"));
                    }
                    writer.WriteEndElement();
                }
                else if ((source is NameObjectCollectionBase) && HasEmptyConstructor(objType))
                {
                    XmlSerializeNameObjects(writer, source as NameObjectCollectionBase, null);
                }
                else if ((source is IDictionary) && HasEmptyConstructor(objType))
                {
                    XmlSerializeIDictionary(writer, source as IDictionary, null, null);
                }
                else if ((source is IList) && (objType.IsArray || HasEmptyConstructor(objType)))
                {
                    XmlSerializeIList(writer, source as IList, null);
                }
                else
                {
                    try
                    {
                        new XmlSerializer(objType).Serialize(writer, source);
                    }
                    catch (Exception ex)
                    {
                        throw new NotSupportedException(string.Format("The {0} is not supported by XmlSerialize.", objType.FullName), ex);
                    }
                }
            }
        }

        public static Dictionary<string, Type> KnownTypes
        {
            get
            {
                if (_KnownTypes == null)
                {
                    _KnownTypes = new Dictionary<string, Type>(SimpleTypes);
                    _KnownTypes["Type"] = typeof(Type);
                    _KnownTypes["Color"] = typeof(Color);
                    _KnownTypes["DBNull"] = typeof(DBNull);
                    _KnownTypes["ArrayList"] = typeof(ArrayList);
                    _KnownTypes["Hashtable"] = typeof(Hashtable);
                    _KnownTypes["HybridDictionary"] = typeof(HybridDictionary);
                    _KnownTypes["Queue"] = typeof(Queue);
                    _KnownTypes["Stack"] = typeof(Stack);
                    _KnownTypes["SortedList"] = typeof(SortedList);
                    _KnownTypes["NameValueCollection"] = typeof(NameValueCollection);
                    _KnownTypes["TraceObject"] = typeof(TraceObject);
                }
                return _KnownTypes;
            }
        }

        private static Dictionary<string, Type> SimpleTypes
        {
            get
            {
                if (_SimpleTypes == null)
                {
                    _SimpleTypes = new Dictionary<string, Type>();
                    _SimpleTypes["string"] = Generic<string>.Type;
                    _SimpleTypes["dateTime"] = Generic<DateTime>.Type;
                    _SimpleTypes["boolean"] = Generic<bool>.Type;
                    _SimpleTypes["unsignedByte"] = Generic<byte>.Type;
                    _SimpleTypes["char"] = Generic<char>.Type;
                    _SimpleTypes["decimal"] = Generic<decimal>.Type;
                    _SimpleTypes["double"] = Generic<double>.Type;
                    _SimpleTypes["guid"] = Generic<Guid>.Type;
                    _SimpleTypes["short"] = Generic<short>.Type;
                    _SimpleTypes["int"] = Generic<int>.Type;
                    _SimpleTypes["long"] = Generic<long>.Type;
                    _SimpleTypes["byte"] = Generic<sbyte>.Type;
                    _SimpleTypes["float"] = Generic<float>.Type;
                    _SimpleTypes["unsignedShort"] = Generic<ushort>.Type;
                    _SimpleTypes["unsignedInt"] = Generic<uint>.Type;
                    _SimpleTypes["unsignedLong"] = Generic<ulong>.Type;
                    _SimpleTypes["base64Binary"] = Generic<byte[]>.Type;
                }
                return _SimpleTypes;
            }
        }
    }
}
