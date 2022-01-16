using BassClefStudio.DocLang;
using BassClefStudio.DocLang.Web;
using BassClefStudio.DocLang.Xml;
using System.CommandLine;
using System.Net.Mime;

Dictionary<ShellContentType, DocumentType> supportedTypes = new Dictionary<ShellContentType, DocumentType>()
{
    { ShellContentType.DocLang, new DocumentType(DocLangXml.ContentType) },
    { ShellContentType.Web, new DocumentType(MediaTypeNames.Text.Html) },
};

IEnumerable<IDocValidator> validators = new IDocValidator[]
{
    new DocLangValidator()
};

IEnumerable<IDocFormatter> formatters = new IDocFormatter[]
{
    new WebDocFormatter()
};

var inputOption = new Option<FileInfo>(
        new string[] { "--input" },
        "A path to an input DocLang XML document.").ExistingOnly();

var outputTypeOption = new Option<ShellContentType>(
        new string[] { "-t", "--type" },
        () => ShellContentType.DocLang,
        "The type of content represented by the output file.");

var outputOption = new Option<FileInfo>(
        new string[] { "--output" },
        "The desired path of the output file.");

Command convertCommand = new Command("make", "Attempts to make a new file of the selected type from a DocLang document.")
{
    inputOption,
    outputTypeOption,
    outputOption
};

convertCommand.SetHandler<FileInfo, ShellContentType, FileInfo>(
    ConvertAsync,
    inputOption,
    outputTypeOption,
    outputOption);

RootCommand root = new RootCommand("A unifed, XML-based semantic document markup language.")
{
    convertCommand
};

await root.InvokeAsync(args);

async Task ConvertAsync(FileInfo inFile, ShellContentType outContent, FileInfo outFile)
{
    using (var readFile = inFile.OpenRead())
    using (var writeFile = outFile.Exists ? outFile.OpenWrite() : outFile.Create())
    {
        var inputDocType = supportedTypes[ShellContentType.DocLang];
        var outputDocType = supportedTypes[outContent];

        var validator = validators.FirstOrDefault(v => v.DocType.Is(inputDocType));
        if (validator is null)
        {
            throw new InvalidOperationException($"Could not find a schema validator for the provided content type {inputDocType}.");
        }

        inputDocType = await validator.ValidateAsync(readFile, inputDocType);
        Console.WriteLine($"Document validated: {inputDocType}");

        readFile.Seek(0, SeekOrigin.Begin);

        var formatter = formatters.FirstOrDefault(f => f.InputType.Is(inputDocType) && f.OutputType.Is(outputDocType));
        if (formatter is null)
        {
            throw new InvalidOperationException($"Could not find a document formatter for converting between {inputDocType} and {outputDocType}.");
        }
        Console.WriteLine($"In: {formatter.InputType}; Out: {formatter.OutputType}");
        await formatter.InitializeAsync();
        await formatter.ConvertAsync(readFile, writeFile);
    }
    Console.WriteLine($"Saved converted document as \"{outFile.Name}\".");
}

public enum ShellContentType
{
    DocLang = 0,
    Web = 1
}