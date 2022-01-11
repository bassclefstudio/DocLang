using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Provides a means through which DocLang can be configured to create various <see cref="IDocNode"/> nodes from XML content and vice-versa.
    /// </summary>
    public interface IDocParseConfig
    {
        IDictionary<string, Type> NodeTypes { get; }
    }
}
