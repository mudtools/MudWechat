// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 物流助手「商家寄件」下单请求体（<c>POST /cgi-bin/express/business/order/add</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_addorder"/>。</para>
/// <para>
/// <b>频率与前置</b>：调用前须已绑定快递账号且开通下单权限；官方建议单商户日下单量超 10 万时联系快递公司申请提额。
/// <c>add_source=2</c>（App/AppH5 订单）时不发送物流服务通知；<c>expect_time</c> 顺丰必传，
/// 0 表示已事先约定取件时间，否则传预期揽件时间戳（秒），需大于当前时间；选择预期揽件时间后<b>不要自己打单</b>。
/// 平台型小程序 <c>tagid</c> 须与入驻方账号一一对应。
/// </para>
/// <para>面单预览 <c>template/preview</c> 的 <c>custom</c> 字段与本请求体同构（官方原文：“格式是商户侧下单 addOrder 接口中的请求体”）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddOrderRequest
{
    /// <summary>订单 ID（<c>order_id</c>，必填；须保证全局唯一，不超过 512 字节）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>用户 OpenId（<c>openid</c>，必填；<c>add_source=2</c> 时无需填写）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填；参见 <c>delivery/getall</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递客户编码或现付编码（<c>biz_id</c>，必填；参见 <c>account/getall</c>）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递备注信息（<c>custom_remark</c>，可选；例如“易碎物品”，不超过 1024 字节）。</summary>
    [JsonPropertyName("custom_remark")]
    public string? CustomRemark { get; set; }

    /// <summary>订单标签 ID（<c>tagid</c>，可选；平台型小程序区分入驻方，须与入驻方账号一一对应）。</summary>
    [JsonPropertyName("tagid")]
    public int? TagId { get; set; }

    /// <summary>订单来源（<c>add_source</c>，必填；0 为小程序订单，2 为 App 或 H5 订单，填 2 则不发送物流服务通知）。</summary>
    [JsonPropertyName("add_source")]
    public int AddSource { get; set; }

    /// <summary>App 或 H5 的 AppId（<c>wx_appid</c>，可选；<c>add_source=2</c> 时必填，须与开通物流助手的小程序绑定同一开放账号）。</summary>
    [JsonPropertyName("wx_appid")]
    public string? WxAppId { get; set; }

    /// <summary>发件人信息（<c>sender</c>，必填），见 <see cref="WxaExpressContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>，必填），见 <see cref="WxaExpressContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressContact? Receiver { get; set; }

    /// <summary>包裹信息（<c>cargo</c>，必填；将传递给快递公司），见 <see cref="WxaExpressCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressCargo? Cargo { get; set; }

    /// <summary>商品信息（<c>shop</c>，必填；展示到物流服务通知和电子面单中），见 <see cref="WxaExpressShop"/>。</summary>
    [JsonPropertyName("shop")]
    public WxaExpressShop? Shop { get; set; }

    /// <summary>保价信息（<c>insured</c>，必填），见 <see cref="WxaExpressInsured"/>。</summary>
    [JsonPropertyName("insured")]
    public WxaExpressInsured? Insured { get; set; }

    /// <summary>服务类型（<c>service</c>，必填；顺丰必填，其它快递公司选填），见 <see cref="WxaExpressService"/>。</summary>
    [JsonPropertyName("service")]
    public WxaExpressService? Service { get; set; }

    /// <summary>预期的上门揽件时间（<c>expect_time</c>，可选；Unix 时间戳秒；0 表示已事先约定取件时间；顺丰必传）。</summary>
    [JsonPropertyName("expect_time")]
    public long? ExpectTime { get; set; }

    /// <summary>分单策略（<c>take_mode</c>，可选；0 线下网点签约，1 总部签约结算，不传默认线下网点签约；目前支持圆通）。</summary>
    [JsonPropertyName("take_mode")]
    public int? TakeMode { get; set; }
}
