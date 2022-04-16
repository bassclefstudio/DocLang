using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Sites
{
    /// <summary>
    /// Represents a service which can build multiple DocLang documents into some finished output.
    /// </summary>
    public interface ISiteBuilder
    {
        /// <summary>
        /// Builds a new site from a collection of DocLang, etc. files in a source <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="source">An <see cref="IStorageFolder"/> containing the source docuemnts and whatever configuration and assets are required to build and transform those DocLang documents into a final output.</param>
        /// <exception cref="SiteBuilderException"></exception>
        /// <returns>The <see cref="IStorageFolder"/> (which may be located inside of <paramref name="source"/>) containing the transformed output files.</returns>
        Task<IStorageFolder> BuildSiteAsync(IStorageFolder source);
    }
}
