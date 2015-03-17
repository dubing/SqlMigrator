using System.Xml;

namespace SqlMigratorWinform.Utility
{
    public static class XmlCache
    {
        private static readonly FileCache Cache = new FileCache("XmlCache", OnLoadXmlDocument);

        public static XmlDocument GetXmlDocument(params string[] filenames)
        {
            if ((filenames == null) || (filenames.Length == 0))
            {
                return null;
            }
            return (Cache.ReadFile(filenames) as XmlDocument);
        }

        private static object OnLoadXmlDocument(params string[] filenames)
        {
            var doc = new XmlDocument();
            doc.Load(filenames[0]);
            return doc;
        }
    }
}

