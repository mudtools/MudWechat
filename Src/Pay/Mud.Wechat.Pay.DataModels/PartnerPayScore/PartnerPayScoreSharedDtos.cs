// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerPayScore;

/// <summary>
/// 后付费项目（<c>post_payments[]</c>，服务商支付分创单 / 完结 / 修改 / 两支消息端点共用）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>（创建支付分订单），
/// 2026-10-14 逐字段核验。<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692408"/>（完结）
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013394914"/>（发送扣费提醒）同表 ⇒ <b>共用</b>本类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePostPayment
{
    /// <summary>付费名称（<c>name</c>，选填 string(20)）：须严格按官方「post_payments 字段传参说明」按行业规程填写。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>付费金额（<c>amount</c>，选填整型，单位分，<b>≥0</b>；<c>0</c> 表示不扣费）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>付费说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>付费数量（<c>count</c>，选填整型，创单页范围 <b>[1,100]</b>）：相同后付费项目官方建议合并计算 <c>amount</c> 与 <c>count</c>。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>
/// 商户优惠（<c>post_discounts[]</c>，最多 30 条）。
/// </summary>
/// <remarks>
/// <b>收款总额校验式</b>：官方在本域三处（创单 / 完结 / 修改）都注明
/// <c>total_amount = post_payments.amount 之和 − post_discounts.amount 之和</c> ⇒ 本类型<b>必须</b>带
/// <c>amount</c>，否则该等式无法成立。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePostDiscount
{
    /// <summary>优惠名称（<c>name</c>，选填 string(20)）：官方注明<b>同一单多个优惠项目名称不可重复</b>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>优惠金额（<c>amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>优惠数量（<c>count</c>，选填整型，创单页范围 <b>[1,100]</b>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>
/// 服务时间段（<c>time_range</c>）。
/// </summary>
/// <remarks>
/// <para><b>本域两套时间格式并存的第一套</b>：本节字段是<b>紧凑格式</b>
/// <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c>，<b>不是</b> rfc3339（守卫 PS-B3 锁定）；
/// 而 <c>complete_time</c> / <c>cancel_sign_time</c> / <c>plan_over_time</c> 走 rfc3339。</para>
/// <para><b>官方按精度分别校验</b>：<c>yyyyMMdd</c> 校验到日、<c>yyyyMMddHHmmss</c> 校验到秒，
/// 且 <c>end_time</c> 必须与 <c>start_time</c> <b>格式一致</b>（<c>start_time</c> 取
/// <see cref="PartnerPayScoreStartTimeKeywords.OnAccept"/> 时 <c>end_time</c> 必须用 <c>yyyyMMddHHmmss</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreTimeRange
{
    /// <summary>
    /// 服务开始时间（<c>start_time</c>，string(14)）：支持 <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c> /
    /// <b><c>OnAccept</c></b>（以创单成功时间为服务开始时间，见 <see cref="PartnerPayScoreStartTimeKeywords.OnAccept"/>）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>服务结束时间（<c>end_time</c>，选填 string(14)）：须与 <c>start_time</c> 同格式。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>服务开始时间备注（<c>start_time_remark</c>，选填 string(20)）。</summary>
    [JsonPropertyName("start_time_remark")]
    public string? StartTimeRemark { get; set; }

    /// <summary>服务结束时间备注（<c>end_time_remark</c>，选填 string(20)）。</summary>
    [JsonPropertyName("end_time_remark")]
    public string? EndTimeRemark { get; set; }
}

/// <summary>服务位置（<c>location</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreLocation
{
    /// <summary>服务开始地点（<c>start_location</c>，选填 string(50)）。</summary>
    [JsonPropertyName("start_location")]
    public string? StartLocation { get; set; }

    /// <summary>服务结束地点（<c>end_location</c>，选填 string(50)）。</summary>
    [JsonPropertyName("end_location")]
    public string? EndLocation { get; set; }
}

/// <summary>
/// 设备信息（<c>device</c>，创单 / 完结 / 修改三端点共用）。
/// </summary>
/// <remarks>
/// <b><c>materiel_no</c> 是 <c>device</c> 的子字段，不是请求顶层字段</b> —— 官方服务商创单页的示例报文里
/// <c>materiel_no</c> 就嵌在 <c>device</c> 对象内，把它提到顶层会静默丢字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreDevice
{
    /// <summary>服务开始的设备 ID（<c>start_device_id</c>，选填 string(50)）：官方注明<b>售货机 / 充电宝 / 充电桩等无人自助设备行业必传</b>。</summary>
    [JsonPropertyName("start_device_id")]
    public string? StartDeviceId { get; set; }

    /// <summary>服务结束的设备 ID（<c>end_device_id</c>，选填 string(50)）：同上，无人自助设备行业必传。</summary>
    [JsonPropertyName("end_device_id")]
    public string? EndDeviceId { get; set; }

    /// <summary>物料 URL（<c>materiel_no</c>，选填 string(100)）：参与政策时填微信支付运营给到的加参 URL。</summary>
    [JsonPropertyName("materiel_no")]
    public string? MaterielNo { get; set; }
}

/// <summary>
/// 服务风险金（<c>risk_fund</c>，创单请求<b>必填</b>、应答选填）。
/// </summary>
/// <remarks>
/// <b>必填子字段照录</b>：官方外层在应答里标选填、<c>name</c>/<c>amount</c> 仍标必填（照录，不「修平」）。
/// <c>amount</c> 官方要求 <b>&gt; 0</b> 且 <b>≤ 服务 ID 风险金额上限</b>（上限须与微信支付运营确认）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreRiskFund
{
    /// <summary>风险名称（<c>name</c>，必填 string(30)）：官方本域字段表「固定传」<see cref="PartnerPayScoreRiskFundNames.EstimateOrderCost"/>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>风险金额（<c>amount</c>，必填整型，单位分，官方注明<b>必须 &gt; 0</b> 且 ≤ 服务 ID 风险金额上限）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>风险说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
