using System.Net.Mime;
using System.Reflection.Metadata;

namespace BassClefStudio.DocLang.Base;

/// <summary>
/// An <see cref="IDocValidator"/> which arbitrarily validates a given type of document.
/// </summary>
public class RawDocValidator : IDocValidator
{
    /// <inheritdoc/>
    public DocumentType DocType { get; }

    /// <summary>
    /// Creates a new <see cref="RawDocValidator"/> for the desired <see cref="DocumentType"/>.
    /// </summary>
    /// <param name="desiredType">The <see cref="DocumentType"/> this <see cref="RawDocValidator"/> handles.</param>
    public RawDocValidator(DocumentType desiredType)
    {
        DocType = desiredType;
    }

    /// <inheritdoc/>
    public async Task<DocumentType> ValidateAsync(Stream inputStream, DocumentType inputType)
        => inputType;
    
    /// <inheritdoc/>
    public void Dispose()
    { }
}