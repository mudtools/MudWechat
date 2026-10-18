// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantFund;

/// <summary>
/// 商户账户资金收入记录查询应答（<b>商户号维度</b>，<c>GET /v3/merchantfund/merchant/income-records</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012718647"/>，
/// 2026-10-18 逐字段核验，<b>支持商户：【普通服务商】</b>。</para>
/// <para><b>与服务商维度应答<b>不共用</b></b>：两页除 <c>data[]</c> 首字段不同
/// （本页 <c>mchid</c>、服务商页 <c>sub_mchid</c>）外逐字段一致，故按「表不同则分建」独立建模，
/// 仅 <see cref="MerchantFundIncomeRecordLinks"/> 分页链接表共用。</para>
/// <para><b>数据含<b>银行账号与户名</b></b>：<c>bank_account_number</c> / <c>bank_account_name</c> 属个人金融信息
/// ⇒ 宿主<b>不得</b>写入日志、遥测或异常消息，也不得原样回显到前端。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class MerchantFundIncomeRecordQueryResponse : WechatPayResponse
{
    /// <summary>记录总数（<c>total_count</c>，必填 integer）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>当前偏移量（<c>offset</c>，必填 integer）：与请求一致。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>本页条数上限（<c>limit</c>，必填 integer）：与请求一致。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>收入记录列表（<c>data</c>，选填），见 <see cref="MerchantFundIncomeRecord"/>。</summary>
    [JsonPropertyName("data")]
    public List<MerchantFundIncomeRecord>? Data { get; set; }

    /// <summary>分页链接（<c>links</c>，选填），见 <see cref="MerchantFundIncomeRecordLinks"/>。</summary>
    [JsonPropertyName("links")]
    public MerchantFundIncomeRecordLinks? Links { get; set; }
}

/// <summary>商户号维度的单条收入记录（<c>data[]</c>，<b>商户页</b>）。</summary>
/// <remarks>
/// <para><b><c>mchid</c> 是本页首字段</b>：服务商维度页同位置为 <c>sub_mchid</c> ⇒ 两类型分建、不得互换。</para>
/// <para><b><c>income_record_type</c> 决定业务含义</b>：取值见 <see cref="MerchantFundIncomeRecordTypes"/>
/// （<c>OFFLINERECHARGE</c> 线下充值 / <c>ENTERPRISEDIRECTREVENUE</c> 企业直连收入）
/// ⇒ 汇总时须按类型分组，不得把两种来源混为一个口径。</para>
/// <para><b>金额单位是分</b>：官方类型 integer，本 SDK 以 <c>long</c> 承载，避免大额记录在 int32 边界静默溢出。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class MerchantFundIncomeRecord
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>账户类型（<c>account_type</c>，必填 string(32)）：取值见 <see cref="MerchantFundAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>收入记录类型（<c>income_record_type</c>，必填 string(64)）：取值见 <see cref="MerchantFundIncomeRecordTypes"/>。</summary>
    [JsonPropertyName("income_record_type")]
    public string? IncomeRecordType { get; set; }

    /// <summary>收入记录单号（<c>income_record_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("income_record_id")]
    public string? IncomeRecordId { get; set; }

    /// <summary>收入金额（<c>amount</c>，必填 integer）：单位<b>分</b>，本 SDK 以 <c>long</c> 承载。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>入账成功时间（<c>success_time</c>，必填 string(32)，<b>rfc3339</b> 且官方要求北京时间 <c>+08:00</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>开户银行（<c>bank_name</c>，必填 string(256)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行户名（<c>bank_account_name</c>，必填 string(256)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_account_name")]
    public string? BankAccountName { get; set; }

    /// <summary>银行账号（<c>bank_account_number</c>，必填 string(64)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_account_number")]
    public string? BankAccountNumber { get; set; }

    /// <summary>充值备注（<c>recharge_remark</c>，必填 string(256)）。</summary>
    [JsonPropertyName("recharge_remark")]
    public string? RechargeRemark { get; set; }
}

/// <summary>
/// 服务商账户资金收入记录查询应答（<b>子商户维度</b>，<c>GET /v3/merchantfund/partner/income-records</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012718620"/>，
/// 2026-10-18 逐字段核验，<b>支持商户：【普通服务商】</b>。</para>
/// <para><b>与商户号维度应答的唯一差异是 <c>data[].sub_mchid</c></b>（其余字段、分页链接完全一致）
/// ⇒ 两应答类型分建，分页链接类型共用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class PartnerIncomeRecordQueryResponse : WechatPayResponse
{
    /// <summary>记录总数（<c>total_count</c>，必填 integer）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>当前偏移量（<c>offset</c>，必填 integer）：与请求一致。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>本页条数上限（<c>limit</c>，必填 integer）：与请求一致。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>收入记录列表（<c>data</c>，选填），见 <see cref="PartnerIncomeRecord"/>。</summary>
    [JsonPropertyName("data")]
    public List<PartnerIncomeRecord>? Data { get; set; }

    /// <summary>分页链接（<c>links</c>，选填），见 <see cref="MerchantFundIncomeRecordLinks"/>。</summary>
    [JsonPropertyName("links")]
    public MerchantFundIncomeRecordLinks? Links { get; set; }
}

/// <summary>子商户维度的单条收入记录（<c>data[]</c>，<b>服务商页</b>）。</summary>
/// <remarks>
/// <para><b><c>sub_mchid</c> 是本页首字段</b>：商户号维度页同位置为 <c>mchid</c> ⇒ 两类型分建、不得互换。</para>
/// <para><b>银行账号与户名属个人金融信息</b> ⇒ 不得进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class PartnerIncomeRecord
{
    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>账户类型（<c>account_type</c>，必填 string(32)）：取值见 <see cref="MerchantFundAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>收入记录类型（<c>income_record_type</c>，必填 string(64)）：取值见 <see cref="MerchantFundIncomeRecordTypes"/>。</summary>
    [JsonPropertyName("income_record_type")]
    public string? IncomeRecordType { get; set; }

    /// <summary>收入记录单号（<c>income_record_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("income_record_id")]
    public string? IncomeRecordId { get; set; }

    /// <summary>收入金额（<c>amount</c>，必填 integer）：单位<b>分</b>，本 SDK 以 <c>long</c> 承载。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>入账成功时间（<c>success_time</c>，必填 string(32)，<b>rfc3339</b> 且官方要求北京时间 <c>+08:00</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>开户银行（<c>bank_name</c>，必填 string(256)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行户名（<c>bank_account_name</c>，必填 string(256)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_account_name")]
    public string? BankAccountName { get; set; }

    /// <summary>银行账号（<c>bank_account_number</c>，必填 string(64)）：<b>个人金融信息，不得入日志/遥测</b>。</summary>
    [JsonPropertyName("bank_account_number")]
    public string? BankAccountNumber { get; set; }

    /// <summary>充值备注（<c>recharge_remark</c>，必填 string(256)）。</summary>
    [JsonPropertyName("recharge_remark")]
    public string? RechargeRemark { get; set; }
}

/// <summary>
/// 资金收入记录分页链接（<c>links</c>）：<b>两页逐字段相同</b> ⇒ 共用一型。
/// </summary>
/// <remarks>官方三键均 string(2048)；<c>next</c> 为空表示已是最后一页，<c>prev</c> 为空表示第一页。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class MerchantFundIncomeRecordLinks
{
    /// <summary>下一页链接（<c>next</c>，选填 string(2048)）：为空表示无下一页。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>上一页链接（<c>prev</c>，选填 string(2048)）：为空表示已是第一页。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>当前页链接（<c>self</c>，选填 string(2048)）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}
