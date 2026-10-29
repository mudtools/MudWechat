// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商家合作统计数据（官方 <c>shop_detail.data_info</c>，<c>headsupplier/shop/get</c>）。</summary>
/// <remarks>
/// 官方 <c>settle_amount</c> 的说明原文误写为「历史合作商品数」，与其字段名和示例（金额）不符 ⇒
/// 本仓<b>照录官方原文</b>并在此标注，不代为「修正」语义。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierShopDataInfo
{
    /// <summary>获取或设置合作动销 GMV（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置历史合作商品数（官方 <c>product_number</c>）。</summary>
    [JsonPropertyName("product_number")]
    public long? ProductNumber { get; set; }

    /// <summary>获取或设置已结算金额（官方 <c>settle_amount</c>；官方说明原文写「历史合作商品数」，属文档笔误）。</summary>
    [JsonPropertyName("settle_amount")]
    public long? SettleAmount { get; set; }

    /// <summary>获取或设置预计待结算服务费（官方 <c>unsettle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("unsettle_amount")]
    public long? UnsettleAmount { get; set; }

    /// <summary>获取或设置今日新增合作商品数（官方 <c>product_number_today</c>）。</summary>
    [JsonPropertyName("product_number_today")]
    public long? ProductNumberToday { get; set; }

    /// <summary>获取或设置今日动销商品数（官方 <c>product_number_sold_today</c>）。</summary>
    [JsonPropertyName("product_number_sold_today")]
    public long? ProductNumberSoldToday { get; set; }
}
