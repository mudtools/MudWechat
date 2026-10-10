// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Plugin;

/// <summary>插件申请管理请求体（<c>POST /wxa/devplugin</c>，插件开发者视角）。</summary>
/// <remarks>
/// <para>官方文档：<c>plugin-management/api_managepluginapplication.html</c>。</para>
/// <para>
/// <c>action</c>（必填）：<c>dev_apply_list</c>（分页获取申请列表，配合 <see cref="Page"/>/<see cref="Num"/>）、
/// <c>dev_agree</c> / <c>dev_refuse</c>（修改申请状态，配合 <see cref="UserName"/> 定位申请方）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaDevPluginRequest
{
    /// <summary>操作类型（<c>action</c>，必填）：<c>dev_apply_list</c> / <c>dev_agree</c> / <c>dev_refuse</c>。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>页码（<c>page</c>，<c>dev_apply_list</c> 时必填，从 <c>0</c> 开始）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页条数（<c>num</c>，<c>dev_apply_list</c> 时必填）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }

    /// <summary>插件使用方 appid（<c>user_name</c>，<c>dev_agree</c> / <c>dev_refuse</c> 时必填）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
}

/// <summary>插件使用方申请信息（<c>apply_list[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaPluginApplyItem
{
    /// <summary>插件使用方 appid（<c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>申请方头像（<c>avatar_url</c>）。</summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>申请方昵称（<c>nick_name</c>）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>申请方类目（<c>categories</c>）。</summary>
    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }
}

/// <summary>插件申请管理应答（<c>apply_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaDevPluginResponse : WxaResponse
{
    /// <summary>插件使用方申请列表（<c>apply_list</c>），见 <see cref="WxaPluginApplyItem"/>。</summary>
    [JsonPropertyName("apply_list")]
    public List<WxaPluginApplyItem>? ApplyList { get; set; }
}

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

/// <summary>已添加插件信息（<c>plugin_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaPluginItem
{
    /// <summary>插件 appid（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>插件昵称（<c>nick_name</c>）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }
}

/// <summary>插件管理应答（<c>plugin_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Plugin")]
public class WxaPluginResponse : WxaResponse
{
    /// <summary>已添加的插件列表（<c>plugin_list</c>），见 <see cref="WxaPluginItem"/>。</summary>
    [JsonPropertyName("plugin_list")]
    public List<WxaPluginItem>? PluginList { get; set; }
}