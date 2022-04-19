using System.Xml.Linq;
using BassClefStudio.BassScript.Runtime;
using BassClefStudio.DocLang.Xml;
using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Web.Sites;

/// <summary>
/// Represents a DocLang <see cref="DocTemplate"/> which can be compiled into a <see cref="WebSiteBuilder"/>'s site output.
/// </summary>
/// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="Template"/>'s DocLang XMl file.</param>
/// <param name="Name">The friendly name of the <see cref="Template"/>.</param>
/// <param name="Resolver">A <see cref="IContentResolver"/> which is used specifically by this <see cref="Template"/> to resolve XML expressions.</param>
/// <param name="Validator">The <see cref="IDocValidator"/> used to validate the content of this DocLang page.</param>
/// <param name="Formatter">The <see cref="IDocFormatter"/> used to convert the DocLang content of this DocLang page to web (HTML) format.</param>
public record DocTemplate(IStorageFile AssetFile, string Name, IContentResolver Resolver, IDocValidator Validator, IDocFormatter Formatter) : Template(AssetFile, Name, Resolver)
{
    public override async Task<XElement> CompileAsync(RuntimeContext context)
    {
        using (Stream tempInStream = new MemoryStream())
        using (Stream tempOutStream = new MemoryStream())
        using (IFileContent pageContent = await AssetFile.OpenFileAsync())
        using (Stream pageStream = pageContent.GetReadStream())
        {
            DocumentType docType = await Validator.ValidateAsync(pageStream, new DocumentType(DocLangXml.ContentType));
            pageStream.Seek(0, SeekOrigin.Begin);
            XElement content = await XElement.LoadAsync(pageStream, LoadOptions.None, CancellationToken.None);
            await Resolver.ResolveAsync(content, context);
            await content.SaveAsync(tempInStream, SaveOptions.None, CancellationToken.None);
            tempInStream.Seek(0, SeekOrigin.Begin);
            await Formatter.ConvertAsync(tempInStream, tempOutStream);
            tempOutStream.Seek(0, SeekOrigin.Begin);
                    
            return await XElement.LoadAsync(tempOutStream, LoadOptions.None, CancellationToken.None);
        }
    }
}