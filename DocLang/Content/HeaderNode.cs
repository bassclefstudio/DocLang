using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// An <see cref="IDocHeaderNode"/> and <see cref="IDocContentNode"/> header with attached <see cref="IDocNode"/> contents.
    /// </summary>
    public class HeaderNode : IDocHeaderNode, IDocContentNode
    {
        /// <inheritdoc/>
        public bool DirectContent { get; } = false;

        private string? id;
        /// <inheritdoc/>
        public string Id 
        { 
            get => id ?? throw new InvalidOperationException("IDocNode's ID has not been set."); 
            set => id = value;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public IList<IDocNode> Title { get; }

        /// <inheritdoc/>
        public IList<IDocNode> Content { get; }

        /// <summary>
        /// Creates a new <see cref="HeaderNode"/>.
        /// </summary>
        public HeaderNode()
        {
            Name = string.Empty;
            Title = new List<IDocNode>();
            Content = new List<IDocNode>();
        }
    }
}
