namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// Provides an <see cref="IDocValidator"/> for base DocLang XML files.
    /// </summary>
    public class DocLangValidator : XsdDocValidator
    {
        /// <inheritdoc/>
        public override DocumentType DocType { get; }
            = new DocumentType(DocLangXml.ContentType);

        /// <inheritdoc/>
        protected override async Task<Stream> GetSchemaAsync(DocumentType docType)
            => DocLangXml.GetSchema(docType.SchemaVersion ?? DocLangXml.LatestVersion);

        /// <inheritdoc/>
        protected override DocumentType GetDocType(DocumentType docType)
            => new DocumentType(
                DocLangXml.ContentType,
                docType.Is(DocType) && docType.SchemaVersion is not null
                    ? docType.SchemaVersion
                    : DocLangXml.LatestVersion);
    }
}
