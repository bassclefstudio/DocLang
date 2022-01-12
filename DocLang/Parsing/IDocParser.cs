using BassClefStudio.DocLang.Content;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Represents a full service (usually made up of one or more <see cref="IDocParseService"/>s) for parsing <see cref="IDocNode"/> nodes to and from XML.
    /// </summary>
    public interface IDocParser
    {
        /// <summary>
        /// Parses data contained in an <see cref="IDocNode"/> to an <see cref="XNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="IDocNode"/> being parsed.</param>
        /// <returns>A <see cref="XNode"/> object representing <paramref name="node"/>.</returns>
        public XNode Write(IDocNode node);

        /// <summary>
        /// Parses data contained in an <see cref="XNode"/> to an <see cref="IDocNode"/>.
        /// </summary>
        /// <param name="data">The <see cref="XNode"/> being parsed.</param>
        /// <returns>An <see cref="IDocNode"/> node equivalent to the representation of <paramref name="data"/>.</returns>
        public IDocNode Read(XNode data);
    }

    /// <summary>
    /// A base implementation of <see cref="IDocParser"/> that iterates through a collection of <see cref="IDocParseService"/>s.
    /// </summary>
    public class DocParser : IDocParser
    {
        /// <summary>
        /// A collection of <see cref="IDocParseService"/>s for parsing nodes.
        /// </summary>
        public IEnumerable<IDocParseService> Services { get; }

        /// <summary>
        /// The <see cref="IDocParseConfig"/> for configuring and constructing new <see cref="IDocNode"/>s.
        /// </summary>
        public IDocParseConfig Config { get; }

        /// <summary>
        /// Creates a new <see cref="DocParser"/>.
        /// </summary>
        /// <param name="services">A collection of <see cref="IDocParseService"/>s for parsing nodes.</param>
        /// <param name="config">The <see cref="IDocParseConfig"/> for configuring and constructing new <see cref="IDocNode"/>s.</param>
        public DocParser(IEnumerable<IDocParseService> services, IDocParseConfig config)
        {
            Services = services;
            Config = config;
        }

        /// <inheritdoc/>
        public IDocNode Read(XNode data)
        {
            Guard.IsNotNull(Services, nameof(Services));
            //// Improve IDocNode constructor code?
            IDocNode? node;
            if (data is XElement element)
            {
                node = Config.CreateNode(element.Name.LocalName);
            }
            else if (data is XText text)
            {
                node = new TextNode(text.Value);
            }
            else
            {
                throw new ParseSchemaException($"Failed to create new IDocNode: No node type registered for {data}.");
            }

            foreach (var service in Services)
            {
                service.Read(node, data);
            }
            return node;
        }

        /// <inheritdoc/>
        public XNode Write(IDocNode node)
        {
            Guard.IsNotNull(Services, nameof(Services));
            //// Improve XNode constructor code?
            XNode element = node is IDocTextNode textNode ? new XText(textNode.Content) : new XElement(string.Empty);
            foreach (var service in Services)
            {
                service.Write(node, element);
            }
            return element;
        }
    }
}
