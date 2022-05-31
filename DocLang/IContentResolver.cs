using BassClefStudio.BassScript.Runtime;
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
    /// Provides an interface for documents to resolve and interpret content with compile-time expressions.
    /// </summary>
    public interface IContentResolver
    {
        /// <summary>
        /// Asynchronously resolves the given <see cref="string"/> content and replaces compile-time expressions with their resulting values.
        /// </summary>
        /// <param name="content">A <see cref="string"/> containing content and DocLang compile-time expressions.</param>
        /// <param name="context">The <see cref="RuntimeContext"/> from which the <paramref name="content"/>'s expressions.</param>
        /// <returns>The collection of nullable <see cref="object"/> items that represent the resolved tokens of the given <paramref name="content"/>.</returns>
        Task<IEnumerable<object?>> ResolveAsync(string content, RuntimeContext context);
        
        /// <summary>
        /// Asynchronously resolves the given <see cref="XNode"/> content and replaces compile-time expressions with their resulting values.
        /// </summary>
        /// <param name="content">A <see cref="XNode"/> containing content and DocLang compile-time expressions.</param>
        /// <param name="context">The <see cref="RuntimeContext"/> from which the <paramref name="content"/>'s expressions.</param>
        Task ResolveAsync(XNode content, RuntimeContext context);

        /// <summary>
        /// Asynchronously resolves the given <see cref="XAttribute"/> content and replaces compile-time expressions with their resulting values.
        /// </summary>
        /// <param name="content">A <see cref="XAttribute"/> containing content and DocLang compile-time expressions.</param>
        /// <param name="context">The <see cref="RuntimeContext"/> from which the <paramref name="content"/>'s expressions.</param>
        Task ResolveAsync(XAttribute content, RuntimeContext context);
    }
}
