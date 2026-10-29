// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerPayScore;

/// <summary>
/// 服务商创建支付分订单请求体（<b>POST /v3/payscore/partner/serviceorder</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>，2026-10-14 逐字段核验。
/// 官方注明<b>支持原参重入</b>：相同参数重复调用返回成功（幂等）。</para>
/// <para><b>本族 AppID 形态与服务商基础支付相反</b>：本域用 <c>appid</c>（服务商）+ <c>sub_appid</c>（子商户），
/// <b>没有</b> <c>sp_appid</c>/<c>sp_mchid</c>；应答里的服务商商户号叫 <c>mchid</c> 而非 <c>sp_mchid</c>
/// —— 与 <c>PartnerJsapiPrepayRequest</c> 的商户标识四分形态<b>不可互抄</b>（守卫 PS-B3 锁定）。</para>
/// <para><b><c>openid</c> 与 <c>sub_openid</c> 二选一</b>：传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填。
/// 微信支付分用该标识判断用户年龄门槛（电商场景需满 18 周岁，其余 14 周岁）。</para>
/// <para><b><c>need_user_confirm</c> 在本域固定 <c>false</c></b>（免确认模式）：官方字段表原文「固定传 false」，
/// 填 <c>true</c> 属非法入参，不得据直连「预确认模式」经验补值。</para>
/// <para><b><c>out_order_no</c> 不是交易单号</b>：官方原文警告该参数<b>不可</b>用于申请退款接口的
/// <c>out_trade_no</c>；退款须用查询应答 <c>collection.details[].transaction_id</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCreateOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：由微信支付运营在权限审核通过后下发。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）：须与服务商户号有绑定关系；<b>完结与取消订单须与创单传入值一致</b>。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）：普通服务商为特约商户号，平台收付通为二级商家号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）：上送 <c>sub_openid</c> 时必填。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>
    /// 商户服务订单号（<c>out_order_no</c>，必填 string(32)）：32 字符内、仅数字大小写字母与 <c>_-|*</c>、
    /// 同一商户号下唯一；<b>不得</b>当作退款接口的 <c>out_trade_no</c> 使用。
    /// </summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）：汉字/数字/字母/特殊符号均按 1 字符计。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填，最多 100 条）：须按所属行业规程传参。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条）：完结时收款总额须等于后付费之和减优惠之和。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，<b>必填</b>），见 <see cref="PartnerPayScoreRiskFund"/>；<c>amount</c> 影响评估通过率。</summary>
    [JsonPropertyName("risk_fund")]
    public PartnerPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间（<c>time_range</c>，<b>必填</b>），见 <see cref="PartnerPayScoreTimeRange"/>；本节走紧凑时间格式而非 rfc3339。</summary>
    [JsonPropertyName("time_range")]
    public PartnerPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填），见 <see cref="PartnerPayScoreLocation"/>。</summary>
    [JsonPropertyName("location")]
    public PartnerPayScoreLocation? Location { get; set; }

    /// <summary>用户在服务商 <c>appid</c> 下的标识（<c>openid</c>，选填 string(128)）：与 <c>sub_openid</c> 二选一。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户在子商户 <c>sub_appid</c> 下的标识（<c>sub_openid</c>，选填 string(128)）：传则 <c>sub_appid</c> 必填。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>是否需要用户确认（<c>need_user_confirm</c>，必填 boolean）：官方本域<b>固定传 <c>false</c></b>。</summary>
    [JsonPropertyName("need_user_confirm")]
    public bool? NeedUserConfirm { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，必填 string(256)）：接收支付成功回调通知，须符合官方 notify_url 填写规范。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）：用户不可见，须先 urlencode，支付成功回调原样回传。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>设备信息（<c>device</c>，选填），见 <see cref="PartnerPayScoreDevice"/>；无人自助设备行业必传设备 ID。</summary>
    [JsonPropertyName("device")]
    public PartnerPayScoreDevice? Device { get; set; }
}

/// <summary>
/// 服务商取消支付分订单请求体（<b>POST /v3/payscore/partner/serviceorder/{out_order_no}/cancel</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458758"/>，2026-10-14 逐字段核验。</para>
/// <para><b><c>service_id</c> 在此为选填</b>（创单/完结页为必填）：照录官方，不得「对齐」成必填。</para>
/// <para><b>本端点应答为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>，不建响应类型。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCancelOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）：用户微信收到的取消订单通知会展示该字段。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 服务商完结支付分订单请求体（<b>POST /v3/payscore/partner/serviceorder/{out_order_no}/complete</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692408"/>，2026-10-14 逐字段核验。</para>
/// <para><b><c>service_id</c> 在本页必填</b>（取消页与修改页为选填）—— 三页字段表各不同，勿互相复制。</para>
/// <para><b>收款总额校验式</b>：<c>total_amount</c> = <c>post_payments.amount</c> 之和 − <c>post_discounts.amount</c> 之和，
/// 且 ≤ 服务 ID 风险金额上限（上限与 BD 确认）。</para>
/// <para><b>两套时间格式在本请求内并存</b>：<c>time_range</c> 用 <c>yyyyMMddHHmmss</c>/<c>yyyyMMdd</c>，
/// 而 <c>complete_time</c> 用 rfc3339（官方原文「ISO 8601」）。</para>
/// <para><b>本端点应答为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCompleteOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，<b>必填</b> string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b>，最多 100 条）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填整型，单位分）：须满足官方收款总额校验式且 ≤ 服务 ID 风险金额上限。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填）：完结页对 <c>end_time</c> 额外要求「不晚于调完结接口的当前时间」。</summary>
    [JsonPropertyName("time_range")]
    public PartnerPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PartnerPayScoreLocation? Location { get; set; }

    /// <summary>
    /// 分账标记（<c>profit_sharing</c>，选填 boolean）：<c>true</c> 表示收款后<b>冻结资金</b>待请求分账；
    /// 未分账且距支付完成超 30 天资金自动解冻。不传默认 <c>false</c>。
    /// </summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

    /// <summary>
    /// 完结服务时间（<c>complete_time</c>，选填 string(64)，<b>rfc3339</b>）：
    /// 官方要求须<b>晚于或等于 <c>end_time</c>、且早于或等于调接口当前时间</b>。
    /// </summary>
    [JsonPropertyName("complete_time")]
    public string? CompleteTime { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）：代金券配置了标记时必须传其一才可用券。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>设备信息（<c>device</c>，选填），见 <see cref="PartnerPayScoreDevice"/>。</summary>
    [JsonPropertyName("device")]
    public PartnerPayScoreDevice? Device { get; set; }
}

/// <summary>
/// 服务商修改订单金额请求体（<b>POST /v3/payscore/partner/serviceorder/{out_order_no}/modify</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692360"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与完结页的三处差异</b>：本族页 <c>service_id</c> 为<b>选填</b>；<b>无</b> <c>time_range</c>/<c>location</c>/
/// <c>profit_sharing</c>/<c>complete_time</c>/<c>goods_tag</c>；<b>新增必填</b> <c>reason</c> ⇒ 两类型不共用。</para>
/// <para><b>本端点应答为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreModifyOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填整型，单位分）：官方同页给出与完结页一致的收款总额校验式。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>修改原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>设备信息（<c>device</c>，选填），见 <see cref="PartnerPayScoreDevice"/>。</summary>
    [JsonPropertyName("device")]
    public PartnerPayScoreDevice? Device { get; set; }
}

/// <summary>
/// 服务商「下发服务费用待处理消息」与「下发服务预扣费消息」的共用请求体
/// （<b>POST /v3/payscore/partner/serviceorder/{out_order_no}/send-pendingmessage</b> 与
/// <b>…&#47;send-prepaidmessage</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013394914"/>（待处理消息）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013394929"/>（预扣费消息），2026-10-14 逐页比对：
/// 两页 <c>path</c> + <c>body</c> 参数表<b>逐项一致</b> ⇒ 按「表相同则共用」合并为一个类型（守卫 PS-B6 锁定该共用关系）。</para>
/// <para><b>本族 <c>post_payments</c> 必填、<c>service_id</c> 必填</b>，且<b>没有</b> <c>time_range</c>/
/// <c>location</c>/<c>device</c> —— 与完结/修改页字段表不同，不得互抄。</para>
/// <para><b><c>prepaid_time</c> 只有 8 位</b>：官方 <c>string(8)</c> + <c>YYYYMMDD</c>（精确到日），
/// 且须<b>不早于调接口时间 + 7 天</b>；这与 <c>time_range</c> 的 14 位格式是第三种时间形态。</para>
/// <para><b>两支端点应答均为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreMessageRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：该服务 ID 须有本接口对应产品的权限。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)），由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b>，最多 100 条）：用于用户侧展示与完结时总额计算。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>预计扣费时间（<c>prepaid_time</c>，必填 <b>string(8)</b>，<c>YYYYMMDD</c>）：须 ≥ 调接口时间 + 7 天。</summary>
    [JsonPropertyName("prepaid_time")]
    public string? PrepaidTime { get; set; }
}

/// <summary>
/// 服务商同步服务订单状态请求体（<b>POST /v3/payscore/partner/serviceorder/{out_order_no}/sync</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458816"/>，2026-10-14 逐字段核验。
/// 用途：用户<b>在商户自有渠道</b>已付款后，把该笔收款同步给支付分（对应
/// <c>collection.details[].paid_type</c> = <see cref="PartnerPayScoreCollectionPaidTypes.Merchant"/>）。</para>
/// <para><b><c>type</c> 官方固定值</b>：<see cref="PartnerPayScoreSyncTypes.OrderPaid"/>（「Order_Paid」，
/// 大小写照官方原文，含下划线前的驼峰形态），不得凭「全大写枚举」直觉改写。</para>
/// <para><b>本端点应答为 204 无包体</b> ⇒ 接口返回 <see cref="Task"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreSyncOrderRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>场景类型（<c>type</c>，必填 string(32)）：商户<b>固定传</b> <see cref="PartnerPayScoreSyncTypes.OrderPaid"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>收款详情（<c>detail</c>，必填）：<c>type</c>=Order_Paid 时必填，见 <see cref="PartnerPayScoreSyncOrderDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public PartnerPayScoreSyncOrderDetail? Detail { get; set; }
}

/// <summary>同步订单请求的收款详情（<c>detail</c>）。</summary>
/// <remarks>
/// <b>时间窗口是硬约束</b>：官方要求 <c>商户调完结接口时间 &lt; paid_time ≤ 调同步接口时间 + 60 秒</c>；
/// 格式支持 <c>yyyyMMddHHmmss</c> 与 <c>yyyyMMdd</c>（后者按当天 0 点 0 分 0 秒计），<b>不是</b> rfc3339。
/// 微信支付分据此更新用户侧守约/负面记录 ⇒ 必须如实填写用户实际付款时间。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreSyncOrderDetail
{
    /// <summary>收款成功时间（<c>paid_time</c>，必填 string(64)，<c>yyyyMMddHHmmss</c> 或 <c>yyyyMMdd</c>）：受官方 60 秒窗口约束。</summary>
    [JsonPropertyName("paid_time")]
    public string? PaidTime { get; set; }
}
