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
    }
}
