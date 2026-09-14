﻿﻿﻿using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;

using System.Diagnostics;
using System.Globalization;

namespace CodeAssistant;

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

/// <summary>在当前光标位置插入一个新的 GUID（默认带连字符的 D 格式）。</summary>
[VisualStudioContribution]
internal class InsertGuidCommand : Command
{
    private readonly TraceSource _logger;

    public InsertGuidCommand(TraceSource traceSource)
    {
        _logger = Requires.NotNull(traceSource, nameof(traceSource));
    }

    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new("%InsertGuidCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        // 仅在编辑器打开文件时可用（避免 ClientContext 约束，原因见 FormatCodeCommand）
        EnabledWhen = ActivationConstraint.EditorContentType("any"),
    };

    /// <inheritdoc/>
    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        return base.InitializeAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        Requires.NotNull(context, nameof(context));

        // D 格式：xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx，是 VS 中最常见的 GUID 形式
        var newGuidString = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);

        using var textView = await context.GetActiveTextViewAsync(cancellationToken);
        if (textView is null)
        {
            _logger.TraceInformation("There was no active text view when command is executed.");
            return;
        }

        await this.Extensibility.Editor().EditAsync(
            batch =>
            {
                var editor = textView.Document.AsEditable(batch);

                // 记录选区起点（插入位置）。Replace 后默认光标会移动到新文本末尾，
                // 这里显式把光标设置到 GUID 之后，语义更直观。
                var insertStart = textView.Selection.Extent.Start;
                var afterGuid = insertStart + newGuidString.Length;

                // 替换当前选区（若无选区则在光标处插入）
                editor.Replace(textView.Selection.Extent, newGuidString);

                // 把光标移到插入的 GUID 之后
                textView.AsEditable(batch).SetSelections(
                    [new Selection(activePosition: afterGuid, anchorPosition: afterGuid, insertionPosition: afterGuid)]);
            },
            cancellationToken);
    }
}
