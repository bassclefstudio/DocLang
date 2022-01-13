using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// An <see cref="IDocContentNode"/> representing an offset paragraph containing some content.
    /// </summary>
    public class ParagraphNode : IDocContentNode
    {
        /// <inheritdoc/>
        public bool DirectContent { get; } = true;

        /// <inheritdoc/>
        public IList<IDocNode> Content { get; }

        /// <summary>
        /// Creates a new <see cref="ParagraphNode"/>.
        /// </summary>
        public ParagraphNode()
        {
            Content = new List<IDocNode>();
        }

        /// <summary>
        /// Creates a new <see cref="ParagraphNode"/>.
        /// </summary>
        /// <param name="content">A collection of child <see cref="IDocNode"/> content elements.</param>
        public ParagraphNode(params IDocNode[] content)
        {
            Content = new List<IDocNode>(content);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is IDocNode node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(IDocNode? other)
        {
            return other is ParagraphNode para
                && para.Content.SequenceEqual(this.Content);
        }
    }
}
