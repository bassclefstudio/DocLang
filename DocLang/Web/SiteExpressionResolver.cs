using BassClefStudio.DocLang.Web.Sites;
using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;
using BassClefStudio.SymbolicLanguage.Parsers;
using BassClefStudio.SymbolicLanguage.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using BassClefStudio.SymbolicLanguage.Data;
using System.Text.RegularExpressions;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// Provides a <see cref="WebSiteBuilder"/>-specific <see cref="IExpressionResolver"/> which can handle static site expressions and variables.
    /// </summary>
    public class SiteExpressionResolver : IExpressionResolver, IRuntimeObject
    {
        #region Data

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed CSS <see cref="StyleSheet"/> assets.
        /// </summary>
        public IDictionary<string, StyleSheet> Styles { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed generic <see cref="Asset"/>s.
        /// </summary>
        public IDictionary<string, Asset> Assets { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed <see cref="Template"/> assets.
        /// </summary>
        public IDictionary<string, Template> Templates { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed DocLang <see cref="Page"/> assets.
        /// </summary>
        public IDictionary<string, Page> Pages { get; }
        
        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed generic <see cref="object"/> constants used for site generation.
        /// </summary>
        public IDictionary<string, object?> Fields { get; }

        /// <summary>
        /// The current <see cref="Page"/> (usually from <see cref="Pages"/>) representing the current data context of the <see cref="IExpressionResolver"/>.
        /// </summary>
        public Page? Body { get; set; }

        #region Expressions

        /// <summary>
        /// The <see cref="IExpressionRuntime"/> used to evaluate <see cref="IExpression"/> compile-time expressions.
        /// </summary>
        public IExpressionRuntime Runtime { get; }

        /// <summary>
        /// A <see cref="Regex"/> expression which can detect and replace compile-time expressions with their <see cref="Runtime"/> evaluations.
        /// </summary>
        private Regex ExpressionMatch { get; }

        /// <summary>
        /// An <see cref="ExpressionParser"/> for turning <see cref="string"/> representations of compile-time expressions into their <see cref="IExpression"/> equivalents.
        /// </summary>
        private ExpressionParser Parser { get; }

        /// <inheritdoc/>
        public IDictionary<string, RuntimeMethod> Methods { get; }

        /// <inheritdoc/>
        public object? this[string key]
        {
            get
            {
                if (key == "body") return Body;
                if (Fields.ContainsKey(key)) return Fields[key];
                if (Methods.ContainsKey(key)) return Methods[key];
                else throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.");
            }
            set
            {
                if (value is RuntimeMethod method) Methods[key] = method;
                else Fields[key] = value;
            }
        }

        #endregion
        #endregion
        #region Initialize

        /// <summary>
        /// Creates and initializes a new <see cref="SiteExpressionResolver"/>.
        /// </summary>
        public SiteExpressionResolver()
        {
            // Initialize data fields:
            Styles = new Dictionary<string, StyleSheet>();
            Assets = new Dictionary<string, Asset>();
            Templates = new Dictionary<string, Template>();
            Pages = new Dictionary<string, Page>();
            Fields = new Dictionary<string, object?>();

            // Initialize expressions root:
            Methods = new Dictionary<string, RuntimeMethod>();
            Runtime = new ExpressionRuntime();
            ExpressionMatch = new Regex(@"\${([^{]*)}");
            Parser = new ExpressionParser();
            InitializeExpressions();
        }

        private void InitializeExpressions()
        {
            Fields.Add("styles", Styles);
            Fields.Add("templates", Templates);
            Fields.Add("pages", Pages);
            Fields.Add("assets", Assets);
            Fields.Add("this", this);

            Methods.Add("let", ExpressionRuntime.Let(this));
        }

        #endregion
        #region Resolution
        #region Expressions

        private async Task<XElement?> CompileBody()
        {
            if(Body == null)
            {
                return null;
            }
            else if (!Body.Compiled)
            {
                await Body.CompileAsync(this);
            }
            return Body.Content;
        }

        #endregion

        /// <inheritdoc/>
        public async Task ResolveAsync(XNode content)
        {
            // Recursive resolution of child elements:
            if (content is XContainer container)
            {
                foreach (var node in container.Nodes().ToArray())
                {
                    await ResolveAsync(node);
                }

                if (content is XElement element)
                {
                    foreach (var attribute in element.Attributes().ToArray())
                    {
                        await ResolveAsync(attribute);
                    }
                }
            }

            if (content is XText text)
            {
                IEnumerable<object?> newContent = await ResolveAsync(text.Value);
                text.ReplaceWith(newContent.Select(c => (c is string s) ? new XText(s) : c).ToArray());
            }
        }

        /// <inheritdoc/>
        public async Task ResolveAsync(XAttribute content)
        {
            content.SetValue(string.Concat(await ResolveAsync(content.Value)));
        }

        private async Task<IEnumerable<object?>> ResolveAsync(string content)
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
                        newContent.Add(await Runtime.ExecuteAsync(expData, this));
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
