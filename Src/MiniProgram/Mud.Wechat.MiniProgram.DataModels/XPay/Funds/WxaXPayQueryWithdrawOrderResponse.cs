// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询提现单」响应：提现单全貌。
/// </summary>
/// <remarks>
/// <para><b>三支时间/金额字段官方类型为 <c>string</c></b>（<c>withdraw_amount</c> 元、<c>withdraw_success_timestamp</c> 秒级时间戳、<c>create_time</c> 创建时间）⇒ 照官方类型落字符串，<b>不得</b>「顺手」改成数字/时间类型。</para>
/// <para><b>提现状态取值集 1/2/3 且从 1 开始</b>（1 创建成功提现中 / 2 成功 / 3 失败，官方写在说明列、无枚举表）⇒ <c>fail_reason</c> 仅在 3 时有意义。</para>
/// <para><b>敏感值</b>：提现金额与商户单号 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_withdraw_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryWithdrawOrderResponse : WxaResponse
{
    /// <summary>withdraw_no（官方类型 <c>string</c>）：提现单号</summary>
    [JsonPropertyName("withdraw_no")]
    public string? WithdrawNo { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：提现状态，1-创建成功，提现中 2-提现成功 3-提现失败</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>withdraw_amount（官方类型 <c>string</c>）：提现金额</summary>
    [JsonPropertyName("withdraw_amount")]
    public string? WithdrawAmount { get; set; }

    /// <summary>wx_withdraw_no（官方类型 <c>string</c>）：提现单的微信侧单号</summary>
    [JsonPropertyName("wx_withdraw_no")]
    public string? WxWithdrawNo { get; set; }

    /// <summary>withdraw_success_timestamp（官方类型 <c>string</c>）：提现单成功的秒级时间戳</summary>
    [JsonPropertyName("withdraw_success_timestamp")]
    public string? WithdrawSuccessTimestamp { get; set; }

    /// <summary>create_time（官方类型 <c>string</c>）：提现单创建时间</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>fail_reason（官方类型 <c>string</c>）：提现失败的原因</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}
