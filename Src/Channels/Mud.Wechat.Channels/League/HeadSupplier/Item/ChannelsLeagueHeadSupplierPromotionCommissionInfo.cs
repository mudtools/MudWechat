// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构商品推广详情中的佣金信息（官方 <c>item.commission_info</c> 对象）。</summary>
/// <remarks>
/// <para>官方约束：<c>commission_type</c> 为 <c>1</c>（机构指定达人佣金）时该计划<b>没有达人佣金率</b>，
/// 必须先创建子计划（<c>subitem/add</c>）指定达人佣金率，才能把商品添加到达人橱窗。</para>
/// <para>与商家侧优选联盟的 <c>ChannelsLeagueTalentCommissionInfo</c> / <c>ChannelsLeaguePromotionCommissionInfo</c>
/// <b>字段集不同</b>（本对象含 <c>plan_type</c> / <c>commission_type</c> / <c>service_ratio</c>）⇒ 分建 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierPromotionCommissionInfo
{
    /// <summary>获取或设置计划类型（官方 <c>plan_type</c>：1 定向计划 / 2 公开计划）。</summary>
    [JsonPropertyName("plan_type")]
    public int? PlanType { get; set; }

    /// <summary>获取或设置佣金分配类型（官方 <c>commission_type</c>：0 商家指定达人佣金 / 1 机构指定达人佣金）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }

    /// <summary>获取或设置佣金费率（官方 <c>ratio</c>，取值范围 <c>[0, 1000000]</c>，<c>1000000</c> 表示 100%）。</summary>
    [JsonPropertyName("ratio")]
    public long? Ratio { get; set; }

    /// <summary>获取或设置服务费率（官方 <c>service_ratio</c>，取值范围 <c>[0, 1000000]</c>）。</summary>
    [JsonPropertyName("service_ratio")]
    public long? ServiceRatio { get; set; }

    /// <summary>获取或设置计划开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置计划结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置普通推广下的佣金信息（官方 <c>normal_commission_info</c>；机构商品上架中且普通推广上架中才有效）。</summary>
    [JsonPropertyName("normal_commission_info")]
    public ChannelsLeagueHeadSupplierNormalCommissionInfo? NormalCommissionInfo { get; set; }
}
