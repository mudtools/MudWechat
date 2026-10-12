// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>电商数据概览（官方 <c>data</c> 对象，<c>getshopoverall</c>，官方类型 string）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_gmv")]
    public string? PayGmv { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_uv")]
    public string? PayUv { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_order_cnt</c>，官方类型 string）。</summary>
    [JsonPropertyName("pay_order_cnt")]
    public string? PayOrderCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public string? PayRefundGmv { get; set; }

    /// <summary>获取或设置直播成交金额（官方 <c>live_pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("live_pay_gmv")]
    public string? LivePayGmv { get; set; }

    /// <summary>获取或设置短视频成交金额（官方 <c>feed_pay_gmv</c>，单位：分，官方类型 string）。</summary>
    [JsonPropertyName("feed_pay_gmv")]
    public string? FeedPayGmv { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>，官方类型 string）。</summary>
    [JsonPropertyName("product_click_uv")]
    public string? ProductClickUv { get; set; }
}
