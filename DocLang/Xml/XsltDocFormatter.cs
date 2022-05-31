using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using BassClefStudio.DocLang.Base;
using BassClefStudio.Storage;

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
        protected XsltDocFormatter(bool debug = false)
        {
            Transform = new XslCompiledTransform(debug);
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream"/> corresponding to the XSL transform data used in this <see cref="XsltDocFormatter"/>.
        /// </summary>
        /// <returns>The XSLT <see cref="Stream"/>.</returns>
        protected abstract Task<Stream> GetTransformAsync();

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await using (var styleStream = await GetTransformAsync())
            using (var styleReader = XmlReader.Create(styleStream))
            {
                await Task.Run(() =>
                    Transform.Load(styleReader, XsltSettings.Default, new XmlUrlResolver()));
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
        public virtual void Dispose()
        { }
    }

    /// <summary>
    /// Represents an <see cref="XsltDocFormatter"/> with an <see cref="IStorageFile"/> as the source of the XSL transform.
    /// </summary>
    public class XsltFileFormatter : XsltDocFormatter
    {
        /// <inheritdoc/>
        public override DocumentType InputType { get; } = BaseFormats.Types["xml"];
        
        /// <inheritdoc/>
        public override DocumentType OutputType { get; } = BaseFormats.Types["web"];

        /// <summary>
        /// The <see cref="IStorageFile"/> reference to the XSLT being used to transform documents.
        /// </summary>
        protected IStorageFile File { get; }
        
        /// <summary>
        /// Creates a new <see cref="XsltFileFormatter"/> from the given file.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> reference to the XSLT being used to transform documents.</param>
        public XsltFileFormatter(IStorageFile file)
        {
            File = file;
        }
        
        /// <summary>
        /// The <see cref="IFileContent"/> content of <see cref="File"/>, which is loaded to get the data for <see cref="GetTransformAsync"/>.
        /// </summary>
        private IFileContent? FileContent { get; set; }
        
        /// <inheritdoc/>
        protected override async Task<Stream> GetTransformAsync()
        {
            FileContent ??= await File.OpenFileAsync();
            return FileContent.GetReadStream();
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            FileContent?.Dispose();
            base.Dispose();
        }
    }
}
