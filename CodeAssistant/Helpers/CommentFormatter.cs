using Microsoft.CodeAnalysis.CSharp;

namespace CodeAssistant.Helpers;

internal static class CommentFormatter
{
    /// <summary>
    /// 注释压缩
    /// </summary>
    /// <param name="sourceCode"></param>
    /// <returns></returns>
    public static string FormatSummaryToSingleLine(string sourceCode)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetCompilationUnitRoot();

        var rewriter = new SummaryCommentRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.ToFullString();
    }
}