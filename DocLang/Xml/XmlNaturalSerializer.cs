using Autofac.Features.Indexed;
using BassClefStudio.NET.Serialization.Natural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// A <see cref="NaturalSerializer{T, TData, TKey, TDataKey}"/> implementation designed for serializing <see cref="IDocNode"/>s to XML by type name association.
    /// </summary>
    public class XmlNaturalSerializer : NaturalSerializer<IDocNode, XNode, Type, string>
    {
        /// <inheritdoc/>
        public XmlNaturalSerializer(IEnumerable<INaturalSerializerService<IDocNode, XNode>> services, IIndex<string, Func<IDocNode>> objects, IIndex<Type, Func<XNode>> data) : base(services, objects, data)
        { }

        /// <inheritdoc/>
        protected override string GetKey(XNode data)
        {
            if (data is XElement element)
            {
                return element.Name.LocalName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        protected override Type GetKey(IDocNode content)
        {
            return content.GetType();
        }
    }
}
