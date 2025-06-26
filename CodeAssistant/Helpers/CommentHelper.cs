using CodeAssistant.Settings;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAssistant.Helpers;



internal class CommentHelper
{
    private CommentSetting CommentSetting = new CommentSetting();

    // 处理所有summary注释并返回替换列表
    public List<CommentReplacement> ProcessMultiSummary(CompilationUnitSyntax root)
    {
        var replacements = new List<CommentReplacement>();

        // 提取所有单行文档注释
        var docComments = root.DescendantTrivia()
            .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            .ToList();

        foreach (var comment in docComments)
        {
            var commentText = comment.ToString();
            var originalSpan = comment.Span;
            // 查找<summary>起始行
            int startIndex = FindStartSummaryIndex(commentText);

            // 查找</summary>结束行
            int endIndex = FindEndSummaryIndex(commentText);

            // 处理summary块
            if (startIndex >= 0 && endIndex > startIndex)
            {
                // 提取summary内容行（不包含标签本身）
                var contentLines = commentText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                    .Skip(startIndex + 1)  // 跳过<summary>标签行
                    .Take(endIndex - startIndex - 1)
                    .Select(x => x.Replace("///", "").TrimStart())// 取内容行
                    .ToList();

                // 合并为单行并清理空白字符
                string singleLineContent = string.Join(" ", contentLines)
                    .Trim()
                    .Replace("  ", " ")  // 替换连续空格
                    .Replace("  ", " "); // 重复一次确保无连续空格

                // 构建新的单行summary，因为原来就带///，所以不用新增

                string newSummaryContent = "";
                if (CommentSetting.WithSpace)
                {
                    newSummaryContent = $"<summary> {singleLineContent} </summary>";
                }
                else
                {
                    newSummaryContent = $"<summary>{singleLineContent}</summary>";
                }

                // 获取<summary>标签的文本范围（而不是整个注释块）
                var summaryLines = commentText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                var startLine = summaryLines[startIndex];
                var endLine = summaryLines[endIndex];

                // 计算<summary>标签的起始和结束位置
                int summaryStartPos = originalSpan.Start + commentText.IndexOf(startLine);
                int summaryEndPos = originalSpan.Start + commentText.IndexOf(endLine) + endLine.Length;

                // 添加替换记录
                replacements.Add(new CommentReplacement(summaryStartPos, summaryEndPos, newSummaryContent));
            }
        }

        return replacements;
    }

    /// <summary>
    /// 查找summary开始标签在文本中的位置
    /// </summary>
    /// <param name="commentText"></param>
    /// <returns></returns>
    private static int FindStartSummaryIndex(string commentText, string findString = "<summary>")
    {
        // 按行分割注释文本
        var lines = commentText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart().StartsWith(findString))
            {
                return i; // 返回行索引
            }
        }

        return -1; // 未找到
    }

    /// <summary>
    /// 查找summary结束标签在文本中的位置
    /// </summary>
    /// <param name="commentText"></param>
    /// <returns></returns>
    private static int FindEndSummaryIndex(string commentText, string findString = "</summary>")
    {
        // 按行分割注释文本
        var lines = commentText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimEnd().EndsWith(findString))
            {
                return i; // 返回行索引
            }
        }

        return -1; // 未找到
    }
}

// 定义替换信息类
public class CommentReplacement
{
    public int Start { get; }
    public int End { get; }
    public string NewText { get; }

    public CommentReplacement(int start, int end, string newText)
    {
        Start = start;
        End = end;
        NewText = newText;
    }
}