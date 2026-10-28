// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「特殊发货报备」请求体（<c>POST /wxa/sec/order/opspecialorder</c>，适用于预售商品订单与测试订单）。
/// </summary>
/// <remarks>
/// <para><b><c>order_id</c> 与录入侧的 <c>order_key</c> 不是同一形态</b>：本端点用<b>单个字符串</b>
/// 承载「微信支付单号或商户单号」，不再有 <c>order_number_type</c> 判别位。</para>
/// <para><b><c>delay_to</c> 条件必填</b>（官方原文「type为1时必填，type为2可省略」）：预售订单的预计发货时间
/// 必须在订单支付时间的 <b>15 天之后、330 天之内</b>。</para>
/// <para><b>测试订单报备后无需发货</b>（官方原文）：如需退回资金须前往商户号后台操作退款或按正常发货流程处理，
/// 本接口不触发退款。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_opspecialorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderSpecialReportRequest
{
    /// <summary>order_id（官方类型 <c>string</c>，必填）：需要特殊发货报备的订单号，可传入微信支付单号或商户单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>type（官方类型 <c>number</c>，必填）：特殊发货报备类型，1为预售商品订单，2为测试订单。取值见 <see cref="WxaOrderSpecialReportTypeValues"/></summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>delay_to（官方类型 <c>number</c>）：预计发货时间的unix时间戳，type为1时必填，type为2可省略</summary>
    [JsonPropertyName("delay_to")]
    public long? DelayTo { get; set; }
}
