// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 交易组件侧退款记录（官方 <c>RefundRecord</c> 表）。
/// </summary>
/// <remarks>
/// <para><b>与合单支付侧 <c>WxaShopCombineRefundRecord</c> 同名不同型</b>：本表多一支 <c>type</c>（退款类型，1 售后退款）、
/// 时间字段是 <c>string</c>；对方无 type、时间是秒级时间戳 ⇒ 不得合并（守卫 SO5 锁定）。</para>
/// <para><b>官方结果枚举原文含一对不配对的值</b>：<c>refund_result</c> 表给 SUCCESS 退款成功 / <b>CLOSE</b> 退款关闭 /
/// PROCESSING 处理中 / ABNORMAL 异常（官方此处写 CLOSE 而非 CLOSED，与分账结果的 CLOSED 不同）⇒ 字符串照抄比较，不得自行归一。</para>
/// <para><b>官方两处说明串列</b>：本表 <c>create_time</c> 说明印成「分账创建时间」、<c>fail_reason</c> 印成「分账失败原因」
/// （退款表复用了分账表的说明列）⇒ 语义按退款理解，字面留档。</para>
/// <para><b>敏感值</b>：<c>refund_id</c>（微信支付系统退款单号）不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopRefundRecord
{
    /// <summary>type（官方类型 <c>number</c>）：退款类型，1 售后退款。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：金额，单位分。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>refund_no（官方类型 <c>string</c>）：视频号系统退款单号。</summary>
    [JsonPropertyName("refund_no")]
    public string? RefundNo { get; set; }

    /// <summary>refund_id（官方类型 <c>string</c>）：微信支付系统退款单号（敏感，不得进日志）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>create_time（官方类型 <c>string</c>）：退款申请时间（官方说明列误印「分账创建时间」）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>finish_time（官方类型 <c>string</c>）：退款完成时间（官方说明列误印「分账完成时间」）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>result（官方类型 <c>string</c>）：退款结果，SUCCESS / CLOSE / PROCESSING / ABNORMAL。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>fail_reason（官方类型 <c>string</c>）：退款失败原因。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}
