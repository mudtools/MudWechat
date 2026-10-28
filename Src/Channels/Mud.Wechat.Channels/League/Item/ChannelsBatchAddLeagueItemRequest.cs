// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>批量新增联盟商品（<c>item/batchadd</c>）请求体。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：请求体参数表字段为 <c>talent_appids</c>，代码示例写作 <c>promoter_ids</c>
/// ——SDK 按参数表建模（官方变更日志 2026-06-17：删除 finder_ids/promoter_ids/fail_promoter_ids/
/// fail_finder_ids/info_id，新增 talent_appids/promotion_plan_id/fail_talent_appids）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddLeagueItemRequest
{
    /// <summary>获取或设置获取商品推广类别（官方 <c>type</c>，必填，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>list</c>，必填；<c>product_id</c> 不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueProductRatio> List { get; set; } = new();

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置带货者 appid 列表（官方 <c>talent_appids</c>，选填）。</summary>
    [JsonPropertyName("talent_appids")]
    public List<string>? TalentAppIds { get; set; }
}
