using Microsoft.Extensions.Logging;
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
    public interface IDocParser : IParser<IDocNode>
    {
        /// <summary>
        /// Gets a collection of <see cref="string"/> XML node names or <see cref="IDocNode.NodeType"/>s that this <see cref="IDocParser"/> supports.
        /// </summary>
        IEnumerable<string> SupportedNodes { get; }
    }

    public abstract class DocParser : IDocParser
    {
        /// <inheritdoc/>
        public IEnumerable<string> SupportedNodes { get; }

        /// <summary>
        /// An <see cref="IDocParserCollection"/> containing <see cref="IDocParser"/>s for any child items.
        /// </summary>
        public IDocParserCollection? ChildParsers { get; set; }

        /// <summary>
        /// Creates a new <see cref="DocParser"/>.
        /// </summary>
        /// <param name="supported">The <see cref="string"/> XML node names or <see cref="IDocNode.NodeType"/>s that this <see cref="DocParser"/> supports.</param>
        public DocParser(params string[] supported)
        {
            SupportedNodes = supported;
        }

        /// <inheritdoc/>
        public abstract IDocNode Read(XElement element);

        /// <inheritdoc/>
        public abstract XNode Write(IDocNode node);
    }
}
