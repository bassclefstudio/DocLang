using System.Collections;
using System.Net.Mime;
using BassClefStudio.Storage;
using System.Xml.Linq;
using BassClefStudio.BassScript.Runtime;
using BassClefStudio.DocLang.Base;
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
        public static readonly string ConfigFile = "site-config.xml";

        /// <summary>
        /// The <see cref="string"/> XML namespace for <see cref="WebSiteBuilder"/> configuration files.
        /// </summary>
        public static readonly string ConfigNamespace = "https://bassclefstudio.dev/DocLang/v1/Config";

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
            
            SiteContentResolver resolver = new SiteContentResolver();
            
            IStorageFile configFile = await source.GetFileAsync(ConfigFile);
            using (FormatSpecs formatBase = BaseFormats.GetFormats())
            using (FormatSpecs formats = formatBase.Copy())
            using (var fileStream = await configFile.OpenFileAsync())
            {
                XDocument config = XDocument.Load(fileStream.GetReadStream());
                if (config.Root is null)
                {
                    throw new SiteBuilderException(
                        "Failed to open the root element of the XML website configuration file."
                    );
                }
                
                var location = config.Root.Attribute("Location")?.Value;
                if (string.IsNullOrWhiteSpace(location))
                {
                    throw new SiteBuilderException(
                        "Failed to create a site with no Location specified; a path must be given.");
                }
                
                Site site = new Site(location);
                // Create the context and the core methods for evaluating expressions.
                RuntimeContext context = new RuntimeContext();
                InitializeContext(
                    context,
                    site,
                    output);
                    
                async Task LoadStyle(XElement style)
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
                        site.Styles[key] = new StyleSheet(
                            await stylesheet.CopyToAsync(styleFolder, CollisionOptions.Overwrite, fileName),
                            key);
                        this.LogSuccess(
                            "Loaded stylesheet {0} -> {1}.",
                            stylesheet.GetRelativePath(source),
                            site.Styles[key].AssetFile.GetRelativePath(source));
                    }
                }

                foreach (var style in config.Root.Elements(XName.Get("Style", ConfigNamespace)))
                {
                    await LoadStyle(style);
                }

                async Task LoadAsset(XElement asset)
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
                        site.Assets[key] = new Asset(
                            await assetFile.CopyToAsync(assetsFolder, CollisionOptions.Overwrite, fileName),
                            key);
                        this.LogSuccess(
                            "Loaded asset {0} -> {1}.",
                            assetFile.GetRelativePath(source),
                            site.Assets[key].AssetFile.GetRelativePath(source));
                    }
                }

                foreach (var asset in config.Root.Elements(XName.Get("Asset", ConfigNamespace)))
                {
                    await LoadAsset(asset);
                }

                async Task<object?> ResolveConfigObject(XElement data, RuntimeContext myContext)
                {
                    if (data.HasElements)
                    {
                        var childContext = myContext.Copy();
                        foreach (var child in data.Elements())
                        {
                            childContext.Local.Add(
                                child.Name.LocalName,
                                await ResolveConfigObject(child, childContext));
                        }
                        return childContext.Local;
                    }
                    else
                    {
                        var tokens = (await resolver.ResolveAsync(data.Value, myContext)).ToArray();
                        return tokens.Length switch
                        {
                            0 => null,
                            1 => tokens[0],
                            _ => tokens
                        };
                    }
                }

                async Task LoadConstant(XElement constant)
                {
                    string? key = constant.Attribute("Name")?.Value;
                    if (string.IsNullOrEmpty(key))
                    {
                        this.LogWarning("Ignoring Variable {0} without name.", constant);
                    }
                    else
                    {
                        site.Constants.Add(key, await ResolveConfigObject(constant, context));
                        this.LogSuccess("let {0} = \"{1}\"", key, site.Constants[key]);
                    }
                }

                foreach (var constant in config.Root.Elements(XName.Get("Constant", ConfigNamespace)))
                {
                    await LoadConstant(constant);
                }

                async Task LoadFormat(XElement format, FormatSpecs formatSpecs)
                {
                    var formatPath = format.Attribute("Transform")?.Value;
                    if (string.IsNullOrEmpty(formatPath))
                    {
                        this.LogWarning("Ignoring format {0} without transform path.", format);
                    }
                    else
                    {
                        var validatorPath = format.Attribute("Schema")?.Value;
                        if (string.IsNullOrEmpty(validatorPath))
                        {
                            this.LogWarning("Ignoring format {0} without schema path.", format);
                        }
                        else
                        {
                            var key = format.Attribute("Key")?.Value;
                            if (string.IsNullOrEmpty(key))
                            {
                                this.LogWarning("Ignoring format {0} without key.", format);
                            }
                            else
                            {
                                var formatType = format.Attribute("Type")?.Value ?? "xml";
                                IDocValidator customValidator = formatType switch
                                {
                                    "xml" => new XsdFileValidator(await source.GetFileAsync(validatorPath)),
                                    _ => throw new SiteBuilderException(
                                        $"Failed to find a format type with key {formatType}.")
                                };
                                IDocFormatter customFormat = formatType switch
                                {
                                    "xml" => new XsltFileFormatter(await source.GetFileAsync(formatPath)),
                                    _ => throw new SiteBuilderException(
                                        $"Failed to find a format type with key {formatType}.")
                                };
                                formatSpecs[key] = new FormatSpec(
                                    customValidator,
                                    customFormat);
                            }
                        }
                    }
                }

                foreach (var format in config.Root.Elements(XName.Get("Format", ConfigNamespace)))
                {
                    await LoadFormat(format, formats);
                }
                
                // Initialize formats with all the new custom formats as well.
                await formats.InitializeAsync();
                
                async Task LoadTemplate(XElement doc, Group group)
                {
                    var path = doc.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring template {0} without path.", doc);
                    }
                    else
                    {
                        string? formatKey = doc.Attribute("Format")?.Value;
                        if (string.IsNullOrEmpty(formatKey))
                        {
                            this.LogWarning("Ignoring template {0} without format.", doc);
                        }
                        else
                        {
                            var format = formats[formatKey];
                            IStorageFile pageFile = await source.GetFileAsync(path);
                            string key = doc.Attribute("Key")?.Value ?? pageFile.GetNameWithoutExtension();
                            this.LogInformation(
                                "Found template '{0}' at '{1}'.",
                                key,
                                pageFile.GetRelativePath(source));
                            group.Templates[key] = new Template(
                                pageFile,
                                key,
                                resolver,
                                format.Validator,
                                format.Formatter);
                        }
                    }
                }

                async Task LoadPage(XElement page, Group group)
                {
                    var path = page.Attribute("Destination")?.Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        this.LogWarning("Ignoring page {0} without destination.", page);
                    }
                    else
                    {
                        string name = page.Attribute("Name")?.Value ?? path;
                        string? templateName = page.Attribute("Template")?.Value;
                        if (string.IsNullOrEmpty(templateName))
                        {
                            this.LogWarning("Ignoring page {0} without root.", page);
                        }
                        else
                        {
                            var bodyName = page.Attribute("Body")?.Value;
                            var body = string.IsNullOrEmpty(bodyName) ? null : group.GetTemplate(bodyName);
                            var template = group.GetTemplate(templateName);
                            IStorageFile destinationFile = await output.CreateFileRecursiveAsync(
                                Path.Combine(path, "index.html"),
                                CollisionOptions.Overwrite);

                            IDictionary<string, object?> propDict =
                                await ResolveConfigObject(page, context) as IDictionary<string, object?> ??
                                new Dictionary<string, object?>();
                            group.Pages[name] = new Page(
                                destinationFile,
                                path,
                                resolver,
                                propDict,
                                template,
                                body);
                        }
                    }
                }

                async Task LoadGroup(Group parent, XElement groupData)
                {
                    foreach (var doc in groupData.Elements(XName.Get("Template", ConfigNamespace)))
                    {
                        await LoadTemplate(doc, parent);
                    }

                    foreach (var subGroupData in groupData.Elements(XName.Get("Group", ConfigNamespace)))
                    {
                        string? groupName = subGroupData.Attribute("Name")?.Value;
                        if (string.IsNullOrEmpty(groupName))
                        {
                            this.LogWarning("Ignoring subgroup {0} without name.", subGroupData);
                        }
                        else
                        {
                            var subGroup = new Group();
                            await LoadGroup(subGroup, subGroupData);
                            parent.Groups[groupName] = subGroup;
                        }
                    }

                    foreach (var page in groupData.Elements(XName.Get("Page", ConfigNamespace)))
                    {
                        await LoadPage(page, parent);
                    }
                }

                await LoadGroup(site, config.Root);

                foreach (Page page in site)
                {
                    this.LogInformation("Compiling '{0}' (body: {1})...", page.Template.Name, page.Body?.Name);
                    using (IFileContent destination =
                           await page.AssetFile.OpenFileAsync(FileOpenMode.ReadWrite))
                    using (Stream destinationStream = destination.GetWriteStream())
                    {
                        XElement result = await page.Template.CompileAsync(
                            context.SetSelf(
                                page,
                                new KeyValuePair<string, object?>("body", page.Body)));
                        await result.SaveAsync(destinationStream, SaveOptions.None, CancellationToken.None);
                        await destinationStream.FlushAsync();
                        this.LogSuccess(
                            "Compiled page {0} -> {1}.",
                            (page.Body?.AssetFile ?? page.Template.AssetFile).GetRelativePath(source),
                            page.AssetFile.GetRelativePath(source));
                    }
                }
            }
            return output;
        }

        private void InitializeContext(RuntimeContext context, Site site, IStorageFolder output)
        {
            context.Core.Add("site", site);
            context.Core.Add("null", null);
            
            context.Core.Add(
                "getPath",
                ExpressionRuntime.MakeMethod<IStorageItem>(
                    async (_, f1) => Path.Combine(site.Location, f1.GetRelativePath(output))));

            context.Core.Add(
                "getLink",
                ExpressionRuntime.MakeMethod<Page>(
                    async (_, page) => Path.Combine(
                        site.Location,
                        Path.GetDirectoryName(page.AssetFile.GetRelativePath(output)) ?? string.Empty)));
            
            context.Core.Add(
                "select",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector)
                        => await Task.WhenAll(GetCollection(items).Select(i => selector(myContext, new[] {i})))));
            context.Core.Add(
                "filter",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector)
                        =>
                    {
                        var itemArray = GetCollection(items).ToArray();
                        var keys = await Task.WhenAll(itemArray.Select(i => selector(myContext, new[] {i})));
                        return itemArray.Zip(keys).Where(i => i.Second is bool b && b).Select(i => i.First);
                    }));
            context.Core.Add(
                "orderBy",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector) 
                        =>
                    {
                        var itemArray = GetCollection(items).ToArray();
                        var keys = await Task.WhenAll(itemArray.Select(i => selector(myContext, new[] {i})));
                        return itemArray.Zip(keys).OrderBy(i => i.Second).Select(i => i.First);
                    }));
            context.Core.Add(
                "groupBy",
                ExpressionRuntime.MakeMethod<IEnumerable, RuntimeMethod>(
                    async (myContext, items, selector) 
                        =>
                    {
                        var itemArray = GetCollection(items).ToArray();
                        var keys = await Task.WhenAll(itemArray.Select(i => selector(myContext, new[] {i})));
                        return itemArray.Zip(keys).GroupBy(i => i.Second, i => i.First);
                    }));
            context.Core.Add(
                "reverse",
                ExpressionRuntime.MakeMethod<IEnumerable>(
                    async (_, items) => items.Cast<object?>().Reverse()));
            context.Core.Add(
                "take",
                ExpressionRuntime.MakeMethod<IEnumerable, int>(
                    async (_, items, num) => items.Cast<object?>().Take(num)));
            context.Core.Add(
                "skip",
                ExpressionRuntime.MakeMethod<IEnumerable, int>(
                    async (_, items, num) => items.Cast<object?>().Skip(num)));
            context.Core.Add(
                "any",
                ExpressionRuntime.MakeMethod<IEnumerable>(
                    async (_, items) => items.Cast<object?>().Any()));
            context.Core.Add(
                "index",
                ExpressionRuntime.MakeMethod<IEnumerable, int>(
                    async (_, items, num) => items.Cast<object?>().ElementAt(num)));
            context.Core.Add(
                "getItem",
                ExpressionRuntime.MakeMethod<IDictionary, string>(
                    async (_, items, key) => items[key]));
            context.Core.Add(
                "if",
                ExpressionRuntime.MakeMethod<bool, RuntimeMethod, RuntimeMethod>(
                    async (myContext, cond, tRun, fRun) 
                        => cond ? await tRun(myContext) : await fRun(myContext)));
            context.Core.Add(
                "dateTime",
                ExpressionRuntime.MakeMethod<string>(
                    async (_, date) => DateTime.Parse(date)));
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
