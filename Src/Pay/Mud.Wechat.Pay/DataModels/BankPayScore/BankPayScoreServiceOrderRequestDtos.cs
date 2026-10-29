// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.BankPayScore;

/// <summary>
/// 创建支付分订单请求体（<b>POST /v3/payscore/acquiringbank/serviceorder</b>，免确认模式）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691326"/>，2026-10-14 逐字段核验。
/// <para><b><c>need_user_confirm</c> 必填且固定 <c>false</c></b>：本域只开放免确认模式，传 <c>true</c> 属参数错。</para>
/// <para><b><c>openid</c> / <c>sub_openid</c> 二选一必传</b>：传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填
/// （守卫 BS-B3 锁定该组合）。</para>
/// <para><c>risk_fund</c> 与 <c>time_range</c> 在本域为<b>必填</b>，且官方对 <c>time_range</c> 的时间精度有校验：
/// <c>yyyyMMdd</c> 精确到日、<c>yyyyMMddHHmmss</c> 精确到秒，<c>end_time</c> 须与 <c>start_time</c> 同格式。</para>
/// <para><b>业务限制</b>：用户达周期内创单上限时官方返回 <c>403 RULE_LIMIT</c>，须用户主动在支付分「服务管理页」去除限制或次日再下单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCreateOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>从业机构公众账号 APPID（<c>appid</c>，必填 string(32)）：完结/取消订单须与创单传入的一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）：传 <c>sub_openid</c> 时必填。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）：同一商户号下唯一。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填 array，最多 100 条）。</summary>
    [JsonPropertyName("post_payments")]
    public List<BankPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填 array，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<BankPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，<b>必填</b> object）。</summary>
    [JsonPropertyName("risk_fund")]
    public BankPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，<b>必填</b> object）：紧凑格式时间，非 rfc3339。</summary>
    [JsonPropertyName("time_range")]
    public BankPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填 object）。</summary>
    [JsonPropertyName("location")]
    public BankPayScoreLocation? Location { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：与 <c>sub_openid</c> 二选一必传。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户标识（<c>sub_openid</c>，选填 string(128)）：与 <c>openid</c> 二选一必传，传此值则 <c>sub_appid</c> 必填。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>是否需要用户确认（<c>need_user_confirm</c>，<b>必填</b> boolean）：官方固定 <c>false</c>。</summary>
    [JsonPropertyName("need_user_confirm")]
    public bool? NeedUserConfirm { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，<b>必填</b> string(256)）：接收「商户预下单」与「支付成功」两支回调通知。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）：须先 urlencode，用户不可见，支付成功回调原样回传。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }
}

/// <summary>
/// 取消支付分订单请求体（<b>POST …/serviceorder/{out_order_no}/cancel</b>，成功 <b>204 无包体</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458146"/>，2026-10-14 逐字段核验。
/// <para><c>service_id</c> 在本端点为<b>选填</b>（与完结/修改/同步的必填不同形），
/// 官方必填组合是 <c>sub_mchid</c> + <c>channel_id</c> + <c>reason</c>。</para>
/// <para><c>reason</c> 会展示在用户收到的取消通知消息里，≤50 字符。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCancelOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）：会展示给用户。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 完结支付分订单请求体（<b>POST …/serviceorder/{out_order_no}/complete</b>，成功 <b>204 无包体</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460791"/>，2026-10-14 逐字段核验。
/// <para><b>金额必须自洽</b>：<c>total_amount = post_payments.amount 总和 − post_discounts.amount 总和</c>，
/// 且 ≤「服务 ID 风险金额上限」。</para>
/// <para><c>post_payments</c> 与 <c>total_amount</c> 必填；<c>time_range</c>/<c>location</c>/<c>complete_time</c> 选填。</para>
/// <para><c>complete_time</c> 是<b>本端点独有的 rfc3339 字段</b>（ISO 8601 四种写法均可），
/// 官方约束落在 <c>[t−24h, t+60s]</c>，<c>t</c> 为调用完结接口的当前时间。
/// 同页 <c>location</c> 子字段官方写 <c>string(20)</c> 而创单页写 <c>string(50)</c> —— 照原文共用一个类型，不拆分。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCompleteOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b> array，最多 100 条）。</summary>
    [JsonPropertyName("post_payments")]
    public List<BankPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填 array，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<BankPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，<b>必填</b> integer）：单位为分，须等于后付费总和减优惠总和。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填 object）。</summary>
    [JsonPropertyName("time_range")]
    public BankPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填 object）。</summary>
    [JsonPropertyName("location")]
    public BankPayScoreLocation? Location { get; set; }

    /// <summary>完结服务时间（<c>complete_time</c>，选填）：ISO 8601 / rfc3339 字符串，须落在 <c>[t−24h, t+60s]</c>。</summary>
    [JsonPropertyName("complete_time")]
    public string? CompleteTime { get; set; }
}

/// <summary>
/// 修改支付分订单金额请求体（<b>POST …/serviceorder/{out_order_no}/modify</b>，成功 <b>204 无包体</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691363"/>，2026-10-14 逐字段核验
/// （SKIT 清单该行误挂「取消订单页」4012458146，官方修改金额有独立页）。
/// <para><b>可调用窗口</b>：仅 <c>state=DOING</c> 且 <c>state_description=MCH_COMPLETE</c>，且只能<b>下调</b>收款金额。</para>
/// <para><b>扣款单互斥</b>：存在登记生效的扣款单时须等其 24h 失效、或先调清算机构（银联/网联）关单，才能改金额。</para>
/// <para>官方特别警示：本端点的 <c>out_order_no</c> <b>不可</b>用作申请退款接口的 <c>out_trade_no</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreModifyOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b> array）。</summary>
    [JsonPropertyName("post_payments")]
    public List<BankPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填 array，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<BankPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，<b>必填</b> integer）：单位为分，须等于后付费总和减优惠总和。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>修改原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 同步支付分订单状态请求体（<b>POST …/serviceorder/{out_order_no}/sync</b>，成功 <b>204 无包体</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691378"/>，2026-10-14 逐字段核验。
/// <para>用于「用户已走<b>其它渠道</b>付款」的补登记：<c>type</c> 官方只开放
/// <see cref="BankPayScoreSyncTypes.OrderPaid"/> 一值，<c>detail</c> 在该 <c>type</c> 下必填。</para>
/// <para><b>与完结同样的窗口与扣款单约束</b>：仅 <c>state=DOING</c> 且 <c>state_description=MCH_COMPLETE</c> 可调，
/// 存在生效扣款单时须先失效或关单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreSyncOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>场景类型（<c>type</c>，必填 string(32)）：固定 <c>Order_Paid</c>，见 <see cref="BankPayScoreSyncTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>内容信息详情（<c>detail</c>，<b>必填</b> object）：<c>type=Order_Paid</c> 时必填。</summary>
    [JsonPropertyName("detail")]
    public BankPayScoreSyncDetail? Detail { get; set; }
}

/// <summary>
/// 登记扣款信息请求体（<b>POST …/serviceorder/{out_order_no}/registerdeductinfo</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458414"/>，2026-10-14 逐字段核验。
/// <para><b>本端点是「清算机构（银联/网联）委托代扣」的前置登记</b>：登记的 <c>out_trade_no</c> 与
/// <c>description</c> 必须与后续调用清算机构代扣接口时传的商户订单号、商品描述<b>逐字一致</b>，否则代扣失败。</para>
/// <para><b>时效 24 小时</b>：生效期间该订单<b>不得</b>修改金额（modify）或同步状态（sync），
/// 须等扣款单失效或先走清算机构关单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreRegisterDeductRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>从业机构公众账号 APPID（<c>appid</c>，必填 string(32)）：须与创单传入的一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>商户扣款单号（<c>out_trade_no</c>，必填 string(32)）：须与清算机构代扣接口的商户订单号一致。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
