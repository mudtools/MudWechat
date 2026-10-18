// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PayScore;

/// <summary>
/// 「下发服务费用待处理消息」与「下发服务预扣费消息」的共用请求体
/// （<b>POST /v3/payscore/serviceorder/{out_order_no}/send-pendingmessage</b> 与
/// <b>…&#47;send-prepaidmessage</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013394606"/>（待处理消息）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013394615"/>（预扣费消息），2026-10-18 逐页比对：
/// 两页 <c>path</c> + <c>body</c> 参数表<b>逐项一致</b> ⇒ 按「表相同则共用」合并为一个类型。</para>
/// <para><b><c>prepaid_time</c> 只有 8 位</b>：官方 <c>string(8)</c> + <c>YYYYMMDD</c>（精确到日），
/// 且须<b>不早于调接口时间 + 7 天</b>；这与 <c>time_range</c> 的 14 位格式是另一种时间形态。</para>
/// <para><b>两支端点应答均为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreMessageRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：该服务 ID 须有本接口对应产品的权限。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b>，最多 100 条），见 <see cref="PayScorePostPayment"/>。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条），见 <see cref="PayScorePostDiscount"/>。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>预计扣费时间（<c>prepaid_time</c>，必填 <b>string(8)</b>，<c>YYYYMMDD</c>）：须 ≥ 调接口时间 + 7 天。</summary>
    [JsonPropertyName("prepaid_time")]
    public string? PrepaidTime { get; set; }
}
