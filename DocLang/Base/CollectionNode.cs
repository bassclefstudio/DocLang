using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Base
{
    /// <summary>
    /// A basic implementation of <see cref="IDocCollectionNode"/> that implements the parent/child hierarchy.
    /// </summary>
    public abstract class CollectionNode : IDocCollectionNode
    {
        protected List<IDocNode> ChildList { get; }
        /// <inheritdoc/>
        public IEnumerable<IDocNode> Children => ChildList.AsReadOnly();

        /// <inheritdoc/>
        public IDocNode? Parent { get; set; }

        /// <summary>
        /// Creates a new empty <see cref="CollectionNode"/> with the given parent node.
        /// </summary>
        /// <param name="parent">The parent <see cref="IDocNode"/> object, or <c>null</c> if this is the root <see cref="IDocNode"/>.</param>
        public CollectionNode(IDocNode? parent)
        {
            Parent = parent;
            ChildList = new List<IDocNode>();
        }

        /// <summary>
        /// Creates a new <see cref="CollectionNode"/> with the given parent and child nodes.
        /// </summary>
        /// <param name="parent">The parent <see cref="IDocNode"/> object, or 'null' if this is the root <see cref="IDocNode"/>.</param>
        /// <param name="children">The collection of child <see cref="IDocNode"/> nodes which this <see cref="IDocCollectionNode"/> manages.</param>
        public CollectionNode(IDocNode? parent, IEnumerable<IDocNode> children) : this(parent)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        /// <inheritdoc/>
        public void AddChild(IDocNode child)
        {
            ChildList.Add(child);
            child.Parent = this;
        }

        /// <inheritdoc/>
        public bool RemoveChild(IDocNode child)
        {
            child.Parent = null;
            return ChildList.Remove(child);
        }
    }
}
