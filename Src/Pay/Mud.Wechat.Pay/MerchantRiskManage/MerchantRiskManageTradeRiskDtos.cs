// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantRiskManage;

/// <summary>
/// 「查询风险信息」应答（<b>GET /v3/merchant-risk-manage/trade-risk-information</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467535"/>，
/// 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】，2026-10-14 逐字段核验。</para>
/// <para><b>本支是按 <c>out_trade_no</c> 的单笔查询</b>，官方页没有任何列表 / 分页节点
/// —— 计划清单把它记作「获取风险订单列表」是转述层错判；分页形态在 <c>ec-trade-risk/query-risk-trades</c>
/// 那一族（<see cref="MerchantRiskManagePartnerRiskTradeQueryRequest"/>），两支不可混为一谈（守卫 MRB-B1）。</para>
/// <para><b><c>risk_score</c> 是官方唯一标「必填」的应答字段</b>（integer），但官方<b>未印取值范围</b>
/// （仅说明「分数越高风险越高」）⇒ 消费侧按单调性用，不得假设 0~100。</para>
/// <para><b><c>risk_type</c> / <c>punish_type</c> 在此是 string</b>（值集见
/// <see cref="MerchantRiskManageTradeRiskTypes"/> 与 <see cref="MerchantRiskManagePunishTypes"/>），
/// 与风险订单同步面的同名 integer 字段是两套体系。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeRiskInfoResponse : WechatPayResponse
{
    /// <summary>风险系数值（<c>risk_score</c>，官方标<b>必填</b> integer）：分数越高风险越高，<b>官方未印取值范围</b>。</summary>
    [JsonPropertyName("risk_score")]
    public int? RiskScore { get; set; }

    /// <summary>风险类型（<c>risk_type</c>，选填 string，5 值）：见 <see cref="MerchantRiskManageTradeRiskTypes"/>。</summary>
    [JsonPropertyName("risk_type")]
    public string? RiskType { get; set; }

    /// <summary>处置手段（<c>punish_type</c>，选填 string，10 值）：见 <see cref="MerchantRiskManagePunishTypes"/>。</summary>
    [JsonPropertyName("punish_type")]
    public string? PunishType { get; set; }

    /// <summary>处置手段描述（<c>punish_description</c>，选填 string(255)）。</summary>
    [JsonPropertyName("punish_description")]
    public string? PunishDescription { get; set; }
}

/// <summary>
/// 「处理结果回传」请求（<b>POST /v3/merchant-risk-manage/trade-risk-result</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467540"/>，
/// 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】，2026-10-14 逐字段核验。
/// （计划清单原把出处记成 <c>4012692160</c> —— 那页只文档「上报订单关联信息」，本支另有独立页面，已在方案内勘误。）</para>
/// <para><b><c>punish_type</c> 为本支唯一必填字段</b>（string，10 值）；<c>out_trade_no</c> 官方反而标<b>选填</b>。</para>
/// <para><b><c>sub_mchid</c> 的「表选填 + 描述条件必填」</b>：官方描述写「调用方为服务商/从业机构/渠道商时该字段必填」
/// ⇒ 必填性不在参数表里，SDK 不校验、由官方以 <c>INVALID_REQUEST</c> 拒绝。</para>
/// <para><b><c>additional_punish_type</c> 是自由文本</b>（string(255)）：官方在「处置手段选其他」时使用，
/// 回传页示例形态为 <c>"SUSPEND_SETTLEMENT_NDAYS:10"</c>（冒号分隔的键值串），<b>官方未给该串的取值字典</b>
/// ⇒ 建成 string 透传，不得据示例建枚举。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeRiskResultRequest
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，选填 string(32)）：子商户调用时须传对应服务商号。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>从业机构商户号（<c>acquiring_bank_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道号（<c>channel_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，官方标选填 string(32)）：<b>调用方为服务商/从业机构/渠道商时官方要求必填</b>。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，官方标选填 string(64)）：官方描述沿用查询页文案「用于查询该交易单号对应的风险信息」。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>风险类型（<c>risk_type</c>，选填 string，5 值）：见 <see cref="MerchantRiskManageTradeRiskTypes"/>。</summary>
    [JsonPropertyName("risk_type")]
    public string? RiskType { get; set; }

    /// <summary>处置手段（<c>punish_type</c>，<b>必填</b> string，10 值）：见 <see cref="MerchantRiskManagePunishTypes"/>。</summary>
    [JsonPropertyName("punish_type")]
    public string? PunishType { get; set; }

    /// <summary>处置手段补充（<c>additional_punish_type</c>，选填 string(255)）：选「其它处罚」时写实际手段，<b>官方未给取值字典</b>。</summary>
    [JsonPropertyName("additional_punish_type")]
    public string? AdditionalPunishType { get; set; }

    /// <summary>投诉信息（<c>complaints_information</c>，选填 string(255)）：商户侧投诉的具体文本。</summary>
    [JsonPropertyName("complaints_information")]
    public string? ComplaintsInformation { get; set; }
}

/// <summary>
/// 「处理结果回传」应答：官方回显标识与判定结果，其中 <c>punish_type</c> <b>在应答侧仍标必填</b>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467540"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与请求侧的字段差</b>：应答<b>多出 <c>punish_description</c></b>（string(255)，请求侧是
/// <c>additional_punish_type</c>）⇒ 两支字段名不同，不得共用一个类型（守卫 MRB-B6）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeRiskResultResponse : WechatPayResponse
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>从业机构商户号（<c>acquiring_bank_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道号（<c>channel_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string(64)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>风险类型（<c>risk_type</c>，选填 string，5 值）：见 <see cref="MerchantRiskManageTradeRiskTypes"/>。</summary>
    [JsonPropertyName("risk_type")]
    public string? RiskType { get; set; }

    /// <summary>处置手段（<c>punish_type</c>，官方应答侧标<b>必填</b> string，10 值）。</summary>
    [JsonPropertyName("punish_type")]
    public string? PunishType { get; set; }

    /// <summary>处置手段补充（<c>additional_punish_type</c>，选填 string(255)）。</summary>
    [JsonPropertyName("additional_punish_type")]
    public string? AdditionalPunishType { get; set; }

    /// <summary>处置手段描述（<c>punish_description</c>，选填 string(255)）：<b>应答侧独有</b>，请求侧无此字段。</summary>
    [JsonPropertyName("punish_description")]
    public string? PunishDescription { get; set; }
}
