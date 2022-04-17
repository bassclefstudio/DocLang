using BassClefStudio.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents a generic asset file copied into a <see cref="WebSiteBuilder"/>'s site output.
    /// </summary>
    /// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="Asset"/>'s content.</param>
    /// <param name="Name">The friendly name of the <see cref="Asset"/>.</param>
    public record Asset(IStorageFile AssetFile, string Name);
}
