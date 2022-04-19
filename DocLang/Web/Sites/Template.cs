using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;
using BassClefStudio.BassScript.Runtime;
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
    /// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="Template"/>'s DocLang XMl file.</param>
    /// <param name="Name">The friendly name of the <see cref="Template"/>.</param>
    /// <param name="Resolver">A <see cref="IContentResolver"/> which is used specifically by this <see cref="Template"/> to resolve XML expressions.</param>
    public record Template(IStorageFile AssetFile, string Name, IContentResolver Resolver) : Asset(AssetFile, Name)
    {
        /// <inheritdoc/>
        public override object? this[string key] 
        {
            get
            {
                return key switch
                {
                    "compile" => CompileMethod,
                    "name" => Name,
                    _ => throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.")
                };
            }
            set => throw new NotImplementedException(); 
        }

        private RuntimeMethod? compileMethod = null;
        /// <summary>
        /// Gets the <see cref="RuntimeMethod"/> equivalent method for <see cref="CompileAsync(RuntimeContext)"/>.
        /// </summary>
        private RuntimeMethod CompileMethod
        {
            get
            {
                if (compileMethod == null)
                {
                    compileMethod = async (context, inputs) =>
                    {
                        Guard.HasSizeEqualTo(inputs, 0, nameof(inputs));
                        return await CompileAsync(context);
                    };
                }

                return compileMethod;
            }
        }

        /// <summary>
        /// Compiles the given DocLang <see cref="Template"/> source file.
        /// </summary>
        /// <param name="context">A <see cref="RuntimeContext"/> used as the context for the <see cref="IExpressionRuntime"/> which evaluates compile-time expressions.</param>
        /// <returns>The resulting <see cref="XElement"/> item.</returns>
        public virtual async Task<XElement> CompileAsync(RuntimeContext context)
        {
            using (Stream tempOutStream = new MemoryStream())
            using (IFileContent pageContent = await AssetFile.OpenFileAsync())
            using (Stream pageStream = pageContent.GetReadStream())
            {
                XElement content = await XElement.LoadAsync(pageStream, LoadOptions.None, CancellationToken.None);
                await Resolver.ResolveAsync(content, context);
                await content.SaveAsync(tempOutStream, SaveOptions.None, CancellationToken.None);
                tempOutStream.Seek(0, SeekOrigin.Begin);
                return await XElement.LoadAsync(tempOutStream, LoadOptions.None, CancellationToken.None);
            }
        }
    }
}
