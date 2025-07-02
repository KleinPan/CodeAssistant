using Microsoft.VisualStudio.Extensibility;

using Microsoft.VisualStudio.Extensibility.Settings;

namespace CodeAssistant;
#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

/// <summary>暂时不能用</summary>
internal static class SettingDefinitions
{
    [VisualStudioContribution]
    public static SettingCategory ParentCategory { get; } = new("parentCategory", "%ParentCategory.DisplayName%")
    {
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    public static SettingCategory ChildCategory { get; } = new("childCategory", "%ChildCategory.DisplayName%", ParentCategory)
    {
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    public static Setting.String AllConfig { get; } = new("allConfig", "%AllConfig.DisplayName%", ParentCategory, defaultValue: "");

    [VisualStudioContribution]
    public static Setting.Boolean MultiSummaryToSingle { get; } = new("multiSummaryToSingle", "%AllConfig.MultiSummaryToSingle%", ParentCategory, defaultValue: true);

    [VisualStudioContribution]
    public static Setting.Boolean WithSpace { get; } = new("withSpace", "%WithSpace.DisplayName%", ParentCategory, defaultValue: true);
}