using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents a CSS stylesheet copied into a <see cref="WebSiteBuilder"/>'s site output.
    /// </summary>
    /// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="StyleSheet"/>'s CSS content.</param>
    /// <param name="Name">The friendly name of the <see cref="StyleSheet"/>.</param>
    public record StyleSheet(IStorageFile AssetFile, string Name) : Asset(AssetFile, Name);
}
