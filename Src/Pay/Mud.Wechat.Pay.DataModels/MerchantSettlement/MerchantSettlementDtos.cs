// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantSettlement;

/// <summary>
/// 创建「商户结算规则申请单」请求体（<b>从业机构面</b>的四字段，无嵌套对象）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>，2026-10-14 逐字段核验。</para>
/// <para><b>三套标识 + 一个规则号</b>：<c>acquiring_bank_id</c>（收单银行侧商户号）、<c>channel_id</c>（渠道商商户号）、
/// <c>sub_mchid</c>（特约商户号）、<c>settle_rule_id</c>（结算规则编号）⇒ 与本仓其余服务商域的四分体系
/// （<c>sp_</c>/<c>sub_</c> 前缀）都不同，照抄 <c>PartnerTransactions</c> 的标识写法即错（守卫 MSET-B3）。</para>
/// <para><b><c>settle_rule_id</c> 官方是 integer</b>（不是 string），本 SDK 以 <c>long</c> 承载 ⇒
/// 宿主传字符串会落到 <c>code = PARAM_ERROR</c>，而不会得到编译期提示。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantSettlement")]
public class MerchantSettlementRuleApplicationCreateRequest
{
    /// <summary>收单机构商户号（<c>acquiring_bank_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>结算规则编号（<c>settle_rule_id</c>，必填 integer）。</summary>
    [JsonPropertyName("settle_rule_id")]
    public long? SettleRuleId { get; set; }
}

/// <summary>
/// 结算规则申请单应答（<b>创建支与查询支共用</b>本型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：创建 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>、
/// 查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012698574"/>，2026-10-14 逐字段核验。</para>
/// <para><b>两页应答表逐字段相同</b>（8 个键、同名同类型同必填性，只有 <c>application_id</c> 必填）
/// ⇒ 按「表相同则共用」归并（守卫 MSET-B6）。两页的差异全在<b>请求侧</b>：创建走包体四字段、
/// 查询只走 path 的 <c>application_id</c>。</para>
/// <para><b><c>update_time</c> 是 rfc3339 字符串</b>（官方 string(128)）⇒ 类型落 <c>string</c> 不转
/// <see cref="DateTimeOffset"/>：本包全域时间字段一律 <c>string</c>，避免跨 TFM 与两套官方格式
/// （紧凑 <c>yyyyMMddHHmmss</c> 与 rfc3339）混用时的解析歧义。</para>
/// <para><b><c>application_process_info</c> 是给人看的描述串</b>（string(256)），不是结构化进度节点
/// ⇒ 不得据它做状态判定，判定只看 <see cref="ApplicationState"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantSettlement")]
public class MerchantSettlementRuleApplicationResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>application_id</c>，必填 string(32)）：查询支的定位键。</summary>
    [JsonPropertyName("application_id")]
    public string? ApplicationId { get; set; }

    /// <summary>处理进展描述（<c>application_process_info</c>，选填 string(256)）。</summary>
    [JsonPropertyName("application_process_info")]
    public string? ApplicationProcessInfo { get; set; }

    /// <summary>申请单状态（<c>application_state</c>，选填 string）：取值见 <see cref="MerchantSettlementApplicationStates"/>。</summary>
    [JsonPropertyName("application_state")]
    public string? ApplicationState { get; set; }

    /// <summary>更新时间（<c>update_time</c>，选填 string(128)）：官方 rfc3339 格式，原样承载。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>收单机构商户号（<c>acquiring_bank_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>结算规则编号（<c>settle_rule_id</c>，选填 integer）。</summary>
    [JsonPropertyName("settle_rule_id")]
    public long? SettleRuleId { get; set; }
}
