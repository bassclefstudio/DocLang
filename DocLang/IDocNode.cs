using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Any node or object that can be added to the DocLang document tree.
    /// </summary>
    public interface IDocNode
    {
        /// <summary>
        /// The XML element name describing this <see cref="IDocNode"/>, which identifies its purpose in the document.
        /// </summary>
        string NodeType { get; }
    }

    /// <summary>
    /// An <see cref="IDocNode"/> which is accessible by its unique ID.
    /// </summary>
    public interface IDocRefNode : IDocNode
    {
        /// <summary>
        /// A <see cref="string"/> ID, which can be used to reference this <see cref="IDocRefNode"/> from within or between DocLang documents.
        /// </summary>
        string Id { get; }
    }

    /// <summary>
    /// An <see cref="IDocNode"/> with <see cref="string"/> plain text content.
    /// </summary>
    public interface IDocTextNode : IDocNode
    {
        /// <summary>
        /// The <see cref="string"/> text contained within this <see cref="IDocNode"/>.
        /// </summary>
        string Content { get; set; }
    }

    /// <summary>
    /// An <see cref="IDocNode"/> which contains one or more child <see cref="IDocNode"/>s as content.
    /// </summary>
    public interface IDocContentNode : IDocNode
    {
        /// <summary>
        /// The <see cref="IDocNode"/>s of content that will be contained within this <see cref="IDocContentNode"/>.
        /// </summary>
        IEnumerable<IDocNode> Content { get; }

        /// <summary>
        /// Adds a new <see cref="IDocNode"/> node to the <see cref="Content"/> collection.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> to add.</param>
        void AddChild(IDocNode child);

        /// <summary>
        /// Removes the first instance of the <see cref="IDocNode"/> node from the <see cref="Content"/> collection.
        /// </summary>
        /// <param name="child">The <see cref="IDocNode"/> to remove.</param>
        bool RemoveChild(IDocNode child);
    }
}
