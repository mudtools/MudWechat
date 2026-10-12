// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送预下单响应（<c>POST /cgi-bin/express/local/business/order/pre_add</c>）。</summary>
/// <remarks>
/// <b>运力差异（官方原文）</b>：达达支持返回配送费用 / 距离 / 预计接单时间与 <c>delivery_token</c>（有效期 3 分钟）；
/// 顺丰同城与闪送不返回 <c>delivery_token</c>；美团配送返回 <c>0</c> 仅表示校验通过。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalPreAddOrderResponse : WxaExpressLocalResponse
{
    /// <summary>实际运费（<c>fee</c>，单位：元；运费减去优惠券费用）。</summary>
    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }

    /// <summary>运费（<c>deliverfee</c>，单位：元）。</summary>
    [JsonPropertyName("deliverfee")]
    public decimal DeliverFee { get; set; }

    /// <summary>优惠券费用（<c>couponFee</c>，单位：元；官方本页原文驼峰拼写）。</summary>
    [JsonPropertyName("couponFee")]
    public decimal CouponFee { get; set; }

    /// <summary>小费（<c>tips</c>，单位：元）。</summary>
    [JsonPropertyName("tips")]
    public decimal Tips { get; set; }

    /// <summary>保价费（<c>insurancefee</c>，单位：元）。</summary>
    [JsonPropertyName("insurancefee")]
    public decimal InsuranceFee { get; set; }

    /// <summary>配送距离（<c>distance</c>，单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>运费锁定令牌（<c>delivery_token</c>；配送公司可返回，下单时带上可保证一段时间内运费不变）。</summary>
    [JsonPropertyName("delivery_token")]
    public string? DeliveryToken { get; set; }

    /// <summary>预计骑手接单时间（<c>dispatch_duration</c>，单位：秒；无法预计填 <c>0</c>）。</summary>
    [JsonPropertyName("dispatch_duration")]
    public int DispatchDuration { get; set; }
}
