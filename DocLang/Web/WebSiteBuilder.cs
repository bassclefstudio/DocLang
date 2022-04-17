using BassClefStudio.Storage;
using BassClefStudio.DocLang.Web;
using BassClefStudio.DocLang.Xml;
using System.Xml;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Sites
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

            using WebDocFormatter formatter = new WebDocFormatter();
            await formatter.InitializeAsync();
            using DocLangValidator validator = new DocLangValidator();

            IDictionary<string, IStorageFile> styles = new Dictionary<string, IStorageFile>();
            IDictionary<string, IStorageFile> assets = new Dictionary<string, IStorageFile>();
            IDictionary<string, IStorageFile> templates = new Dictionary<string, IStorageFile>();

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
                        templates[key] = templateFile;
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
                        styles[key] = await stylesheet.CopyToAsync(styleFolder, CollisionOptions.Overwrite, fileName);
                        this.LogSuccess("Loaded stylesheet {0} -> {1}.", stylesheet.GetRelativePath(source), styles[key].GetRelativePath(source));
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
                        assets[key] = await assetFile.CopyToAsync(assetsFolder, CollisionOptions.Overwrite, fileName);
                        this.LogSuccess("Loaded asset {0} -> {1}.", assetFile.GetRelativePath(source), assets[key].GetRelativePath(source));
                    }
                }

                IDictionary<string, string> varList = new Dictionary<string, string>();
                foreach (var style in styles)
                {
                    varList.Add($"styles.{style.Key}", $"assets/css/{style.Key}.css");
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
                            varList.Add(key, value);
                            this.LogSuccess("let {0} = \"{1}\"", key, value);
                        }
                    }
                }

                XmlWriterSettings writeSettings = new XmlWriterSettings() 
                { 
                    Async = true, 
                    OmitXmlDeclaration = true,
                    Indent = false,
                };

                XmlReaderSettings readSettings = new XmlReaderSettings() 
                {
                    Async = true,
                    DtdProcessing = DtdProcessing.Parse
                };

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
                        string? key = page.Attribute("Template")?.Value;
                        if (string.IsNullOrEmpty(key))
                        {
                            this.LogWarning("Ignoring Page {0} without template.", page);
                        }
                        else
                        {
                            string location = page.Attribute("Destination")?.Value ?? $"{pageFile.GetNameWithoutExtension()}.html";
                            IStorageFile destinationFile = await output.CreateFileAsync(location, CollisionOptions.Overwrite);

                            using (Stream tempStream = new MemoryStream())
                            {
                                using (IFileContent pageContent = await pageFile.OpenFileAsync())
                                using (Stream pageStream = pageContent.GetReadStream())
                                {
                                    DocumentType docType = await validator.ValidateAsync(pageStream, new DocumentType(DocLangXml.ContentType));
                                    pageStream.Seek(0, SeekOrigin.Begin);
                                    this.LogInformation("Detected {0} as {1}.", pageFile.GetRelativePath(source), docType);
                                    await formatter.ConvertAsync(pageStream, tempStream);
                                    tempStream.Seek(0, SeekOrigin.Begin);
                                }

                                using (IFileContent templateContent = await templates[key].OpenFileAsync())
                                using (Stream templateStream = templateContent.GetReadStream())
                                using (XmlReader templateReader = XmlReader.Create(templateStream, readSettings))
                                using (XmlReader contentReader = XmlReader.Create(tempStream, readSettings))
                                using (IFileContent destination = await destinationFile.OpenFileAsync(FileOpenMode.ReadWrite))
                                using (Stream destinationStream = destination.GetWriteStream())
                                using (XmlWriter destinationWriter = XmlWriter.Create(destinationStream, writeSettings))
                                using (WebTemplateReader reader = new WebTemplateReader(templateReader, contentReader, varList))
                                {
                                    await destinationWriter.WriteNodeAsync(reader, true);
                                    await destinationWriter.FlushAsync();
                                    this.LogSuccess("Loaded page {0} -> {1}.", pageFile.GetRelativePath(source), destinationFile.GetRelativePath(source));
                                }
                            }
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
