using BassClefStudio.BassScript.Runtime;
using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Web.Sites;

/// <summary>
/// Represents a DocLang <see cref="BassClefStudio.DocLang.Web.Sites.Template"/> which can be compiled into a <see cref="WebSiteBuilder"/>'s site output.
/// </summary>
/// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="BassClefStudio.DocLang.Web.Sites.Page"/>'s webpage file.</param>
/// <param name="Name">The friendly name of the <see cref="BassClefStudio.DocLang.Web.Sites.Page"/>.</param>
/// <param name="Template">The <see cref="BassClefStudio.DocLang.Web.Sites.Template"/> used to build this page.</param>
/// <param name="Body">The <see cref="BassClefStudio.DocLang.Web.Sites.Template"/>, if applicable, used to build the body of the page within the provided <see cref="Template"/>.</param>
public record Page(
    IStorageFile AssetFile,
    string Name,
    IContentResolver Resolver,
    IDictionary<string, object?> Properties,
    Template Template,
    Template? Body = null) : Asset(AssetFile, Name)
{
    /// <inheritdoc/>
    public override object? this[string key]
    {
        get
        {
            return key switch
            {
                "file" => AssetFile,
                "template" => Template,
                "body" => Body,
                "name" => Name,
                _ => Properties[key]
            };
        }
        set => throw new NotImplementedException();
    }
}