using System.Collections;
using BassClefStudio.BassScript.Runtime;

namespace BassClefStudio.DocLang.Web.Sites;

/// <summary>
/// Represents a grouped collection of <see cref="Template"/>s, <see cref="Page"/>s, and sub-<see cref="Group"/>s which implement a hierarchical organization for a given <see cref="Site"/>.
/// </summary>
public class Group : IRuntimeObject, IEnumerable<Page>
{
    /// <summary>
    /// An <see cref="IDictionary{TKey, TValue}"/> of keyed <see cref="Template"/> assets.
    /// </summary>
    public IDictionary<string, Template> Templates { get; }

    /// <summary>
    /// An <see cref="IDictionary{TKey,TValue}"/> of keyed <see cref="Page"/> pages that are a member of this group.
    /// </summary>
    public IDictionary<string, Page> Pages { get; }

    /// <summary>
    /// An <see cref="IDictionary{TKey, TValue}"/> of keyed <see cref="Group"/> sub-groups.
    /// </summary>
    public IDictionary<string, Group> Groups { get; }

    /// <inheritdoc/>
    public virtual object? this[string key]
    {
        get
        {
            if (Groups.ContainsKey(key)) return Groups[key];
            if (key == "templates") return Templates;
            if (key == "pages") return Pages;
            else return Pages[key];
        }
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new <see cref="Group"/> object.
    /// </summary>
    public Group()
    {
        Templates = new Dictionary<string, Template>();
        Pages = new Dictionary<string, Page>();
        Groups = new Dictionary<string, Group>();
    }

    /// <inheritdoc/>
    public IEnumerator<Page> GetEnumerator() => Pages.Values.Concat(Groups.Values.SelectMany(g => g.Pages.Values)).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the <see cref="Template"/> object (if found) located at the given <see cref="string"/> path.
    /// </summary>
    /// <param name="path">A slash-delimited path locating the location (including subgroups) of the desired <see cref="Template"/>.</param>
    /// <returns>The <see cref="Template"/> object, if found.</returns>
    /// <exception cref="KeyNotFoundException">No template was found at the location indicated by <paramref name="path"/>.</exception>
    public Template GetTemplate(string path)
    {
        Template GetTemplateInternal(Group group, string[] parts)
        {
            if (parts.Length == 0) throw new KeyNotFoundException("No template was found at the given path.");
            else if (parts.Length == 1) return group.Templates[parts[0]];
            else return GetTemplateInternal(group.Groups[parts[0]], parts[1..]);
        }
        string[] pathParts = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        return GetTemplateInternal(this, pathParts);
    }
}