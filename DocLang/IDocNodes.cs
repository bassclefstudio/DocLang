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
    /// Indicates that a given <see cref="IDocNode"/> is a container for a collection of <see cref="IDocNode"/> content items, over which it provides additional formatting, syntax, or style information.
    /// </summary>
    public interface IDocContentNode : IDocNode
    {
        /// <summary>
        /// Gets the collection of child <see cref="IDocNode"/> nodes which this <see cref="IDocContentNode"/> manages as content.
        /// </summary>
        IEnumerable<IDocNode> Content { get; }

        /// <summary>
        /// Adds a new <see cref="IDocNode"/> node to <see cref="Content"/>.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> to add.</param>
        void AddContent(IDocNode child);

        /// <summary>
        /// Attempts to remove the first instance of an <see cref="IDocNode"/> from <see cref="Content"/>.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> to remove.</param>
        /// <returns><c>true</c> if the node is successfully removed; otherwise, <c>false</c>.</returns>
        bool RemoveContent(IDocNode child);
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
}
