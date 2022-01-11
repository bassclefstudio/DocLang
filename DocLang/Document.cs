using BassClefStudio.DocLang.Content;
using BassClefStudio.DocLang.Metadata;
using BassClefStudio.DocLang.Parsing;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// The in-memory representation of a full document written in DocLang.
    /// </summary>
    public class Document : Heading
    {
        /// <summary>
        /// The XML/<see cref="IDocNode.NodeType"/> type for <see cref="Document"/>s.
        /// </summary>
        public new const string Type = "Document";

        /// <summary>
        /// The list of all <see cref="Author"/>s that were involved in the creation of the <see cref="Document"/>.
        /// </summary>
        public List<Author> Authors { get; }

        /// <summary>
        /// Creates a new, empty <see cref="Document"/>.
        /// </summary>
        /// <inheritdoc/>
        public Document(string id, string name, IDocNode title) : base(id, name, title, Type)
        {
            Authors = new List<Author>();
        }
    }

    /// <summary>
    /// An <see cref="IDocParser"/> for the <see cref="Document"/> node (based on <see cref="HeadingParser"/>).
    /// </summary>
    public class DocumentParser : DocParser
    {
        /// <summary>
        /// An <see cref="IParser{T}"/> for <see cref="Author"/> objects.
        /// </summary>
        public IParser<Author>? AuthorParser { get; set; }

        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        public ILogger<DocumentParser>? Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="DocumentParser"/>.
        /// </summary>
        public DocumentParser() : base(Document.Type)
        { }

        /// <inheritdoc/>
        public override IDocNode Read(XElement element)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            Guard.IsNotNull(AuthorParser, nameof(AuthorParser));
            Document document = new Document(element.GetAttribute("Id").Value, element.GetAttribute("Name").Value, ChildParsers.Read(element.GetElement("Title")));
            document.Authors.AddRange(element.Elements("Author").Select(AuthorParser.Read));
            document.ReadContent(element.GetElement("Content"), ChildParsers, Logger);
            return document;
        }

        /// <inheritdoc/>
        public override XNode Write(IDocNode node)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            Guard.IsNotNull(AuthorParser, nameof(AuthorParser));
            Guard.IsAssignableToType<Document>(node, nameof(node));
            Document document = (Document)node;
            XElement element = new XElement(Document.Type);
            element.SetAttributeValue("Id", document.Id);
            element.SetAttributeValue("Name", document.Name);
            element.Add(ChildParsers.Write(document.Title));
            foreach (var author in document.Authors)
            {
                element.Add(AuthorParser.Write(author));
            }
            element.CreateElement("Content").Add(document.WriteContent(ChildParsers, Logger));
            return element;
        }
    }
}
