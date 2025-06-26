using CodeAssistant.Services;
using CodeAssistant.Settings;

using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;

using System.Runtime.Serialization;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace CodeAssistant.ToolWindows;

/// <summary>ViewModel for the MyToolWindowContent remote user control.</summary>
[DataContract]
internal class MyToolWindowVM : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility extensibility;
    private readonly Settings.ParentCategoryObserver settingsObserver;

    private XmlFormatSettings xmlFormatSettings;
    private ConfigService ConfigService;

    public MyToolWindowVM(VisualStudioExtensibility extensibility, Settings.ParentCategoryObserver settingsObserver, ConfigService configService)
    {
        this.extensibility = Requires.NotNull(extensibility);
        this.settingsObserver = Requires.NotNull(settingsObserver);
        ConfigService = Requires.NotNull(configService);
        settingsObserver.Changed += this.SettingsObserver_ChangedAsync;

        xmlFormatSettings = ConfigService.GetXMLSettings(new CancellationToken()).Result;

        #region InitCommand

        TestCommand = new AsyncCommand((parameter, clientContext, cancellationToken) =>
        {
            Text = $"Hello {parameter as string}!";
            return Task.CompletedTask;
        });

        SaveCommand = new AsyncCommand(async (parameter, clientContext, cancellationToken) =>
        {
            Text = $"Save {parameter as string}!";

            xmlFormatSettings.NewLineOnAttributes = NewLineOnAttributesSetting;
            xmlFormatSettings.IndentChars = IndentCharsSetting;
            xmlFormatSettings.AddEmptyLineBetweenGroups = AddEmptyLineBetweenGroupsSetting;
            //ConfigService.xmlFormatSettings = xmlFormatSettings;

            await ConfigService.SaveXMLSettings(cancellationToken);
        });

        #endregion InitCommand
    }

    /// <summary>Changed 事件处理程序始终至少被调用一次，且使用当前设置值，因此无需读取初始值。</summary>
    /// <param name="snapshot"></param>
    /// <returns></returns>
    private Task SettingsObserver_ChangedAsync(ParentCategorySnapshot snapshot)
    {
        //第一次读会更新，后续写也会更新，但是值已经提前赋过
        this.IndentCharsSetting = snapshot.IndentCharsSetting.ValueOrDefault(SettingDefinitions.IndentCharsSetting.DefaultValue);
        this.NewLineOnAttributesSetting = snapshot.NewLineOnAttributesSetting.ValueOrDefault(SettingDefinitions.NewLineOnAttributesSetting.DefaultValue);
        this.AddEmptyLineBetweenGroupsSetting = snapshot.AddEmptyLineBetweenGroupsSetting.ValueOrDefault(SettingDefinitions.AddEmptyLineBetweenGroupsSetting.DefaultValue);

        return Task.CompletedTask;
    }

    #region Command

    [DataMember]
    public AsyncCommand TestCommand { get; }

    [DataMember]
    public AsyncCommand ReadCommand { get; }

    
    [DataMember]
    public AsyncCommand SaveCommand { get; }

    #endregion Command

    #region PropNF

    private string _text = string.Empty;

    [DataMember]
    public string Text
    {
        get => _text;
        set => SetProperty(ref this._text, value);
    }

    private string indentCharsSetting = string.Empty;

    [DataMember]
    public string IndentCharsSetting
    {
        get => indentCharsSetting;
        set => SetProperty(ref this.indentCharsSetting, value);
    }

    private bool newLineOnAttributes;

    [DataMember]
    public bool NewLineOnAttributesSetting
    {
        get => newLineOnAttributes;
        set => SetProperty(ref this.newLineOnAttributes, value);
    }

    private bool addEmptyLineBetweenGroups;

    [DataMember]
    public bool AddEmptyLineBetweenGroupsSetting
    {
        get => addEmptyLineBetweenGroups;
        set => SetProperty(ref this.addEmptyLineBetweenGroups, value);
    }

    #endregion PropNF
}