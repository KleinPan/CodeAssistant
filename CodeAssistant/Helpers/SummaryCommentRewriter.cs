﻿﻿﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAssistant.Helpers;

internal class SummaryCommentRewriter : CSharpSyntaxRewriter
{
    // 这些块级 XML 文档标签天然需要换行表示，强行压缩为单行会破坏语义结构。
    // 命中其中任一标签时，该 summary 保持原样不动。
    private static readonly HashSet<string> BlockLevelTags = new(StringComparer.Ordinal)
    {
        "list", "code", "para", "note", "permission", "remarks", "example", "returns", "value",
    };

    private readonly bool _withSpace;

    public SummaryCommentRewriter(bool withSpace = true)
    {
        _withSpace = withSpace;
    }

    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            return base.VisitTrivia(trivia);

        if (trivia.GetStructure() is not DocumentationCommentTriviaSyntax docComment)
            return trivia;

        var newContent = new List<XmlNodeSyntax>();

        foreach (var node in docComment.Content)
        {
            if (node is XmlElementSyntax element &&
                element.StartTag.Name.LocalName.Text == "summary")
            {
                // 含块级元素（<list>/<code>/<para> 等）的 summary 不压缩，
                // 避免破坏段落/列表/代码块的换行语义。
                if (ContainsBlockLevelElement(element))
                {
                    newContent.Add(node);
                    continue;
                }

                var rawText = ExtractSummaryText(element);
                // withSpace=false 时去掉首尾空格，true 时保证两侧恰好一个空格
                var text = _withSpace
                    ? $" {rawText.Trim()} "
                    : rawText.Trim();

                // 保留原 StartTag 上的属性（如 lang、xml:lang 等），仅替换内容
                var newSummary = SyntaxFactory.XmlElement(
                    element.StartTag,
                    SyntaxFactory.SingletonList<XmlNodeSyntax>(SyntaxFactory.XmlText(text)),
                    element.EndTag);

                newContent.Add(newSummary);
            }
            else
            {
                newContent.Add(node);
            }
        }

        var newDocComment = docComment.WithContent(
            SyntaxFactory.List(newContent));

        return SyntaxFactory.Trivia(newDocComment);
    }

    private static bool ContainsBlockLevelElement(XmlElementSyntax summaryElement)
    {
        foreach (var node in summaryElement.Content)
        {
            if (node is XmlElementSyntax nested &&
                BlockLevelTags.Contains(nested.StartTag.Name.LocalName.Text))
            {
                return true;
            }
        }
        return false;
    }

    private static string ExtractSummaryText(XmlElementSyntax summaryElement)
    {
        var textParts = new List<string>();

        foreach (var node in summaryElement.Content)
        {
            switch (node)
            {
                case XmlTextSyntax textNode:
                    var text = textNode.TextTokens
                        .Select(t => t.Text.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t));
                    textParts.AddRange(text);
                    break;

                case XmlEmptyElementSyntax emptyElement:
                    // 保留 <see cref="..."/>、<paramref name="..."/> 等空元素标签
                    textParts.Add(emptyElement.ToFullString().Trim());
                    break;

                case XmlElementSyntax nestedElement:
                    // 命中此处说明 ContainsBlockLevelElement 已放行（块级元素被保留原样），
                    // 这里只剩行内嵌套元素（如 <c>code</c>），原样保留其文本表示。
                    textParts.Add(nestedElement.ToFullString().Trim());
                    break;
            }
        }

        return string.Join(" ", textParts);
    }
}
