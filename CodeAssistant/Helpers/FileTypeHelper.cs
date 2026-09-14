﻿﻿using Microsoft.VisualStudio.Extensibility.Editor;

namespace CodeAssistant.Helpers;

internal static class FileTypeHelper
{
    /// <summary>
    /// 获取当前文档的扩展名（小写，含点）。若文档未保存到磁盘则返回 string.Empty。
    /// </summary>
    public static string GetFileExtension(ITextViewSnapshot textView)
    {
        var uri = textView.Document.Uri;
        if (!uri.IsFile)
            return string.Empty;

        return Path.GetExtension(uri.LocalPath).ToLowerInvariant();
    }

    public static bool IsCSharp(ITextViewSnapshot textView) => GetFileExtension(textView) == ".cs";

    public static bool IsCSProj(ITextViewSnapshot textView) => GetFileExtension(textView) == ".csproj";
}
