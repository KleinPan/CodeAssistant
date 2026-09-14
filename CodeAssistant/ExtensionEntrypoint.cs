﻿﻿﻿﻿﻿using CodeAssistant.Services;
using CodeAssistant.ToolWindows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Documents;

#pragma warning disable VSEXTPREVIEW_SETTINGS
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

namespace CodeAssistant;

[VisualStudioContribution]
public class ExtensionEntrypoint : Extension
{
    private static OutputChannel? _outputChannel;

    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "CodeAssistant.f84a9b49-806c-440d-b121-5a806ba0b4f8",
                version: this.ExtensionAssemblyVersion,
                publisherName: "EricPan",
                displayName: "CodeAssistant",
                description: "This is a code assistant,include some code formate functions..."),
    };

    protected override async Task OnInitializedAsync(VisualStudioExtensibility extensibility, CancellationToken cancellationToken)
    {
        _outputChannel = await extensibility.Views().Output.CreateOutputChannelAsync("%CodeAssistant.DisplayName%", cancellationToken);
        await base.OnInitializedAsync(extensibility, cancellationToken);
    }

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MyToolWindowVM>();
        serviceCollection.AddSingleton<ConfigService>();
        base.InitializeServices(serviceCollection);
    }

    /// <summary>
    /// 写入一条消息到 VS 输出窗口。若输出通道尚未初始化则忽略。
    /// </summary>
    public static async Task WriteToOutputWindowAsync(string message)
    {
        if (_outputChannel is null)
            return;
        await _outputChannel.WriteLineAsync(message);
    }

    [VisualStudioContribution]
    public static MenuConfiguration MyMenu => new("%CodeAssistant.DisplayName%")
    {
        Placements = new CommandPlacement[]
        {
            CommandPlacement.KnownPlacements.ExtensionsMenu
        },
        Children = new[]
        {
            MenuChild.Command<MyToolWindowCommand>(),
            MenuChild.Command<InsertGuidCommand>(),
            MenuChild.Command<FormatCodeCommand>(),
        },
    };

    // [工具栏回归测试] VS 2026 18.10 较初报版本（DevCom #11086730）已跨多个
    // 小版本，重测自定义工具栏是否仍导致其他工具栏按钮状态错乱：
    // - 若已修复：图标常驻 CodeAssistant 工具栏（View→Toolbars 勾选显示）。
    // - 若仍错乱：删除此 ToolbarConfiguration，仅保留扩展菜单入口。
    [VisualStudioContribution]
    public static ToolbarConfiguration CodeAssistantToolbar =>
        new("%CodeAssistant.DisplayName%")
        {
            Children =
            [
                ToolbarChild.Command<FormatCodeCommand>(),
            ]
        };
}
