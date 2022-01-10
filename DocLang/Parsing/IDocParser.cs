using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Represents a service that can parse <see cref="IDocNode"/>s to and from their XML representations.
    /// </summary>
    public interface IDocParser
    {
        /// <summary>
        /// Checks whether the <see cref="IDocParser"/> can parse <see cref="IDocNode"/>s of the given type..
        /// </summary>
        /// <param name="nodeType">The desired <see cref="IDocNode.NodeType"/> of nodes to parse. When reading from XML, this is equivalent to the <see cref="XElement.Name"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether this <see cref="IDocParser"/> can handle parsing nodes of type <paramref name="nodeType"/>.</returns>
        bool CanParse(string nodeType);

        /// <summary>
        /// Reads and parses the supported (see <see cref="CanParse(string)"/>) XML as an <see cref="IDocNode"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> XML to parse.</param>
        /// <returns>The <see cref="IDocNode"/> represented by <paramref name="element"/>.</returns>
        IDocNode Read(XElement element);

        /// <summary>
        /// Writes the contents of the supported (see <see cref="CanParse(string)"/>) <see cref="IDocNode"/> as XML.
        /// </summary>
        /// <param name="node">The <see cref="IDocNode"/> to serialize as XML.</param>
        /// <returns>An <see cref="XElement"/> describing the XML corresponding to <paramref name="node"/>.</returns>
        XElement Write(IDocNode node);
    }
}
