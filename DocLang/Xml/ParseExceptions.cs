using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when an expected XML element or attribute was not found when serializing or deserializing an <see cref="IDocNode"/>.
    /// </summary>
    [Serializable]
    public class ParseSchemaException : Exception
    {
        public ParseSchemaException(string message) : base(message) { }
        public ParseSchemaException(string message, Exception inner) : base(message, inner) { }
    }
}
