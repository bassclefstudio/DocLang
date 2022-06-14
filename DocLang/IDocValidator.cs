namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Provides an interface for services that can validate DocLang documents of a specific schema.
    /// </summary>
    public interface IDocValidator : IDisposable
    {
        /// <summary>
        /// The type of documents this <see cref="IDocValidator"/> checks.
        /// </summary>
        DocumentType DocType { get; }

        /// <summary>
        /// Attempts to asynchronously validate the document read from the given <see cref="Stream"/> per this <see cref="IDocValidator"/>'s schema definition.
        /// </summary>
        /// <param name="inputStream">The readable <see cref="Stream"/> containing the DocLang document.</param>
        /// <param name="inputType">A <see cref="DocumentType"/> indicating the known type of the document (within this service's <see cref="DocType"/>), which may omit things such as version number to have them be resolved dynamically by this <see cref="IDocValidator"/>.</param>
        /// <returns>A specific <see cref="DocumentType"/> indicating the exact schema specifications against which this document was validated. Usually a subset of <paramref name="inputType"/>.</returns>
        Task<DocumentType> ValidateAsync(Stream inputStream, DocumentType inputType);
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown when an <see cref="IDocValidator"/> attempts to validate part of a document that does not match the registered schema.
    /// </summary>
    [Serializable]
    public class SchemaValidationException : Exception
    {
        /// <inheritdoc/>
        public SchemaValidationException() { }

        /// <inheritdoc/>
        public SchemaValidationException(string message) : base(message) { }
        
        /// <inheritdoc/>
        public SchemaValidationException(string message, Exception inner) : base(message, inner) { }

        /// <inheritdoc/>
        protected SchemaValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
