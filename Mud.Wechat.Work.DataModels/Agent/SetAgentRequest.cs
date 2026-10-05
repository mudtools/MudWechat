// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 设置应用请求体（<c>/cgi-bin/agent/set</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方权限口径：仅企业可调用，可设置当前凭证对应的应用；第三方以及代开发自建应用不可调用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class SetAgentRequest
{
    /// <summary>获取或设置企业应用的 id（官方必填）。</summary>
    [JsonPropertyName("agentid")]
    public int Agentid { get; set; }

    /// <summary>获取或设置是否打开地理位置上报：0 - 不上报；1 - 进入会话上报。</summary>
    [JsonPropertyName("report_location_flag")]
    public int? ReportLocationFlag { get; set; }

    /// <summary>获取或设置应用头像的 mediaid（通过素材管理接口上传图片获得）。</summary>
    [JsonPropertyName("logo_mediaid")]
    public string? LogoMediaid { get; set; }

    /// <summary>获取或设置应用名称（不超过 32 个 utf8 字符）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置应用详情（4 至 120 个 utf8 字符）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置应用可信域名。
    /// <para>须通过域名所有权校验，否则 jssdk 受限（错误码 85005）。</para>
    /// </summary>
    [JsonPropertyName("redirect_domain")]
    public string? RedirectDomain { get; set; }

    /// <summary>获取或设置是否上报用户进入应用事件：0 - 不接收；1 - 接收。</summary>
    [JsonPropertyName("isreportenter")]
    public int? Isreportenter { get; set; }

    /// <summary>获取或设置应用主页 url（必须以 http 或 https 开头）。</summary>
    [JsonPropertyName("home_url")]
    public string? HomeUrl { get; set; }
}
