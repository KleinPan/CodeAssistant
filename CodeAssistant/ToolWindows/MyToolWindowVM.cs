﻿﻿using CodeAssistant.Services;
using CodeAssistant.Settings;

using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.UI;

using System.Runtime.Serialization;

#pragma warning disable VSEXTPREVIEW_SETTINGS // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace CodeAssistant.ToolWindows;

/// <summary>ViewModel for the MyToolWindowContent remote user control.
/// 必须为 public：MyToolWindow 的 public 构造函数以它为参数（SDK 反射激活要求）。</summary>
[DataContract]
public class MyToolWindowVM : NotifyPropertyChangedObject
{
    private readonly VisualStudioExtensibility _extensibility;
    private readonly ConfigService _configService;

    private string _text = string.Empty;
    private bool _isBusy;

    public MyToolWindowVM(VisualStudioExtensibility extensibility, ConfigService configService)
    {
        _extensibility = Requires.NotNull(extensibility);
        _configService = Requires.NotNull(configService);

        SaveCommand = new AsyncCommand(async (parameter, clientContext, cancellationToken) =>
        {
            try
            {
                IsBusy = true;
                await _configService.SaveAsync(cancellationToken);
                Text = "保存成功";
            }
            catch (Exception ex)
            {
                Text = $"保存失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        });

        ReloadCommand = new AsyncCommand(async (parameter, clientContext, cancellationToken) =>
        {
            try
            {
                IsBusy = true;
                await _configService.LoadAsync(cancellationToken);
                // 触发所有绑定刷新
                RaiseNotifyPropertyChangedEvent(nameof(IndentChars));
                RaiseNotifyPropertyChangedEvent(nameof(NewLineOnAttributes));
                RaiseNotifyPropertyChangedEvent(nameof(AddEmptyLineBetweenGroups));
                RaiseNotifyPropertyChangedEvent(nameof(WithSpace));
                RaiseNotifyPropertyChangedEvent(nameof(MultiSummaryToSingle));
                Text = "已重新加载";
            }
            catch (Exception ex)
            {
                Text = $"加载失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        });
    }

    /// <summary>在工具窗口内容创建后异步初始化（读取持久化设置）。</summary>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _configService.LoadAsync(cancellationToken);
            RaiseNotifyPropertyChangedEvent(nameof(IndentChars));
            RaiseNotifyPropertyChangedEvent(nameof(NewLineOnAttributes));
            RaiseNotifyPropertyChangedEvent(nameof(AddEmptyLineBetweenGroups));
            RaiseNotifyPropertyChangedEvent(nameof(WithSpace));
            RaiseNotifyPropertyChangedEvent(nameof(MultiSummaryToSingle));
        }
        catch (Exception ex)
        {
            Text = $"初始化失败: {ex.Message}";
        }
    }

    #region Commands

    [DataMember]
    public AsyncCommand SaveCommand { get; }

    [DataMember]
    public AsyncCommand ReloadCommand { get; }

    #endregion Commands

    #region Properties

    [DataMember]
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    [DataMember]
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                RaiseNotifyPropertyChangedEvent(nameof(IsIdle));
            }
        }
    }

    /// <summary>IsBusy 的取反，便于 XAML 直接绑定到 IsEnabled。</summary>
    [DataMember]
    public bool IsIdle => !IsBusy;

    [DataMember]
    public string IndentChars
    {
        get => _configService.Current.CSProjFormatSettings.IndentChars;
        set
        {
            if (_configService.Current.CSProjFormatSettings.IndentChars != value)
            {
                _configService.Current.CSProjFormatSettings.IndentChars = value;
                RaiseNotifyPropertyChangedEvent(nameof(IndentChars));
            }
        }
    }

    [DataMember]
    public bool NewLineOnAttributes
    {
        get => _configService.Current.CSProjFormatSettings.NewLineOnAttributes;
        set
        {
            if (_configService.Current.CSProjFormatSettings.NewLineOnAttributes != value)
            {
                _configService.Current.CSProjFormatSettings.NewLineOnAttributes = value;
                RaiseNotifyPropertyChangedEvent(nameof(NewLineOnAttributes));
            }
        }
    }

    [DataMember]
    public bool AddEmptyLineBetweenGroups
    {
        get => _configService.Current.CSProjFormatSettings.AddEmptyLineBetweenGroups;
        set
        {
            if (_configService.Current.CSProjFormatSettings.AddEmptyLineBetweenGroups != value)
            {
                _configService.Current.CSProjFormatSettings.AddEmptyLineBetweenGroups = value;
                RaiseNotifyPropertyChangedEvent(nameof(AddEmptyLineBetweenGroups));
            }
        }
    }

    [DataMember]
    public bool WithSpace
    {
        get => _configService.Current.CommentSetting.WithSpace;
        set
        {
            if (_configService.Current.CommentSetting.WithSpace != value)
            {
                _configService.Current.CommentSetting.WithSpace = value;
                RaiseNotifyPropertyChangedEvent(nameof(WithSpace));
            }
        }
    }

    [DataMember]
    public bool MultiSummaryToSingle
    {
        get => _configService.Current.CommentSetting.MultiSummaryToSingle;
        set
        {
            if (_configService.Current.CommentSetting.MultiSummaryToSingle != value)
            {
                _configService.Current.CommentSetting.MultiSummaryToSingle = value;
                RaiseNotifyPropertyChangedEvent(nameof(MultiSummaryToSingle));
            }
        }
    }

    #endregion Properties
}
