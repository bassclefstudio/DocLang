using BassClefStudio.Storage;
using BassClefStudio.DocLang.Xml;
using System.Xml;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Web
{
    /// <summary>
    /// Represents the configuration and content of a static site generated using DocLang.
    /// </summary>
    public class WebSiteBuilder : ISiteBuilder
    {
        /// <summary>
        /// The name of the configuration document file within every source folder describing a website.
        /// </summary>
        public static readonly string ConfigFile = "config.xml";

        /// <summary>
        /// The <see cref="string"/> XML namespace for <see cref="WebSiteBuilder"/> configuration files.
        /// </summary>
        public static readonly string ConfigNamespace = "http://bassclefstudio.dev/DocLang/v1/Config";

        /// <summary>
        /// The default name of the output folder within the source <see cref="IStorageFolder"/>.
        /// </summary>
        public static readonly string OutputFolder = ".site";

        /// <summary>
        /// Creates a new <see cref="WebSiteBuilder"/>.
        /// </summary>
        public WebSiteBuilder()
        { }

        /// <inheritdoc/>
        public async Task<IStorageFolder> BuildSiteAsync(IStorageFolder source)
        {
            if(!await source.ContainsItemAsync(ConfigFile))
            {
                throw new StorageAccessException($"Cannot find configuration file \"{ConfigFile}\" within the source folder {source}.");
            }

            // Remove the existing .site/ folder if it currently exists.
            if(await source.ContainsItemAsync(OutputFolder))
            {
                IStorageFolder existing = await source.GetFolderAsync(OutputFolder);
                await existing.RemoveAsync();
            }
            IStorageFolder output = await source.CreateFolderAsync(OutputFolder, CollisionOptions.Overwrite);
            
            IStorageFolder assetsFolder = await output.CreateFolderAsync("assets");
            IStorageFolder styleFolder = await assetsFolder.CreateFolderAsync("css");

            SiteExpressionResolver resolver = new SiteExpressionResolver();

            IStorageFile configFile = await source.GetFileAsync(ConfigFile);
            using (var fileStream = await configFile.OpenFileAsync(FileOpenMode.Read))
            {
                XDocument config = XDocument.Load(fileStream.GetReadStream());
                if (config.Root is null)
                {
                    throw new SiteBuilderException("Failed to open the root element of the XML website configuration file.");
                }

                foreach (var template in config.Root.Elements(XName.Get("Template", ConfigNamespace)))
                {
                    var path = template.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Template {0} without path.", template);
                    }
                    else
                    {
                        IStorageFile templateFile = await source.GetFileAsync(path);
                        string key = template.Attribute("Key")?.Value ?? templateFile.GetNameWithoutExtension();
                        this.LogSuccess("Loaded template {0}.", templateFile.GetRelativePath(source));
                        resolver.Templates[key] = templateFile;
                    }
                }

                foreach (var style in config.Root.Elements(XName.Get("Style", ConfigNamespace)))
                {
                    var path = style.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Style {0} without path.", style);
                    }
                    else
                    {
                        IStorageFile stylesheet = await source.GetFileAsync(path);
                        string key = style.Attribute("Key")?.Value ?? stylesheet.GetNameWithoutExtension();
                        string fileName = $"{key}.css";
                        resolver.Styles[key] = await stylesheet.CopyToAsync(styleFolder, CollisionOptions.Overwrite, fileName);
                        this.LogSuccess("Loaded stylesheet {0} -> {1}.", stylesheet.GetRelativePath(source), resolver.Styles[key].GetRelativePath(source));
                    }
                }

                foreach (var asset in config.Root.Elements(XName.Get("Asset", ConfigNamespace)))
                {
                    var path = asset.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Asset {0} without path.", asset);
                    }
                    else
                    {
                        IStorageFile assetFile = await source.GetFileAsync(path);
                        string key = asset.Attribute("Key")?.Value ?? assetFile.GetNameWithoutExtension();
                        string fileName = $"{key}.{assetFile.FileType}";
                        resolver.Assets[key] = await assetFile.CopyToAsync(assetsFolder, CollisionOptions.Overwrite, fileName);
                        this.LogSuccess("Loaded asset {0} -> {1}.", assetFile.GetRelativePath(source), resolver.Assets[key].GetRelativePath(source));
                    }
                }

                foreach (var constant in config.Root.Elements(XName.Get("Constant", ConfigNamespace)))
                {
                    var value = constant.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        this.LogWarning("Ignoring Variable {0} without value.", constant);
                    }
                    else
                    {
                        string? key = constant.Attribute("Name")?.Value;
                        if (string.IsNullOrEmpty(key))
                        {
                            this.LogWarning("Ignoring Variable {0} without name.", constant);
                        }
                        else
                        {
                            resolver.Constants.Add(key, value);
                            this.LogSuccess("let {0} = \"{1}\"", key, value);
                        }
                    }
                }

                IDictionary<string, string?> pageTemplates = new Dictionary<string, string?>();
                foreach (var page in config.Root.Elements(XName.Get("Page", ConfigNamespace)))
                {
                    var path = page.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Page {0} without path.", page);
                    }
                    else
                    {
                        IStorageFile pageFile = await source.GetFileAsync(path);
                        string key = page.Attribute("Destination")?.Value ?? pageFile.GetNameWithoutExtension();
                        this.LogSuccess("Found page {0} at '{1}'.", key, pageFile.GetRelativePath(source));
                        resolver.Pages[key] = pageFile;
                        pageTemplates[key] = page.Attribute("Template")?.Value;
                    }
                }

                foreach (var page in resolver.Pages)
                {
                    string? templateName = pageTemplates[page.Key];
                    if (string.IsNullOrEmpty(templateName))
                    {
                        this.LogWarning("Ignoring Page {0} without template.", page);
                    }
                    else
                    {
                        string location = $"{page.Key}.html";
                        IStorageFile destinationFile = await output.CreateFileAsync(location, CollisionOptions.Overwrite);

                        using (IFileContent templateContent = await resolver.Templates[templateName].OpenFileAsync())
                        using (Stream templateStream = templateContent.GetReadStream())
                        using (IFileContent destination = await destinationFile.OpenFileAsync(FileOpenMode.ReadWrite))
                        using (Stream destinationStream = destination.GetWriteStream())
                        {
                            resolver.Body = page.Value;
                            XElement template = await XElement.LoadAsync(templateStream, LoadOptions.None, CancellationToken.None);
                            await resolver.ResolveAsync(template);
                            await template.SaveAsync(destinationStream, SaveOptions.None, CancellationToken.None);
                            await destinationStream.FlushAsync();
                            this.LogSuccess("Compiled page {0} -> {1}.", page.Value.GetRelativePath(source), destinationFile.GetRelativePath(source));
                        }
                    }
                }
                return output;
            }
        }
        private void LogInformation(string format, params object?[] args)
        {
            ConsoleColor returnColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(format, args);
            Console.ForegroundColor = returnColor;
        }

        private void LogWarning(string format, params object?[] args)
        {
            ConsoleColor returnColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
            Console.ForegroundColor = returnColor;
        }

        private void LogError(string format, params object?[] args)
        {
            ConsoleColor returnColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = returnColor;
        }

        private void LogSuccess(string format, params object?[] args)
        {
            ConsoleColor returnColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(format, args);
            Console.ForegroundColor = returnColor;
        }
    }
}
