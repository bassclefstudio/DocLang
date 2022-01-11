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

namespace BassClefStudio.DocLang.Content
{
    public class Heading : ContentNode
    {
        /// <summary>
        /// The XML/<see cref="IDocNode.NodeType"/> type for <see cref="Heading"/>s.
        /// </summary>
        public const string Type = "Heading";

        /// <summary>
        /// A unique <see cref="string"/> ID in the document tree that identifies this <see cref="Heading"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The <see cref="string"/> name of the <see cref="Heading"/>, which is shown externally.
        /// </summary>
        public string Name { get; set; }

        private IDocNode title;
        /// <summary>
        /// An <see cref="IDocNode"/> describing the content of the <see cref="Heading"/>'s title.
        /// </summary>
        public IDocNode Title
        {
            get => title;
            [MemberNotNull(nameof(title))]
            set
            {
                if (title is not null)
                {
                    title.Parent = null;
                }
                title = value;
                title.Parent = this;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Heading"/>.
        /// </summary>
        /// <param name="id">A unique <see cref="string"/> ID in the document tree that identifies this <see cref="Heading"/>.</param>
        /// <param name="name">The <see cref="string"/> name of the <see cref="Heading"/>, which is shown externally.</param>
        /// <param name="title">An <see cref="IDocNode"/> describing the content of the <see cref="Heading"/>'s title.</param>
        public Heading(string id, string name, IDocNode title, string? nodeType = null) : base(nodeType ?? Type)
        {
            Id = id;
            Name = name;
            Title = title;
        }
    }

    /// <summary>
    /// An <see cref="IDocParser"/> for the <see cref="Heading"/> node.
    /// </summary>
    public class HeadingParser : DocParser
    {
        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        public ILogger<HeadingParser>? Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="HeadingParser"/>.
        /// </summary>
        public HeadingParser() : base(Heading.Type)
        { }

        /// <inheritdoc/>
        public override IDocNode Read(XElement element)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            Heading heading = new Heading(element.GetAttribute("Id").Value, element.GetAttribute("Name").Value, ChildParsers.Read(element.GetElement("Title")));
            heading.ReadContent(element.GetElement("Content"), ChildParsers, Logger);
            return heading;
        }

        /// <inheritdoc/>
        public override XNode Write(IDocNode node)
        {
            Guard.IsNotNull(ChildParsers, nameof(ChildParsers));
            Guard.IsAssignableToType<Heading>(node, nameof(node));
            Heading heading = (Heading)node;
            XElement element = new XElement(Heading.Type);
            element.SetAttributeValue("Id", heading.Id);
            element.SetAttributeValue("Name", heading.Name);
            element.Add(ChildParsers.Write(heading.Title));
            element.CreateElement("Content").Add(heading.WriteContent(ChildParsers, Logger));
            return element;
        }
    }
}
