using BassClefStudio.Storage;
using BassClefStudio.BassScript.Runtime;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents a generic asset file copied into a <see cref="WebSiteBuilder"/>'s site output.
    /// </summary>
    /// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="Asset"/>'s content.</param>
    /// <param name="Name">The friendly name of the <see cref="Asset"/>.</param>
    public record Asset(IStorageFile AssetFile, string Name) : IRuntimeObject
    {
        /// <inheritdoc/>
        public virtual object? this[string key]
        {
            get => key switch
            {
                "name" => this.Name,
                "file" => this.AssetFile,
                _ => throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.")
            };
            set => throw new NotImplementedException();
        }
    }
}
