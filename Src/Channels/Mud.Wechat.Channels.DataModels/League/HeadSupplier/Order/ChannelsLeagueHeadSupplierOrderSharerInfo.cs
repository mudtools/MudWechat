// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>分佣信息中的推客分佣信息（官方 <c>commission_info.sharer_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderSharerInfo
{
    /// <summary>获取或设置推客 appid（官方 <c>sharer_appid</c>）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppid { get; set; }

    /// <summary>获取或设置推客昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置佣金率（官方 <c>ratio</c>，区间 <c>[0, 1000000]</c>）。</summary>
    [JsonPropertyName("ratio")]
    public long? Ratio { get; set; }

    /// <summary>获取或设置佣金（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置推客的 opensharerid（官方 <c>opensharerid</c>）。</summary>
    [JsonPropertyName("opensharerid")]
    public string? OpenSharerId { get; set; }

    /// <summary>获取或设置头像（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }
}
