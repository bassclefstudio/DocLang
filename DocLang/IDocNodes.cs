using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Represents any node or item in a DocLang object tree.
    /// </summary>
    public interface IDocNode
    {
        /// <summary>
        /// Gets a <see cref="string"/> indicating the type of <see cref="IDocNode"/> this object represents. This value is used when parsing the DocLang object tree.
        /// </summary>
        string NodeType { get; }

        /// <summary>
        /// Gets the parent <see cref="IDocNode"/> object, or <c>null</c> if this is the root <see cref="IDocNode"/>.
        /// </summary>
        IDocNode? Parent { get; set; }
    }

    /// <summary>
    /// Indicates that a given <see cref="IDocNode"/> is a container for another <see cref="IDocNode"/>, over which it provides additional formatting, syntax, or style information.
    /// </summary>
    public interface IDocContainerNode : IDocNode
    {
        /// <summary>
        /// The child <see cref="IDocNode"/> node which this <see cref="IDocContainerNode"/> manages.
        /// </summary>
        IDocNode Child { get; set; }
    }

    /// <summary>
    /// Indicates that a given <see cref="IDocNode"/> is a container for a collection of child <see cref="IDocNode"/>s, over which it provides additional formatting, syntax, or style information.
    /// </summary>
    public interface IDocCollectionNode : IDocNode
    {
        /// <summary>
        /// Gets the collection of child <see cref="IDocNode"/> nodes which this <see cref="IDocCollectionNode"/> manages.
        /// </summary>
        IEnumerable<IDocNode> Children { get; }

        /// <summary>
        /// Adds a new <see cref="IDocNode"/> node to <see cref="Children"/>.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> to add.</param>
        void AddChild(IDocNode child);

        /// <summary>
        /// Attempts to remove the first instance of a child node from this <see cref="IDocCollectionNode"/>.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> in <see cref="Children"/> to remove from the collection.</param>
        /// <returns><c>true</c> if the node is successfully removed; otherwise, <c>false</c>.</returns>
        bool RemoveChild(IDocNode child);
    }

    /// <summary>
    /// Indicates that a given <see cref="IDocNode"/> is a container for plain text (stored as a <see cref="string"/>), over which it provides additional formatting, syntax, or style information.
    /// </summary>
    public interface IDocTextNode : IDocNode
    {
        /// <summary>
        /// The <see cref="string"/> text that is contained in this <see cref="IDocTextNode"/>.
        /// </summary>
        string Text { get; set; }
    }

    /// <summary>
    /// Indicates that a given <see cref="IDocNode"/> provides <typeparamref name="T"/> metadata for its enclosing <see cref="IDocNode"/>.
    /// </summary>
    public interface IDocMetadataNode<T> : IDocNode
    {
        /// <summary>
        /// The unique <see cref="string"/> key within the enclosing <see cref="IDocNode"/> for this metadata.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The <typeparamref name="T"/> data stored in this metadata node.
        /// </summary>
        T Value { get; }
    }
}
