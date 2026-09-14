﻿﻿﻿﻿﻿﻿﻿﻿using CodeAssistant.Helpers;
using CodeAssistant.Services;
using CodeAssistant.Settings;

using Microsoft;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;

using System.Diagnostics;

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

namespace CodeAssistant;

// https://learn.microsoft.com/zh-cn/visualstudio/extensibility/visualstudio.extensibility/editor/editor-concepts?view=visualstudio

/// <summary>格式化当前文档（.cs / .csproj）。</summary>
[VisualStudioContribution]
internal class FormatCodeCommand : Command
{
    private readonly TraceSource _logger;
    private readonly ConfigService _configService;

    public FormatCodeCommand(TraceSource traceSource, ConfigService configService)
    {
        _logger = Requires.NotNull(traceSource, nameof(traceSource));
        _configService = Requires.NotNull(configService, nameof(configService));
    }

    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new("%FormatCodeCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.CleanData, IconSettings.None),
        // 不要用 CommandPlacement.VsctParent 挂内置工具栏组：VS 2026 不支持
        // 进程外扩展命令放置到工具栏组（DevCom #11100776；VsctParent 挂
        // 菜单组可用，见 VSExtensibility #475）。工具栏入口由
        // ExtensionEntrypoint.CodeAssistantToolbar 提供。
        // 仅在编辑器打开文件时可用。EditorContentType("any") 是进程内原生求值的
        // 规则约束（"any" 为编辑器内容类型层次结构的根，匹配任何文件类型）。
        // 不要用 ActivationConstraint.ClientContext(...)（重量级求值路径）。
        EnabledWhen = ActivationConstraint.EditorContentType("any"),
    };

    /// <inheritdoc/>
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await base.InitializeAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        Requires.NotNull(context, nameof(context));

        using var textView = await context.GetActiveTextViewAsync(cancellationToken);
        if (textView is null)
        {
            _logger.TraceInformation("There was no active text view when command is executed.");
            return;
        }

        // 在 EditAsync 外预先异步读取设置，避免 sync-over-async 死锁
        AllFormatSetting settings;
        try
        {
            settings = await _configService.LoadAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await ExtensionEntrypoint.WriteToOutputWindowAsync($"读取设置失败: {ex.Message}");
            settings = _configService.Current;
        }

        var extension = FileTypeHelper.GetFileExtension(textView);
        var oldText = textView.Document.Text;
        var oldContent = oldText.CopyToString();

        string? newText = null;
        try
        {
            newText = extension switch
            {
                ".cs" => FormatHelper.CodeFormat(oldContent, settings.CommentSetting),
                ".csproj" => FormatHelper.CSProjFormat(oldContent, settings.CSProjFormatSettings),
                _ => null,
            };
        }
        catch (Exception ex)
        {
            await ExtensionEntrypoint.WriteToOutputWindowAsync($"格式化失败: {ex.Message}");
            await ExtensionEntrypoint.WriteToOutputWindowAsync(ex.ToString());
            return;
        }

        if (newText is null || newText == oldContent)
        {
            // 不支持的文件类型或没有变化，不修改文档
            return;
        }

        await this.Extensibility.Editor().EditAsync(batch =>
        {
            ITextDocumentEditor editor = textView.Document.AsEditable(batch);

            var fullRange = new TextRange(
                new TextPosition(oldText.Document, 0),
                new TextPosition(oldText.Document, oldContent.Length));

            editor.Replace(fullRange, newText);
        }, cancellationToken);
    }
}
