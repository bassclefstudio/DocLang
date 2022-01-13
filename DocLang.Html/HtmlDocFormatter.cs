using BassClefStudio.DocLang.Content;
using BassClefStudio.DocLang.IO;
using BassClefStudio.DocLang.Parsing;
using BassClefStudio.DocLang.Parsing.Base;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace BassClefStudio.DocLang.Html
{
    /// <summary>
    /// An <see cref="IDocFormatter"/> format that uses XSL Transforms (XSLT) to convert DocLang XML into basic HTML for presentation on the web and other places.
    /// </summary>
    public class HtmlDocFormatter : IDocFormatter
    {
        /// <summary>
        /// <see cref="IDocParser"/> for turning <see cref="Document"/>s into XML.
        /// </summary>
        public IDocParser XmlParser { get; }

        /// <inheritdoc/>
        public bool Initialized { get; private set; }

        /// <summary>
        /// The <see cref="XslCompiledTransform"/> XSLT used to transform DocLang <see cref="XDocument"/>s to HTML. 
        /// </summary>
        private XslCompiledTransform Transform { get; }

        /// <summary>
        /// Creates a new <see cref="HtmlDocFormatter"/>.
        /// </summary>
        public HtmlDocFormatter(bool debug = false)
        {
            //// Enforces schema version v1.0 for resulting XML.
            XmlParser = new XmlParser(BaseDocLangSchema.Version1);
            Transform = new XslCompiledTransform(debug);
            Initialized = false;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            string resourceName = "Transform.xsl";
            using var styleStream = typeof(HtmlDocFormatter).Assembly.GetManifestResourceStream(typeof(HtmlDocFormatter), resourceName);
            if (styleStream is null)
            {
                throw new FileNotFoundException($"Could not find '{resourceName}' embedded XSLT resource file.");
            }
            else
            {
                using (var styleReader = XmlReader.Create(styleStream))
                {
                    await Task.Run(() =>
                        Transform.Load(styleReader));
                }
                Initialized = true;
            }
        }

        /// <inheritdoc/>
        public async Task WriteDocumentAsync(Document document, Stream dataStream)
        {
            XDocument parsedData = XmlParser.WriteDocument(document);
            using (var writer = XmlWriter.Create(dataStream, new XmlWriterSettings() { Async = true }))
            {
                Transform.Transform(parsedData.CreateReader(), writer);
                await writer.FlushAsync();
            }
        }
    }
}
