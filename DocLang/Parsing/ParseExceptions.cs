using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Parsing
{

    [Serializable]
    public class ParseSchemaException : Exception
    {
        public ParseSchemaException(string message) : base(message) { }
        public ParseSchemaException(string message, Exception inner) : base(message, inner) { }
    }
}
