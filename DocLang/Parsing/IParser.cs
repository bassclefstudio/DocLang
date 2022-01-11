using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// A generic XML parsing service for any <typeparamref name="T"/> items.
    /// </summary>
    public interface IParser<T>
    {
        /// <summary>
        /// Reads and parses the XML as a <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> XML to parse.</param>
        /// <returns>The <typeparamref name="T"/> represented by <paramref name="element"/>.</returns>
        T Read(XElement element);

        /// <summary>
        /// Writes the contents of the a <typeparamref name="T"/> object as XML.
        /// </summary>
        /// <param name="node">The <typeparamref name="T"/> to serialize as XML.</param>
        /// <returns>An <see cref="XNode"/> describing the XML corresponding to <paramref name="node"/>.</returns>
        XNode Write(T node);
    }
}
