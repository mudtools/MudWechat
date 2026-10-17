// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Funds;

/// <summary>
/// <c>POST /v3.0/fund_transfer/add</c> 的请求体（2026-10-17 官方页逐表核验，10 键）。
/// </summary>
/// <remarks>
/// 必填：<c>account_id</c> / <c>fund_type</c> / <c>amount</c> / <c>transfer_type</c>。
/// 金额区间、转账类型与财务自理标签的搭配等约束全部依赖账户状态（SDK 不可见）⇒ 不做本地拦截，
/// 越界由应答 <c>code</c> 表达（明细写进 <c>IWechatAdsFundsService.AddTransferAsync</c> 的 XML）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundTransferAddRequest
{
    /// <summary>推广帐号 id（官方 <c>account_id</c>，必填，<c>integer</c>；子客账户 id 或二代服务商 id）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>资金账户类型（官方 <c>fund_type</c>，必填，<c>enum</c>；可选值
    /// <c>{ FUND_TYPE_CASH, FUND_TYPE_GIFT, FUND_TYPE_SHARED, FUND_TYPE_BANK, FUND_TYPE_CREDIT_ROLL,
    /// FUND_TYPE_CREDIT_TEMPORARY, FUND_TYPE_COMPENSATE_VIRTUAL, FUND_TYPE_INTERNAL_QUOTA,
    /// FUND_TYPE_TEST_VIRTUAL }</c>，2026-10-17 逐字核验）。</summary>
    [JsonPropertyName("fund_type")]
    public string? FundType { get; set; }

    /// <summary>转账金额（官方 <c>amount</c>，必填，<c>integer</c>，单位为分；最小 5,000、最大 2,000,000,000）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>转账类型（官方 <c>transfer_type</c>，必填，<c>string</c>；可选值
    /// <c>{ AGENCY_TO_ADVERTISER, ADVERTISER_TO_AGENCY, AGENCY_TO_SUB_AGENCY, SUB_AGENCY_TO_AGENCY,
    /// AGENCY_TO_SUB_ADVERTISER, SUB_ADVERTISER_TO_AGENCY }</c>；每种取值对二代服务商「财务自理标签」
    /// 有配套要求，原文见 <c>IWechatAdsFundsService.AddTransferAsync</c> 的 remarks）。</summary>
    [JsonPropertyName("transfer_type")]
    public string? TransferType { get; set; }

    /// <summary>调用方订单号（官方 <c>external_bill_no</c>；10–35 字节；需带调用方标识前缀、
    /// <b>同一客户下唯一</b>、<b>不支持大写字母</b> —— 具幂等语义，重复提交返回 <c>is_repeated == true</c>）。</summary>
    [JsonPropertyName("external_bill_no")]
    public string? ExternalBillNo { get; set; }

    /// <summary>备注信息（官方 <c>memo</c>；1–64 字节）。</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    /// <summary>尽可能转账（官方 <c>transfer_try_best</c>，<c>integer</c> 0/1；仅对
    /// <c>ADVERTISER_TO_AGENCY</c> / <c>SUB_AGENCY_TO_AGENCY</c> / <c>SUB_ADVERTISER_TO_AGENCY</c>
    /// 生效；不传默认按尽可能转账处理）。</summary>
    [JsonPropertyName("transfer_try_best")]
    public long? TransferTryBest { get; set; }

    /// <summary>高频转账（官方 <c>high_frequency_transfer</c>，<c>integer</c> 0/1；仅对
    /// <c>AGENCY_TO_ADVERTISER</c> 生效；不传默认 0）。</summary>
    [JsonPropertyName("high_frequency_transfer")]
    public long? HighFrequencyTransfer { get; set; }

    /// <summary>获取可转账金额（官方 <c>pre_fetch_amount</c>，<c>integer</c> 0/1；
    /// 1 = 取系统推荐最大可转账金额、<b>不进行转账</b>；0 = 直接转账）。</summary>
    [JsonPropertyName("pre_fetch_amount")]
    public long? PreFetchAmount { get; set; }

    /// <summary>转账前提校验（官方 <c>pre_check</c>，<c>integer</c> 0/1；
    /// 1 = 提前校验能否转账成功、<b>不进行实际转账</b>；0 = 不做校验直接转账）。</summary>
    [JsonPropertyName("pre_check")]
    public long? PreCheck { get; set; }
}

/// <summary><c>fund_transfer/add</c> 的 <c>data</c> 载荷（7 字段，2026-10-17 逐表核验）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundTransferData
{
    /// <summary>资金账户类型（官方 <c>fund_type</c>，<c>enum</c>；可选值同请求）。</summary>
    [JsonPropertyName("fund_type")]
    public string? FundType { get; set; }

    /// <summary>金额（官方 <c>amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>调用方订单号（官方 <c>external_bill_no</c>；语义同请求）。</summary>
    [JsonPropertyName("external_bill_no")]
    public string? ExternalBillNo { get; set; }

    /// <summary>交易时间戳（官方 <c>time</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>是否重复转账（官方 <c>is_repeated</c>，<c>boolean</c>；true = 同一
    /// <c>external_bill_no</c> 已转过，本次未重复执行）。</summary>
    [JsonPropertyName("is_repeated")]
    public bool? IsRepeated { get; set; }

    /// <summary>系统推荐最大可转账金额（官方 <c>recommend_amount</c>，<c>integer</c>，单位为分；
    /// 与 <c>pre_fetch_amount = 1</c> 配套出现）。</summary>
    [JsonPropertyName("recommend_amount")]
    public long? RecommendAmount { get; set; }

    /// <summary>预校验结果（官方 <c>check_result</c>，<c>struct</c>；与 <c>pre_check = 1</c> 配套出现）。</summary>
    [JsonPropertyName("check_result")]
    public AdsFundTransferCheckResult? CheckResult { get; set; }
}

/// <summary>转账预校验结果（官方 <c>check_result</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundTransferCheckResult
{
    /// <summary>预校验是否通过（官方 <c>pass</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("pass")]
    public bool? Pass { get; set; }

    /// <summary>预校验不通过明细集（官方 <c>errors</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("errors")]
    public List<AdsFundTransferCheckError>? Errors { get; set; }
}

/// <summary>
/// 转账预校验不通过条目（官方 <c>check_result.errors[]</c> 元素）。
/// </summary>
/// <remarks>
/// <b><c>messageCn</c> 是官方原文的 camelCase 拼写</b>（本域其余字段皆 <c>snake_case</c>，
/// 唯此键例外）⇒ <c>[JsonPropertyName]</c> 照抄官方原文，勿「顺手修正」为 <c>message_cn</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundTransferCheckError
{
    /// <summary>预校验不通过错误码（官方 <c>code</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("code")]
    public long? Code { get; set; }

    /// <summary>预校验不通过错误描述（官方 <c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>预校验不通过错误描述（中文）（官方 <c>messageCn</c>，camelCase 照原文）。
    /// 注意与外层信封的 <c>message_cn</c>（snake_case）拼写不同 —— 官方两处形态不一致。</summary>
    [JsonPropertyName("messageCn")]
    public string? MessageCn { get; set; }
}

/// <summary><c>POST /v3.0/fund_transfer/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsFundTransferAddResponse : AdsResponse<AdsFundTransferData>
{
}
