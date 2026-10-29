// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商商品明细数据（官方 <c>spu_data_list</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveSpuData
{
    /// <summary>获取或设置商品基础信息（官方 <c>base_data</c>）。</summary>
    [JsonPropertyName("base_data")]
    public ChannelsLiveSpuBaseData? BaseData { get; set; }

    /// <summary>获取或设置商品曝光人数（官方 <c>exp_uv</c>）。</summary>
    [JsonPropertyName("exp_uv")]
    public long? ExpUv { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>clk_uv</c>）。</summary>
    [JsonPropertyName("clk_uv")]
    public long? ClkUv { get; set; }

    /// <summary>获取或设置商品分享人数（官方 <c>share_uv</c>）。</summary>
    [JsonPropertyName("share_uv")]
    public long? ShareUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>clk_pv</c>）。</summary>
    [JsonPropertyName("clk_pv")]
    public long? ClkPv { get; set; }

    /// <summary>获取或设置商品曝光点击率（官方 <c>exp_clk_ratio</c>）。</summary>
    [JsonPropertyName("exp_clk_ratio")]
    public double? ExpClkRatio { get; set; }

    /// <summary>获取或设置商品点击成交率（官方 <c>clk_pay_ratio</c>）。</summary>
    [JsonPropertyName("clk_pay_ratio")]
    public double? ClkPayRatio { get; set; }

    /// <summary>获取或设置商品成交金额（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置商品成交订单数（官方 <c>pay_pv</c>）。</summary>
    [JsonPropertyName("pay_pv")]
    public long? PayPv { get; set; }

    /// <summary>获取或设置商品退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>获取或设置商品退款订单数（官方 <c>refund_pv</c>）。</summary>
    [JsonPropertyName("refund_pv")]
    public long? RefundPv { get; set; }
}
