using BassClefStudio.DocLang.Metadata;
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
    { }

    /// <summary>
    /// An <see cref="IDocNode"/> which is accessible by its unique ID.
    /// </summary>
    public interface IDocRefNode : IDocNode
    {
        /// <summary>
        /// A <see cref="string"/> ID, which can be used to reference this <see cref="IDocRefNode"/> from within or between DocLang documents.
        /// </summary>
        string Id { get; set; }
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
        /// Gets a <see cref="bool"/> indicating whether the <see cref="Content"/> of this node is direct content. If <c>false</c>, content is parsed into its own "content" XML element.
        /// </summary>
        bool DirectContent { get; }

        /// <summary>
        /// The <see cref="IDocNode"/>s of content that will be contained within this <see cref="IDocContentNode"/>.
        /// </summary>
        IList<IDocNode> Content { get; }
    }

    /// <summary>
    /// An <see cref="IDocRefNode"/> which contains a name and <see cref="IDocNode"/> content title.
    /// </summary>
    public interface IDocHeaderNode : IDocRefNode
    {
        /// <summary>
        /// The <see cref="string"/> name of the header.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The <see cref="IDocNode"/>s of content of the title element for this header.
        /// </summary>
        IList<IDocNode> Title { get; }
    }

    /// <summary>
    /// An <see cref="IDocRefNode"/> which is attributed to a set of <see cref="Author"/> values.
    /// </summary>
    public interface IDocAttributedNode : IDocRefNode
    {
        /// <summary>
        /// The collection of <see cref="Author"/>s responsible for the creation of the given <see cref="IDocAttributedNode"/> and any child content it has.
        /// </summary>
        IList<Author> Authors { get; }
    }
}
