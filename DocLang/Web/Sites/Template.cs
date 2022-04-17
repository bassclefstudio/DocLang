using BassClefStudio.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Web.Sites
{
    /// <summary>
    /// Represents a template file used to compile a <see cref="WebSiteBuilder"/>'s site output.
    /// </summary>
    /// <param name="AssetFile">The <see cref="IStorageFile"/> reference to this <see cref="Template"/>'s content.</param>
    /// <param name="Name">The friendly name of the <see cref="Template"/>.</param>
    public record Template(IStorageFile AssetFile, string Name) : Asset(AssetFile, Name);
}
