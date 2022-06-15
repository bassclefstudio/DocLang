using BassClefStudio.BassScript.Parsers;
using BassClefStudio.BassScript.Runtime;
using System.Collections;
using System.Xml.Linq;
using BassClefStudio.BassScript.Data;
using System.Text.RegularExpressions;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// Provides a <see cref="WebSiteBuilder"/>-specific <see cref="IContentResolver"/> which can handle static site expressions and variables.
    /// </summary>
    public class SiteContentResolver : IContentResolver
    {
        #region Data
        #region Expressions

        /// <summary>
        /// The <see cref="IExpressionRuntime"/> used to evaluate <see cref="IExpression"/> compile-time expressions.
        /// </summary>
        private IExpressionRuntime Runtime { get; }

        /// <summary>
        /// A <see cref="Regex"/> expression which can detect and replace compile-time expressions with their <see cref="Runtime"/> evaluations.
        /// </summary>
        private Regex ExpressionMatch { get; }

        /// <summary>
        /// An <see cref="ExpressionParser"/> for turning <see cref="string"/> representations of compile-time expressions into their <see cref="IExpression"/> equivalents.
        /// </summary>
        private ExpressionParser Parser { get; }

        #endregion
        #endregion
        #region Initialize

        /// <summary>
        /// Creates and initializes a new <see cref="SiteContentResolver"/>.
        /// </summary>
        public SiteContentResolver()
        {
            // Initialize expressions root:
            Runtime = new ExpressionRuntime();
            ExpressionMatch = new Regex(@"\${([^{]*)}");
            Parser = new ExpressionParser();
        }

        #endregion
        #region Resolution

        /// <inheritdoc/>
        public async Task ResolveAsync(XNode content, RuntimeContext context)
        {
            // Recursive resolution of child elements:
            if (content is XContainer container)
            {
                foreach (var node in container.Nodes().ToArray())
                {
                    await ResolveAsync(node, context);
                }

                if (content is XElement element)
                {
                    foreach (var attribute in element.Attributes().ToArray())
                    {
                        await ResolveAsync(attribute, context);
                    }
                }
            }

            if (content is XText text)
            {
                IEnumerable<object?> newContent = await ResolveAsync(text.Value, context);
                text.ReplaceWith(newContent.Select(c => (c is string s) ? new XText(s) : c).ToArray());
            }
        }

        /// <inheritdoc/>
        public async Task ResolveAsync(XAttribute content, RuntimeContext context)
        {
            content.SetValue(string.Concat(await ResolveAsync(content.Value, context)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<object?>> ResolveAsync(string content, RuntimeContext context)
        {
            var matches = ExpressionMatch.Matches(content);
            int[] matchStarts = matches.Select(m => m.Index).ToArray();
            int[] matchLengths = matches.Select(m => m.Length).ToArray();
            string[] matchExps = matches.Select(m => m.Groups[1].Value).ToArray();
            int position = 0;
            int index = 0;
            List<object?> newContent = new List<object?>();
            while (position < content.Length)
            {
                if (index < matchStarts.Length)
                {
                    if (position == matchStarts[index])
                    {
                        IExpression expData = Parser.BuildExpression(matchExps[index]);
                        var result = await Runtime.ExecuteAsync(expData, context);
                        if (result is not string && result is IEnumerable results)
                        {
                            newContent.AddRange(results.Cast<object?>());
                        }
                        else
                        {
                            newContent.Add(result);
                        }

                        position += matchLengths[index];
                        index++;
                    }
                    else
                    {
                        newContent.Add(content.Substring(position, matchStarts[index] - position));
                        position = matchStarts[index];
                    }
                }
                else
                {
                    newContent.Add(content.Substring(position));
                    position = content.Length;
                }
            }
            return newContent;
        }

        #endregion
    }
}
