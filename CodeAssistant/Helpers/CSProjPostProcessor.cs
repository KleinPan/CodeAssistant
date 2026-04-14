using CodeAssistant.Settings;

using System.Text;
using System.Text.RegularExpressions;

namespace CodeAssistant.Helpers;

internal static class CSProjPostProcessor
{
    public static string Process(string xml, CSProjFormatSettings settings)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        // Command 属性特殊换行
        xml = FormatCommandAttributes(xml, settings);

        // 分组间插入空行
        if (settings.AddEmptyLineBetweenGroups)
        {
            xml = AddGroupSpacing(xml, settings);
        }

        // 删除多余连续空行
        xml = PlainTextFormatter.RemoveExtraBlankLines(xml);

        return xml;
    }

    private static string FormatCommandAttributes(string xml, CSProjFormatSettings settings)
    {
        return Regex.Replace(
            xml,
            "Command=\"([^\"]*)\"",
            match =>
            {
                string value = match.Groups[1].Value;

                if (!value.Contains("&#xD;&#xA;"))
                    return match.Value;

                string indent = settings.IndentChars + settings.IndentChars;

                value = value.Replace(
                    "&#xD;&#xA;",
                    $"&#xD;&#xA;{Environment.NewLine}{indent}");

                return $"Command=\"{value}\"";
            },
            RegexOptions.Multiline);
    }

    private static string AddGroupSpacing(string xml, CSProjFormatSettings settings)
    {
        var lines = xml.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.None);

        var sb = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            sb.AppendLine(lines[i]);

            if (ShouldAddEmptyLine(lines, i, settings))
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static bool ShouldAddEmptyLine(string[] lines, int index, CSProjFormatSettings settings)
    {
        if (index >= lines.Length - 1)
            return false;

        string current = lines[index].Trim();
        string next = lines[index + 1].Trim();

        foreach (var element in settings.ElementsWithNewLine)
        {
            if (current.StartsWith($"</{element}>") &&
                !string.IsNullOrWhiteSpace(next))
            {
                return true;
            }
        }

        return false;
    }


}