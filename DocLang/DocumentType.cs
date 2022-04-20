using System.Net.Mime;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// Describes the type of an input or output document (usually provided as a <see cref="Stream"/>) in terms of its content type and schema (if applicable).
    /// </summary>
    public struct DocumentType
    {
        /// <summary>
        /// If applicable to the document, the <see cref="Version"/> schema version of the content type which this document implements.
        /// </summary>
        public Version? SchemaVersion { get; }

        /// <summary>
        /// The MIME type of the content contained in this document.
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// Creates a new <see cref="DocumentType"/>.
        /// </summary>
        /// <param name="type">The MIME type of the content contained in this document.</param>
        /// <param name="version">If applicable to the document, the <see cref="Version"/> schema version of the content type which this document implements.</param>
        public DocumentType(ContentType type, Version? version = null)
        {
            ContentType = type;
            SchemaVersion = version;
        }

        /// <summary>
        /// Creates a new <see cref="DocumentType"/>.
        /// </summary>
        /// <param name="type">The MIME type of the content contained in this document.</param>
        /// <param name="version">If applicable to the document, the <see cref="Version"/> schema version of the content type which this document implements.</param>
        public DocumentType(string type, Version? version = null)
        {
            ContentType = new ContentType(type);
            SchemaVersion = version;
        }

        /// <summary>
        /// Checks whether this <see cref="DocumentType"/> is an compatible (assignable to) another <see cref="DocumentType"/>.
        /// </summary>
        /// <param name="other">The other <see cref="DocumentType"/> being checked.</param>
        /// <returns>A <see cref="bool"/> indicating whether <c>this</c> is assignable to <paramref name="other"/>.</returns>
        public bool Is(DocumentType other)
        {
            return other.ContentType.Equals(ContentType) && (other.SchemaVersion is null || other.SchemaVersion == SchemaVersion);
        }

        /// <summary>
        /// Creates an unversioned <see cref="DocumentType"/> from this <see cref="ContentType"/> value.
        /// </summary>
        /// <param name="type">The <see cref="ContentType"/> type of the content.</param>
        /// <returns>A <see cref="DocumentType"/> equivalent to <paramref name="type"/>[any].</returns>
        public static implicit operator DocumentType(ContentType type)
        {
            return new DocumentType(type);
        }
        
        /// <summary>
        /// Creates an unversioned <see cref="DocumentType"/> from a <see cref="string"/> mime-type.
        /// </summary>
        /// <param name="type">The <see cref="string"/> mime-type of the content.</param>
        /// <returns>A <see cref="DocumentType"/> equivalent to <paramref name="type"/>[any].</returns>
        public static implicit operator DocumentType(string type)
        {
            return new DocumentType(type);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (SchemaVersion is null)
            {
                return $"{ContentType} [any]";
            }
            else
            {
                return $"{ContentType} [v{SchemaVersion}]";
            }
        }
    }
}
