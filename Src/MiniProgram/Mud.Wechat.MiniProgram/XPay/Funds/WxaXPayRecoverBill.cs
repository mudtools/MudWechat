// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 广告金回收记录条目（官方 <c>Res.bill_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>同名不同形的共用禁止证据</b>：本表与 <see cref="WxaXPayFundsBill"/> 的官方子表都叫 <c>bill_list</c>，但字段集交集只有 <c>bill_id</c>/<c>settle_begin</c>/<c>settle_end</c>/<c>fund_id</c> ⇒ 分立建模，守卫 XP5 锁定。</para>
/// <para><b><c>refund_order_list</c> 官方类型为 <c>array</c> 且说明为「对应的退款订单 id」</b> ⇒ 线格式落 <c>List&lt;string&gt;?</c>（本域 <c>array</c> 默认字符串数组的口径）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_recover_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayRecoverBill
{
    /// <summary>bill_id（官方类型 <c>string</c>）：回收单 ID</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>recover_time（官方类型 <c>number</c>）：回收时间，unix秒级时间戳</summary>
    [JsonPropertyName("recover_time")]
    public long? RecoverTime { get; set; }

    /// <summary>settle_begin（官方类型 <c>number</c>）：结算周期开始时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_begin")]
    public long? SettleBegin { get; set; }

    /// <summary>settle_end（官方类型 <c>number</c>）：结算周期结束时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_end")]
    public long? SettleEnd { get; set; }

    /// <summary>fund_id（官方类型 <c>string</c>）：对应的发放广告金ID</summary>
    [JsonPropertyName("fund_id")]
    public string? FundId { get; set; }

    /// <summary>recover_account_name（官方类型 <c>string</c>）：回收广告金账户</summary>
    [JsonPropertyName("recover_account_name")]
    public string? RecoverAccountName { get; set; }

    /// <summary>recover_amount（官方类型 <c>number</c>）：回收金额，单位：分</summary>
    [JsonPropertyName("recover_amount")]
    public long? RecoverAmount { get; set; }

    /// <summary>refund_order_list（官方类型 <c>array</c>）：对应的退款订单 id</summary>
    [JsonPropertyName("refund_order_list")]
    public List<string>? RefundOrderList { get; set; }
}
