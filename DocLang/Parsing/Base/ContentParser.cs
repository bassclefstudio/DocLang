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
    public class ContentParser : DocParseService<IDocContentNode, XElement>
    {
        public ContentParser() : base(1)
        { }

        /// <summary>
        /// An <see cref="IDocParser"/> used for parsing child elements.
        /// </summary>
        public IDocParser? ChildParser { get; set; }

        /// <inheritdoc/>
        protected override bool ReadInternal(IDocContentNode node, XElement element)
        {
            Guard.IsNotNull(ChildParser, nameof(ChildParser));
            XContainer contentElement = node.DirectContent ? element : element.EnforceElement("Content");
            IEnumerable<IDocNode> content = contentElement.Nodes().Select(ChildParser.Read);
            foreach (var child in content)
            {
                node.Content.Add(child);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteInternal(IDocContentNode node, XElement element)
        {
            Guard.IsNotNull(ChildParser, nameof(ChildParser));
            XNode[] content = node.Content.Select(ChildParser.Write).ToArray();
            if (node.DirectContent)
            {
                element.Add(content);
            }
            else
            {
                element.Add(new XElement("Content", content));
            }
            return true;
        }
    }
}
