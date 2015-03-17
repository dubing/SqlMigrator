using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace SqlMigratorWinform.Utility
{
    public static class XslCache
    {
        private static FileCache Cache = new FileCache("XslCache", new ReadFileHandler(XslCache.OnLoadXslTransform));

        public static XslCompiledTransform GetXslTransform(params string[] filenames)
        {
            if ((filenames == null) || (filenames.Length == 0))
            {
                return null;
            }
            return (Cache.ReadFile(filenames) as XslCompiledTransform);
        }

        private static object OnLoadXslTransform(params string[] filenames)
        {
            XslCompiledTransform trans = new XslCompiledTransform();

            Assembly asm = Assembly.GetExecutingAssembly();
            Stream xmlres = asm.GetManifestResourceStream(filenames[0]);          
            XmlReader xr = XmlReader.Create(xmlres);
            trans.Load(xr);
            return trans;
        }
    }
}

