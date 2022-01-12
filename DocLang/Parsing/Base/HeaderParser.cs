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
    /// An <see cref="DocParseService{TNode, TData}"/> for <see cref="IDocContentNode"/>s that parses child elements.
    /// </summary>
    public class HeaderParser : DocParseService<IDocHeaderNode, XElement>
    {
        /// <summary>
        /// An <see cref="IDocParser"/> used for parsing child elements.
        /// </summary>
        public IDocParser? ChildParser { get; set; }

        /// <summary>
        /// Creates a new <see cref="HeaderParser"/>.
        /// </summary>
        public HeaderParser()
        { }

        /// <inheritdoc/>
        protected override bool ReadInternal(IDocHeaderNode node, XElement element)
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
        protected override bool WriteInternal(IDocHeaderNode node, XElement element)
        {
            Guard.IsNotNull(ChildParser, nameof(ChildParser));
            XNode[] content = node.Title.Select(ChildParser.Write).ToArray();
            element.SetElementValue("Title", content);
            element.SetAttributeValue("Name", node.Name);
            return true;
        }
    }
}
