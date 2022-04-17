using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// Provides a <see cref="WebSiteBuilder"/>-specific <see cref="IExpressionResolver"/> which can handle static site expressions and variables.
    /// </summary>
    public class SiteExpressionResolver : IExpressionResolver
    {
        #region Data

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed CSS stylesheet <see cref="IStorageFile"/>s.
        /// </summary>
        public IDictionary<string, IStorageFile> Styles { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed asset <see cref="IStorageFile"/>s.
        /// </summary>
        public IDictionary<string, IStorageFile> Assets { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed HTML template <see cref="IStorageFile"/>s.
        /// </summary>
        public IDictionary<string, IStorageFile> Templates { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed DocLang document <see cref="IStorageFile"/>s.
        /// </summary>
        public IDictionary<string, IStorageFile> Pages { get; }
        
        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed <see cref="string"/> constants used for site generation.
        /// </summary>
        public IDictionary<string, string> Constants { get; }

        /// <summary>
        /// The current <see cref="IStorageFile"/> (usually from <see cref="Pages"/>) representing the current content being compiled.
        /// </summary>
        public IStorageFile? Body { get; set; }

        #endregion
        #region Initialize

        /// <summary>
        /// Creates and initializes a new <see cref="SiteExpressionResolver"/>.
        /// </summary>
        public SiteExpressionResolver()
        {
            Styles = new Dictionary<string, IStorageFile>();
            Assets = new Dictionary<string, IStorageFile>();
            Templates = new Dictionary<string, IStorageFile>();
            Pages = new Dictionary<string, IStorageFile>();
            Constants = new Dictionary<string, string>();
        }

        #endregion
        #region Resolution
        #region Parsing

        

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

            if (content is XText text && text.Value.Contains("${Body}") && Body is not null)
            {
                content.ReplaceWith(await GetPageAsync(Body));
            }
        }

        /// <inheritdoc/>
        public async Task ResolveAsync(XAttribute content)
        { }

        /// <summary>
        /// Compiles the given DocLang <see cref="IStorageFile"/> file and returns the <see cref="XNode"/> contents.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> containing the DocLang information being processed.</param>
        /// <returns>The resulting <see cref="XNode"/> site content.</returns>
        private async Task<XNode> GetPageAsync(IStorageFile file)
        {
            using WebDocFormatter formatter = new WebDocFormatter();
            await formatter.InitializeAsync();
            using DocLangValidator validator = new DocLangValidator();
            using (Stream tempStream = new MemoryStream())
            {
                using (IFileContent pageContent = await file.OpenFileAsync())
                using (Stream pageStream = pageContent.GetReadStream())
                {
                    DocumentType docType = await validator.ValidateAsync(pageStream, new DocumentType(DocLangXml.ContentType));
                    pageStream.Seek(0, SeekOrigin.Begin);
                    await formatter.ConvertAsync(pageStream, tempStream);
                    tempStream.Seek(0, SeekOrigin.Begin);
                    XNode node = await XElement.LoadAsync(tempStream, LoadOptions.None, CancellationToken.None);
                    await ResolveAsync(node);
                    return node;
                }
            }
        }

        #endregion
    }
}
