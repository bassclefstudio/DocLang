using System.Xml;
using System.Xml.Schema;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// Provides an <see cref="IDocValidator"/> for base DocLang XML files.
    /// </summary>
    public class DocLangValidator : IDocValidator
    {
        /// <inheritdoc/>
        public DocumentType DocType { get; }
            = new DocumentType(DocLangXml.ContentType);

        /// <inheritdoc/>
        public async Task<DocumentType> ValidateAsync(Stream inputStream, DocumentType inputType)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Async = true,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            };

            Version schemaVersion = inputType.SchemaVersion ?? DocLangXml.LatestVersion;
            using (Stream schemaStream = DocLangXml.GetSchema(schemaVersion))
            using (var schemaReader = XmlReader.Create(schemaStream))
            {
                settings.Schemas.Add(null, schemaReader);
                settings.Schemas.ValidationEventHandler += SchemaValidationEvent;
            }

            using (var docReader = XmlReader.Create(inputStream, settings))
            {
                while (await docReader.ReadAsync()) { }
            }

            return new DocumentType(DocLangXml.ContentType, schemaVersion);
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

        /// <inheritdoc/>
        public void Dispose()
        { }
    }
}
