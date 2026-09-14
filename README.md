# CodeAssistant

一个 Visual Studio 2022 扩展，提供代码格式化与辅助功能。

## 功能

- **格式化 C# 代码**：将多行 `<summary>` 注释压缩为单行，清理多余空行。
- **格式化 .csproj 文件**：标准 XML 格式化 + CSProj 专属规则（分组间空行、Exec Command 换行缩进等）。
- **插入 GUID**：在光标处插入带连字符的 GUID。
- **工具窗口**：可视化配置格式化选项。

## 使用方法

1. 在编辑器中打开 `.cs` 或 `.csproj` 文件。
2. 通过 `扩展` 菜单 → `CodeAssistant` → `格式化代码`，或工具栏按钮触发格式化。
3. 通过 `扩展` 菜单 → `CodeAssistant` → `显示工具窗口` 打开配置面板。

## 配置项

| 配置 | 说明 | 默认值 |
| --- | --- | --- |
| `WithSpace` | summary 文字两侧是否带空格 | `true` |
| `MultiSummaryToSingle` | 多行 summary 合并为单行（含 `<list>`/`<code>`/`<para>` 等块级元素时跳过） | `true` |
| `IndentChars` | CSProj 缩进字符 | `"  "` (两空格) |
| `NewLineOnAttributes` | CSProj 属性是否换行 | `false` |
| `AddEmptyLineBetweenGroups` | ItemGroup/PropertyGroup 之间添加空行 | `true` |
| `ExpandEmptyElements` | 将 `<Foo/>` 展开为 `<Foo></Foo>` | `false` |

## 更新日志

### 2.1.0 (2025-08-13)

- 修复 `AddEmptyLineBetweenGroups` 关闭后仍补空行的 Bug；
- 修复 `NewLineOnAttributes` 不生效的 Bug；
- 修复插入 GUID 后光标位置错误（停留在 GUID 前）的 Bug；
- 修复格式化后丢失文件末尾换行符的 Bug；
- summary 压缩保留 StartTag 属性，含块级元素（`<list>`/`<code>`/`<para>` 等）时跳过压缩；
- 重构设置存储为单一 JSON 源，消除 `MultiSummaryToSingle`/`WithSpace` 独立开关与 `AllConfig` 的双重同步；
- `ConfigService.Current` 加锁保护并发读写；
- `CSProjPostProcessor` 正则改为编译缓存；
- 清理死代码：`DeleteSetting`、未使用配置项、`IsXaml`、`ChildCategory`、`ParentCategoryObserver` 注入；
- 落实 `ExpandEmptyElements` 配置（原仅声明未实现）。

### 2.0.0 (2025-07-20)

- 修复 `WithSpace` 保存时写入错误值的 Bug；
- 修复工具窗口绑定错误（多余的 CheckBox、`HelloCommand` 不存在等）；
- 消除 `sync-over-async` 死锁风险，统一异步初始化；
- 统一设置数据流：VM 直接修改 `ConfigService.Current`，Save 时整体持久化；
- 让 `WithSpace` / `MultiSummaryToSingle` 配置真正生效；
- 改用 `Docked`(DocumentWell) 工具窗口布局；
- GUID 格式改为带连字符的 `D` 格式；
- 删除已废弃的 `XMLFormatter.cs`；
- 完善本地化资源、命名规范。

### 1.2.0 (2025-07-02)

- 整理设置相关代码；
- 优化配置页面。

### 1.1.0 (2025-06-26)

- 新增格式化 summary 注释功能。

### 1.0.0 (2025-03-13)

- 新增格式化 Command 命令。

## 参考文档

- [VisualStudio.Extensibility](https://learn.microsoft.com/zh-cn/visualstudio/extensibility/visualstudio.extensibility/?view=visualstudio)
- [VSExtensibility 示例](https://github.com/microsoft/VSExtensibility/tree/main)
