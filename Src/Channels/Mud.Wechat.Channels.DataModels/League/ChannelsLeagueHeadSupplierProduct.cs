// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟机构推广商品条目（官方 <c>list</c> 数组元素，<c>item/headsupplier/batchadd</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填，不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置推广佣金（官方 <c>ratio</c>，必填，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("ratio")]
    public int Ratio { get; set; }

    /// <summary>获取或设置推广服务费（官方 <c>service_ratio</c>，必填，取值 <c>[0, 90]</c>%）。</summary>
    [JsonPropertyName("service_ratio")]
    public int ServiceRatio { get; set; }

    /// <summary>获取或设置机构计划类型（官方 <c>commission_type</c>，选填，见 <see cref="ChannelsLeagueHeadSupplierPlanTypes"/>）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }
}
