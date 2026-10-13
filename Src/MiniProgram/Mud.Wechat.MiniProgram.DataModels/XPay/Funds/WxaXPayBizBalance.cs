// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 可提现余额（官方 <c>Res.balance_available</c>）。
/// </summary>
/// <remarks>
/// <para><b>金额是「元」为单位的字符串</b>（官方原文「单位元」）⇒ 与本域订单侧「分」为单位的数字相反，提现与营收对账须显式换算并固定精度。</para>
/// <para><b>敏感值</b>：商户账户余额 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_biz_balance.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayBizBalance
{
    /// <summary>amount（官方类型 <c>string</c>）：可提现余额，单位元</summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>currency_code（官方类型 <c>string</c>）：币种（一般为CNY）</summary>
    [JsonPropertyName("currency_code")]
    public string? CurrencyCode { get; set; }
}
