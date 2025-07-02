namespace CodeAssistant.Settings;

public class AllFormateSetting
{
    public DeleteSetting DeleteSetting { get; set; } = new DeleteSetting();
    //public CommentSetting CommentSetting { get; set; } = new CommentSetting();
    public CSProjFormatSettings CSProjFormatSettings { get; set; } = new CSProjFormatSettings();
}

public class CommentSetting
{
    public bool MultiSummaryToSingle = true;

    /// <summary>文字周围是否带空格</summary>
    public bool WithSpace = true;
}

public class DeleteSetting
{
    /// <summary>删除连续的空白行</summary>
    public bool DeleteMultiSpaceLines = true;
}

/// <summary>基础格式</summary>
public class CSProjFormatSettings
{
    /// <summary>是否保留元素Value文本中的换行</summary>
    public bool PreserveElementValueNewLines = false;

    /// <summary>缩进字符</summary>
    public string IndentChars = "  ";

    /// <summary>属性换行</summary>
    public bool NewLineOnAttributes = false;

    /// <summary> 在<ItemGroup>等元素间添加空行 </summary>
    public bool AddEmptyLineBetweenGroups = true;

    /// <summary>展开空元素（如 <ItemGroup/> → <ItemGroup></ItemGroup>）</summary>
    public bool ExpandEmptyElements = false;

    /// <summary>特定元素处理</summary>
    public HashSet<string> ElementsWithAttributeAlignment = new()
    {
        "PackageReference", "ProjectReference", "Reference"
    };

    /// <summary>特定元素处理</summary>
    public HashSet<string> ElementsWithNewLine = new()
    {
        "ItemGroup", "PropertyGroup"
    };
}