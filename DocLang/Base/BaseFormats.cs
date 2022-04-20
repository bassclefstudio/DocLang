
using System.Collections.Generic;
using System.Net.Mime;
using BassClefStudio.DocLang.Web;
using BassClefStudio.DocLang.Xml;

namespace BassClefStudio.DocLang.Base;

/// <summary>
/// Provides basic static references to <see cref="IDocValidator"/>s and <see cref="IDocFormatter"/>s commonly used by site generators and shell commands.
/// </summary>
public static class BaseFormats
{
    /// <summary>
    /// Gets an <see cref="IDictionary{TKey,TValue}"/> pairing <see cref="string"/> format names with their associated <see cref="FormatSpec"/>s.
    /// </summary>
    /// <returns>The resulting <see cref="IDictionary{TKey,TValue}"/>. Note that <see cref="FormatSpec"/>s should be disposed when this dictionary is used.</returns>
    public static FormatSpecs GetFormats() => new FormatSpecs()
    {
        { "doclang-base", new FormatSpec(new DocLangValidator(), new WebDocFormatter()) },
        { "web", new FormatSpec(new RawDocValidator(MediaTypeNames.Text.Html), new RawDocFormatter(MediaTypeNames.Text.Html)) },
    };
    
    /// <summary>
    /// An <see cref="IDictionary{TKey,TValue}"/> pairing <see cref="string"/> monikers with their <see cref="DocumentType"/> equivalents.
    /// </summary>
    public static readonly IDictionary<string, DocumentType> Types = new Dictionary<string, DocumentType>()
    {
        { "doclang", DocLangXml.ContentType },
        { "web", MediaTypeNames.Text.Html },
        { "xml", MediaTypeNames.Application.Xml }
    };

    /// <summary>
    /// An <see cref="IDictionary{TKey,TValue}"/> pairing <see cref="string"/> keys with <see cref="ISiteBuilder"/> implementations.
    /// </summary>
    public static readonly IDictionary<string, ISiteBuilder> SiteBuilders = new Dictionary<string, ISiteBuilder>()
    {
        { "web", new WebSiteBuilder() }
    };
}

/// Provides a disposable wrapper over <see cref="Dictionary{TKey,TValue}"/> for <see cref="FormatSpec"/> values.
public class FormatSpecs : Dictionary<string, FormatSpec>, IDisposable
{
    /// <summary>
    /// Initializes all the component <see cref="IDocFormatter"/>s that make up this <see cref="FormatSpecs"/> collection.
    /// </summary>
    public async Task InitializeAsync()
        => await Task.WhenAll(this.Values.Select(i => i.Formatter.InitializeAsync()).ToArray());
    
    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var v in Values)
        {
            v.Dispose();
        }
    }
}

/// <summary>
/// Provides a basic specification for a paired <see cref="IDocValidator"/> and <see cref="IDocFormatter"/>.
/// </summary>
/// <param name="Validator">The <see cref="IDocValidator"/> used for validating documents.</param>
/// <param name="Formatter">The <see cref="IDocFormatter"/> which can compile/format documents.</param>
public record FormatSpec(IDocValidator Validator, IDocFormatter Formatter) : IDisposable
{
    /// <inheritdoc/>
    public void Dispose()
    {
        Validator.Dispose();
        Formatter.Dispose();
    }
}