using BassClefStudio.NET.Serialization.Natural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml.Base
{
    /// <summary>
    /// Provides a consise way to implement <see cref="NaturalSerializerService{T, TActual, TData, TDataActual}"/> for DocLang XML parsers.
    /// </summary>
    public abstract class DocParseService<T, TData> : NaturalSerializerService<IDocNode, T, XNode, TData> where T : IDocNode where TData : XNode
    {
        /// <inheritdoc/>
        public DocParseService(uint priority = 0) : base(priority)
        { }
    }
}
