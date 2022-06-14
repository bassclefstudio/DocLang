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
    public class Site : Group
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
        /// An <see cref="IDictionary{TKey, TValue}"/> of keyed generic <see cref="object"/> constants used for site generation.
        /// </summary>
        public IDictionary<string, object?> Constants { get; }

        /// <summary>
        /// The root path/URL used for calculating links within the entirety of the <see cref="Site"/>.
        /// </summary>
        public string Location { get; }
        
        /// <inheritdoc/>
        public override object? this[string key]
        {
            get => key switch
            {
                "location" => Location,
                "styles" => Styles,
                "assets" => Assets,
                "templates" => Templates,
                "pages" => Pages,
                "constants" => Constants,
                "groups" => Groups,
                _ => throw new KeyNotFoundException($"Could not find \"{key}\" in the current context.")
            };
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new empty <see cref="Site"/>.
        /// </summary>
        public Site(string location)
        {
            Location = location;
            // Initialize data fields:
            Styles = new Dictionary<string, StyleSheet>();
            Assets = new Dictionary<string, Asset>();
            Constants = new Dictionary<string, object?>();
        }
    }
}
