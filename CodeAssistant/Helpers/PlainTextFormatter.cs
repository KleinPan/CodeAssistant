namespace CodeAssistant.Helpers;

internal static class PlainTextFormatter
{
    public static string RemoveExtraBlankLines(string text, int maxBlankLines = 1)
    {
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

        return string.Join(Environment.NewLine, result);
    }
}