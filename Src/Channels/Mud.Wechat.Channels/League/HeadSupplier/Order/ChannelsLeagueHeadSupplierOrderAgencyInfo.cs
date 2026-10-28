// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构分佣信息（官方 <c>sub_agency_info</c> / <c>promoter_agency_info</c> / <c>head_agency_info</c> 三处字段集完全同形 ⇒ 一支类型复用）。
/// 官方三处说明均写「帮卖机构昵称」，属文档复制痕迹 ⇒ 照录不改。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderAgencyInfo
{
    /// <summary>获取或设置机构 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置机构昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置服务费率（官方 <c>ratio</c>，区间 <c>[0, 1000000]</c>）。</summary>
    [JsonPropertyName("ratio")]
    public long? Ratio { get; set; }

    /// <summary>获取或设置服务费（官方 <c>amount</c>，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置服务费结算时间（官方 <c>profit_sharding_suc_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("profit_sharding_suc_time")]
    public long? ProfitShardingSucTime { get; set; }

    /// <summary>获取或设置头像（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }
}
