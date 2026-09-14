using CodeAssistant.Settings;

using Microsoft.VisualStudio.Extensibility;

using System.Text.Json;

#pragma warning disable VSEXTPREVIEW_SETTINGS
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW

namespace CodeAssistant.Services;

/// <summary>
/// 配置服务，负责读写 VS 设置存储中的 CodeAssistant 配置。
/// 参考: https://learn.microsoft.com/zh-cn/visualstudio/extensibility/visualstudio.extensibility/settings/settings?view=vs-2022
/// </summary>
public class ConfigService
{
    private readonly VisualStudioExtensibility _visualStudioExtensibility;
    // 保护 Current 的并发读写：工具窗口 VM 修改与 FormatCodeCommand 读取可能并发发生。
    private readonly object _currentLock = new();

    /// <summary>当前内存中的所有格式化设置。VM 直接修改此实例，Save 时整体持久化。</summary>
    public AllFormatSetting Current
    {
        get
        {
            lock (_currentLock)
            {
                return _current;
            }
        }
        private set
        {
            lock (_currentLock)
            {
                _current = value;
            }
        }
    }

    private AllFormatSetting _current = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        // 使用属性而不是字段，更稳定可控
        IncludeFields = false,
    };

    public ConfigService(VisualStudioExtensibility visualStudioExtensibility)
    {
        _visualStudioExtensibility = visualStudioExtensibility;
    }

    /// <summary>从 VS 设置存储中加载所有配置到 <see cref="Current"/>。</summary>
    public async Task<AllFormatSetting> LoadAsync(CancellationToken cancellationToken)
    {
        var result = await _visualStudioExtensibility.Settings().ReadEffectiveValueAsync(
            SettingDefinitions.AllConfig, cancellationToken);

        var json = result.ValueOrDefault(string.Empty);
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<AllFormatSetting>(json, JsonOptions);
                if (loaded is not null)
                {
                    Current = loaded;
                }
            }
            catch (Exception ex)
            {
                await ExtensionEntrypoint.WriteToOutputWindowAsync(
                    $"读取设置失败，使用默认配置。错误: {ex.Message}");
            }
        }

        return Current;
    }

    /// <summary>将 <see cref="Current"/> 持久化到 VS 设置存储。</summary>
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        // 快照序列化，减少持锁时间
        var json = JsonSerializer.Serialize(Current, JsonOptions);

        try
        {
            await _visualStudioExtensibility.Settings().WriteAsync(
                batch =>
                {
                    batch.WriteSetting(SettingDefinitions.AllConfig, value: json);
                },
                description: "Updating the CodeAssistant settings",
                cancellationToken);
        }
        catch (Exception ex)
        {
            await ExtensionEntrypoint.WriteToOutputWindowAsync($"保存设置失败: {ex.Message}");
            throw;
        }
    }
}
