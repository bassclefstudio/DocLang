using System.Net.Mime;
using System.Xml;
using System.Xml.Schema;
using BassClefStudio.Storage;

namespace BassClefStudio.DocLang.Xml;

/// <summary>
/// Provides an abstract implementation of <see cref="IDocValidator"/> which validates an XML file based on a provided XSD schema.
/// </summary>
public abstract class XsdDocValidator : IDocValidator
{
    /// <inheritdoc/>
    public abstract DocumentType DocType { get; }
    
    /// <inheritdoc/>
        public async Task<DocumentType> ValidateAsync(Stream inputStream, DocumentType inputType)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Async = true,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            };

            DocumentType docType = GetDocType(inputType);
            await using (Stream schemaStream = await GetSchemaAsync(docType))
            using (var schemaReader = XmlReader.Create(schemaStream))
            {
                settings.Schemas.Add(null, schemaReader);
                settings.Schemas.ValidationEventHandler += SchemaValidationEvent;
            }

            using (var docReader = XmlReader.Create(inputStream, settings))
            {
                while (await docReader.ReadAsync()) { }
            }

            return docType;
        }

    /// <summary>
    /// Records information passed from the <see cref="XmlSchemaSet"/> about a parsed document.
    /// </summary>
    /// <param name="sender">The <see cref="XmlSchemaSet"/> that triggered the event.</param>
    /// <param name="e">The <see cref="ValidationEventArgs"/> schema validation message.</param>
    private void SchemaValidationEvent(object? sender, ValidationEventArgs e)
    {
        using (var errStream = Console.OpenStandardError())
        using (var err = new StreamWriter(errStream))
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                err.Write("WARNING: ");
                err.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                err.Write("ERROR: ");
                err.WriteLine(e.Message);
            }

            err.Flush();
        }
    }

    /// <summary>
    /// Gets the <see cref="Stream"/> contents of the XSD schema being used to validate provided XML.
    /// </summary>
    /// <param name="docType">The <see cref="DocumentType"/> of the document in question.</param>
    /// <returns>The <see cref="Stream"/> contents of an XSD schema file.</returns>
    protected abstract Task<Stream> GetSchemaAsync(DocumentType docType);
    
    /// <summary>
    /// Gets the <see cref="DocumentType"/> which will be validated for the provided <see cref="DocumentType"/> input.
    /// </summary>
    /// <param name="docType">The <see cref="DocumentType"/> input type of the document being loaded.</param>
    /// <returns>The specific <see cref="DocumentType"/> being validated.</returns>
    protected abstract DocumentType GetDocType(DocumentType docType);
    
    /// <inheritdoc/>
    public virtual void Dispose()
    { }
}

public class XsdFileValidator : XsdDocValidator
{
    /// <inheritdoc/>
    public override DocumentType DocType { get; } = MediaTypeNames.Application.Xml;

    /// <summary>
    /// The <see cref="IStorageFile"/> reference to the XSLT being used to transform documents.
    /// </summary>
    protected IStorageFile File { get; }

    /// <summary>
    /// Creates a new <see cref="XsdFileValidator"/> from the given file.
    /// </summary>
    /// <param name="file">The <see cref="IStorageFile"/> reference to the XSD schema used to validate documents.</param>
    public XsdFileValidator(IStorageFile file)
    {
        File = file;
    }

    /// <summary>
    /// The <see cref="IFileContent"/> content of <see cref="File"/>, which is loaded to get the data for <see cref="GetSchemaAsync"/>.
    /// </summary>
    private IFileContent? FileContent { get; set; }

    /// <inheritdoc/>
    protected override async Task<Stream> GetSchemaAsync(DocumentType docType)
    {
        FileContent ??= await File.OpenFileAsync();
        return FileContent.GetReadStream();
    }

    /// <inheritdoc/>
    protected override DocumentType GetDocType(DocumentType docType)
    {
        return docType.Is(DocType) ? docType : DocType;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        FileContent?.Dispose();
        base.Dispose();
    }
}