using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// An <see cref="IDocHeadingNode"/> and <see cref="IDocContentNode"/> header with attached <see cref="IDocNode"/> contents.
    /// </summary>
    public class HeadingNode : IDocHeadingNode, IDocContentNode
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
        /// Creates a new <see cref="HeadingNode"/>.
        /// </summary>
        public HeadingNode()
        {
            Name = string.Empty;
            Title = new List<IDocNode>();
            Content = new List<IDocNode>();
        }

        /// <summary>
        /// Creates a new <see cref="HeadingNode"/> with the given name and ID.
        /// </summary>
        /// <param name="id">A <see cref="string"/> ID, which can be used to reference this <see cref="IDocRefNode"/> from within or between DocLang documents.</param>
        /// <param name="name">The <see cref="string"/> name of the header.</param>
        public HeadingNode(string id, string name)
        {
            Id = id;
            Name = name;
            Title = new List<IDocNode>();
            Content = new List<IDocNode>();
        }
    }
}
