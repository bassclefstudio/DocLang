using BassClefStudio.DocLang.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.IO
{
    /// <summary>
    /// A formatter capable of taking a DocLang <see cref="Document"/> and producing an output file of a specified type.
    /// </summary>
    public interface IDocFormatter
    {
        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the <see cref="IDocFormatter"/> service needs to be initialized before <see cref="WriteDocumentAsync(Document, Stream)"/> can be called.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Asynchronously initializes the <see cref="IDocFormatter"/> service with any data or resources required to format <see cref="Document"/>s.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Asynchronously writes the DocLang <see cref="Document"/> to a data <see cref="Stream"/>.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> being written.</param>
        /// <param name="dataStream">The <see cref="Stream"/> to write file data to.</param>
        Task WriteDocumentAsync(Document document, Stream dataStream);
    }
}
