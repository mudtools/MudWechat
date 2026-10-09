// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Refund;

/// <summary>
/// 发起异常退款请求体（微信支付 APIv3；退款的 <c>status = ABNORMAL</c> 时使用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791905"/>（2026-10-09 逐字段核验）。
/// <b>POST</b> <c>/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund</c>，支持商户类型：<b>普通商户</b>。
/// </para>
/// <para>
/// <b>路由以 <c>refund_id</c>（微信支付退款单号）为 path 参数</b>，<b>非</b>商户退款单号 ——
/// 与「查询单笔退款」以 <c>out_refund_no</c> 为 path 参数不同，勿混用。
/// </para>
/// <para>
/// <b>业务限制（官方原文要点）</b>：① 仅当退款单状态为 <c>ABNORMAL</c>（如用户账户异常、银行卡注销）时可用；
/// ② <c>type = USER_BANK_CARD</c> 时须由用户侧确认退款银行卡信息，<c>MERCHANT_BANK_CARD</c> 时退至商户指定银行卡；
/// ③ 该操作会<b>变更退款去向</b>，属不可逆操作。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class AbnormalRefundRequest
{
    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 异常退款类型（<c>type</c>，必填 string(32)）：<c>USER_BANK_CARD</c>（退至用户银行卡）/
    /// <c>MERCHANT_BANK_CARD</c>（退至商户指定银行卡）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>银行类型（<c>bank_type</c>，选填 string(32)），<c>type = MERCHANT_BANK_CARD</c> 时必填。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>银行卡号（<c>bank_account</c>，选填 string(64)），<c>type = MERCHANT_BANK_CARD</c> 时必填。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>持卡人姓名（<c>real_name</c>，选填 string(64)，<b>敏感字段</b>：请求侧经平台证书 RSA-OAEP 加密）。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }
}
