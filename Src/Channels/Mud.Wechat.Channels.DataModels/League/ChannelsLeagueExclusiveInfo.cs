// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟特殊推广信息·响应形态（官方 <c>exclusive_info</c> 对象，<c>item/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueExclusiveInfo
{
    /// <summary>获取或设置推广计划 id（官方 <c>promotion_plan_id</c>）。</summary>
    [JsonPropertyName("promotion_plan_id")]
    public long? PromotionPlanId { get; set; }

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }

    /// <summary>获取或设置推广带货者数量（官方 <c>talent_num</c>）。</summary>
    [JsonPropertyName("talent_num")]
    public int? TalentNum { get; set; }
}
