namespace CodeAssistant.Helpers;

internal class CodeFormatHelper
{
    // 现有属性和方法...

    // 处理空白行，将多个连续空白行替换为单个空白行
    public List<CommentReplacement> ProcessEmptyLines(string content)
    {
        var replacements = new List<CommentReplacement>();
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        int currentLine = 0;
        int startEmptyLine = -1;

        while (currentLine < lines.Length)
        {
            // 如果是空白行
            if (string.IsNullOrWhiteSpace(lines[currentLine]))
            {
                // 记录空白行开始位置
                if (startEmptyLine == -1)
                {
                    startEmptyLine = currentLine;
                }
            }
            else
            {
                // 如果遇到非空白行，且之前有连续空白行
                if (startEmptyLine != -1)
                {
                    // 计算连续空白行的结束位置
                    int endEmptyLine = currentLine - 1;

                    // 如果有多个连续空白行，需要替换
                    if (endEmptyLine > startEmptyLine)
                    {
                        // 计算替换范围
                        int startPos = GetPositionInContent(lines, startEmptyLine);
                        int endPos = GetPositionInContent(lines, endEmptyLine + 1);

                        // 替换为单个空白行
                        replacements.Add(new CommentReplacement(startPos, endPos, Environment.NewLine));
                    }

                    // 重置空白行标记
                    startEmptyLine = -1;
                }
            }

            currentLine++;
        }

        // 处理文本末尾的连续空白行
        if (startEmptyLine != -1 && startEmptyLine < lines.Length - 1)
        {
            int startPos = GetPositionInContent(lines, startEmptyLine);
            int endPos = content.Length;

            replacements.Add(new CommentReplacement(startPos, endPos, Environment.NewLine));
        }

        return replacements;
    }

    // 辅助方法：计算行在原始文本中的位置
    private int GetPositionInContent(string[] lines, int lineIndex)
    {
        int position = 0;
        for (int i = 0; i < lineIndex; i++)
        {
            position += lines[i].Length + Environment.NewLine.Length;
        }
        return position;
    }
}