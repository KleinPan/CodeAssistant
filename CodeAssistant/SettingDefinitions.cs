using Microsoft.VisualStudio.Extensibility;

using Microsoft.VisualStudio.Extensibility.Settings;

namespace CodeAssistant;
#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

/// <summary>暂时不能用</summary>
internal static class SettingDefinitions
{
    [VisualStudioContribution]
    public static SettingCategory ParentCategory { get; } = new("parentCategory", "Parent Category")
    {
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    public static SettingCategory ChildCategory { get; } = new("childCategory", "Child Category", ParentCategory)
    {
        GenerateObserverClass = true,
    };

    [VisualStudioContribution]
    public static Setting.String IndentCharsSetting { get; } = new("IndentChars", "%IndentChars.DisplayName%", ChildCategory, defaultValue: "  ");

    [VisualStudioContribution]
    public static Setting.Boolean NewLineOnAttributesSetting { get; } = new("NewLineOnAttributes", "%NewLineOnAttributes.DisplayName%", ChildCategory, defaultValue: false);

    [VisualStudioContribution]
    public static Setting.Boolean AddEmptyLineBetweenGroupsSetting { get; } = new("AddEmptyLineBetweenGroups", "%AddEmptyLineBetweenGroups.DisplayName%", ChildCategory, defaultValue: false);
}