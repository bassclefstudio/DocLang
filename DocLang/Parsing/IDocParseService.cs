using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Represents a chainable, adaptible parser service for parsing <see cref="IDocNode"/>s to/from XML content.
    /// </summary>
    public interface IDocParseService
    {
        /// <summary>
        /// Gets a <see cref="uint"/> priority which defines in which order <see cref="IDocParseService"/>s should read and write content. Generally, this is in descending order.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// Reads content from the <see cref="XNode"/> XML and adds it to the <see cref="IDocNode"/> being parsed.
        /// </summary>
        /// <param name="node">The <see cref="IDocNode"/> currently being created/parsed.</param>
        /// <param name="data">The <see cref="XNode"/> representing the full desired content in XML format.</param>
        /// <returns>A <see cref="bool"/> indicating whether the <see cref="IDocParseService"/> was able to parse the provided data.</returns>
        bool Read(IDocNode node, XNode data);

        /// <summary>
        /// Writes content from the <see cref="IDocNode"/> node into the <see cref="XNode"/> XML.
        /// </summary>
        /// <param name="node">The <see cref="IDocNode"/> data.</param>
        /// <param name="data">An <see cref="XNode"/> currently being created/parsed.</param>
        /// <returns>A <see cref="bool"/> indicating whether the <see cref="IDocParseService"/> was able to parse the provided data.</returns>
        bool Write(IDocNode node, XNode data);
    }

    /// <summary>
    /// A base implementation of <see cref="IDocParseService"/> with support for strongly-typed inputs.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="IDocNode"/> being parsed.</typeparam>
    /// <typeparam name="TData">The XML data object that <typeparamref name="TNode"/> nodes are parsed to/from.</typeparam>
    public abstract class DocParseService<TNode, TData> : IDocParseService where TNode : IDocNode where TData : XNode
    {
        /// <inheritdoc/>
        public uint Priority { get; }

        /// <summary>
        /// Creates a new <see cref="DocParseService{TNode, TData}"/> with the given priority.
        /// </summary>
        /// <param name="priority">A <see cref="uint"/> priority which defines in which order <see cref="IDocParseService"/>s should read and write content. Generally, this is in descending order.</param>
        public DocParseService(uint priority = 0)
        {
            Priority = priority;
        }

        /// <inheritdoc/>
        public bool Read(IDocNode node, XNode element)
        {
            if (node is TNode tNode && element is TData tData)
            {
                return ReadInternal(tNode, tData);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="Read(IDocNode, XNode)"/>
        protected abstract bool ReadInternal(TNode node, TData element);

        /// <inheritdoc/>
        public bool Write(IDocNode node, XNode element)
        {
            if (node is TNode tNode && element is TData tData)
            {
                return WriteInternal(tNode, tData);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="Write(IDocNode, XNode)"/>
        protected abstract bool WriteInternal(TNode node, TData element);

    }
}
