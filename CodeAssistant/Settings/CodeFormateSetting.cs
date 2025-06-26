namespace CodeAssistant.Settings;

internal class CodeFormateSetting
{
    public DeleteSetting DeleteSetting { get; set; }
    public CommentSetting CommentSetting { get; set; }
}

internal class CommentSetting
{
    public bool MultiSummaryToSingle { get; set; } = true;

    /// <summary>
    /// 文字周围是否带空格
    /// </summary>
    public bool WithSpace { get; set; } = true;
}

internal class DeleteSetting
{
    /// <summary>
    /// 删除连续的空白行
    /// </summary>
    public bool DeleteMultiSpaceLines { get; set; } = true;
}