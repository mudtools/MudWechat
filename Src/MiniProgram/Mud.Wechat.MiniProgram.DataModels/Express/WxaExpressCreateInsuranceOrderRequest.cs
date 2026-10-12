// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>发货时投保请求体（<c>POST /wxa/business/insurance_freight/createorder</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_createorder"/>（接口英文名 <c>insurance_freight_createorder</c>）。</para>
/// <para>
/// <b>官方约束</b>：<c>openid</c> 必须和理赔 <c>openid</c> 一致；一个微信支付单号<b>只能投保一次</b>；
/// <c>pay_time</c> 与微信支付时间误差须在 3 天内。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateInsuranceOrderRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，必填；必须和理赔 openid 一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，必填；一个微信支付单号只能投保一次）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>微信支付时间（<c>pay_time</c>，必填；秒级时间戳，与官方时间误差 3 天内）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>微信支付金额（<c>pay_amount</c>，必填；单位：分）。</summary>
    [JsonPropertyName("pay_amount")]
    public long PayAmount { get; set; }

    /// <summary>发货运单号（<c>delivery_no</c>，必填）。</summary>
    [JsonPropertyName("delivery_no")]
    public string? DeliveryNo { get; set; }

    /// <summary>发货地址（<c>delivery_place</c>，必填），见 <see cref="WxaExpressInsuranceFreightPlace"/>。</summary>
    [JsonPropertyName("delivery_place")]
    public WxaExpressInsuranceFreightPlace? DeliveryPlace { get; set; }

    /// <summary>收货地址（<c>receipt_place</c>，必填；结构与 <c>delivery_place</c> 相同），见 <see cref="WxaExpressInsuranceFreightPlace"/>。</summary>
    [JsonPropertyName("receipt_place")]
    public WxaExpressInsuranceFreightPlace? ReceiptPlace { get; set; }

    /// <summary>投保订单信息（<c>product_info</c>，必填），见 <see cref="WxaExpressInsuranceFreightProductInfo"/>。</summary>
    [JsonPropertyName("product_info")]
    public WxaExpressInsuranceFreightProductInfo? ProductInfo { get; set; }
}
