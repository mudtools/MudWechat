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
