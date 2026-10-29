// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 广告金充值记录条目（官方 <c>Res.bill_list</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>request_id</c> 原样回显请求值</b>（官方原文「调用接口 create_funds_bill 进行广告金充值时传入的 request_id 字段」）⇒ 可直接用作幂等核对键。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_funds_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayFundsBill
{
    /// <summary>bill_id（官方类型 <c>string</c>）：充值单 ID</summary>
    [JsonPropertyName("bill_id")]
    public string? BillId { get; set; }

    /// <summary>oper_time（官方类型 <c>number</c>）：充值时间，unix秒级时间戳</summary>
    [JsonPropertyName("oper_time")]
    public long? OperTime { get; set; }

    /// <summary>settle_begin（官方类型 <c>number</c>）：对应广告金结算周期开始时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_begin")]
    public long? SettleBegin { get; set; }

    /// <summary>settle_end（官方类型 <c>number</c>）：对应广告金结算周期结束时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_end")]
    public long? SettleEnd { get; set; }

    /// <summary>fund_id（官方类型 <c>string</c>）：对应广告金ID</summary>
    [JsonPropertyName("fund_id")]
    public string? FundId { get; set; }

    /// <summary>transfer_account_name（官方类型 <c>string</c>）：充值账户</summary>
    [JsonPropertyName("transfer_account_name")]
    public string? TransferAccountName { get; set; }

    /// <summary>transfer_account_uid（官方类型 <c>number</c>）：充值账户UID</summary>
    [JsonPropertyName("transfer_account_uid")]
    public long? TransferAccountUid { get; set; }

    /// <summary>transfer_amount（官方类型 <c>number</c>）：充值金额，单位：分</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：广告金充值状态：0-充值中，1-充值成功，2-充值失败</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>request_id（官方类型 <c>string</c>）：调用接口 create_funds_bill 进行广告金充值时传入的 request_id 字段</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}
