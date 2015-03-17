using System.IO;
using System.Xml;

namespace SqlMigratorWinform.Utility
{
    internal sealed class XmlSkipingReader : XmlTextReader
    {
        public XmlSkipingReader(TextReader input)
            : base(input)
        {
            RemoveXmlDeclaration = false;
            OnlySkipEmptyPrfix = false;
        }

        private bool ConsumeNamespaceAttributes()
        {
            while (((base.Prefix == "xmlns") && !OnlySkipEmptyPrfix) || ((base.Prefix.Length == 0) && (LocalName == "xmlns")))
            {
                bool isOk = base.MoveToNextAttribute();
                if (!isOk)
                {
                    return false;
                }
            }
            return true;
        }

        public override string LookupNamespace(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return string.Empty;
            }
            return (OnlySkipEmptyPrfix ? base.LookupNamespace(prefix) : string.Empty);
        }

        public override bool MoveToFirstAttribute()
        {
            return (base.MoveToFirstAttribute() && ConsumeNamespaceAttributes());
        }

        public override bool MoveToNextAttribute()
        {
            return (base.MoveToNextAttribute() && ConsumeNamespaceAttributes());
        }

        public override bool Read()
        {
            bool okContinue = base.Read();
            while ((okContinue && RemoveXmlDeclaration) && (NodeType == XmlNodeType.XmlDeclaration))
            {
                okContinue = base.Read();
            }
            return okContinue;
        }

        public override string Name
        {
            get
            {
                return (OnlySkipEmptyPrfix ? base.Name : LocalName);
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return (((base.Prefix.Length > 0) && OnlySkipEmptyPrfix) ? base.NamespaceURI : string.Empty);
            }
        }     

        public override string Prefix
        {
            get
            {
                return (OnlySkipEmptyPrfix ? base.Prefix : string.Empty);
            }
        }

        public bool OnlySkipEmptyPrfix { get; set; }

        public bool RemoveXmlDeclaration { get; set; }
    }
}
