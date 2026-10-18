// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PayScore;

/// <summary>
/// 创建支付分计划请求体（<b>POST /v3/payscore/plan/payscore-plans</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467232"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。</para>
/// <para><b>四条一致性校验照录</b>（不满足即 400）：① <c>deduction_quantity</c> 必须等于
/// <c>plan_detail_list</c> 条数；② 明细实付金额须<b>依序不递增</b>（前一笔 ≥ 后一笔）；
/// ③ 每条明细 <c>original_price</c> ≥ <c>actual_price</c>；④ <c>total_actual_price</c> /
/// <c>total_original_price</c> 须与明细对应金额之和一致，且总实付不得为 0。</para>
/// <para><b>明细编号由微信侧按列表顺序从 1 生成</b>：商户不能自报 <c>plan_detail_no</c>，
/// 后续「创建签约计划服务订单」须<b>依序</b>使用该序号。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCreatePlanRequest
{
    /// <summary>计划所属服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>计划名称（<c>plan_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>计划有效期（<c>plan_duration</c>，必填整型，<b>单位天</b>）：不得超过服务规定的计划最大有效期。</summary>
    [JsonPropertyName("plan_duration")]
    public int? PlanDuration { get; set; }

    /// <summary>计划扣费次数（<c>deduction_quantity</c>，必填整型）：官方要求<b>与计划明细条数相等</b>。</summary>
    [JsonPropertyName("deduction_quantity")]
    public int? DeductionQuantity { get; set; }

    /// <summary>计划原总金额（<c>total_original_price</c>，必填整型，单位分）：须等于各明细原金额之和。</summary>
    [JsonPropertyName("total_original_price")]
    public long? TotalOriginalPrice { get; set; }

    /// <summary>计划实际扣费总金额（<c>total_actual_price</c>，必填整型，单位分）：须等于各明细实付金额之和且不得为 0。</summary>
    [JsonPropertyName("total_actual_price")]
    public long? TotalActualPrice { get; set; }

    /// <summary>计划明细列表（<c>plan_detail_list</c>，选填）：数量须等于 <c>deduction_quantity</c>，见 <see cref="PayScorePlanDetailInput"/>。</summary>
    [JsonPropertyName("plan_detail_list")]
    public List<PayScorePlanDetailInput>? PlanDetailList { get; set; }

    /// <summary>商户侧计划号（<c>merchant_plan_no</c>，必填 string(32)）：同一商户号下唯一 —— 查询/终止计划以它为 path 键。</summary>
    [JsonPropertyName("merchant_plan_no")]
    public string? MerchantPlanNo { get; set; }
}

/// <summary>创建计划时上送的支付分计划明细（<c>plan_detail_list[]</c>）。</summary>
/// <remarks>
/// <b>请求形态无 <c>plan_detail_no</c></b>：序号由微信侧按列表顺序从 1 生成；应答侧是
/// <see cref="PayScorePlanDetailItem"/>（多一个 <c>plan_detail_no</c>）⇒ 两类型分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePlanDetailInput
{
    /// <summary>明细原支付金额（<c>original_price</c>，必填整型，单位分）：须 ≥ 本条 <c>actual_price</c>。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>明细优惠说明（<c>plan_discount_description</c>，选填 string(20)）。</summary>
    [JsonPropertyName("plan_discount_description")]
    public string? PlanDiscountDescription { get; set; }

    /// <summary>明细实际支付金额（<c>actual_price</c>，必填整型，单位分）：官方要求依序<b>不递增</b>（前一笔 ≥ 后一笔）。</summary>
    [JsonPropertyName("actual_price")]
    public long? ActualPrice { get; set; }

    /// <summary>明细名称（<c>plan_detail_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_detail_name")]
    public string? PlanDetailName { get; set; }
}

/// <summary>
/// 支付分计划应答（创建 / 查询 / 终止三支端点<b>共用</b>同一张计划表）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467232"/>（创建）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470804"/>（查询）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470851"/>（终止），
/// 2026-10-18 逐页比对：三页应答表<b>逐项一致</b> ⇒ 按「表相同则共用」合并为一个类型。</para>
/// <para><b>查询与终止的定位键是 <c>merchant_plan_no</c>（商户侧计划号）而非 <c>plan_id</c></b>；
/// <c>plan_id</c> 只出现在应答与「创建用户签约计划」的入参里。</para>
/// <para><b><c>stop_mchid</c>/<c>stop_time</c> 仅计划已终止时返回</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePlanResponse : WechatPayResponse
{
    /// <summary>支付分计划 ID（<c>plan_id</c>，必填 string(32)）：创建用户签约计划时上送的 <c>plan_id</c>。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>计划所属服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户侧计划号（<c>merchant_plan_no</c>，必填 string(32)）：查询与终止计划的 path 定位键。</summary>
    [JsonPropertyName("merchant_plan_no")]
    public string? MerchantPlanNo { get; set; }

    /// <summary>计划名称（<c>plan_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>计划有效期（<c>plan_duration</c>，必填整型，单位天）：用户签约后当天起最长有效天数。</summary>
    [JsonPropertyName("plan_duration")]
    public int? PlanDuration { get; set; }

    /// <summary>计划状态（<c>plan_state</c>，必填 string）：<c>NORMAL</c> 可签约 / <c>STOP</c> 停签但已签约可继续履约。</summary>
    [JsonPropertyName("plan_state")]
    public string? PlanState { get; set; }

    /// <summary>计划原总金额（<c>total_original_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("total_original_price")]
    public long? TotalOriginalPrice { get; set; }

    /// <summary>计划扣费次数（<c>deduction_quantity</c>，必填整型）：对应计划明细数。</summary>
    [JsonPropertyName("deduction_quantity")]
    public int? DeductionQuantity { get; set; }

    /// <summary>计划实际扣费总金额（<c>total_actual_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("total_actual_price")]
    public long? TotalActualPrice { get; set; }

    /// <summary>计划明细列表（<c>plan_detail_list</c>，选填），见 <see cref="PayScorePlanDetailItem"/>；明细数量与扣费次数一致。</summary>
    [JsonPropertyName("plan_detail_list")]
    public List<PayScorePlanDetailItem>? PlanDetailList { get; set; }

    /// <summary>终止方商户号（<c>stop_mchid</c>，选填 string(32)）：操作终止计划的商户号。</summary>
    [JsonPropertyName("stop_mchid")]
    public string? StopMchId { get; set; }

    /// <summary>终止时间（<c>stop_time</c>，选填 string(32)，<b>rfc3339</b> 且北京时间 <c>+08:00</c>）。</summary>
    [JsonPropertyName("stop_time")]
    public string? StopTime { get; set; }
}

/// <summary>支付分计划明细（应答形态，<c>plan_detail_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePlanDetailItem
{
    /// <summary>计划明细序号（<c>plan_detail_no</c>，必填整型）：由微信侧从 1 开始按创建顺序编号，<b>不得跳号使用</b>。</summary>
    [JsonPropertyName("plan_detail_no")]
    public int? PlanDetailNo { get; set; }

    /// <summary>明细名称（<c>plan_detail_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_detail_name")]
    public string? PlanDetailName { get; set; }

    /// <summary>明细原支付金额（<c>original_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>明细优惠说明（<c>plan_discount_description</c>，选填 string(20)）。</summary>
    [JsonPropertyName("plan_discount_description")]
    public string? PlanDiscountDescription { get; set; }

    /// <summary>明细实际支付金额（<c>actual_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("actual_price")]
    public long? ActualPrice { get; set; }
}

/// <summary>
/// 「基于签约计划创建支付分服务订单」请求体（<b>POST /v3/payscore/sign-plan/serviceorder</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489351"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。</para>
/// <para><b><c>out_trade_no</c> 必须与创建签约计划时该明细的 <c>merchant_plan_detail_no</c> 一致</b>
/// —— 传成任意新单号会被拒。</para>
/// <para><b>同时只有一个签约计划可有服务中订单</b>：创建下一条明细的订单前，须先取消上一订单，
/// 或完结并等待支付成功；且<b>不允许跳序号</b>创建。</para>
/// <para><b>本请求体不含 <c>post_payments</c> / <c>risk_fund</c> / <c>total_amount</c></b>：
/// 金额与风险金全部由订单对应的签约计划详情生成，商户不得重复上报。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanServiceOrderRequest
{
    /// <summary>支付分服务 ID（<c>service_id</c>，必填 string(32)）：须是待创建订单所属计划的服务 ID。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>签约计划 ID（<c>sign_plan_id</c>，必填 string(32)）：本商户创建的、待用户签约的支付分计划 ID。</summary>
    [JsonPropertyName("sign_plan_id")]
    public string? SignPlanId { get; set; }

    /// <summary>计划明细序号（<c>plan_detail_no</c>，必填整型）：须依序使用，跳号或存在在途订单时官方拒绝。</summary>
    [JsonPropertyName("plan_detail_no")]
    public int? PlanDetailNo { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>用户标识（<c>openid</c>，必填 string(128)）：用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>回调地址（<c>notify_url</c>，必填 string(256)）：接收用户确认订单或扣款成功通知。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）：官方注明不填时默认取订单对应签约计划的名称。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务时间（<c>time_range</c>，<b>必填</b>）：本端点仅用于用户侧展示，<c>start_time</c> 必填。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreSignPlanTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）：官方注明「创建订单可填、完结订单不填」。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(256)）：须先 URLEncode，总长不大于 256 字符。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>
    /// 商户侧订单号（<c>out_trade_no</c>，必填 string(32)）：<b>字段名是 <c>out_trade_no</c> 而非 <c>out_order_no</c></b>，
    /// 且官方要求必须与创建签约计划时该明细的 <c>merchant_plan_detail_no</c> 一致。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}

/// <summary>
/// 签约计划服务订单的服务时间（<c>time_range</c>）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="PayScoreTimeRange"/> 字段表不同</b>：本族用 <c>notes</c> 承载时间备注，
/// 而服务订单族是 <c>start_time_remark</c> / <c>end_time_remark</c> 两字段 ⇒ 不共用，防两处字段漂移。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanTimeRange
{
    /// <summary>服务开始时间（<c>start_time</c>，必填 string(14)）：<c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c>。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>服务结束时间（<c>end_time</c>，选填 string(14)）：须与 <c>start_time</c> 同格式。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>服务时间备注（<c>notes</c>，选填 string(20)）。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

/// <summary>
/// 「基于签约计划创建支付分服务订单」应答：订单表<b>子集</b>，且明细子表字段比服务订单族更窄。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489351"/>，2026-10-18 逐字段核验。
/// 本应答<b>无</b> <c>total_amount</c> / <c>need_collection</c> / <c>collection</c> / <c>promotion_detail</c> /
/// <c>openid</c> / <c>package</c>。</para>
/// <para><b>金额字段是派生值</b>：官方注明 <c>post_payments</c>/<c>post_discounts</c>/<c>risk_fund</c> 全部
/// 「取自订单对应的用户签约计划详情」，故各子表只返回计划里存在的那几个字段 ⇒ 独立建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanServiceOrderResponse : WechatPayResponse
{
    /// <summary>商户订单号（<c>out_order_no</c>，必填 string(32)）：官方应答表此处字段名为 <c>out_order_no</c>（请求侧才是 <c>out_trade_no</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填 string）：见 <see cref="PayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string）：本页取值为 <c>USER_CONFIRM</c> / <c>MERCHANT_COMPLETE</c>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）：由订单对应的签约计划详情生成，见 <see cref="PayScoreSignPlanPostPayment"/>。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScoreSignPlanPostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填），见 <see cref="PayScoreSignPlanPostDiscount"/>。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScoreSignPlanPostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）：本页官方只列 <c>amount</c> 一个子字段，见 <see cref="PayScoreSignPlanRiskFund"/>。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreSignPlanRiskFund? RiskFund { get; set; }

    /// <summary>服务时间（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreSignPlanTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填 string(64)）：与商户侧订单号一一对应。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>签约计划服务订单应答的后付费项目（<c>post_payments[]</c>，本页字段表<b>无 <c>count</c></b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanPostPayment
{
    /// <summary>付费名称（<c>name</c>，选填 string(20)）：官方注明取自签约计划所属支付分计划名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>付费金额（<c>amount</c>，选填整型，单位分）：取自签约计划详情的实际付费金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>付费说明（<c>description</c>，选填 string(30)）：取自签约计划详情的名称。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>签约计划服务订单应答的商户优惠（<c>post_discounts[]</c>，本页字段表<b>只有两项</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanPostDiscount
{
    /// <summary>优惠名称（<c>name</c>，选填 string(20)）：取自签约计划详情的计划明细优惠说明。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠金额（<c>amount</c>，选填整型，单位分）：由计划明细的「原价 − 实付价」得出。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>签约计划服务订单应答的服务风险金（<c>risk_fund</c>，本页字段表<b>只有 <c>amount</c></b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlanRiskFund
{
    /// <summary>风险金额（<c>amount</c>，必填整型，单位分）：取自订单对应计划明细的实际付款金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>
/// 创建用户签约计划请求体（<b>POST /v3/payscore/sign-plan/user-sign-plans</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489420"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。</para>
/// <para><b><c>sign_plan_detail</c> 上送的是「明细序号 + 商户侧单号」配对</b>：该
/// <c>merchant_plan_detail_no</c> 后续即「基于签约计划创建服务订单」的 <c>out_trade_no</c>，两处必须一致。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCreateUserSignPlanRequest
{
    /// <summary>支付分服务 ID（<c>service_id</c>，必填 string(32)）：待签约计划须属于该服务。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>支付分计划 ID（<c>plan_id</c>，必填 string(32)）：本商户创建的、待用户签约的计划 ID。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>待签约用户标识（<c>openid</c>，必填 string(128)）：用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商户签约计划单号（<c>merchant_sign_plan_no</c>，必填 string(32)）：同一商户号下唯一 —— 查询/解除签约计划的 path 定位键。</summary>
    [JsonPropertyName("merchant_sign_plan_no")]
    public string? MerchantSignPlanNo { get; set; }

    /// <summary>签约成功通知回调地址（<c>notify_url</c>，必填 <b>string(1024)</b>）：本页上限 1024，别按服务订单族的 256 校验。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>签约计划明细配对列表（<c>sign_plan_detail</c>，<b>必填</b>），见 <see cref="PayScoreUserSignPlanDetail"/>。</summary>
    [JsonPropertyName("sign_plan_detail")]
    public List<PayScoreUserSignPlanDetail>? SignPlanDetail { get; set; }
}

/// <summary>签约计划明细的商户侧单号配对（<c>sign_plan_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreUserSignPlanDetail
{
    /// <summary>计划明细序号（<c>plan_detail_no</c>，必填整型）：按创建计划时的明细顺序编号，从 1 开始。</summary>
    [JsonPropertyName("plan_detail_no")]
    public int? PlanDetailNo { get; set; }

    /// <summary>商户侧计划明细使用订单号（<c>merchant_plan_detail_no</c>，必填 string(32)）：后续作为该明细服务订单的商户侧订单号。</summary>
    [JsonPropertyName("merchant_plan_detail_no")]
    public string? MerchantPlanDetailNo { get; set; }
}

/// <summary>
/// 创建用户签约计划应答（<b>包装形态</b> <c>{sign_plan, package}</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489420"/>，2026-10-18 依应答示例核验：
/// 本支是<b>三支签约计划端点中唯一的包装形态</b> —— 查询与解除直接把签约计划字段<b>平铺</b>在应答根
/// （见 <see cref="PayScoreSignPlan"/>），故二者不可合并为一个类型。</para>
/// <para><b><c>package</c> 必填</b>：官方注明为「跳转微信侧确认签约小程序的数据」，
/// 须先 URLEncode 后用于拉起用户签约页 —— 别当装饰字段丢弃。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCreateUserSignPlanResponse : WechatPayResponse
{
    /// <summary>用户的签约计划（<c>sign_plan</c>，必填），见 <see cref="PayScoreSignPlan"/>。</summary>
    [JsonPropertyName("sign_plan")]
    public PayScoreSignPlan? SignPlan { get; set; }

    /// <summary>拉起签约页数据（<c>package</c>，必填 string）：官方注明<b>须先 URLEncode</b> 再传给小程序。</summary>
    [JsonPropertyName("package")]
    public string? Package { get; set; }
}

/// <summary>
/// 用户签约计划（查询与解除两支端点<b>平铺</b>返回；创建应答把它嵌套在 <c>sign_plan</c> 下）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489420"/>（创建应答的嵌套
/// <c>sign_plan</c>）、<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012470990"/>（查询，平铺）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471050"/>（解除，平铺），
/// 2026-10-18 逐页比对三处字段表<b>逐项一致</b> ⇒ 按「表相同则共用」建<b>一个</b>类型同时承载嵌套与平铺两种位置。</para>
/// <para><b>平铺位置需要错误面，故本类型继承 <see cref="WechatPayResponse"/></b>：作为嵌套对象时官方报文内
/// 不含 <c>code</c>/<c>message</c>，继承带来的 <c>IsSuccess</c> 在嵌套场景恒为 <c>true</c>（业务失败只出现在根层）。</para>
/// <para><b>签约计划的定位键是 <c>merchant_sign_plan_no</c></b>（商户侧签约计划号），
/// 而 <c>sign_plan_id</c> 是微信侧主键；「基于签约计划创建服务订单」用的是 <c>sign_plan_id</c>。</para>
/// <para><b>金额字段名与计划表不同源</b>：本表用 <c>total_origin_price</c>（origin），
/// 支付分计划表用 <c>total_original_price</c>（original）—— 官方两套拼写，勿统一。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignPlan : WechatPayResponse
{
    /// <summary>计划签约 ID（<c>sign_plan_id</c>，必填 string(32)）：签约计划主键，创建签约计划服务订单时上送。</summary>
    [JsonPropertyName("sign_plan_id")]
    public string? SignPlanId { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>计划所属服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户侧签约计划号（<c>merchant_sign_plan_no</c>，必填 string(32)）：查询与解除签约计划的 path 定位键。</summary>
    [JsonPropertyName("merchant_sign_plan_no")]
    public string? MerchantSignPlanNo { get; set; }

    /// <summary>商户回调地址（<c>merchant_callback_url</c>，必填 string(1024)）：即创建时上送的 <c>notify_url</c>，应答侧改名返回。</summary>
    [JsonPropertyName("merchant_callback_url")]
    public string? MerchantCallbackUrl { get; set; }

    /// <summary>签约所属计划 ID（<c>plan_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>当前进行到的计划明细序号（<c>going_detail_no</c>，必填整型）：从 1 开始，<c>0</c> 代表尚未使用任何明细。</summary>
    [JsonPropertyName("going_detail_no")]
    public int? GoingDetailNo { get; set; }

    /// <summary>计划签约状态（<c>sign_state</c>，必填 string）：取值须按官方页面值表判定。</summary>
    [JsonPropertyName("sign_state")]
    public string? SignState { get; set; }

    /// <summary>签约计划取消时间（<c>cancel_sign_time</c>，选填 string(32)，rfc3339）。</summary>
    [JsonPropertyName("cancel_sign_time")]
    public string? CancelSignTime { get; set; }

    /// <summary>签约计划取消类型（<c>cancel_sign_type</c>，选填 string）：取值须按官方页面值表判定。</summary>
    [JsonPropertyName("cancel_sign_type")]
    public string? CancelSignType { get; set; }

    /// <summary>签约计划取消原因（<c>cancel_reason</c>，选填 string(128)）。</summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }

    /// <summary>签约计划名称（<c>plan_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_name")]
    public string? PlanName { get; set; }

    /// <summary>签约计划过期时间（<c>plan_over_time</c>，必填 string(32)，rfc3339 + 北京时间 <c>+08:00</c>）。</summary>
    [JsonPropertyName("plan_over_time")]
    public string? PlanOverTime { get; set; }

    /// <summary>签约计划原总金额（<c>total_origin_price</c>，必填整型，单位分）：官方本表拼写为 <c>origin</c>。</summary>
    [JsonPropertyName("total_origin_price")]
    public long? TotalOriginPrice { get; set; }

    /// <summary>签约计划扣费次数（<c>deduction_quantity</c>，必填整型）。</summary>
    [JsonPropertyName("deduction_quantity")]
    public int? DeductionQuantity { get; set; }

    /// <summary>签约计划实际总金额（<c>total_actual_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("total_actual_price")]
    public long? TotalActualPrice { get; set; }

    /// <summary>签约计划明细列表（<c>signed_detail_list</c>，选填），见 <see cref="PayScoreSignedDetailItem"/>。</summary>
    [JsonPropertyName("signed_detail_list")]
    public List<PayScoreSignedDetailItem>? SignedDetailList { get; set; }
}

/// <summary>签约计划明细（<c>signed_detail_list[]</c>）。</summary>
/// <remarks>
/// <b>三种 rfc3339 时间在明细层可能带毫秒</b>：官方示例写 <c>2021-05-20T13:29:35.120+08:00</c>
/// ⇒ 解析须容忍毫秒段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreSignedDetailItem
{
    /// <summary>计划明细序号（<c>plan_detail_no</c>，必填整型）：按创建计划时明细顺序，从 1 开始。</summary>
    [JsonPropertyName("plan_detail_no")]
    public int? PlanDetailNo { get; set; }

    /// <summary>明细原支付金额（<c>original_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }

    /// <summary>明细优惠说明（<c>plan_discount_description</c>，选填 string(20)）。</summary>
    [JsonPropertyName("plan_discount_description")]
    public string? PlanDiscountDescription { get; set; }

    /// <summary>明细实际支付金额（<c>actual_price</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("actual_price")]
    public long? ActualPrice { get; set; }

    /// <summary>计划明细状态（<c>plan_detail_state</c>，必填 string）：取值须按官方页面值表判定。</summary>
    [JsonPropertyName("plan_detail_state")]
    public string? PlanDetailState { get; set; }

    /// <summary>明细对应的支付分服务单号（<c>order_id</c>，选填 string(32)）：官方注明<b>仅使用中/已使用/已取消三态存在</b>。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户侧计划明细使用订单号（<c>merchant_plan_detail_no</c>，必填 string(32)）：创建该明细服务订单时的 <c>out_trade_no</c>。</summary>
    [JsonPropertyName("merchant_plan_detail_no")]
    public string? MerchantPlanDetailNo { get; set; }

    /// <summary>计划详情名称（<c>plan_detail_name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("plan_detail_name")]
    public string? PlanDetailName { get; set; }

    /// <summary>明细对应订单实际支付金额（<c>actual_pay_price</c>，选填整型，单位分）：订单完结后用户实付此值。</summary>
    [JsonPropertyName("actual_pay_price")]
    public long? ActualPayPrice { get; set; }

    /// <summary>详情使用时间（<c>use_time</c>，选填 string(32)，rfc3339 含毫秒）：明细使用后回传。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }

    /// <summary>详情完成时间（<c>complete_time</c>，选填 string(32)，rfc3339 含毫秒）：对应订单实际完成支付时间。</summary>
    [JsonPropertyName("complete_time")]
    public string? CompleteTime { get; set; }

    /// <summary>详情取消时间（<c>cancel_time</c>，选填 string(32)，rfc3339 含毫秒）。</summary>
    [JsonPropertyName("cancel_time")]
    public string? CancelTime { get; set; }

    /// <summary>明细签约时间（<c>sign_time</c>，选填 string(32)，rfc3339）：本页把它列在明细层。</summary>
    [JsonPropertyName("sign_time")]
    public string? SignTime { get; set; }
}

/// <summary>
/// 解除用户签约计划请求体
/// （<b>POST /v3/payscore/sign-plan/user-sign-plans/merchant-sign-plan-no/{merchant_sign_plan_no}/stop</b>）。
/// </summary>
/// <remarks>
/// <b>与「终止支付分计划」的差异</b>：本请求体必填 <c>stop_reason</c>（string(128)）；
/// 计划侧终止请求<b>无请求体</b>，两形态不共用。
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471050"/>，2026-10-18 核验。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreStopUserSignPlanRequest
{
    /// <summary>停止签约计划原因（<c>stop_reason</c>，必填 string(128)）。</summary>
    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }
}
