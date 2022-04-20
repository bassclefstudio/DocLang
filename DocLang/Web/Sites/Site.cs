using BassClefStudio.BassScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents all of the contained data that makes up a compiled DocLang website.
    /// </summary>
    public class Site : IRuntimeObject
    {
        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed CSS <see cref="StyleSheet"/> assets.
        /// </summary>
        public IDictionary<string, StyleSheet> Styles { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed generic <see cref="Asset"/>s.
        /// </summary>
        public IDictionary<string, Asset> Assets { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed <see cref="Template"/> assets.
        /// </summary>
        public IDictionary<string, Template> Templates { get; }

        /// <summary>
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed generic <see cref="object"/> constants used for site generation.
        /// </summary>
        public IDictionary<string, object?> Constants { get; }

        /// <inheritdoc/>
        public object? this[string key]
        {
            get => key switch
            {
                "styles" => Styles,
                "assets" => Assets,
                "templates" => Templates,
                "constants" => Constants,
                _ => throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.")
            };
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new empty <see cref="Site"/>.
        /// </summary>
        public Site()
        {
            // Initialize data fields:
            Styles = new Dictionary<string, StyleSheet>();
            Assets = new Dictionary<string, Asset>();
            Templates = new Dictionary<string, Template>();
            Constants = new Dictionary<string, object?>();
        }
    }
}
