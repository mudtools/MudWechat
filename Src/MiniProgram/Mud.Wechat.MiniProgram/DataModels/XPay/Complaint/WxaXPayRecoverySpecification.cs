// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 被管控原因与解脱路径（官方 <c>Res.recovery_specifications</c> 元素，12 字段全为字符串）。
/// </summary>
/// <remarks>
/// <para><b>官方本域无「管控原因」枚举表</b>（<c>limitation_reason_type</c> / <c>recover_way</c> / <c>limitation_action_type</c> 三支官方只给字段说明）⇒ <b>不建常量类</b>，照官方类型落字符串，与守卫 XP5「只有官方枚举表才建常量类」的口径一致。</para>
/// <para><b>单据号可关联通知</b>（官方原文：<c>limitation_case_id</c> 可与管控流水通知中的 <c>business_code</c> 关联）⇒ 回调侧与查询侧的 join 键在此。</para>
/// <para><b><c>limitation_start_date</c> 仅延迟管控时返回</b>，实际生效看 <c>limitation_date</c> ⇒ 两支不可互替。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_punishment_reasons.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayRecoverySpecification
{
    /// <summary>limitation_case_id（官方类型 <c>string</c>）：该条管控原因对应的单据号，可与管控流水通知中的 business_code 关联。</summary>
    [JsonPropertyName("limitation_case_id")]
    public string? LimitationCaseId { get; set; }

    /// <summary>limitation_reason_type（官方类型 <c>string</c>）：该条管控原因所属的类型。</summary>
    [JsonPropertyName("limitation_reason_type")]
    public string? LimitationReasonType { get; set; }

    /// <summary>limitation_reason（官方类型 <c>string</c>）：该条管控原因的简要描述。</summary>
    [JsonPropertyName("limitation_reason")]
    public string? LimitationReason { get; set; }

    /// <summary>limitation_reason_describe（官方类型 <c>string</c>）：该条管控原因的进一步说明，便于开发者理解为何被管控。</summary>
    [JsonPropertyName("limitation_reason_describe")]
    public string? LimitationReasonDescribe { get; set; }

    /// <summary>relate_limitations（官方类型 <c>string</c>）：在该条管控原因下，具体受影响的能力列表。</summary>
    [JsonPropertyName("relate_limitations")]
    public string? RelateLimitations { get; set; }

    /// <summary>other_relate_limitations（官方类型 <c>string</c>）：若该条原因下还有未被标准枚举覆盖的管控能力，则通过该字段补充说明。</summary>
    [JsonPropertyName("other_relate_limitations")]
    public string? OtherRelateLimitations { get; set; }

    /// <summary>recover_way（官方类型 <c>string</c>）：微信支付建议的处理路径，开发者据此去微信支付商户平台进一步处理。</summary>
    [JsonPropertyName("recover_way")]
    public string? RecoverWay { get; set; }

    /// <summary>recover_way_param（官方类型 <c>string</c>）：解脱路径对应的补充参数，例如尽调单号、申诉单号、有权机关信息或经营类型确认单号。</summary>
    [JsonPropertyName("recover_way_param")]
    public string? RecoverWayParam { get; set; }

    /// <summary>recover_help_url（官方类型 <c>string</c>）：若微信支付提供进一步说明页面，则通过该字段返回。</summary>
    [JsonPropertyName("recover_help_url")]
    public string? RecoverHelpUrl { get; set; }

    /// <summary>limitation_action_type（官方类型 <c>string</c>）：该条管控原因对应的管控生效方式。</summary>
    [JsonPropertyName("limitation_action_type")]
    public string? LimitationActionType { get; set; }

    /// <summary>limitation_start_date（官方类型 <c>string</c>）：当处置方式为延迟管控时返回的预计开始时间。</summary>
    [JsonPropertyName("limitation_start_date")]
    public string? LimitationStartDate { get; set; }

    /// <summary>limitation_date（官方类型 <c>string</c>）：该条管控原因实际生效的时间。</summary>
    [JsonPropertyName("limitation_date")]
    public string? LimitationDate { get; set; }
}
