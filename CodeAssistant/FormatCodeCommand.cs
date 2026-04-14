using CodeAssistant.Helpers;
using CodeAssistant.Services;

using Microsoft;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;

using System.Diagnostics;

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // The settings API is currently in preview and marked as experimental

namespace CodeAssistant;

//https://learn.microsoft.com/zh-cn/visualstudio/extensibility/visualstudio.extensibility/editor/editor-concepts?view=visualstudio

/// <summary>FormatCSProj handler.</summary>
[VisualStudioContribution]
internal class FormatCodeCommand : Command
{
    private readonly TraceSource logger;
    private ConfigService ConfigService;

    public FormatCodeCommand(TraceSource traceSource, ConfigService configService)
    {
        // This optional TraceSource can be used for logging in the command. You can use dependency
        // injection to access other services here as well.
        this.logger = Requires.NotNull(traceSource, nameof(traceSource));

        ConfigService = Requires.NotNull(configService);
    }

    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new(displayName: "FormatCode")
    {
        // Use this object initializer to set optional parameters for the command. The required
        // parameter, displayName, is set above. To localize the displayName, add an entry in
        // .vsextension\string-resources.json and reference it here by passing
        // "%CodeAssistant.FormatCSProj.DisplayName%" as a constructor parameter.
        //Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.CleanData, IconSettings.None),
    };

    /// <inheritdoc/>
    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        // Use InitializeAsync for any one-time setup or initialization.
        await base.InitializeAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        //await this.Extensibility.Shell().ShowPromptAsync("Hello from an extension!", PromptOptions.OK, cancellationToken);

        Requires.NotNull(context, nameof(context));

        using var textView = await context.GetActiveTextViewAsync(cancellationToken);
        if (textView is null)
        {
            this.logger.TraceInformation("There was no active text view when command is executed.");
            return;
        }

        await this.Extensibility.Editor().EditAsync(batch =>
        {
            //获取 Roslyn Document
            ITextDocumentEditor editor = textView.Document.AsEditable(batch);

            var oldText = textView.Document.Text;
            var oldContent = oldText.CopyToString();

            try
            {
                var settings = ConfigService.GetAllSettingsAsync(cancellationToken).Result;

                var newText = FileTypeHelper.GetFileExtension(textView) switch
                {
                    ".cs" => FormatHelper.CodeFormat(oldContent),
                    ".csproj" => FormatHelper.CSProjFormat(oldContent, settings.CSProjFormatSettings),

                    _ => oldContent
                };

                var fullRange = new TextRange(
                    new TextPosition(oldText.Document, 0),
                    new TextPosition(oldText.Document, oldContent.Length));

                editor.Replace(fullRange, newText);
            }
            catch (Exception ex)
            {
                //更改插入点位置或从扩展中选择文本
                //var caret = textView.Selection.Extent.Start;
                //textView.AsEditable(batch).SetSelections([new Selection(activePosition: caret, anchorPosition: caret, insertionPosition: caret)]);
                ExtensionEntrypoint.WriteToOutputWindowAsync($"Error processing comments: {ex.Message}").Wait();
                ExtensionEntrypoint.WriteToOutputWindowAsync(ex.ToString()).Wait();
            }
        }, cancellationToken);
    }
}