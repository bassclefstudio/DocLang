using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// An <see cref="IDocFormatter"/> that uses an XSL Transform (XSLT) to transform DocLang XML into some other format.
    /// </summary>
    public abstract class XsltDocFormatter : IDocFormatter
    {
        /// <summary>
        /// The <see cref="XslCompiledTransform"/> XSLT used to transform DocLang <see cref="XDocument"/>s to HTML. 
        /// </summary>
        private XslCompiledTransform Transform { get; }

        /// <inheritdoc/>
        public abstract DocumentType InputType { get; }

        /// <inheritdoc/>
        public abstract DocumentType OutputType { get; }

        /// <summary>
        /// Creates a new <see cref="XsltDocFormatter"/>.
        /// </summary>
        /// <param name="debug">A <see cref="bool"/> indicating whether the XSLT should be compiled with debug mode enabled.</param>
        public XsltDocFormatter(bool debug = false)
        {
            Transform = new XslCompiledTransform(debug);
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream"/> corresponding to the XSL transform data used in this <see cref="XsltDocFormatter"/>.
        /// </summary>
        /// <returns>The XSLT <see cref="Stream"/>.</returns>
        protected abstract Task<Stream> GetTransformFile();

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            using (var styleStream = await GetTransformFile())
            using (var styleReader = XmlReader.Create(styleStream))
            {
                await Task.Run(() =>
                    Transform.Load(styleReader));
            }
        }

        /// <inheritdoc/>
        public async Task ConvertAsync(Stream inputStream, Stream outputStream)
        {
            using (var reader = XmlReader.Create(inputStream))
            {
                Transform.Transform(reader, null, outputStream);
                await outputStream.FlushAsync();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        { }
    }
}
