using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Provides an interface for DocLang documents and sites to resolve and interpret compile-time expressions.
    /// </summary>
    public interface IExpressionResolver
    {
        /// <summary>
        /// Asynchronously resolves the given <see cref="XNode"/> content and replaces compile-time expressions with their resulting values.
        /// </summary>
        /// <param name="content">A <see cref="XNode"/> containing content and DocLang compile-time expressions.</param>
        Task ResolveAsync(XNode content);
    }
}
