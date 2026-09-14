﻿﻿﻿using CodeAssistant.Settings;

namespace CodeAssistant.Helpers;

internal static class FormatHelper
{
    /// <summary>对 C# 源代码进行格式化（summary 注释压缩 + 多余空行清理）。</summary>
    public static string CodeFormat(string text, CommentSetting commentSetting)
    {
        if (commentSetting.MultiSummaryToSingle)
        {
            text = CommentFormatter.FormatSummaryToSingleLine(text, commentSetting.WithSpace);
        }

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
