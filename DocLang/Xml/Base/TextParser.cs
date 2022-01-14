using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml.Base
{
    /// <summary>
    /// An <see cref="DocParseService{TNode, TData}"/> for <see cref="IDocTextNode"/>s that parses plain text content.
    /// </summary>
    public class TextParser : DocParseService<IDocTextNode, XText>
    {
        /// <inheritdoc/>
        protected override bool ReadInternal(IDocTextNode node, XText element)
        {
            node.Content = element.Value;
            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteInternal(IDocTextNode node, XText element)
        {
            element.Value = node.Content;
            return true;
        }
    }
}
