using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;
using BassClefStudio.SymbolicLanguage.Runtime;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents a DocLang page compiled into a <see cref="WebSiteBuilder"/>'s site output.
    /// </summary>
    /// <param name="XmlFile">The <see cref="IStorageFile"/> reference to this <see cref="Page"/>'s DocLang XMl file.</param>
    /// <param name="Name">The friendly name of the <see cref="Page"/>.</param>
    /// <param name="Template">The <see cref="Sites.Template"/> template used to compile the DocLang XML.</param>
    /// <param name="Resolver">A <see cref="IContentResolver"/> which is used specifically by this <see cref="Page"/> to resolve XML expressions.</param>
    /// <param name="IsDebug">A <see cref="bool"/> value indicating whether this <see cref="Page"/> should only be rendered and compiled in debug configurations.</param>
    public record Page(IStorageFile XmlFile, string Name, Template Template, IContentResolver Resolver, bool IsDebug = false) : Asset(XmlFile, Name), IRuntimeObject
    {
        #region Expressions

        /// <inheritdoc/>
        public object? this[string key] 
        {
            get
            {
                return key switch
                {
                    "compile" => new RuntimeMethod(async inputs => await CompileBody(inputs)),
                    "content" => this.Content,
                    "name" => this.Name,
                    "title" => this.Title,
                    "date" => this.PublishedDate,
                    _ => throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.")
                };
            }
            set => throw new NotImplementedException(); 
        }

        private async Task<object?> CompileBody(RuntimeContext context, object?[] inputs)
        {
            Guard.HasSizeEqualTo(inputs, 0, nameof(inputs));
            await CompileAsync(context);
            return null;
        }

        #endregion

        /// <summary>
        /// If <see cref="Compiled"/> is true (see <see cref="CompileAsync(IContentResolver)"/>), provides the <see cref="XElement"/> content of the website-compiled version of the DocLang <see cref="Asset.AssetFile"/>.
        /// </summary>
        public XElement? Content { get; private set; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether <see cref="CompileAsync(IContentResolver)"/> was successfully run and a compiled output <see cref="XElement"/> is available at <see cref="Content"/>.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Content))]
        public bool Compiled => Content != null;

        /// <summary>
        /// The <see cref="string"/> title of the page.
        /// </summary>
        [NotNullIfNotNull(nameof(Content))]
        public string? Title => Compiled ? Content?.Attribute("Name")?.Value ?? string.Empty : null;

        /// <summary>
        /// The indicated <see cref="DateTime"/> when this <see cref="Page"/> was released/published.
        /// </summary>
        [NotNullIfNotNull(nameof(Content))]
        public DateTime? PublishedDate => Compiled ? ((DateTime?)Content?.Attribute("Date")) ?? DateTime.Today : null;

        /// <summary>
        /// Compiles the given DocLang <see cref="Page"/> source file and sets <see cref="Content"/> appropriately.
        /// </summary>
        /// <param name="context">A <see cref="RuntimeContext"/> used as the context for the <see cref="IExpressionRuntime"/> which evaluates compile-time expressions.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        public async Task CompileAsync(RuntimeContext context)
        {
            if (!Compiled)
            {
                using WebDocFormatter formatter = new WebDocFormatter();
                await formatter.InitializeAsync();

                using DocLangValidator validator = new DocLangValidator();
                using (Stream tempInStream = new MemoryStream())
                using (Stream tempOutStream = new MemoryStream())
                using (IFileContent pageContent = await AssetFile.OpenFileAsync())
                using (Stream pageStream = pageContent.GetReadStream())
                {
                    DocumentType docType = await validator.ValidateAsync(pageStream, new DocumentType(DocLangXml.ContentType));
                    pageStream.Seek(0, SeekOrigin.Begin);
                    XElement content = await XElement.LoadAsync(pageStream, LoadOptions.None, CancellationToken.None);
                    await Resolver.ResolveAsync(content, context);
                    await content.SaveAsync(tempInStream, SaveOptions.None, CancellationToken.None);

                    tempInStream.Seek(0, SeekOrigin.Begin);
                    await formatter.ConvertAsync(tempInStream, tempOutStream);
                    tempOutStream.Seek(0, SeekOrigin.Begin);

                    XElement web = await XElement.LoadAsync(tempOutStream, LoadOptions.None, CancellationToken.None);
                    Content = web;
                }
            }
        }
    }
}
