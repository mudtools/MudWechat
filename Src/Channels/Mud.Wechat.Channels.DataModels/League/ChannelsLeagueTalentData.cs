// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>带货者数据信息（官方 <c>data</c> 对象，<c>promoter/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueTalentData
{
    /// <summary>获取或设置累计销售额（官方 <c>total_sale_gmv</c>）。</summary>
    [JsonPropertyName("total_sale_gmv")]
    public long? TotalSaleGmv { get; set; }

    /// <summary>获取或设置近 30 日销售额（官方 <c>sale_gmv_30d</c>）。</summary>
    [JsonPropertyName("sale_gmv_30d")]
    public long? SaleGmv30d { get; set; }

    /// <summary>获取或设置累计订单数（官方 <c>total_order_num</c>）。</summary>
    [JsonPropertyName("total_order_num")]
    public long? TotalOrderNum { get; set; }

    /// <summary>获取或设置近 30 日成交订单数（官方 <c>order_num_30d</c>）。</summary>
    [JsonPropertyName("order_num_30d")]
    public long? OrderNum30d { get; set; }

    /// <summary>获取或设置近 30 日成交商品数（官方 <c>sale_product_num_30d</c>）。</summary>
    [JsonPropertyName("sale_product_num_30d")]
    public int? SaleProductNum30d { get; set; }

    /// <summary>获取或设置橱窗商品数（官方 <c>window_product_num</c>）。</summary>
    [JsonPropertyName("window_product_num")]
    public int? WindowProductNum { get; set; }
}
