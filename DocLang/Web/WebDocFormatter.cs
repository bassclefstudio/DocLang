using BassClefStudio.DocLang.Xml;
using System.Net.Mime;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// An <see cref="XsltDocFormatter"/> for using the embedded HTML/CSS transform on DocLang documents.
    /// </summary>
    public class WebDocFormatter : XsltDocFormatter
    {
        /// <inheritdoc/>
        public override DocumentType InputType { get; } = new DocumentType(DocLangXml.ContentType, new Version(1, 0));

        /// <inheritdoc/>
        public override DocumentType OutputType { get; } = new DocumentType(MediaTypeNames.Text.Html, new Version(5, 0));

        /// <summary>
        /// The key/relative path of the XSL transform file to use.
        /// </summary>
        private string ResourceName { get; }

        /// <summary>
        /// Creates a new <see cref="WebDocFormatter"/> using the specified built-in XSL transform.
        /// </summary>
        /// <param name="resourceName">The key/relative path of the XSL transform file to use.</param>
        public WebDocFormatter(string resourceName = "Content-v1.xsl")
        {
            ResourceName = resourceName;
        }
        
        /// <inheritdoc/>
        protected override async Task<Stream> GetTransformAsync()
        {
            var transformStream = typeof(DocLangXml).Assembly.GetManifestResourceStream(
                typeof(WebDocFormatter),
                ResourceName);
            if (transformStream is null)
            {
                throw new FileNotFoundException($"Could not find the XSLT for web transforms.");
            }
            return transformStream;
        }
    }
}
