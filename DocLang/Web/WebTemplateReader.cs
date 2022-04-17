using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// 
    /// </summary>
    internal class WebTemplateReader : XmlReader
    {
        #region Templates

        public static readonly string ContentVar = "Body";

        /// <summary>
        /// An <see cref="XmlReader"/> for the current template file, which describes how the <see cref="ContentReader"/>'s content is displayed.
        /// </summary>
        public XmlReader TemplateReader { get; }

        /// <summary>
        /// An <see cref="XmlReader"/> for the content which will be inserted inside of the template.
        /// </summary>
        public XmlReader ContentReader { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> containing <see cref="string"/> values for the associated compile-time variables this <see cref="WebTemplateReader"/> can interpret.
        /// </summary>
        public IDictionary<string, string> Variables { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="ContentReader"/> is being read.
        /// </summary>
        private bool readingContent = false;

        /// <summary>
        /// Gets the <see cref="XmlReader"/> currently reading nodes.
        /// </summary>
        private XmlReader currentReader => readingContent ? ContentReader : TemplateReader;

        /// <summary>
        /// Creates a new <see cref="WebTemplateReader"/> from its component <see cref="XmlReader"/>s.
        /// </summary>
        /// <param name="template">An <see cref="XmlReader"/> for the current template file, which describes how the <see cref="ContentReader"/>'s content is displayed.</param>
        /// <param name="content">An <see cref="XmlReader"/> for the content which will be inserted inside of the template.</param>
        /// <param name="vars">An <see cref="IDictionary{TKey, TValue}"/> containing <see cref="string"/> values for the associated compile-time variables this <see cref="WebTemplateReader"/> can interpret.</param>
        public WebTemplateReader(XmlReader template, XmlReader content, IDictionary<string, string> vars)
        {
            TemplateReader = template;
            ContentReader = content;
            Variables = vars;
        }

        #endregion
        #region Reading

        private readonly Regex VarMatch = new Regex(@"@\{([^}]*)\}");

        /// <summary>
        /// Checks to see if a given <see cref="string"/> represents a variable access.
        /// </summary>
        /// <param name="content">The <see cref="string"/> being checked.</param>
        /// <returns>A <see cref="bool"/> that is true iff <paramref name="content"/> is non-null and begins (after trimming) with the '@' character.</returns>
        private bool IsVar([NotNullWhen(true)] string? content)
            => string.IsNullOrEmpty(content) ? false : VarMatch.Match(content).Success;

        /// <summary>
        /// Given a Regex <see cref="Match"/> corresponding to the <see cref="VarMatch"/> check, returns the <see cref="string"/> variable name represented.
        /// </summary>
        /// <param name="match">The produced Regex <see cref="Match"/>.</param>
        /// <returns>A <see cref="string"/> variable name.</returns>
        private string GetVar(Match match) => match.Groups[1].Value;

        /// <inheritdoc/>
        public override bool Read()
        {
            bool success = currentReader.Read();

            if (currentReader.EOF && readingContent)
            {
                readingContent = false;
                return currentReader.Read();
            }
            else if (currentReader.NodeType == XmlNodeType.Text && IsVar(currentReader.Value))
            {
                string varName = GetVar(VarMatch.Match(currentReader.Value));
                if (!readingContent && varName == ContentVar)
                {
                    readingContent = true;
                    return currentReader.Read();
                }
                else
                {
                    return success;
                }
            }
            else
            {
                return success;
            }
        }

        #endregion
        #region Override

        /// <inheritdoc/>
        public override int AttributeCount => currentReader.AttributeCount;

        /// <inheritdoc/>
        public override string BaseURI => currentReader.BaseURI;

        /// <inheritdoc/>
        public override int Depth => readingContent ? ContentReader.Depth + TemplateReader.Depth : TemplateReader.Depth;

        /// <inheritdoc/>
        public override bool EOF => currentReader.EOF;

        /// <inheritdoc/>
        public override bool IsEmptyElement => currentReader.IsEmptyElement;

        /// <inheritdoc/>
        public override string LocalName => currentReader.LocalName;

        /// <inheritdoc/>
        public override string NamespaceURI => currentReader.NamespaceURI;

        /// <inheritdoc/>
        public override XmlNameTable NameTable => currentReader.NameTable;

        /// <inheritdoc/>
        public override XmlNodeType NodeType => currentReader.NodeType;

        /// <inheritdoc/>
        public override string Prefix => currentReader.Prefix;

        /// <inheritdoc/>
        public override ReadState ReadState => currentReader.ReadState;

        /// <inheritdoc/>
        public override string Value
        {
            get
            {
                if(IsVar(currentReader.Value))
                {
                    return VarMatch.Replace(currentReader.Value, 
                        m =>
                        {
                            if (m.Success)
                            {
                                string key = GetVar(m);
                                if (Variables.ContainsKey(key))
                                {
                                    return Variables[key];
                                }
                                else
                                {
                                    throw new SiteBuilderException($"Could not resolve compile-time constant \"{key}\".");
                                }
                            }
                            else
                            {
                                return m.Value;
                            }
                        });
                }
                else
                {
                    return currentReader.Value;
                }
            }
        }

        /// <inheritdoc/>
        public override string GetAttribute(int i) => currentReader.GetAttribute(i);

        /// <inheritdoc/>
        public override string? GetAttribute(string name) => currentReader.GetAttribute(name);

        /// <inheritdoc/>
        public override string? GetAttribute(string name, string? namespaceURI) => currentReader.GetAttribute(name, namespaceURI);

        /// <inheritdoc/>
        public override string? LookupNamespace(string prefix)
        {
            string? name = currentReader.LookupNamespace(prefix);
            if (name == null && readingContent)
            {
                return TemplateReader.LookupNamespace(prefix);
            }
            else
            {
                return name;
            }
        }

        /// <inheritdoc/>
        public override bool MoveToAttribute(string name) => currentReader.MoveToAttribute(name);

        /// <inheritdoc/>
        public override bool MoveToAttribute(string name, string? ns) => currentReader.MoveToAttribute(name, ns);

        /// <inheritdoc/>
        public override bool MoveToElement() => currentReader.MoveToElement();

        /// <inheritdoc/>
        public override bool MoveToFirstAttribute() => currentReader.MoveToFirstAttribute();

        /// <inheritdoc/>
        public override bool MoveToNextAttribute() => currentReader.MoveToNextAttribute();

        /// <inheritdoc/>
        public override bool ReadAttributeValue() => currentReader.ReadAttributeValue();

        /// <inheritdoc/>
        public override void ResolveEntity() => currentReader.ResolveEntity();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TemplateReader.Dispose();
                ContentReader.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
