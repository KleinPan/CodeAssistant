﻿﻿using Microsoft.VisualStudio.Extensibility;

using Microsoft.VisualStudio.Extensibility.Settings;

namespace CodeAssistant;

#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

internal static class SettingDefinitions
{
    [VisualStudioContribution]
    public static SettingCategory ParentCategory { get; } = new("parentCategory", "%ParentCategory.DisplayName%")
    {
        GenerateObserverClass = true,
    };

    /// <summary>序列化后的所有配置 JSON。所有设置项均由该 JSON 单一存储，避免双重同步。</summary>
    [VisualStudioContribution]
    public static Setting.String AllConfig { get; } = new("allConfig", "%AllConfig.DisplayName%", ParentCategory, defaultValue: "");
}
