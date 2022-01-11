using BassClefStudio.DocLang.Parsing;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// A basic implementation of <see cref="IDocTextNode"/> for plain text.
    /// </summary>
    public class TextNode : IDocTextNode
    {
        /// <summary>
        /// The XML/<see cref="IDocNode.NodeType"/> type for <see cref="TextNode"/>s.
        /// </summary>
        public const string Type = "Text";
        /// <inheritdoc/>
        public string NodeType { get; } = Type;

        /// <inheritdoc/>
        public string Text { get; set; }

        /// <inheritdoc/>
        public IDocNode? Parent { get; set; }

        /// <summary>
        /// Creates a new <see cref="TextNode"/> for the given <see cref="string"/> of text.
        /// </summary>
        /// <param name="text">The <see cref="string"/> text that is contained in this <see cref="IDocTextNode"/>.</param>
        public TextNode(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// An <see cref="IDocParser"/> for the <see cref="TextNode"/> node (note that reading <see cref="TextNode"/>s often takes place in mixed-content parsing - see <seealso cref="DocParserExtensions.WriteContent(IDocContentNode, IDocParserCollection, Microsoft.Extensions.Logging.ILogger?)"/>).
    /// </summary>
    public class TextNodeParser : IDocParser
    {
        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        public ILogger<TextNodeParser>? Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="TextNodeParser"/>.
        /// </summary>
        public TextNodeParser()
        { }

        /// <inheritdoc/>
        public IEnumerable<string> SupportedNodes { get; }
            = new string[]
            {
                TextNode.Type
            };

        /// <inheritdoc/>
        public IDocNode Read(XElement element)
        {
            return new TextNode(element.Value);
        }

        /// <inheritdoc/>
        public XNode Write(IDocNode node)
        {
            Guard.IsAssignableToType<TextNode>(node, nameof(node));
            TextNode textNode = (TextNode)node;
            return new XText(textNode.Text);
        }
    }
}
