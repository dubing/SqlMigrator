using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace SqlMigratorWinform.Utility
{
    public static class XmlHelper
    {
        public const string XSLURI = "http://www.w3.org/1999/XSL/Transform";

        public static XmlAttribute AddAttribute(XmlNode source, XmlAttribute attr)
        {
            return attr != null ? SetAttribute(source, attr.Name, attr.Value, attr.NamespaceURI) : null;
        }

        public static XmlNode AddNode(XmlNode destination, XmlNode child)
        {
            if ((destination == null) || (child == null))
            {
                return null;
            }
            bool isOwnerDocument;
            var destnationDocument = GetXmlDocument(destination, out isOwnerDocument);
            var appendNode = CloneNode(destnationDocument, child, true);
            if (appendNode != null)
            {
                if (isOwnerDocument)
                {
                    if (destnationDocument.DocumentElement == null)
                    {
                        destnationDocument.AppendChild(appendNode);
                    }
                    else
                    {
                        destnationDocument.DocumentElement.AppendChild(appendNode);
                    }
                    return appendNode;
                }
                destination.AppendChild(appendNode);
            }
            return appendNode;
        }

        public static XmlDocument ChangeNamespace(XmlDocument node, string sourceNamespace, string destinationNamespace, bool changeAttributeNamespaces)
        {
            XmlDocument retVal = node;
            if (node != null)
            {
                XmlNamespaceManager nsManager = null;
                if (sourceNamespace != null)
                {
                    nsManager = new XmlNamespaceManager(node.NameTable);
                    nsManager.AddNamespace("changenamespacetestprefix", sourceNamespace);
                }
                if (((sourceNamespace != null) && (node.SelectSingleNode("//changenamespacetestprefix:*", nsManager) != null)) || (sourceNamespace == null))
                {
                    retVal = ChangeNamespace(new XmlNodeReader(node), sourceNamespace, destinationNamespace, changeAttributeNamespaces);
                }
            }
            return retVal;
        }

        public static XmlDocument ChangeNamespace(XmlReader reader, string sourceNamespace, string destinationNamespace, bool changeAttributeNamespaces)
        {
            XmlDocument newDoc = null;
            if ((reader != null) && (destinationNamespace != null))
            {
                newDoc = new XmlDocument();
                XmlNode currentInsertionPoint = newDoc;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string nodeNamespace = reader.NamespaceURI;
                        if ((sourceNamespace == null) || (reader.NamespaceURI == sourceNamespace))
                        {
                            nodeNamespace = destinationNamespace;
                        }
                        currentInsertionPoint = newDoc.CreateElement(reader.LocalName, nodeNamespace);
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if ((reader.LocalName != "xmlns") || ((sourceNamespace != null) && (reader.Value != sourceNamespace)))
                                {
                                    string attributeNamespace = reader.NamespaceURI;
                                    if (changeAttributeNamespaces && ((sourceNamespace == null) || (reader.NamespaceURI == sourceNamespace)))
                                    {
                                        attributeNamespace = destinationNamespace;
                                    }
                                    SetAttribute(currentInsertionPoint, reader.Name, reader.Value, attributeNamespace);
                                }
                            }
                            reader.MoveToElement();
                        }
                    }
                    else if (reader.HasValue)
                    {
                        XmlNode textNode = newDoc.CreateTextNode(reader.Value);
                        currentInsertionPoint.AppendChild(textNode);
                    }
                    if ((reader.NodeType == XmlNodeType.EndElement) || ((reader.NodeType == XmlNodeType.Element) && (!reader.IsStartElement() || reader.IsEmptyElement)))
                    {
                        currentInsertionPoint = currentInsertionPoint.ParentNode;
                    }
                }
            }
            return newDoc;
        }

        public static XmlNode CloneNode(XmlDocument refDocument, XmlNode node, bool deep)
        {
            if (node == null)
            {
                return null;
            }
            bool isOwnerDocument = false;
            XmlDocument nodeDocument = GetXmlDocument(node, out isOwnerDocument);
            if ((refDocument == null) || (refDocument == nodeDocument))
            {
                return node.CloneNode(deep);
            }
            if (isOwnerDocument)
            {
                if (nodeDocument.DocumentElement == null)
                {
                    return null;
                }
                return refDocument.ImportNode(nodeDocument.DocumentElement, deep);
            }
            return refDocument.ImportNode(node, deep);
        }

        public static bool ContainsAttribute(XmlNode source, string attributeName)
        {
            return ContainsAttribute(source, attributeName, string.Empty);
        }

        public static bool ContainsAttribute(XmlNode source, string attributeName, string namespaceUri)
        {
            return (GetAttribute(source, attributeName, namespaceUri, false) != null);
        }

        public static bool ContainsNode(XmlNode source, string nodeName)
        {
            return ContainsNode(source, nodeName, string.Empty);
        }

        public static bool ContainsNode(XmlNode source, string nodeName, string namespaceUri)
        {
            return (GetNode(source, nodeName, namespaceUri, false) != null);
        }

        public static void CopyNodeContents(XmlNode destinationNode, XmlNode sourceNode)
        {
            if ((destinationNode != null) && (sourceNode != null))
            {
                if (sourceNode.ChildNodes != null)
                {
                    foreach (XmlNode node in sourceNode.ChildNodes)
                    {
                        AddNode(destinationNode, node);
                    }
                }
                if (sourceNode.Attributes != null)
                {
                    foreach (XmlAttribute attr in sourceNode.Attributes)
                    {
                        AddAttribute(destinationNode, attr);
                    }
                }
            }
        }

        public static XmlDocument CreateXmlDocument(XmlNode node)
        {
            XmlDocument resultDoc = new XmlDocument();
            XmlNode childNode = null;
            if (node is XmlDocument)
            {
                childNode = CloneNode(resultDoc, (node as XmlDocument).DocumentElement, true);
            }
            else if (node is XmlElement)
            {
                childNode = CloneNode(resultDoc, node, true);
            }
            if (childNode != null)
            {
                resultDoc.AppendChild(childNode);
            }
            return resultDoc;
        }

        public static XmlDocument CreateXmlDocument(string rootName, string rootNameSpace)
        {
            if (string.IsNullOrEmpty(rootName))
            {
                return null;
            }
            XmlDocument resultDoc = new XmlDocument();
            XmlNode root = null;
            if (string.IsNullOrEmpty(rootNameSpace))
            {
                root = resultDoc.CreateElement(rootName);
            }
            else
            {
                root = resultDoc.CreateElement(rootName, rootNameSpace);
            }
            resultDoc.AppendChild(root);
            return resultDoc;
        }

        public static XslCompiledTransform CreateXslCompiledTransform(string version, params XmlNode[] nodes)
        {
            XmlDocument document = null;
            if ((nodes != null) && (nodes.Length == 1))
            {
                if (nodes[0].NodeType == XmlNodeType.Document)
                {
                    document = nodes[0].CloneNode(true) as XmlDocument;
                }
                else if ((nodes[0].NodeType == XmlNodeType.Element) && string.Equals(nodes[0].Name, "xsl:stylesheet", StringComparison.InvariantCulture))
                {
                    document = CreateXmlDocument(nodes[0]);
                }
            }
            if ((document == null) && (nodes != null))
            {
                document = CreateXmlDocument("xsl:stylesheet", "http://www.w3.org/1999/XSL/Transform");
                SetAttribute(document.DocumentElement, "version", version);
                foreach (XmlNode node in nodes)
                {
                    AddNode(document.DocumentElement, node);
                }
            }
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(document.CreateNavigator(), null, new XmlUrlResolver());
            return transform;
        }

        public static XmlDocument DataTableToXml(DataTable table)
        {
            ParameterChecker.CheckNull("XmlHelper.DataTableToXml", "table", table);
            if (string.IsNullOrEmpty(table.TableName))
            {
                table.TableName = "Table1";
            }
            using (MemoryStream ms = new MemoryStream())
            {
                table.WriteXml(ms);
                ms.Position = 0L;
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                return doc;
            }
        }

        public static XmlAttribute DeleteAttribute(XmlAttribute attribute)
        {
            if ((attribute != null) && (attribute.OwnerElement != null))
            {
                return attribute.OwnerElement.Attributes.Remove(attribute);
            }
            return null;
        }

        public static XmlAttribute DeleteAttribute(XmlNode source, string attributeName)
        {
            return DeleteAttribute(source, attributeName, string.Empty);
        }

        public static XmlAttribute DeleteAttribute(XmlNode source, string attributeName, string namespaceUri)
        {
            XmlAttribute attr = GetAttribute(source, attributeName, namespaceUri, false);
            if (attr != null)
            {
                return source.Attributes.Remove(attr);
            }
            return null;
        }

        public static XmlNode DeleteNode(XmlNode source)
        {
            if ((source != null) && (source.ParentNode != null))
            {
                return source.ParentNode.RemoveChild(source);
            }
            if (source is XmlAttribute)
            {
                return DeleteAttribute(source as XmlAttribute);
            }
            return null;
        }

        public static XmlNode DeleteNode(XmlNode source, string nodeName)
        {
            return DeleteNode(source, nodeName, string.Empty);
        }

        public static XmlNode DeleteNode(XmlNode source, string nodeName, string namespaceUri)
        {
            XmlNode node = GetNode(source, nodeName, namespaceUri, false);
            if ((node != null) && (node.ParentNode != null))
            {
                return node.ParentNode.RemoveChild(node);
            }
            return null;
        }

        public static void ElevateChildren(XmlNode node)
        {
            if ((node != null) && (node.ParentNode != null))
            {
                XmlNode parentNode = node.ParentNode;
                XmlNode[] nodes = new XmlNode[node.ChildNodes.Count];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = node.ChildNodes[i];
                }
                foreach (XmlNode temp in nodes)
                {
                    parentNode.InsertBefore(temp, node);
                }
            }
        }

        public static XmlAttribute GetAttribute(XmlNode source, string attributeName)
        {
            return GetAttribute(source, attributeName, string.Empty, false);
        }

        public static XmlAttribute GetAttribute(XmlNode source, string attributeName, string namespaceUri, bool createIfMissing)
        {
            if (namespaceUri == null)
            {
                namespaceUri = string.Empty;
            }
            XmlAttribute attr = null;
            if ((source != null) && (source.Attributes != null))
            {
                if (attributeName.IndexOf(':') < 0)
                {
                    attr = source.Attributes[attributeName, namespaceUri];
                }
                else
                {
                    attr = source.Attributes[attributeName];
                }
                if ((createIfMissing && (attr == null)) && (source.OwnerDocument != null))
                {
                    attr = source.OwnerDocument.CreateAttribute(attributeName, namespaceUri);
                    source.Attributes.Append(attr);
                }
            }
            return attr;
        }

        public static string GetAttributeValue(XmlNode source, string attributeName)
        {
            return GetAttributeValue(source, attributeName, null, string.Empty);
        }

        public static string GetAttributeValue(XmlNode source, string attributeName, string defaultValue)
        {
            return GetAttributeValue(source, attributeName, defaultValue, string.Empty);
        }

        public static string GetAttributeValue(XmlNode source, string attributeName, string defaultValue, string namespaceUri)
        {
            XmlAttribute attr = GetAttribute(source, attributeName, namespaceUri, false);
            if (attr == null)
            {
                return defaultValue;
            }
            return attr.Value;
        }

        public static string GetDefinitiveXPath(XmlNode node, out bool isComplex)
        {
            string retVal = null;
            isComplex = false;
            if (node == null)
            {
                return retVal;
            }
            retVal = node.Name;
            if ((node.Attributes == null) || (node.Attributes.Count <= 0))
            {
                return retVal;
            }
            isComplex = true;
            StringBuilder xpath = new StringBuilder();
            xpath.Append(retVal);
            foreach (XmlAttribute attribute in node.Attributes)
            {
                xpath.Append("[@");
                xpath.Append(attribute.Name);
                xpath.Append("=\"");
                xpath.Append(attribute.Value);
                xpath.Append("\"]");
            }
            return xpath.ToString();
        }

        public static string GetInnerText(XmlNode source, string nodeName)
        {
            return GetInnerText(source, nodeName, null, string.Empty);
        }

        public static string GetInnerText(XmlNode source, string nodeName, string defaultValue)
        {
            return GetInnerText(source, nodeName, defaultValue, string.Empty);
        }

        public static string GetInnerText(XmlNode source, string nodeName, string defaultValue, string namespaceUri)
        {
            XmlNode node = GetNode(source, nodeName, namespaceUri, false);
            if (node != null)
            {
                return node.InnerText;
            }
            return defaultValue;
        }

        public static string GetInnerXml(XmlNode source, string nodeName)
        {
            return GetInnerXml(source, nodeName, null, string.Empty);
        }

        public static string GetInnerXml(XmlNode source, string nodeName, string defaultValue)
        {
            return GetInnerXml(source, nodeName, defaultValue, string.Empty);
        }

        public static string GetInnerXml(XmlNode source, string nodeName, string defaultValue, string namespaceUri)
        {
            XmlNode node = GetNode(source, nodeName, namespaceUri, false);
            if (node != null)
            {
                return node.InnerXml;
            }
            return defaultValue;
        }

        public static XmlNode GetNode(XmlNode source, string nodeName)
        {
            return GetNode(source, nodeName, string.Empty, false);
        }

        public static XmlNode GetNode(XmlNode source, string nodeName, string namespaceUri, bool createIfMissing)
        {
            if (namespaceUri == null)
            {
                namespaceUri = string.Empty;
            }
            XmlNode node = null;
            if ((source != null) && (source.ChildNodes != null))
            {
                if (nodeName.IndexOf(':') < 0)
                {
                    node = source[nodeName, namespaceUri];
                }
                else
                {
                    node = source[nodeName];
                }
                if (!createIfMissing || (node != null))
                {
                    return node;
                }
                if (source is XmlDocument)
                {
                    XmlDocument doc = source as XmlDocument;
                    if (doc.DocumentElement == null)
                    {
                        node = doc.CreateElement(nodeName, namespaceUri);
                        source.AppendChild(node);
                    }
                    return node;
                }
                if (source is XmlElement)
                {
                    node = source.OwnerDocument.CreateElement(nodeName, namespaceUri);
                    source.AppendChild(node);
                }
            }
            return node;
        }

        public static XmlDocument GetXmlDocument(XmlNode node)
        {
            bool isOwnerDocument;
            return GetXmlDocument(node, out isOwnerDocument);
        }

        public static XmlDocument GetXmlDocument(XmlNode node, out bool isOwnerDocument)
        {
            XmlDocument retVal = null;
            isOwnerDocument = false;
            if (node == null)
            {
                return retVal;
            }
            if (node.NodeType == XmlNodeType.Document)
            {
                retVal = (XmlDocument)node;
                isOwnerDocument = true;
                return retVal;
            }
            return node.OwnerDocument;
        }

        public static XmlNode LoadToAttributes(XmlNode rootNode, NameValueCollection nameValues, NameValueCollection nameProxy)
        {
            if (((rootNode != null) && (nameValues != null)) && (nameValues.Count > 0))
            {
                string key = null;
                string key2 = null;
                for (int i = 0; i < nameValues.Count; i++)
                {
                    key = nameValues.Keys[i];
                    if (nameProxy != null)
                    {
                        key2 = nameProxy[key];
                    }
                    if (!string.IsNullOrEmpty(key2))
                    {
                        key = key2;
                    }
                    if (!string.IsNullOrEmpty(key))
                    {
                        SetAttribute(rootNode, key.Trim(), nameValues[i]);
                    }
                }
            }
            return rootNode;
        }

        public static XmlNode LoadToChildNodes(XmlNode rootNode, NameValueCollection nameValues)
        {
            return LoadToChildNodes(rootNode, nameValues, null, string.Empty);
        }

        public static XmlNode LoadToChildNodes(XmlNode rootNode, NameValueCollection nameValues, NameValueCollection nameProxy, string namespaceUri)
        {
            if (((rootNode != null) && (nameValues != null)) && (nameValues.Count > 0))
            {
                string key = null;
                string key2 = null;
                for (int i = 0; i < nameValues.Count; i++)
                {
                    key = nameValues.Keys[i];
                    if (nameProxy != null)
                    {
                        key2 = nameProxy[key];
                    }
                    if (!string.IsNullOrEmpty(key2))
                    {
                        key = key2;
                    }
                    if (!string.IsNullOrEmpty(key))
                    {
                        SetInnerText(rootNode, key.Trim(), nameValues[i]);
                    }
                }
            }
            return rootNode;
        }

        public static XmlDocument LoadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        public static XmlNode NextNode(XmlNode node, bool deep)
        {
            if ((node == null) || ((node.NodeType == XmlNodeType.Document) && !deep))
            {
                return null;
            }
            if (node.NodeType == XmlNodeType.Document)
            {
                return ((XmlDocument)node).DocumentElement;
            }
            if (deep && node.HasChildNodes)
            {
                return node.FirstChild;
            }
            XmlNode nodeToReturn = node.NextSibling;
            if (nodeToReturn != null)
            {
                return nodeToReturn;
            }
            return NextNode(node.ParentNode, false);
        }

        public static XmlDocument RemoveNamespace(XmlNode node, bool onlyRemoveEmptyPrfix)
        {
            if (node == null)
            {
                return null;
            }
            XmlSkipingReader reader = new XmlSkipingReader(new StringReader(node.OuterXml))
            {
                OnlySkipEmptyPrfix = onlyRemoveEmptyPrfix,
                RemoveXmlDeclaration = true
            };
            XmlDocument document = new XmlDocument();
            document.Load(reader);
            return document;
        }

        public static XmlNode ReplaceNode(XmlNode source, XmlNode replacement)
        {
            if (source != null)
            {
                if (replacement == null)
                {
                    return DeleteNode(source);
                }
                if (source.OwnerDocument != null)
                {
                    XmlNode newNode = CloneNode(source.OwnerDocument, replacement, true);
                    if (newNode == null)
                    {
                        return DeleteNode(source);
                    }
                    if (source.ParentNode != null)
                    {
                        return source.ParentNode.ReplaceChild(newNode, source);
                    }
                }
            }
            return null;
        }

        public static XmlNode ReplaceTag(XmlNode node, string newTag)
        {
            if ((node == null) || string.IsNullOrEmpty(newTag))
            {
                return null;
            }
            if (string.Equals(node.Name, newTag, StringComparison.InvariantCulture))
            {
                return node;
            }
            bool isOwnerDocument = false;
            XmlNode newNode = GetXmlDocument(node, out isOwnerDocument).CreateElement(newTag, node.NamespaceURI);
            CopyNodeContents(newNode, node);
            if (node.ParentNode != null)
            {
                node.ParentNode.ReplaceChild(newNode, node);
            }
            return newNode;
        }

        public static XmlNode SelectChildNode(XmlNode node, string localName, bool deep, bool ignoreCase)
        {
            if (((node == null) || (node.ChildNodes == null)) || string.IsNullOrEmpty(localName))
            {
                return null;
            }
            XmlNode result = null;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (ignoreCase && string.Equals(child.LocalName, localName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = child;
                }
                else if (string.Equals(child.LocalName, localName, StringComparison.InvariantCulture))
                {
                    result = child;
                }
                else if (deep)
                {
                    result = SelectChildNode(child, localName, true, ignoreCase);
                }
                if (result != null)
                {
                    return result;
                }
            }
            return result;
        }

        public static XmlNode[] SelectChildNodes(XmlNode node, string localName, bool deep, bool ignoreCase)
        {
            if (((node == null) || (node.ChildNodes == null)) || string.IsNullOrEmpty(localName))
            {
                return null;
            }
            List<XmlNode> nodeList = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (ignoreCase && string.Equals(child.LocalName, localName, StringComparison.InvariantCultureIgnoreCase))
                {
                    nodeList.Add(child);
                }
                else if (string.Equals(child.LocalName, localName, StringComparison.InvariantCulture))
                {
                    nodeList.Add(child);
                }
                else if (deep)
                {
                    XmlNode[] nodes = SelectChildNodes(child, localName, true, ignoreCase);
                    if (nodes != null)
                    {
                        nodeList.AddRange(nodes);
                    }
                }
            }
            return nodeList.ToArray();
        }

        public static XPathNodeIterator SelectNodes(XmlNode node, string xpath, ref XPathExpression compiledExpression, bool multiThreaded)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            return SelectNodes(node.CreateNavigator(), xpath, ref compiledExpression, multiThreaded);
        }

        public static XPathNodeIterator SelectNodes(XPathNavigator navigator, string xpath, ref XPathExpression compiledExpression, bool multiThreaded)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException("navigator");
            }
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }
            XPathExpression expression = compiledExpression;
            if (expression == null)
            {
                expression = navigator.Compile(xpath);
                compiledExpression = expression;
            }
            else if (multiThreaded)
            {
                expression = expression.Clone();
            }
            return navigator.Select(expression);
        }

        public static XPathNavigator SelectSingleNode(XmlNode node, string xpath, ref XPathExpression compiledExpression, bool multiThreaded)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            XPathNodeIterator iterator = SelectNodes(node.CreateNavigator(), xpath, ref compiledExpression, multiThreaded);
            return (((iterator != null) && iterator.MoveNext()) ? iterator.Current : null);
        }

        public static XPathNavigator SelectSingleNode(XPathNavigator navigator, string xpath, ref XPathExpression compiledExpression, bool multiThreaded)
        {
            XPathNodeIterator iterator = SelectNodes(navigator, xpath, ref compiledExpression, multiThreaded);
            return (((iterator != null) && iterator.MoveNext()) ? iterator.Current : null);
        }

        public static XmlAttribute SetAttribute(XmlNode source, string attributeName, string attributeValue)
        {
            return SetAttribute(source, attributeName, attributeValue, string.Empty);
        }

        public static XmlAttribute SetAttribute(XmlNode source, string attributeName, string attributeValue, string namespaceUri)
        {
            if (attributeValue == null)
            {
                DeleteAttribute(source, attributeName, namespaceUri);
                return null;
            }
            XmlAttribute attr = GetAttribute(source, attributeName, namespaceUri, true);
            if (attr != null)
            {
                attr.Value = attributeValue;
            }
            return attr;
        }

        public static XmlNode SetInnerText(XmlNode source, string nodeName, string innerText)
        {
            return SetInnerText(source, nodeName, innerText);
        }

        public static XmlNode SetInnerText(XmlNode source, string nodeName, string innerText, string namespaceUri)
        {
            if (innerText == null)
            {
                DeleteNode(source, nodeName, namespaceUri);
                return null;
            }
            XmlNode node = GetNode(source, nodeName, namespaceUri, true);
            if (node != null)
            {
                node.InnerText = innerText;
            }
            return node;
        }

        public static XmlNode SetInnerXml(XmlNode source, string nodeName, string innerXml)
        {
            return SetInnerXml(source, nodeName, innerXml, string.Empty);
        }

        public static XmlNode SetInnerXml(XmlNode source, string nodeName, string innerXml, string namespaceUri)
        {
            if (innerXml == null)
            {
                DeleteNode(source, nodeName, namespaceUri);
                return null;
            }
            XmlNode node = GetNode(source, nodeName, namespaceUri, true);
            if (node != null)
            {
                node.InnerXml = innerXml;
            }
            return node;
        }

        public static XmlDocument TransformToXml(XmlNode sourceNode, XslCompiledTransform transform, XsltArgumentList argumentList)
        {
            return TransformToXml(sourceNode, transform, argumentList, transform.OutputSettings);
        }

        public static XmlDocument TransformToXml(XmlNode sourceNode, XslCompiledTransform transform, XsltArgumentList argumentList, XmlWriterSettings writerSettings)
        {
            XmlDocument resultsDom = new XmlDocument();
            if ((sourceNode != null) && (transform != null))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriter writer = XmlWriter.Create(ms, writerSettings);
                    transform.Transform(sourceNode.CreateNavigator(), argumentList, writer);
                    if (ms != null)
                    {
                        ms.Seek(0L, SeekOrigin.Begin);
                        resultsDom.Load(ms);
                    }
                }
            }
            return resultsDom;
        }

        public static XmlNode UnionizeNode(XmlNode masterNode, XmlNode slaveNode)
        {
            if (masterNode == null)
            {
                return null;
            }
            if (slaveNode != null)
            {
                XmlNode masterChild = null;
                foreach (XmlNode slaveChild in slaveNode.ChildNodes)
                {
                    masterChild = GetNode(masterChild, slaveChild.Name, slaveChild.NamespaceURI, false);
                    if (masterChild == null)
                    {
                        AddNode(masterNode, slaveChild);
                    }
                    else
                    {
                        ReplaceNode(masterChild, slaveChild);
                    }
                }
            }
            return masterNode;
        }

        public static string XmlAttributeDecode(string value)
        {
            string xml = string.Format("<Test Test=\"{0}\"/>", value);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement.Attributes[0].Value;
        }

        public static string XmlAttributeEncode(string value)
        {
            XmlAttribute attr = new XmlDocument().CreateAttribute("Test");
            attr.Value = value;
            string xml = attr.OuterXml;
            int start = xml.IndexOf('"');
            int end = xml.LastIndexOf('"');
            return xml.Substring(start + 1, (end - start) - 1);
        }

        public static string XmlElementDecode(string value)
        {
            XmlNode node = new XmlDocument().CreateElement("Test");
            node.InnerXml = value;
            return node.InnerText;
        }

        public static string XmlElementEncode(string value)
        {
            XmlNode node = new XmlDocument().CreateElement("Test");
            node.InnerText = value;
            return node.InnerXml;
        }
    }
}
