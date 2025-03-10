using CodeAssistant.Services;

using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;

using System.Diagnostics;
using System.Globalization;

namespace CodeAssistant;
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

/// <summary>Command1 handler.</summary>
[VisualStudioContribution]
internal class InsertGuidCommand : Command
{
    private readonly TraceSource logger;
    ConfigService ConfigService;
    /// <summary>Initializes a new instance of the <see cref="InsertGuidCommand"/> class.</summary>
    /// <param name="traceSource">Trace source instance to utilize.</param>
    public InsertGuidCommand(TraceSource traceSource, ConfigService configService)
    {
        // This optional TraceSource can be used for logging in the command. You can use dependency
        // injection to access other services here as well.
        this.logger = Requires.NotNull(traceSource, nameof(traceSource));

        ConfigService = Requires.NotNull(configService);
    }

    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new("%InsertGuidCommand.DisplayName%")
    {
        // Use this object initializer to set optional parameters for the command. The required
        // parameter, displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
        Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
        //Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorContentType, ".+")
    };

    /// <inheritdoc/>
    public override Task InitializeAsync(CancellationToken cancellationToken)
    {
        // Use InitializeAsync for any one-time setup or initialization.
        return base.InitializeAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        Requires.NotNull(context, nameof(context));
        var newGuidString = Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture);

        using var textView = await context.GetActiveTextViewAsync(cancellationToken);
        if (textView is null)
        {
            this.logger.TraceInformation("There was no active text view when command is executed.");
            return;
        }
        await ExtensionEntrypoint.outputChannel!.WriteLineAsync("This is a test of the output channel.");

        await this.Extensibility.Editor().EditAsync(
            batch =>
            {
                var editor = textView.Document.AsEditable(batch);
                //// specify the desired changes here:
                //editor.Replace(textView.Selection.Extent, newGuidString);

                var caret = textView.Selection.Extent.Start;
                editor.Replace(textView.Selection.Extent, newGuidString);
                textView.AsEditable(batch).SetSelections([new Selection(activePosition: caret, anchorPosition: caret, insertionPosition: caret)]);
            },
            cancellationToken);
    }
}