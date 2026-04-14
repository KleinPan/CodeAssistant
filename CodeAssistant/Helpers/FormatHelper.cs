using CodeAssistant.Settings;

namespace CodeAssistant.Helpers;

internal static class FormatHelper
{
    public static string CodeFormat(string text)
    {
        text = CommentFormatter.FormatSummaryToSingleLine(text);

        text = PlainTextFormatter.RemoveExtraBlankLines(text, 1);

        return text;
    }

    public static string CSProjFormat(string xml, CSProjFormatSettings settings)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        // 第一阶段：标准 XML 格式化
        string formatted = StandardXmlFormatter.Format(xml, settings);

        // 第二阶段：CSProj 专属规则增强
        formatted = CSProjPostProcessor.Process(formatted, settings);

        return formatted;
    }
}