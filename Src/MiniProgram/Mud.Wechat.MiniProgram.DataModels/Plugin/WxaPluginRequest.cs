// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Plugin;

/// <summary>插件管理请求体（<c>POST /wxa/plugin</c>，插件使用方视角）。</summary>
/// <remarks>
/// <para>官方文档：<c>plugin-management/api_manageplugin.html</c>。</para>
/// <para>
/// <c>action</c>（必填）：<c>apply</c>（申请插件，配合 <see cref="PluginAppid"/> 与 <see cref="Reason"/>）、
/// <c>list</c>（查询已添加插件）、<c>update</c>（更新插件版本，配合 <see cref="PluginAppid"/> 与 <see cref="Version"/>）、
/// <c>delete</c>（删除插件，配合 <see cref="PluginAppid"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaPluginRequest
{
    /// <summary>操作类型（<c>action</c>，必填）：<c>apply</c> / <c>list</c> / <c>update</c> / <c>delete</c>。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>插件 appid（<c>plugin_appid</c>；<c>apply</c> / <c>update</c> / <c>delete</c> 时必填）。</summary>
    [JsonPropertyName("plugin_appid")]
    public string? PluginAppid { get; set; }

    /// <summary>插件版本号（<c>version</c>；<c>update</c> 时必填）。</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>申请理由（<c>reason</c>；<c>apply</c> 时必填）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
