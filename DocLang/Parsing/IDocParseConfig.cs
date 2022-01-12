using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Contains configuration information for creating and managing <see cref="IDocNode"/>s when parsing DocLang objects.
    /// </summary>
    public interface IDocParseConfig
    {
        /// <summary>
        /// Creates a new <see cref="IDocNode"/> for the given <see cref="string"/> XML element name.
        /// </summary>
        /// <param name="nodeType">The XML name of the element.</param>
        /// <returns>An empty <see cref="IDocNode"/> node of the required type.</returns>
        public IDocNode CreateNode(string nodeType);
    }
}
