using System.Collections;
using BassClefStudio.Storage;
using System.Xml.Linq;
using BassClefStudio.BassScript.Runtime;
using BassClefStudio.DocLang.Web.Sites;
using BassClefStudio.DocLang.Xml;

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

        /// <inheritdoc/>
        public async Task<IStorageFolder> BuildSiteAsync(IStorageFolder source)
        {
            if (!await source.ContainsItemAsync(ConfigFile))
            {
                throw new StorageAccessException(
                    $"Cannot find configuration file \"{ConfigFile}\" within the source folder {source}.");
            }

            // Remove the existing .site/ folder if it currently exists.
            if (await source.ContainsItemAsync(OutputFolder))
            {
                IStorageFolder existing = await source.GetFolderAsync(OutputFolder);
                await existing.RemoveAsync();
            }

            IStorageFolder output = await source.CreateFolderAsync(OutputFolder, CollisionOptions.Overwrite);

            IStorageFolder assetsFolder = await output.CreateFolderAsync("assets");
            IStorageFolder styleFolder = await assetsFolder.CreateFolderAsync("css");
            Site site = new Site();
            SiteContentResolver resolver = new SiteContentResolver();
            // Create the context and the core methods for evaluating expressions.
            RuntimeContext context = new RuntimeContext();
            InitializeContext(
                context,
                site);
            IStorageFile configFile = await source.GetFileAsync(ConfigFile);
            using (var validator = new DocLangValidator())
            using (var formatter = new WebDocFormatter())
            using (var fileStream = await configFile.OpenFileAsync(FileOpenMode.Read))
            {
                await formatter.InitializeAsync();
                XDocument config = XDocument.Load(fileStream.GetReadStream());
                if (config.Root is null)
                {
                    throw new SiteBuilderException(
                        "Failed to open the root element of the XML website configuration file."
                    );
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
                        site.Styles[key] = new Sites.StyleSheet(
                            await stylesheet.CopyToAsync(styleFolder, CollisionOptions.Overwrite, fileName),
                            key);
                        this.LogSuccess(
                            "Loaded stylesheet {0} -> {1}.",
                            stylesheet.GetRelativePath(source),
                            site.Styles[key].AssetFile.GetRelativePath(source));
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
                        site.Assets[key] = new Sites.Asset(
                            await assetFile.CopyToAsync(assetsFolder, CollisionOptions.Overwrite, fileName),
                            key);
                        this.LogSuccess(
                            "Loaded asset {0} -> {1}.",
                            assetFile.GetRelativePath(source),
                            site.Assets[key].AssetFile.GetRelativePath(source));
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
                            site.Constants.Add(key, value);
                            this.LogSuccess("let {0} = \"{1}\"", key, value);
                        }
                    }
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
                        site.Templates[key] = new Template(templateFile, key, resolver);
                    }
                }

                foreach (var doc in config.Root.Elements(XName.Get("Document", ConfigNamespace)))
                {
                    var path = doc.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Document {0} without path.", doc);
                    }
                    else
                    {
                        IStorageFile pageFile = await source.GetFileAsync(path);
                        string key = doc.Attribute("Key")?.Value ?? pageFile.GetNameWithoutExtension();
                        this.LogSuccess("Found document {0} at '{1}'.", key, pageFile.GetRelativePath(source));
                        site.Templates[key] = new DocTemplate(
                            pageFile,
                            key,
                            resolver,
                            validator,
                            formatter);
                    }
                }

                foreach (var page in config.Root.Elements(XName.Get("Page", ConfigNamespace)))
                {
                    var path = page.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring Page {0} without path.", page);
                    }
                    else
                    {
                        string? templateName = page.Attribute("Template")?.Value;
                        if (string.IsNullOrEmpty(templateName))
                        {
                            this.LogWarning("Ignoring Page {0} without template.", page);
                        }
                        else
                        {
                            string? key = page.Attribute("Key")?.Value;
                            Template? body = string.IsNullOrEmpty(key) ? null : site.Templates[key];
                            Template template = site.Templates[templateName];

                            string location = $"{path}.html";
                            IStorageFile destinationFile = await output.CreateFileAsync(
                                location,
                                CollisionOptions.Overwrite);
                            using (IFileContent templateContent = await template.AssetFile.OpenFileAsync())
                            using (Stream templateStream = templateContent.GetReadStream())
                            using (IFileContent destination =
                                   await destinationFile.OpenFileAsync(FileOpenMode.ReadWrite))
                            using (Stream destinationStream = destination.GetWriteStream())
                            {
                                XElement result = await site.Templates[templateName].CompileAsync(
                                    context.SetSelf(
                                        template,
                                        new KeyValuePair<string, object?>("body", body),
                                        new KeyValuePair<string, object?>("destination", output)));
                                await result.SaveAsync(destinationStream, SaveOptions.None, CancellationToken.None);
                                await destinationStream.FlushAsync();
                                this.LogSuccess(
                                    "Compiled page {0} -> {1}.",
                                    (body?.AssetFile ?? template.AssetFile).GetRelativePath(source),
                                    destinationFile.GetRelativePath(source));
                            }
                        }
                    }
                }
            }
            return output;
        }

        private void InitializeContext(RuntimeContext context, Site site)
        {
            context.Core.Add("site", site);
            context.Core.Add("null", null);
            
            context.Core.Add(
                "relative",
                ExpressionRuntime.MakeMethod<IStorageItem, IStorageItem>(
                    async (myContext, f1, f2) => f1.GetRelativePath(f2)));
            context.Core.Add(
                "select",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector)
                        => await Task.WhenAll(GetCollection(items).Select(i => selector(myContext, new[] {i})))));
            context.Core.Add(
                "filter",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector) =>
                    {
                        var itemArray = GetCollection(items);
                        return (await Task.WhenAll(itemArray.Select(i => selector(myContext, new[] {i}))))
                            .Zip(itemArray)
                            .Where(i => i.First is bool b && b)
                            .Select(i => i.Second);
                    }));
            context.Core.Add(
                "orderBy",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector) =>
                    {
                        var itemArray = GetCollection(items);
                        return (await Task.WhenAll(itemArray.Select(i => selector(myContext, new[] {i}))))
                            .Zip(itemArray)
                            .OrderBy(i => i.First)
                            .Select(i => i.Second);
                    }));
            context.Core.Add(
                "reverse",
                ExpressionRuntime.MakeMethod<IEnumerable>(
                    async (myContext, items) => items.Cast<object?>().Reverse()));
            context.Core.Add(
                "get",
                ExpressionRuntime.MakeMethod<IDictionary, string>(
                    async (myContext, items, key) => items[key]));
            context.Core.Add(
                "if",
                ExpressionRuntime.MakeMethod<bool, RuntimeMethod, RuntimeMethod>(
                    async (myContext, cond, tRun, fRun) 
                        => cond ? await tRun(myContext) : await fRun(myContext)));
        }

        private object?[] GetCollection(IEnumerable items)
            => (items is IDictionary dict ? dict.Values.Cast<object?>() : items.Cast<object?>()).ToArray();

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
