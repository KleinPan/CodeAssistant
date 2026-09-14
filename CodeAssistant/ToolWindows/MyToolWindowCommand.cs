﻿﻿﻿using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace CodeAssistant.ToolWindows;

/// <summary>显示 CodeAssistant 工具窗口的命令。</summary>
[VisualStudioContribution]
public class MyToolWindowCommand : Command
{
    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new("%MyToolWindowCommand.DisplayName%")
    {
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
    };

    /// <inheritdoc/>
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        await this.Extensibility.Shell().ShowToolWindowAsync<MyToolWindow>(activate: true, cancellationToken);
    }
}
