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
    public class Document : HeaderNode, IDocAttributedNode
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
    }
}
