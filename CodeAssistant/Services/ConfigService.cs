using CodeAssistant.Settings;

using Microsoft.VisualStudio.Extensibility;

using System.Text.Json;

#pragma warning disable VSEXTPREVIEW_SETTINGS
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

namespace CodeAssistant.Services;

/// <summary>https://learn.microsoft.com/zh-cn/visualstudio/extensibility/visualstudio.extensibility/settings/settings?view=vs-2022</summary>
public class ConfigService
{
    private VisualStudioExtensibility VisualStudioExtensibility;

    public AllFormateSetting allFormatSettings = new();
    private JsonSerializerOptions JsonSerializerOptions;

    public CommentSetting commentSetting = new();

    public ConfigService(VisualStudioExtensibility visualStudioExtensibility)
    {
        VisualStudioExtensibility = visualStudioExtensibility;

        JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // 格式化输出
            IncludeFields = true, // 包括字段
        };
    }

    public async Task<CommentSetting> GetCommentSettingAsync(CancellationToken cancellationToken)
    {
        //读取设置
        //单独读取
        //var result = await VisualStudioExtensibility.Settings().ReadEffectiveValueAsync(SettingDefinitions.IndentCharsSetting, cancellationToken);
        //xmlFormatSettings.IndentChars = result.ValueOrDefault(" ");

        //批量读取
        var results = await VisualStudioExtensibility.Settings().ReadEffectiveValuesAsync([SettingDefinitions.MultiSummaryToSingle,
            SettingDefinitions.WithSpace],
            cancellationToken);

        var value1 = results.ValueOrDefault(SettingDefinitions.MultiSummaryToSingle, true);
        commentSetting.MultiSummaryToSingle = value1;

        var value2 = results.ValueOrDefault(SettingDefinitions.WithSpace, true);
        commentSetting.WithSpace = value2;

        return commentSetting;
    }

    public async Task<AllFormateSetting> GetAllSettingsAsync(CancellationToken cancellationToken)
    {
        //读取设置
        //单独读取
        var result = await VisualStudioExtensibility.Settings().ReadEffectiveValueAsync(SettingDefinitions.AllConfig, cancellationToken);

        var allSetting = new AllFormateSetting();
        try
        {
            allSetting = JsonSerializer.Deserialize<AllFormateSetting>(result.ValueOrDefault(""), JsonSerializerOptions);
        }
        catch (Exception)
        {
            await ExtensionEntrypoint.outputChannel!.WriteLineAsync("读取设置失败！生成默认配置！");
        }

        //批量读取

        return allSetting;
    }

    public async Task SaveAllSettingsAsync(CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(allFormatSettings, JsonSerializerOptions);
        var writeResult = await VisualStudioExtensibility.Settings().WriteAsync(
            batch =>
            {
                batch.WriteSetting(SettingDefinitions.AllConfig, value: json);
                batch.WriteSetting(SettingDefinitions.MultiSummaryToSingle, value: commentSetting.MultiSummaryToSingle);
                batch.WriteSetting(SettingDefinitions.WithSpace, value: commentSetting.MultiSummaryToSingle);
            },
            description: "Updating the settings",
            cancellationToken);
    }
}