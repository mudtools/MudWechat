// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 带货者商品数据（官方 <c>product_data</c> 对象，<c>talent/getproductdatalist</c>）。
/// </summary>
/// <remarks>
/// <b>分建裁决</b>：与 <see cref="ChannelsLeagueTalentData"/> 字段集不同（无 <c>sale_product_num_30d</c>/
/// <c>window_product_num</c>），按端点分建、不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueProductData
{
    /// <summary>获取或设置销售额（官方 <c>total_sale_gmv</c>）。</summary>
    [JsonPropertyName("total_sale_gmv")]
    public long? TotalSaleGmv { get; set; }

    /// <summary>获取或设置 30 天销售额（官方 <c>sale_gmv_30d</c>）。</summary>
    [JsonPropertyName("sale_gmv_30d")]
    public long? SaleGmv30d { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>total_order_num</c>）。</summary>
    [JsonPropertyName("total_order_num")]
    public long? TotalOrderNum { get; set; }

    /// <summary>获取或设置 30 天成交订单数（官方 <c>order_num_30d</c>）。</summary>
    [JsonPropertyName("order_num_30d")]
    public long? OrderNum30d { get; set; }
}
