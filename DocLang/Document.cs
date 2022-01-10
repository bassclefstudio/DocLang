using BassClefStudio.DocLang.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// The in-memory representation of a full document written in DocLang.
    /// </summary>
    public class Document : CollectionNode
    {
        /// <inheritdoc/>
        public override string NodeType { get; } = "Document";

        /// <summary>
        /// Creates a new, empty <see cref="Document"/>.
        /// </summary>
        public Document() : base(null)
        { }

        /// <summary>
        /// Attempts to retrieve the <typeparamref name="T"/> value of the attribute stored in the <see cref="IDocMetadataNode{T}"/> with the given key.
        /// </summary>
        /// <typeparam name="T">The type of metadata to query for.</typeparam>
        /// <param name="key">The <see cref="IDocMetadataNode{T}.Key"/> of the desired metadata item.</param>
        /// <returns>The <typeparamref name="T"/> value if the <see cref="IDocMetadataNode{T}"/> was found; returns a default value otherwise.</returns>
        public T? GetAttribute<T>(string key)
        {
            var item = Children.OfType<IDocMetadataNode<T>>().FirstOrDefault(data => data.Key == key);
            if (item is not null)
            {
                return item.Value;
            }
            else
            {
                return default;
            }
        }
    }
}
