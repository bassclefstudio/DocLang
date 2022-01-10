using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Base
{
    /// <summary>
    /// A basic implementation of <see cref="IDocContainerNode"/> that implements the parent/child hierarchy.
    /// </summary>
    public abstract class ContainerNode : IDocContainerNode
    {
        /// <inheritdoc/>
        public abstract string NodeType { get; }

        /// <inheritdoc/>
        public IDocNode? Parent { get; set; }

        private IDocNode child;
        /// <inheritdoc/>
        public IDocNode Child 
        {
            get => child;
            set
            {
                child.Parent = null;
                child = value;
                child.Parent = this;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ContainerNode"/> with the given parent and child nodes.
        /// </summary>
        /// <param name="parent">The parent <see cref="IDocNode"/> object, or <c>null</c> if this is the root <see cref="IDocNode"/>.</param>
        /// <param name="child">The child <see cref="IDocNode"/> node which this <see cref="IDocContainerNode"/> manages.</param>
        public ContainerNode(IDocNode? parent, IDocNode child)
        {
            Parent = parent;
            this.child = child;
            Child.Parent = this;
        }
    }
}
