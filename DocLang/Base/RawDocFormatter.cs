namespace BassClefStudio.DocLang.Base;

/// <summary>
/// An <see cref="IDocFormatter"/> which returns the raw file contents of a given document.
/// </summary>
public class RawDocFormatter : IDocFormatter
{
    /// <inheritdoc/>
    public DocumentType InputType { get; }
    
    /// <inheritdoc/>
    public DocumentType OutputType { get; }

    /// <summary>
    /// Creates a new <see cref="RawDocFormatter"/> for the desired <see cref="DocumentType"/>.
    /// </summary>
    /// <param name="desiredType">The <see cref="DocumentType"/> this <see cref="RawDocFormatter"/> handles.</param>
    public RawDocFormatter(DocumentType desiredType)
    {
        InputType = desiredType;
        OutputType = desiredType;
    }
    
    /// <inheritdoc/>
    public async Task InitializeAsync()
    { }

    /// <inheritdoc/>
    public async Task ConvertAsync(Stream inputStream, Stream outputStream)
    {
        await inputStream.CopyToAsync(outputStream);
    }
    
    /// <inheritdoc/>
    public void Dispose()
    { }
}