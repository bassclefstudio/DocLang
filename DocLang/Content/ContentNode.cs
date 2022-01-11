using BassClefStudio.DocLang.Parsing;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Content
{
    /// <summary>
    /// A basic implementation of <see cref="IDocContentNode"/> that implements the parent/child hierarchy.
    /// </summary>
    public class ContentNode : IDocContentNode
    {
        public const string ParagraphType = "Paragraph";
        public const string TitleType = "Title";

        /// <inheritdoc/>
        public string NodeType { get; }

        protected List<IDocNode> ChildList { get; }
        /// <inheritdoc/>
        public IEnumerable<IDocNode> Content => ChildList.AsReadOnly();

        /// <inheritdoc/>
        public IDocNode? Parent { get; set; }

        /// <summary>
        /// Creates a new empty <see cref="ContentNode"/>.
        /// </summary>
        /// <param name="nodeType">Gets a <see cref="string"/> indicating the type of <see cref="IDocNode"/> this object represents. This value is used when parsing the DocLang object tree.</param>
        public ContentNode(string nodeType)
        {
            NodeType = nodeType;
            ChildList = new List<IDocNode>();
        }

        /// <summary>
        /// Creates a new <see cref="ContentNode"/> with the given content.
        /// </summary>
        /// <param name="nodeType">Gets a <see cref="string"/> indicating the type of <see cref="IDocNode"/> this object represents. This value is used when parsing the DocLang object tree.</param>
        /// <param name="children">The collection of child <see cref="IDocNode"/> nodes which this <see cref="IDocContentNode"/> manages.</param>
        public ContentNode(string nodeType, IEnumerable<IDocNode> children) : this(nodeType)
        {
            foreach (var child in children)
            {
                AddContent(child);
            }
        }

        /// <inheritdoc/>
        public void AddContent(IDocNode child)
        {
            ChildList.Add(child);
            child.Parent = this;
        }

        /// <inheritdoc/>
        public bool RemoveContent(IDocNode child)
        {
            child.Parent = null;
            return ChildList.Remove(child);
        }
    }

    /// <summary>
    /// An <see cref="IDocParser"/> for basic <see cref="ContentNode"/> nodes.
    /// </summary>
    public class ContentNodeParser : DocParser
    {
        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        public ILogger<ContentNodeParser>? Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="ParagraphParser"/>.
        /// </summary>
        public ContentNodeParser() : base(ContentNode.ParagraphType, ContentNode.TitleType)
        { }

        /// <inheritdoc/>
        public override IDocNode Read(XElement element)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            ContentNode content = new ContentNode(element.Name.LocalName);
            content.ReadContent(element, ChildParsers, Logger);
            return content;
        }

        /// <inheritdoc/>
        public override XNode Write(IDocNode node)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            Guard.IsAssignableToType<ContentNode>(node, nameof(node));
            ContentNode content = (ContentNode)node;
            XElement element = new XElement(content.NodeType);
            element.Add(content.WriteContent(ChildParsers, Logger));
            return element;
        }
    }
}
