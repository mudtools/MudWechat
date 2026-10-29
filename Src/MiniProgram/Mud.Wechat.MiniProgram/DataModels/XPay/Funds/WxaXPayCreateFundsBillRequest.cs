// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「充值广告金」请求体（<c>POST /xpay/create_funds_bill</c>）：把已发放广告金充进绑定账户。
/// </summary>
/// <remarks>
/// <para><b>十支全部必填</b>，且 <c>request_id</c> 是幂等键（官方原文「相同 id 的不同请求视为重复请求」，上限 1024 字符）⇒ 重复请求返回 <c>268490004</c>，须按「已生效」处理。</para>
/// <para><b>充值定位的是「发放记录」而非余额</b>：<c>settle_begin</c>/<c>settle_end</c> 必须落在某条广告金发放的结算周期内（可由 <c>IWxaXPayFundsService.QueryAdverFundsAsync</c> 取得）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_create_funds_bill.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCreateFundsBillRequest
{
    /// <summary>transfer_amount（官方类型 <c>number</c>，<b>必填</b>）：充值金额，单位分</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>transfer_account_uid（官方类型 <c>number</c>，<b>必填</b>）：充值账户 uid</summary>
    [JsonPropertyName("transfer_account_uid")]
    public long? TransferAccountUid { get; set; }

    /// <summary>transfer_account_name（官方类型 <c>string</c>，<b>必填</b>）：充值账户名称</summary>
    [JsonPropertyName("transfer_account_name")]
    public string? TransferAccountName { get; set; }

    /// <summary>transfer_account_agency_id（官方类型 <c>number</c>，<b>必填</b>）：充值账户服务商账号 id</summary>
    [JsonPropertyName("transfer_account_agency_id")]
    public long? TransferAccountAgencyId { get; set; }

    /// <summary>request_id（官方类型 <c>string</c>，<b>必填</b>）：用户定义每一次请求的唯一 id，相同 id 的不同请求视为重复请求(不超过 1024 个字符)</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>settle_begin（官方类型 <c>number</c>，<b>必填</b>）：充值所使用的广告金对应的结算周期开始时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_begin")]
    public long? SettleBegin { get; set; }

    /// <summary>settle_end（官方类型 <c>number</c>，<b>必填</b>）：充值所使用的广告金对应的结算周期结束时间，unix秒级时间戳</summary>
    [JsonPropertyName("settle_end")]
    public long? SettleEnd { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>authorize_advertise（官方类型 <c>number</c>，<b>必填</b>）：是否授权广告数据, 0:否，1:是</summary>
    [JsonPropertyName("authorize_advertise")]
    public long? AuthorizeAdvertise { get; set; }

    /// <summary>fund_type（官方类型 <c>number</c>，<b>必填</b>）：广告金发放原因， 0:通用赠送，1:广告激励，2:定向激励</summary>
    [JsonPropertyName("fund_type")]
    public long? FundType { get; set; }
}
