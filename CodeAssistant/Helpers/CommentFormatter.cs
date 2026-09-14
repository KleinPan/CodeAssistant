﻿﻿﻿using Microsoft.CodeAnalysis.CSharp;

namespace CodeAssistant.Helpers;

internal static class CommentFormatter
{
    /// <summary>
    /// 将多行 summary 注释压缩为单行。
    /// </summary>
    /// <param name="sourceCode">C# 源代码。</param>
    /// <param name="withSpace">summary 文字两侧是否保留一个空格。</param>
    public static string FormatSummaryToSingleLine(string sourceCode, bool withSpace = true)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetCompilationUnitRoot();

        var rewriter = new SummaryCommentRewriter(withSpace);
        var newRoot = rewriter.Visit(root);

        return newRoot.ToFullString();
    }
}
