using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml.Base
{
    /// <summary>
    /// An <see cref="DocParseService{TNode, TData}"/> for <see cref="IDocRefNode"/>s that parses node IDs.
    /// </summary>
    public class IdParser : DocParseService<IDocRefNode, XElement>
    {
        /// <inheritdoc/>
        protected override bool ReadInternal(IDocRefNode node, XElement element)
        {
            node.Id = element.EnforceAttribute("Id").Value;
            return true;
        }

        /// <inheritdoc/>
        protected override bool WriteInternal(IDocRefNode node, XElement element)
        {
            element.SetAttributeValue("Id", node.Id);
            return true;
        }
    }
}
