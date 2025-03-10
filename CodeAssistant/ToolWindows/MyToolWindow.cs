using CodeAssistant.Services;

using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace CodeAssistant.ToolWindows;

/// <summary>A sample tool window.</summary>
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
    private MyToolWindowVM? dataContext;

    /// <summary>依赖注入了</summary>
    /// <param name="settingsObserver"></param>
    public MyToolWindow(Settings.ParentCategoryObserver settingsObserver,ConfigService configService)
    {
        Requires.NotNull(settingsObserver);
        this.Title = "CodeAssistant Tool Window";
        this.dataContext = new MyToolWindowVM(this.Extensibility, settingsObserver, configService);

        //await this.Extensibility.Shell().ShowToolWindowAsync<MyToolWindow>(activate: true, cancellationToken);
        //using ServiceBrokerClient.Rental<IToolWindowManager> toolWindowManager = await base.ServiceBrokerClient.GetProxyAsync<IToolWindowManager>(VisualStudioServices.VS2022_3.ToolWindowManager, cancellationToken);
        //Assumes.NotNull(toolWindowManager.Proxy);
        //await toolWindowManager.Proxy.ShowAsync(toolWindowType.ToString(), activate, cancellationToken);
    }

    /// <inheritdoc/>
    public override ToolWindowConfiguration ToolWindowConfiguration => new()
    {
        // Use this object initializer to set optional parameters for the tool window.
        Placement = ToolWindowPlacement.Floating,
    };

    /// <inheritdoc/>
    public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IRemoteUserControl>(new MyToolWindowContent(this.dataContext));
    }
}