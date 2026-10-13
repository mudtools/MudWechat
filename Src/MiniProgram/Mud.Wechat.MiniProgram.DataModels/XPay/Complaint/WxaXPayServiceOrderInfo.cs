// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 投诉单关联服务单信息（官方子表）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayServiceOrderInfo
{
    /// <summary>order_id（官方类型 <c>string</c>）：微信支付服务订单号，每个微信支付服务订单号与商户号下对应的商户服务订单号一一对应</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>out_order_no（官方类型 <c>string</c>）：商户系统内部服务订单号（不是交易单号），与创建订单时一致</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>state（官方类型 <c>string</c>）：此处上传的是用户发起投诉时的服务单状态，不会实时更新。DOING: 服务订单进行中；REVOKED: 服务订单已取消；WAITPAY: 服务订单待支付；DONE: 服务订单已完成</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
