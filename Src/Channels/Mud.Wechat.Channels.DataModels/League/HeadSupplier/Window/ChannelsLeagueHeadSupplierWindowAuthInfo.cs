// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>橱窗授权链接信息（官方 <c>auth_info</c>，<c>headsupplier/windowauth/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierWindowAuthInfo
{
    /// <summary>获取或设置授权链接（官方 <c>auth_url</c>）。</summary>
    [JsonPropertyName("auth_url")]
    public string? AuthUrl { get; set; }

    /// <summary>获取或设置小程序 name（官方 <c>auth_wxa_username</c>）。</summary>
    [JsonPropertyName("auth_wxa_username")]
    public string? AuthWxaUsername { get; set; }

    /// <summary>获取或设置小程序 appid（官方 <c>auth_wxa_appid</c>）。</summary>
    [JsonPropertyName("auth_wxa_appid")]
    public string? AuthWxaAppid { get; set; }

    /// <summary>获取或设置授权路径（官方 <c>auth_wxa_path</c>）。</summary>
    [JsonPropertyName("auth_wxa_path")]
    public string? AuthWxaPath { get; set; }
}
