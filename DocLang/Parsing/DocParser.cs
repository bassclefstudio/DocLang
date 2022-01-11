using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    public class DocParser
    {
        public IDocParseConfig Config { get; }

        /// <summary>
        /// Creates a new <see cref="DocParser"/> to handle the given configuration.
        /// </summary>
        /// <param name="config">The <see cref="IDocParseConfig"/> DocLang configuration.</param>
        public DocParser(IDocParseConfig config)
        {
            Config = config;
        }

        public IDocNode Read(XDocument xml)
        {

        }
    }
}
