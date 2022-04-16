namespace BassClefStudio.DocLang.Sites
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when an <see cref="ISiteBuilder.BuildSiteAsync(AppModel.Storage.IStorageFolder)"/> encounters an issue building the content structure of the generated site.
    /// </summary>
    [Serializable]
    public class SiteBuilderException : Exception
    { 
        /// <inheritdoc/>
        public SiteBuilderException() { }
        /// <inheritdoc/>
        public SiteBuilderException(string message) : base(message) { }
        /// <inheritdoc/>
        public SiteBuilderException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected SiteBuilderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
