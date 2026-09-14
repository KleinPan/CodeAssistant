﻿﻿namespace CodeAssistant.Helpers;

internal static class PlainTextFormatter
{
    /// <summary>
    /// 将连续的空白行压缩到最多 <paramref name="maxBlankLines"/> 行。
    /// </summary>
    public static string RemoveExtraBlankLines(string text, int maxBlankLines = 1)
    {
        if (string.IsNullOrEmpty(text))
            return text ?? string.Empty;

        var lines = text.Split(["\r\n", "\n"], StringSplitOptions.None);
        var result = new List<string>(lines.Length);

        int blankCount = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                blankCount++;

                if (blankCount <= maxBlankLines)
                    result.Add(line);
            }
            else
            {
                blankCount = 0;
                result.Add(line);
            }
        }

        // 移除末尾的多余空行，但保留一个最终换行符（符合 EditorConfig insert_final_newline 约定）
        while (result.Count > 0 && string.IsNullOrWhiteSpace(result[^1]))
        {
            result.RemoveAt(result.Count - 1);
        }

        return result.Count == 0
            ? string.Empty
            : string.Join(Environment.NewLine, result) + Environment.NewLine;
    }
}
