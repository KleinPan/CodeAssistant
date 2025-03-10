using Microsoft.VisualStudio.Extensibility.UI;

namespace CodeAssistant.ToolWindows;

/// <summary>
/// A remote user control to use as tool window UI content.
/// </summary>
internal class MyToolWindowContent : RemoteUserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyToolWindowContent" /> class.
    /// </summary>
    public MyToolWindowContent(object? dataContext)
        : base(dataContext)
    {
    }
}
