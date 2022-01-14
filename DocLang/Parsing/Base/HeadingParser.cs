using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing.Base
{
    /// <summary>
    /// An <see cref="DocParseService{TNode, TData}"/> for <see cref="IDocHeadingNode"/>s that parses title and name elements.
    /// </summary>
    public class HeadingParser : DocParseService<IDocHeadingNode, XElement>
    {
        public HeadingParser() : base(3)
        { }

        /// <summary>
        /// An <see cref="IDocParser"/> used for parsing child elements.
        /// </summary>
        public IDocParser? ChildParser { get; set; }

        /// <inheritdoc/>
        protected override bool ReadInternal(IDocHeadingNode node, XElement element)
        {
            Guard.IsNotNull(ChildParser, nameof(ChildParser));
            node.Name = element.EnforceAttribute("Name").Value;
            IEnumerable<IDocNode> title = element.EnforceElement("Title").Nodes().Select(ChildParser.Read);
            foreach (var child in title)
            {
                node.Title.Add(child);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteInternal(IDocHeadingNode node, XElement element)
        {
            Guard.IsNotNull(ChildParser, nameof(ChildParser));
            XNode[] content = node.Title.Select(ChildParser.Write).ToArray();
            element.Add(new XElement("Title", content));
            element.SetAttributeValue("Name", node.Name);
            return true;
        }
    }
}
