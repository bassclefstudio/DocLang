using System.Net.Mime;
using System.Xml;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// Contains static information and constants regarding the custom DocLang XML language and schema.
    /// </summary>
    public static class DocLangXml
    {
        /// <summary>
        /// Gets a <see cref="Version"/> indicating the latest available version of the DocLang XML schema.
        /// </summary>
        public static readonly Version LatestVersion = new Version(1, 0);
        
        /// <summary>
        /// Provides an (unofficial) <see cref="System.Net.Mime.ContentType"/> for identifying DocLang XML content.
        /// </summary>

        public static readonly ContentType ContentType = new ContentType("application/doclang+xml");

        /// <summary>
        /// Retrieves a <see cref="Stream"/> from which the DocLang XML schema (.xsd) of the given <see cref="Version"/> can be read.
        /// </summary>
        /// <param name="version">The version of the DocLang schema to retrieve.</param>
        /// <returns>A <see cref="Stream"/> containing the data from the XSD schema.</returns>
        public static Stream GetSchema(Version version)
        {
            string resourceName = $"Base-v{version.Major}.xsd";
            var schemaStream = typeof(DocLangXml).Assembly.GetManifestResourceStream(
                typeof(DocLangXml),
                resourceName);
            if (schemaStream is null)
            {
                throw new FileNotFoundException($"Could not find DocLang schema {version} included in library.");
            }
            return schemaStream;
        }
    }
}
