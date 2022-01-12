using BassClefStudio.DocLang.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing.Base
{
    /// <summary>
    /// An <see cref="DocParseService{TNode, TData}"/> for <see cref="IDocAttributedNode"/>s that parses attributed authors.
    /// </summary>
    public class AttributeParser : DocParseService<IDocAttributedNode, XElement>
    {
        /// <inheritdoc/>
        protected override bool ReadInternal(IDocAttributedNode node, XElement element)
        {
            foreach (var author in element.EnforceElements("Author").Select(ReadAuthor))
            {
                node.Authors.Add(author);
            }
            return true;
        }

        private Author ReadAuthor(XElement data)
        {
            AuthorType type = Enum.Parse<AuthorType>(data.EnforceAttribute("Type").Value);
            return new Author(type, data.Value);
        }

        /// <inheritdoc/>
        protected override bool WriteInternal(IDocAttributedNode node, XElement element)
        {
            element.Add(node.Authors.Select(WriteAuthor).ToArray());
            return true;
        }

        private XElement WriteAuthor(Author author)
        {
            return new XElement(
                "Author",
                new XAttribute("Type", author.Type.ToString()))
            {
                Value = author.Name
            };
        }
    }
}
