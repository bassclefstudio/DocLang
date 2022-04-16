using BassClefStudio.Storage;
using BassClefStudio.DocLang.Sites;
using BassClefStudio.DocLang.Web;
using BassClefStudio.DocLang.Xml;
using System.CommandLine;
using System.Net.Mime;
using BassClefStudio.Storage.Base;

namespace BassClefStudio.DocLang.Shell
{
    static class Program
    {
        public static readonly Dictionary<ShellContentType, DocumentType> SupportedTypes = new Dictionary<ShellContentType, DocumentType>()
        {
            { ShellContentType.DocLang, new DocumentType(DocLangXml.ContentType) },
            { ShellContentType.Web, new DocumentType(MediaTypeNames.Text.Html) },
        };

        public static readonly IEnumerable<IDocValidator> Validators = new IDocValidator[]
        {
            new DocLangValidator()
        };

        public static readonly IEnumerable<IDocFormatter> Formatters = new IDocFormatter[]
        {
            new WebDocFormatter()
        };

        public static readonly IEnumerable<ISiteBuilder> Builders = new ISiteBuilder[]
        {
            new WebSiteBuilder()
        };

        /// <summary>
        /// Attempts to convert DocLang content between two different content types.
        /// </summary>
        public static async Task MakeAsync(ShellContentType inContent, ShellContentType outContent)
        {
            using (Stream inputStream = Console.OpenStandardInput())
            using (Stream outputStream = Console.OpenStandardOutput())
            {
                var inputDocType = SupportedTypes[inContent];
                var outputDocType = SupportedTypes[outContent];

                var formatter = Formatters.FirstOrDefault(f => f.InputType.Is(inputDocType) && f.OutputType.Is(outputDocType));
                if (formatter is null)
                {
                    throw new InvalidOperationException($"Could not find a document formatter for converting between {inputDocType} and {outputDocType}.");
                }
                await formatter.InitializeAsync();
                await formatter.ConvertAsync(inputStream, outputStream);
            }
        }

        /// <summary>
        /// Checks the given document against the provided DocLang IDocValidator for the specified schema.
        /// </summary>
        public static async Task CheckAsync(ShellContentType contentType)
        {
            using (Stream inputStream = Console.OpenStandardInput())
            {
                var inputDocType = SupportedTypes[contentType];
                var validator = Validators.FirstOrDefault(v => v.DocType.Is(inputDocType));
                if (validator is null)
                {
                    throw new InvalidOperationException($"Could not find a schema validator for the provided content type {inputDocType}.");
                }

                inputDocType = await validator.ValidateAsync(inputStream, inputDocType);
                Console.WriteLine($"Validated DocLang document as {inputDocType}");
            }
        }

        public static async Task SiteAsync(DirectoryInfo folder)
        {
            IStorageFolder sourceFolder = folder.AsFolder();
            var builder = Builders.FirstOrDefault();
            if (builder is null)
            {
                throw new InvalidOperationException($"Could not find a site builder for the provided content type.");
            }

            IStorageFolder outputFolder = await builder.BuildSiteAsync(sourceFolder);
            Console.WriteLine($"Content successfully saved to '{outputFolder.GetRelativePath(sourceFolder)}'");
        }

        [STAThread]
        public static async Task Main(string[] args)
        {
            var inputOption = new Option<ShellContentType>(
                new string[] { "-i", "--input" },
                () => ShellContentType.DocLang,
                "The content type of input data.");

            var outputOption = new Option<ShellContentType>(
                    new string[] { "-o", "--output" },
                    () => ShellContentType.DocLang,
                    "The content type of output data.");

            var inputPathOption = new Option<DirectoryInfo>(
                new string[] { "-i", "--input" },
                () => new DirectoryInfo(Environment.CurrentDirectory),
                "The folder containing the source files for site generation.").ExistingOnly();

            var configOption = new Option<FileInfo>(
                new string[] { "-c", "--config" },
                () => new FileInfo(WebSiteBuilder.ConfigFile), // TODO: Make sure that the config file can be changed based on site type!
                "The relative path to the config file describing how to generate the site output.").ExistingOnly();

            Command makeCommand = new Command("make", "Attempts to convert DocLang content between two different content types.")
            { 
                inputOption,
                outputOption
            };

            makeCommand.SetHandler<ShellContentType, ShellContentType>(
                MakeAsync,
                inputOption,
                outputOption);

            Command checkCommand = new Command("check", "Validates the DocLang content against the provided content type and schema.")
            {
                inputOption
            };

            checkCommand.SetHandler<ShellContentType>(
                CheckAsync,
                inputOption);

            Command siteCommand = new Command("site", "Attempts generate a DocLang site from a group of content files and their associated configuration.")
            {
                inputPathOption,
                configOption
            };

            siteCommand.SetHandler<DirectoryInfo>(
                SiteAsync,
                inputPathOption);

            RootCommand root = new RootCommand("A unifed, XML-based semantic document markup language.")
            {
                makeCommand,
                checkCommand,
                siteCommand
            };

            await root.InvokeAsync(args);
        }
    }

    /// <summary>
    /// Associates "friendly name" values to common document and file types.
    /// </summary>
    public enum ShellContentType
    {
        /// <summary>
        /// The DocLang XML format (any version).
        /// </summary>
        DocLang = 0,

        /// <summary>
        /// Web content (i.e. HTML).
        /// </summary>
        Web = 1
    }
}