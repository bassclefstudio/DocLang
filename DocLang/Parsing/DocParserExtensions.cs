using BassClefStudio.DocLang.Content;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Provides extension methods for the <see cref="IDocParser"/> and <see cref="IDocParserCollection"/> interfaces.
    /// </summary>
    public static class DocParserExtensions
    {
        /// <inheritdoc cref="IParser{T}.Read(XElement)"/>
        /// <param name="parsers">An <see cref="IDocParserCollection"/> of all supported <see cref="IDocParser"/>s for the current state.</param>
        public static IDocNode Read(this IDocParserCollection parsers, XElement element)
        {
            return parsers[element.Name.LocalName].Read(element);
        }

        /// <inheritdoc cref="IParser{T}.Write(T)"/>
        /// <param name="parsers">An <see cref="IDocParserCollection"/> of all supported <see cref="IDocParser"/>s for the current state.</param>
        public static XNode Write(this IDocParserCollection parsers, IDocNode node)
        {
            return parsers[node.NodeType].Write(node);
        }

        /// <summary>
        /// Attempts to get the <see cref="XElement"/> of the given <see cref="XName"/> name, throwing an exception if the child cannot be found.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> being queried.</param>
        /// <param name="name">The name of the element desired from <see cref="XContainer.Elements"/>.</param>
        /// <returns>The <see cref="XElement"/> child element with name <paramref name="name"/> inside <paramref name="element"/>.</returns>
        /// <exception cref="KeyNotFoundException">The item at <paramref name="name"/> of <paramref name="element"/> was not found.</exception>
        public static XElement GetElement(this XElement element, XName name)
        {
            XElement? child = element.Element(name);
            Guard.IsNotNull(child, nameof(child));
            return child;
        }

        /// <summary>
        /// Creates a new <see cref="XElement"/> child with the given <see cref="XName"/> name.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> parent object.</param>
        /// <param name="name">The desired name of the new element.</param>
        /// <returns>The <see cref="XElement"/> child element with name <paramref name="name"/> inside <paramref name="element"/>.</returns>
        public static XElement CreateElement(this XElement element, XName name)
        {
            XElement childElement = new XElement(name);
            element.Add(childElement);
            return childElement;
        }

        /// <summary>
        /// Attempts to get the <see cref="XAttribute"/> of the given <see cref="XName"/> name, throwing an exception if the child cannot be found.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> being queried.</param>
        /// <param name="name">The name of the attribute desired from <see cref="XElement.Attributes"/>.</param>
        /// <returns>The <see cref="XAttribute"/> child element with name <paramref name="name"/> inside <paramref name="element"/>.</returns>
        /// <exception cref="KeyNotFoundException">The item at <paramref name="name"/> of <paramref name="element"/> was not found.</exception>
        public static XAttribute GetAttribute(this XElement element, XName name)
        {
            XAttribute? child = element.Attribute(name);
            Guard.IsNotNull(child, nameof(child));
            return child;
        }

        /// <summary>
        /// Adds <see cref="IDocNode"/> content from the given <see cref="XElement"/> into the <see cref="IDocContentNode.Content"/> of the provided node.
        /// </summary>
        /// <param name="node">The <see cref="IDocContentNode"/> being parsed.</param>
        /// <param name="element">The <see cref="XElement"/> containing DocLang content.</param>
        /// <param name="parsers">The <see cref="IDocParserCollection"/> for parsing child nodes.</param>
        /// <param name="logger">An optional <see cref="ILogger"/> to record parsing data.</param>
        public static void ReadContent(this IDocContentNode node, XElement element, IDocParserCollection parsers, ILogger? logger = null)
        {
            foreach (var child in element.Nodes())
            {
                if (child is XElement childElement)
                {
                    node.AddContent(parsers.Read(childElement));
                }
                else if (child is XText textElement)
                {
                    node.AddContent(new TextNode(textElement.Value));
                }
                else if (logger is not null)
                {
                    logger.LogInformation("Ignoring XML node {Node}.", child);
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="XElement"/> XML element containing the DocLang content of the provided <see cref="IDocContentNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="IDocContentNode"/> being parsed.</param>
        /// <param name="parsers">The <see cref="IDocParserCollection"/> for parsing child nodes.</param>
        /// <param name="logger">An optional <see cref="ILogger"/> to record parsing data.</param>
        /// <returns>A collection of <see cref="XNode"/>s containing the DocLang-formatted XML representing the given content.</returns>
        public static XNode[] WriteContent(this IDocContentNode node, IDocParserCollection parsers, ILogger? logger = null)
        {
            List<XNode> data = new List<XNode>();
            foreach (var child in node.Content)
            {
                data.Add(parsers.Write(child));
            }
            return data.ToArray();
        }
    }
}
