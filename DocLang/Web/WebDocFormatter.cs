using BassClefStudio.DocLang.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

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

        /// <inheritdoc/>
        protected override async Task<Stream> GetTransformFile()
        {
            string resourceName = $"Base-v1.xsl";
            var transformStream = typeof(DocLangXml).Assembly.GetManifestResourceStream(
                typeof(WebDocFormatter),
                resourceName);
            if (transformStream is null)
            {
                throw new FileNotFoundException($"Could not find the XSLT for web transforms.");
            }
            return transformStream;
        }
    }
}
