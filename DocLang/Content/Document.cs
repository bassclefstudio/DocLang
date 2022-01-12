using BassClefStudio.DocLang.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// A <see cref="IDocNode"/> representation of a full document written in DocLang.
    /// </summary>
    public class Document : HeadingNode, IDocAttributedNode
    {
        /// <inheritdoc/>
        public IList<Author> Authors { get; }

        /// <summary>
        /// Creates a new <see cref="Document"/>.
        /// </summary>
        public Document() : base()
        {
            Authors = new List<Author>();
        }

        /// <summary>
        /// Creates a new <see cref="Document"/> with the given name and ID.
        /// </summary>
        /// <inheritdoc/>
        public Document(string id, string name) : base(id, name)
        {
            Authors = new List<Author>();
        }
    }
}
