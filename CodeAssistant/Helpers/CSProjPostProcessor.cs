﻿﻿using CodeAssistant.Settings;

using System.Text;
using System.Text.RegularExpressions;

namespace CodeAssistant.Helpers;

internal static class CSProjPostProcessor
{
    // 匹配形如: <Exec Command="..." />
    // Command 值里可能包含 &#xD;&#xA; 实体表示换行。
    // 编译并缓存正则，避免每次格式化重新编译。
    // 无需 Multiline：模式中无 ^/$ 锚点。
    private static readonly Regex ExecCommandPattern = new(
        @"(<Exec\s+Command="")([^""]*)(""[^>]*?/?>)",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(5));

    /// <summary> 对标准 XML 格式化结果做 CSProj 专属后处理： 1) Exec Command 属性里的换行实体特殊缩进； 2) ItemGroup / PropertyGroup 之间插入空行； 3) 删除多余连续空行。 </summary>
    public static string Process(string xml, CSProjFormatSettings settings)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        xml = FormatExecCommandAttributes(xml, settings);
        xml = AddGroupSpacing(xml, settings);
        xml = PlainTextFormatter.RemoveExtraBlankLines(xml);

        return xml;
    }

    /// <summary> 仅对 &amp;lt; Exec Command="..."/ &amp;gt; 元素中的 &amp;#xD; &amp;#xA; 换行实体做缩进处理， 避免影响其他 Command 属性或注释中的字符串。 </summary>
    private static string FormatExecCommandAttributes(string xml, CSProjFormatSettings settings)
    {
        return ExecCommandPattern.Replace(
            xml,
            match =>
            {
                string prefix = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                string suffix = match.Groups[3].Value;

                if (!value.Contains("&#xD;&#xA;"))
                    return match.Value;

                string indent = settings.IndentChars + settings.IndentChars;

                value = value.Replace(
                    "&#xD;&#xA;",
                    $"&#xD;&#xA;{Environment.NewLine}{indent}");

                return $"{prefix}{value}{suffix}";
            });
    }

    private static string AddGroupSpacing(string xml, CSProjFormatSettings settings)
    {
        var lines = xml.Split(
            [Environment.NewLine],
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
        // 用户在 UI 关闭了"分组间补空行"，则永远不补
        if (!settings.AddEmptyLineBetweenGroups)
            return false;

        if (index >= lines.Length - 1)
            return false;

        string current = lines[index].Trim();
        string next = lines[index + 1].Trim();

        if (string.IsNullOrWhiteSpace(next))
            return false;

        foreach (var element in settings.ElementsWithNewLine)
        {
            if (current.StartsWith($"</{element}>"))
            {
                return true;
            }
        }

        return false;
    }
}
