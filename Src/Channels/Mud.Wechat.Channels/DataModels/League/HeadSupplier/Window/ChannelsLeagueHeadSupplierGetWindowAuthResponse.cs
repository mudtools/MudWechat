// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取带货达人橱窗授权链接响应（官方 <c>headsupplier/windowauth/get</c> 返回体）。
/// <c>openfinderid</c> / <c>opentalentid</c> 在参数表位于返回体根、代码示例只回 <c>opentalentid</c> ⇒ 以参数表为权威。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetWindowAuthResponse
{
    /// <summary>获取或设置授权链接信息（官方 <c>auth_info</c>）。</summary>
    [JsonPropertyName("auth_info")]
    public ChannelsLeagueHeadSupplierWindowAuthInfo? AuthInfo { get; set; }

    /// <summary>获取或设置视频号 openfinderid（官方 <c>openfinderid</c>）。</summary>
    [JsonPropertyName("openfinderid")]
    public string? OpenFinderId { get; set; }

    /// <summary>获取或设置达人绑定的带货助手平台 opentalentid（官方 <c>opentalentid</c>）。</summary>
    [JsonPropertyName("opentalentid")]
    public string? OpenTalentId { get; set; }
}
