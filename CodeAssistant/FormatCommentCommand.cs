using CodeAssistant.Helpers;
using CodeAssistant.Services;

using Microsoft;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Editor;

using System.Diagnostics;

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // The settings API is currently in preview and marked as experimental

namespace CodeAssistant;

/// <summary>FormatCSProj handler.</summary>
[VisualStudioContribution]
internal class FormatCommentCommand : Command
{
    private readonly TraceSource logger;
    private ConfigService ConfigService;

    public FormatCommentCommand(TraceSource traceSource, ConfigService configService)
    {
        // This optional TraceSource can be used for logging in the command. You can use dependency
        // injection to access other services here as well.
        this.logger = Requires.NotNull(traceSource, nameof(traceSource));

        ConfigService = Requires.NotNull(configService);
    }

    /// <inheritdoc/>
    public override CommandConfiguration CommandConfiguration => new(displayName: "FormatComment")
    {
        // Use this object initializer to set optional parameters for the command. The required
        // parameter, displayName, is set above. To localize the displayName, add an entry in
        // .vsextension\string-resources.json and reference it here by passing
        // "%CodeAssistant.FormatCSProj.DisplayName%" as a constructor parameter.
        //Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.FormatDocument, IconSettings.IconAndText),
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

        await this.Extensibility.Editor().EditAsync(
               batch =>
          {
              ITextDocumentEditor editor = textView.Document.AsEditable(batch);

              var oldText = textView.Document.Text;
              var oldContent = oldText.CopyToString();

              try
              {
                  // 解析代码文档
                  var tree = CSharpSyntaxTree.ParseText(oldContent);
                  var root = tree.GetCompilationUnitRoot(cancellationToken);

                  // 创建注释处理器并执行处理
                  var processor = new CommentHelper();
                  var replacements = processor.ProcessMultiSummary(root);

                  // 处理完后应用替换
                  foreach (var replacement in replacements)
                  {
                      TextRange aa = new(new TextPosition(oldText.Document, replacement.Start), new TextPosition(oldText.Document, replacement.End));
                      // 应用文本替换
                      editor.Replace(aa, replacement.NewText);
                  }
              }
              catch (Exception ex)
              {
                  ExtensionEntrypoint.outputChannel?.WriteLineAsync($"Error processing comments: {ex.Message}").Wait();
                  this.logger.TraceInformation(ex.ToString());
              }
          },
          cancellationToken);
    }
}