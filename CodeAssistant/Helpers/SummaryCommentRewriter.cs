using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAssistant.Helpers;

internal class SummaryCommentRewriter : CSharpSyntaxRewriter
{
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
                var text = ExtractSummaryText(element);

                var newSummary = SyntaxFactory.XmlElement(
                    SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                    SyntaxFactory.SingletonList<XmlNodeSyntax>(SyntaxFactory.XmlText(text)),
                    SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));

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

    private static string ExtractSummaryText(XmlElementSyntax summaryElement)
    {
        var texts = summaryElement.Content
            .OfType<XmlTextSyntax>()
            .SelectMany(x => x.TextTokens)
            .Select(x => x.Text.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x));

        return string.Join(" ", texts);
    }
}