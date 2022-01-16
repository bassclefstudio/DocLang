using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Provides an interface for services that can transform DocLang XML into a certain output type (such as Markdown, HTML, or Word).
    /// </summary>
    public interface IDocFormatter : IDisposable
    {
        /// <summary>
        /// The <see cref="DocumentType"/> of input data (see <see cref="ConvertAsync(Stream, Stream)"/>).
        /// </summary>
        DocumentType InputType { get; }

        /// <summary>
        /// The <see cref="DocumentType"/> of output data (see <see cref="ConvertAsync(Stream, Stream)"/>).
        /// </summary>
        DocumentType OutputType { get; }

        /// <summary>
        /// Asynchronously initializes any resources or services used by this <see cref="IDocFormatter"/>.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Asynchronously reads from an input <see cref="Stream"/> of DocLang XML and writes the converted output document to a different <see cref="Stream"/>.
        /// </summary>
        /// <param name="inputStream">The readable <see cref="Stream"/> containing DocLang XML.</param>
        /// <param name="outputStream">The output <see cref="Stream"/> where the converted document can be written.</param>
        Task ConvertAsync(Stream inputStream, Stream outputStream);
    }
}
