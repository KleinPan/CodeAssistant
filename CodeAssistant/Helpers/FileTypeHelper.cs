using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeAssistant.Helpers;

internal static class FileTypeHelper
{
    public static string GetFileExtension(ITextViewSnapshot textView)
    {
        return Path.GetExtension(
            textView.Document.Uri.LocalPath)
            .ToLowerInvariant();
    }

    public static bool IsCSharp(ITextViewSnapshot textView)
    {
        return GetFileExtension(textView) == ".cs";
    }
    public static bool IsCSProj(ITextViewSnapshot textView)
    {
        return GetFileExtension(textView) == ".csproj";
    }
    public static bool IsXaml(ITextViewSnapshot textView)
    {
        return GetFileExtension(textView) == ".xaml";
    }
}