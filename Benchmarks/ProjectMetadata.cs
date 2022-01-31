using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal static class ProjectMetadataEnumerableExtensions
{
    public static IEnumerable<ProjectMetadata> SortAlphabetically(this IEnumerable<ProjectMetadata> projects)
    {
        return projects.OrderBy(n => int.Parse(new string(n.Name.Where(c => char.IsDigit(c)).ToArray())));
    }
}

public class ProjectMetadata
{
    private static Regex _nonNumericRegex = new Regex(@"\w+");

    public ProjectMetadata(string name, string dll)
    {
        var (newName, year) = FindAndReplaceYearPart(name);
        DisplayName = $"({year}) {newName}";
        Name = name;
        Dll = dll;
    }

    private static (string result, int year) FindAndReplaceYearPart(string name)
    {
        var underscorePos = name.IndexOf('_');
        var year = underscorePos > 0 ? int.Parse(_nonNumericRegex.Replace(name.Substring(underscorePos + 1), string.Empty)) : 2020;
        if (underscorePos > 0)
        {
            return (name.Substring(0, underscorePos), year);
        }
        return (name, year);
    }

    public string DisplayName { get; }
    public string Name { get; }
    public string Dll { get; }
}
