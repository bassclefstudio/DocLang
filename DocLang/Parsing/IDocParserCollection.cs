using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Provides an interface for a service that can manage a set of enabled <see cref="IDocParser"/>s.
    /// </summary>
    public interface IDocParserCollection
    {
        /// <summary>
        /// Checks whether this <see cref="IDocParserCollection"/> contains a parser for the given <see cref="IDocNode.NodeType"/>.
        /// </summary>
        /// <param name="nodeType">The <see cref="string"/> from the XML node or <see cref="IDocNode.NodeType"/> indicating the type of node being parsed.</param>
        /// <returns>A <see cref="bool"/> indicating whether an <see cref="IDocParser"/> exists in this <see cref="IDocParserCollection"/>.</returns>
        bool ContainsKey(string nodeType);

        /// <summary>
        /// Gets the <see cref="IDocParser"/> associated with the given given <see cref="IDocNode.NodeType"/>.
        /// </summary>
        /// <param name="nodeType">The <see cref="string"/> from the XML node or <see cref="IDocNode.NodeType"/> indicating the type of node being parsed.</param>
        /// <returns>The <see cref="IDocParser"/> registered in this <see cref="IDocParserCollection"/> to handle requests of type <paramref name="nodeType"/>.</returns>
        IDocParser this[string nodeType] { get; }
    }

    public class DocParserCollection : IDocParserCollection
    {
        /// <summary>
        /// The private <see cref="IDictionary{TKey, TValue}"/> associating <see cref="IDocParser"/>s with their <see cref="string"/> supported node types (see <see cref="IDocParser.SupportedNodes"/>).
        /// </summary>
        private IDictionary<string, IDocParser> Parsers { get; }

        /// <inheritdoc/>
        public bool ContainsKey(string nodeType) => Parsers.ContainsKey(nodeType);

        /// <inheritdoc/>
        public IDocParser this[string nodeType]
        {
            get => Parsers[nodeType];
        }

        /// <summary>
        /// Creates a new <see cref="DocParserCollection"/> from a set of <see cref="IDocParser"/>s.
        /// </summary>
        /// <param name="parsers">The provided <see cref="IDocParser"/>s to use for parsing nodes.</param>
        public DocParserCollection(IEnumerable<IDocParser> parsers)
        {
            Parsers = parsers.SelectMany(
                p => p.SupportedNodes.Select(
                    n => new KeyValuePair<string, IDocParser>(n, p)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
