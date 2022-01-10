using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Base
{
    /// <summary>
    /// A basic implementation of <see cref="IDocTextNode"/> for plain text.
    /// </summary>
    public abstract class TextNode : IDocTextNode
    {
        /// <inheritdoc/>
        public abstract string NodeType { get; }

        /// <inheritdoc/>
        public string Text { get; set; }

        /// <inheritdoc/>
        public IDocNode? Parent { get; set; }

        /// <summary>
        /// Creates a new <see cref="TextNode"/> for the given <see cref="string"/> of text.
        /// </summary>
        /// <param name="parent">Gets the parent <see cref="IDocNode"/> object, or <c>null</c> if this is the root <see cref="IDocNode"/>.</param>
        /// <param name="text">The <see cref="string"/> text that is contained in this <see cref="IDocTextNode"/>.</param>
        public TextNode(IDocNode? parent, string text)
        {
            Parent = parent;
            Text = text;
        }
    }
}
