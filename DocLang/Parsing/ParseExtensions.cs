using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Provides extension methods to aid in the creation of <see cref="IDocParser{T}"/>s.
    /// </summary>
    public static class ParseExtensions
    {
        /// <summary>
        /// Enforces the presence of a child <see cref="XElement"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The <see cref="XElement"/> child of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static XElement EnforceElement(this XContainer parent, string key)
        {
            var child = parent.Element(key);
            if (child is null)
            {
                throw new ParseSchemaException($"Expected required element {key} in XML {parent}.");
            }
            return child;
        }

        /// <summary>
        /// Enforces the presence of a child <see cref="XAttribute"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The <see cref="XAttribute"/> child of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static XAttribute EnforceAttribute(this XElement parent, string key)
        {
            var child = parent.Attribute(key);
            if (child is null)
            {
                throw new ParseSchemaException($"Expected required attribute {key} in XML {parent}.");
            }
            return child;
        }

        /// <summary>
        /// Enforces the presence of at least one child <see cref="XElement"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The collection of <see cref="XElement"/> children of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static IEnumerable<XElement> EnforceElements(this XContainer parent, string key)
        {
            var children = parent.Elements(key);
            if (!children.Any())
            {
                throw new ParseSchemaException($"Expected one or more required {key} elements in XML {parent}.");
            }
            return children;
        }
    }
}
