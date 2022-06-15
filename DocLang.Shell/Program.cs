using BassClefStudio.Storage;
using System.CommandLine;
using BassClefStudio.DocLang.Base;
using BassClefStudio.Storage.Base;

namespace BassClefStudio.DocLang.Shell;

public static class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        var formatOption = new Option<string>(
            new string[] { "-f", "--format" },
            () => BaseFormats.Types.Keys.First(),
            "The format specification of input and output content.");
        formatOption.AddCompletions(BaseFormats.Types.Keys.ToArray());
            
        var builderOption = new Option<string>(
            new string[] { "-b", "--builder" },
            () => BaseFormats.SiteBuilders.Keys.First(),
            "The type of site builder to use.");
        builderOption.AddCompletions(BaseFormats.SiteBuilders.Keys.ToArray());
            
        var inputPathOption = new Option<DirectoryInfo>(
            new string[] { "-i", "--input" },
            () => new DirectoryInfo(Environment.CurrentDirectory),
            "The folder containing the source files for site generation.").ExistingOnly();
            
        Command makeCommand = new Command("make", "Attempts to convert DocLang content between two different content types.")
        {
            formatOption
        };

        makeCommand.SetHandler<string>(
            MakeAsync,
            formatOption);

        Command checkCommand = new Command("check", "Validates the DocLang content against the provided content type and schema.")
        {
            formatOption
        };

        checkCommand.SetHandler<string>(
            CheckAsync,
            formatOption);

        Command siteCommand = new Command("site", "Attempts generate a DocLang site from a group of content files and their associated configuration.")
        {
            inputPathOption,
            builderOption
        };

        siteCommand.SetHandler<DirectoryInfo, string>(
            SiteAsync,
            inputPathOption,
            builderOption);

        RootCommand root = new RootCommand("A unifed, XML-based semantic document markup language.")
        {
            makeCommand,
            checkCommand,
            siteCommand
        };

        await root.InvokeAsync(args);
    }
        
    /// <summary>
    /// Attempts to convert DocLang content between two different content types.
    /// </summary>
    public static async Task MakeAsync(string format)
    {
        using (Stream inputStream = Console.OpenStandardInput())
        using (Stream outputStream = Console.OpenStandardOutput())
        using (var formats = BaseFormats.GetFormats())
        {
            var inputDocType = BaseFormats.Types[format];
            var outputDocType = BaseFormats.Types[format];

            var formatter = formats[format].Formatter;
            if (formatter.InputType.Is(inputDocType) && formatter.OutputType.Is(outputDocType))
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
    public static async Task CheckAsync(string format)
    {
        using (Stream inputStream = Console.OpenStandardInput())
        using (var formats = BaseFormats.GetFormats())
        {
            var inputDocType = BaseFormats.Types[format];
            var validator = formats[format].Validator;
            if (validator.DocType.Is(inputDocType))
            {
                throw new InvalidOperationException($"Could not find a schema validator for the provided content type {inputDocType}.");
            }

            inputDocType = await validator.ValidateAsync(inputStream, inputDocType);
            Console.WriteLine($"Validated DocLang document as {inputDocType}");
        }
    }

    /// <summary>
    /// Attempts to build a site from the given source files.
    /// </summary>
    /// <param name="folder">The <see cref="DirectoryInfo"/> for the source file being produced.</param>
    /// <param name="builderType">The <see cref="string"/> type name of the builder used to generate the site.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task SiteAsync(DirectoryInfo folder, string builderType)
    {
        IStorageFolder sourceFolder = folder.AsFolder();
        var builder = BaseFormats.SiteBuilders[builderType];
        if (builder is null)
        {
            throw new InvalidOperationException($"Could not find a site builder for the provided content type.");
        }

        IStorageFolder outputFolder = await builder.BuildSiteAsync(sourceFolder);
        Console.WriteLine($"Content successfully saved to '{outputFolder.GetRelativePath(sourceFolder)}'");
    }
}