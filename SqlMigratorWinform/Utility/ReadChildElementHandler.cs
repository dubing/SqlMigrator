using System.Xml;

namespace SqlMigratorWinform.Utility
{
    public delegate void ReadChildElementHandler(XmlReader reader, params object[] args);
}

