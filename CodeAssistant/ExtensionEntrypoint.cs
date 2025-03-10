using CodeAssistant.Services;
using CodeAssistant.ToolWindows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Documents;
using Microsoft.VisualStudio.ProjectSystem.Query;

using System.Threading;

#pragma warning disable VSEXTPREVIEW_SETTINGS
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

namespace CodeAssistant;

[VisualStudioContribution]
public class ExtensionEntrypoint : Extension
{
    public static OutputChannel? outputChannel;

    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "CodeAssistant.f84a9b49-806c-440d-b121-5a806ba0b4f8",
                version: this.ExtensionAssemblyVersion,
                publisherName: "EricPan",
                displayName: "CodeAssistant",
                description: "This is a code assistant,include some code formate functions..."),
    };

    protected override Task OnInitializedAsync(VisualStudioExtensibility extensibility, CancellationToken cancellationToken)
    {
        outputChannel = extensibility.Views().Output.CreateOutputChannelAsync("%CodeAssistant.DisplayName%", cancellationToken).Result;
        return base.OnInitializedAsync(extensibility, cancellationToken);
    }

    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSettingsObservers();
        serviceCollection.AddSingleton<MyToolWindowVM>();
        serviceCollection.AddSingleton<ConfigService>();
        base.InitializeServices(serviceCollection);
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
            MenuChild.Command<InsertGuidCommand>(),
            MenuChild.Command<FormatCSProjCommand>(),
            MenuChild.Command<MyToolWindowCommand>(),
        },
    };
}