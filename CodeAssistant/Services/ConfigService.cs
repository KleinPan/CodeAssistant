using Microsoft.VisualStudio.Extensibility;

#pragma warning disable VSEXTPREVIEW_SETTINGS

namespace CodeAssistant.Services;

public class ConfigService
{
    private VisualStudioExtensibility VisualStudioExtensibility;
    public XmlFormatSettings xmlFormatSettings = new XmlFormatSettings();

    public ConfigService(VisualStudioExtensibility visualStudioExtensibility)
    {
        VisualStudioExtensibility = visualStudioExtensibility;
    }

    public async Task<XmlFormatSettings> GetXMLSettings(CancellationToken cancellationToken)
    {
        //读取设置
        //单独读取
        //var result = await visualStudioExtensibility.Settings().ReadEffectiveValueAsync(SettingDefinitions.IndentCharsSetting, cancellationToken);
        //xmlFormatSettings.IndentChars = result.ValueOrDefault(" ");

        //批量读取
        var results = await VisualStudioExtensibility.Settings().ReadEffectiveValuesAsync(
        [SettingDefinitions.IndentCharsSetting, SettingDefinitions.NewLineOnAttributesSetting],
        cancellationToken);
        var value1 = results.ValueOrDefault(SettingDefinitions.IndentCharsSetting, "  ");
        xmlFormatSettings.IndentChars = value1;
        var value2 = results.ValueOrDefault(SettingDefinitions.NewLineOnAttributesSetting, false);
        xmlFormatSettings.NewLineOnAttributes = value2;

        return xmlFormatSettings;
    }

    public async Task SaveXMLSettings(CancellationToken cancellationToken)
    {
        var writeResult = await VisualStudioExtensibility.Settings().WriteAsync(
            batch =>
            {
                batch.WriteSetting(SettingDefinitions.IndentCharsSetting, value: xmlFormatSettings.IndentChars);
                batch.WriteSetting(SettingDefinitions.NewLineOnAttributesSetting, value: xmlFormatSettings.NewLineOnAttributes);
                batch.WriteSetting(SettingDefinitions.AddEmptyLineBetweenGroupsSetting, value: xmlFormatSettings.AddEmptyLineBetweenGroups);
            },
            description: "Updating the settings",
            cancellationToken);
    }
}