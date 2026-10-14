// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.BankPayScore;

/// <summary>
/// 后付费项目（<c>post_payments[]</c>），官方构件名 <c>Payment</c>。
/// </summary>
/// <remarks>
/// 官方要求按<b>所属行业规程</b>传参（详见 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013123713"/>），
/// 完结/修改订单时须满足 <c>total_amount = post_payments.amount 总和 − post_discounts.amount 总和</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScorePostPayment
{
    /// <summary>付费名称（<c>name</c>，选填 string(20)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>付费金额（<c>amount</c>，选填 integer）：单位为分，≥0（等于 0 表示不扣费）。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>付费说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>付费数量（<c>count</c>，选填 integer）：同项目建议合并计算 <c>amount</c> 与 <c>count</c>。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>
/// 商户优惠项目（<c>post_discounts[]</c>），官方构件名 <c>ServiceOrderCoupon</c>。
/// </summary>
/// <remarks>最多 30 条，同一单内 <c>name</c> 不可重复。</remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScorePostDiscount
{
    /// <summary>优惠名称（<c>name</c>，选填 string(20)）：同一单多个优惠项目名称不可重复。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>优惠金额（<c>amount</c>，选填 integer）：单位为分。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>优惠数量（<c>count</c>，选填 integer）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>
/// 服务时间段（<c>time_range</c>）。
/// </summary>
/// <remarks>
/// <para><b>本域两套时间格式之一</b>：两个时间用<b>紧凑格式</b>而非 rfc3339 ——
/// <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c>，且 <c>start_time</c> 额外接受关键字
/// <see cref="BankPayScoreStartTimeKeywords.OnAccept"/>（表示「免确认创单成功时间」为服务开始时间）。</para>
/// <para>官方校验按<b>格式精度</b>分档：<c>yyyyMMdd</c> 只校验到日、<c>yyyyMMddHHmmss</c> 校验到秒，
/// 且 <c>end_time</c> 必须与 <c>start_time</c> 格式一致；<c>start_time=OnAccept</c> 时
/// <c>end_time</c> 必须是 <c>yyyyMMddHHmmss</c>。故字段保持 <c>string</c>，不做 <c>DateTimeOffset</c> 往返。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreTimeRange
{
    /// <summary>服务开始时间（<c>start_time</c>，选填 string(14)）：<c>yyyyMMddHHmmss</c>/<c>yyyyMMdd</c>/<c>OnAccept</c>。</summary>
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

/// <summary>
/// 服务位置（<c>location</c>）。
/// </summary>
/// <remarks>
/// 官方对同一字段两处宽度写法不一：创单/查询页写 <c>string(50)</c>、完结页写 <c>string(20)</c>，
/// 而两处的描述文字都是「不超过 20 个字符」⇒ 照原文保留 <c>string</c>，不做长度收敛。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreLocation
{
    /// <summary>服务开始地点（<c>start_location</c>，选填）。</summary>
    [JsonPropertyName("start_location")]
    public string? StartLocation { get; set; }

    /// <summary>服务结束地点（<c>end_location</c>，选填）。</summary>
    [JsonPropertyName("end_location")]
    public string? EndLocation { get; set; }
}

/// <summary>
/// 服务风险金（<c>risk_fund</c>）：创单<b>必填</b>。
/// </summary>
/// <remarks>
/// <c>name</c> 官方只开放 <see cref="BankPayScoreRiskFundNames.EstimateOrderCost"/> 一值；
/// <c>amount</c> 必须 &gt; 0 且 ≤「服务 ID 风险金额上限」（与微信支付运营确认），
/// 金额越高评估通过率越低，评估不通过属正常风控拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreRiskFund
{
    /// <summary>风险名称（<c>name</c>，必填 string(30)）：固定传 <c>ESTIMATE_ORDER_COST</c>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>风险金额（<c>amount</c>，必填 integer）：单位为分，必须 &gt; 0 且 ≤ 服务 ID 风险金额上限。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>风险说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 同步订单的收款详情（<c>detail</c>，<c>type=Order_Paid</c> 时必填）。
/// </summary>
/// <remarks>
/// <para><c>paid_time</c> 用<b>紧凑格式</b>（<c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c>，后者按当天 0 点 0 分 0 秒计），
/// 且官方约束 <c>商户调用完结接口时间 &lt; paid_time ≤ 商户调用同步接口时间 + 60 秒</c>。</para>
/// <para><b>必须如实填写</b>：微信支付分依据该时间更新用户侧守约记录与负面记录，填错会直接造成客诉。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreSyncDetail
{
    /// <summary>收款成功时间（<c>paid_time</c>，必填 string(64)）：<c>yyyyMMddHHmmss</c> 或 <c>yyyyMMdd</c>。</summary>
    [JsonPropertyName("paid_time")]
    public string? PaidTime { get; set; }
}
