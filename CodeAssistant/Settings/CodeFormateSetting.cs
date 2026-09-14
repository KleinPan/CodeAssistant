﻿﻿namespace CodeAssistant.Settings;

/// <summary>所有格式化设置的聚合根。</summary>
public class AllFormatSetting
{
    public CommentSetting CommentSetting { get; set; } = new();

    public CSProjFormatSettings CSProjFormatSettings { get; set; } = new();
}

public class CommentSetting
{
    /// <summary>是否将多行 summary 注释压缩为单行。</summary>
    public bool MultiSummaryToSingle { get; set; } = true;

    /// <summary>summary 文字周围是否带空格。</summary>
    public bool WithSpace { get; set; } = true;
}

/// <summary>CSProj 格式化设置。</summary>
public class CSProjFormatSettings
{
    /// <summary>缩进字符。</summary>
    public string IndentChars { get; set; } = "  ";

    /// <summary>属性是否换行。</summary>
    public bool NewLineOnAttributes { get; set; } = false;

    /// <summary>在 ItemGroup 等元素间添加空行。</summary>
    public bool AddEmptyLineBetweenGroups { get; set; } = true;

    /// <summary>展开空元素（如 &lt;ItemGroup/&gt; → &lt;ItemGroup&gt;&lt;/ItemGroup&gt;）。</summary>
    public bool ExpandEmptyElements { get; set; } = false;

    /// <summary>分组结束后需要补空行的元素名集合。</summary>
    public HashSet<string> ElementsWithNewLine { get; set; } = new()
    {
        "ItemGroup", "PropertyGroup"
    };
}
