using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Metadata
{
    /// <summary>
    /// A representation of a person who contributed to part of a <see cref="Document"/>.
    /// </summary>
    public struct Author
    {
        /// <summary>
        /// The status/position of this author relative to the <see cref="Document"/> the author reference is attached to.
        /// </summary>
        public AuthorType Type { get; }

        /// <summary>
        /// The full name of the <see cref="Author"/>, as a <see cref="string"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new <see cref="Author"/> reference.
        /// </summary>
        /// <param name="type">The status/position of this author relative to the <see cref="Document"/> the author reference is attached to.</param>
        /// <param name="name">The full name of the <see cref="Author"/>, as a <see cref="string"/>.</param>
        public Author(AuthorType type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    /// <summary>
    /// A description of the status of a given <see cref="Author"/>.
    /// </summary>
    public enum AuthorType
    {
        /// <summary>
        /// This <see cref="Author"/> created this <see cref="Document"/>, and/or was the primary contributor.
        /// </summary>
        Creator = 0,

        /// <summary>
        /// This <see cref="Author"/> aided in the creation/writing of this <see cref="Document"/>.
        /// </summary>
        Contributor = 1,

        /// <summary>
        /// This <see cref="Author"/> helped proofread, edit, or otherwise revise the final document.
        /// </summary>
        Editor = 2,

        /// <summary>
        /// This <see cref="Author"/> is responsible for publishing the completed document.
        /// </summary>
        Publisher = 3
    }
}
