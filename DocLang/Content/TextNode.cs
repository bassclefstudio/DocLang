using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// A base implementation of <see cref="IDocTextNode"/> for mixed-content plain text.
    /// </summary>
    public class TextNode : IDocTextNode
    {
        /// <inheritdoc/>
        public string Content { get; set; }

        /// <summary>
        /// Creates a new <see cref="TextNode"/>.
        /// </summary>
        public TextNode()
        {
            Content = string.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="TextNode"/> with the given content.
        /// </summary>
        /// <param name="content">The <see cref="string"/> text content of this <see cref="IDocTextNode"/>.</param>
        public TextNode(string content)
        {
            Content = content;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is IDocNode node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(IDocNode? other)
        {
            return other is TextNode text
                && text.Content == this.Content;
        }
    }
}
