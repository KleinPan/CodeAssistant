﻿using CodeAssistant.Services;

using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace CodeAssistant.ToolWindows;

/// <summary>CodeAssistant 主工具窗口。</summary>
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    private readonly MyToolWindowVM _dataContext;

    /// <summary>通过 DI 注入 VM。构造函数必须为 public：
    /// SDK 的 GeneratedToolWindowProvider 通过反射激活工具窗口，
    /// internal 构造函数会导致激活失败，报 "Unregistered tool window"。</summary>
    public MyToolWindow(MyToolWindowVM dataContext)
    {
        Requires.NotNull(dataContext);

        this.Title = "%CodeAssistant.DisplayName%";
        _dataContext = dataContext;
    }

    /// <inheritdoc/>
    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        Placement = ToolWindowPlacement.DocumentWell,
    };

    /// <inheritdoc/>
    public override async Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        // 在 UI 创建时异步加载配置，避免在构造函数中 sync-over-async
        await _dataContext.InitializeAsync(cancellationToken);
        return new MyToolWindowContent(_dataContext);
    }
}
