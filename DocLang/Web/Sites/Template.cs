using System.Net.Mime;
using System.Xml.Linq;
using BassClefStudio.BassScript.Runtime;
using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Web.Sites;

/// <summary>
/// Represents a DocLang <see cref="BassClefStudio.DocLang.Web.Sites.Template"/> which can be compiled into a <see cref="WebSiteBuilder"/>'s site output.
/// </summary>
/// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="BassClefStudio.DocLang.Web.Sites.Template"/>'s DocLang XMl file.</param>
/// <param name="Name">The friendly name of the <see cref="BassClefStudio.DocLang.Web.Sites.Template"/>.</param>
/// <param name="Resolver">A <see cref="IContentResolver"/> which is used specifically by this <see cref="BassClefStudio.DocLang.Web.Sites.Template"/> to resolve XML expressions.</param>
/// <param name="Validator">The <see cref="IDocValidator"/> used to validate the content of this DocLang page.</param>
/// <param name="Formatter">The <see cref="IDocFormatter"/> used to convert the DocLang content of this DocLang page to web (HTML) format.</param>
public record Template(
    IStorageFile AssetFile,
    string Name,
    IContentResolver Resolver,
    IDocValidator Validator,
    IDocFormatter Formatter) : Asset(AssetFile, Name)
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
                    foreach (var input in inputs)
                    {
                        if (input is DefBinding def)
                            def(context);
                        else
                            throw new ArgumentException(
                                "Inputs to the compile() method must be valid definition bindings!");
                    }

                    return await CompileAsync(context);
                };
            }

            return compileMethod;
        }
    }

    /// <summary>
    /// Compiles the given <see cref="Template"/> within the current <see cref="RuntimeContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="RuntimeContext"/> providing parameters and other context during compilation.</param>
    /// <returns>The resulting <see cref="XElement"/> page content.</returns>
    public async Task<XElement> CompileAsync(RuntimeContext context)
    {
        using (Stream tempInStream = new MemoryStream())
        using (Stream tempOutStream = new MemoryStream())
        using (IFileContent pageContent = await AssetFile.OpenFileAsync())
        using (Stream pageStream = pageContent.GetReadStream())
        {
            DocumentType docType = await Validator.ValidateAsync(pageStream, MediaTypeNames.Application.Xml);
            if (docType.ContentType.MediaType == MediaTypeNames.Application.Xml ||
                docType.ContentType.Equals(DocLangXml.ContentType))
            {
                // TODO: Allow for content types other than XML and DocLang to be resolved (through non-XML means)
                pageStream.Seek(0, SeekOrigin.Begin);
                XElement content = await XElement.LoadAsync(pageStream, LoadOptions.None, CancellationToken.None);
                await Resolver.ResolveAsync(content, context);
                await content.SaveAsync(tempInStream, SaveOptions.None, CancellationToken.None);
            }
            else
            {
                pageStream.Seek(0, SeekOrigin.Begin);
                await pageStream.CopyToAsync(tempInStream);
            }
            
            tempInStream.Seek(0, SeekOrigin.Begin);
            await Formatter.ConvertAsync(tempInStream, tempOutStream);
            tempOutStream.Seek(0, SeekOrigin.Begin);

            return await XElement.LoadAsync(tempOutStream, LoadOptions.None, CancellationToken.None);
        }
    }
}