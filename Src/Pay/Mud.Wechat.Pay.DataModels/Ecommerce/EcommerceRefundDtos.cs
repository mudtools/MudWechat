// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 申请退款·二级商户身份（<c>POST /v3/ecommerce/refunds/apply</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>同一路由有两份契约</b>：本页按 <c>sub_mchid</c> 定位身份，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/> 按 <c>individual_auth_id</c> 定位个人收款方，
/// 两支的请求表与应答表<b>均不重合</b> ⇒ 分建两个请求类型、两个应答类型，不做「一个类两套语义」。
/// </para>
/// <para>
/// <b><c>sub_mchid</c> 官方标选填、注「二级商户收款必填」</b>（条件必填）；
/// <c>transaction_id</c> 与 <c>out_trade_no</c> 均为选填 ⇒ 原交易定位由宿主二选一保证。
/// </para>
/// <para>
/// <b><c>amount.from</c> 与 <c>funds_account</c> 互斥</b>（官方原文「该功能优先级高于通过资金账户
/// （funds_account）参数来指定可用余额出资退款功能，且二者不可同时使用」），且指定多账户出资须满足
/// 「基本账户可用 + 不可用出资金额之和 = 退款金额」「账户类型不能重复」「订单属于分账订单且分账处于待分账或分账中」等条件。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundApplyRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，<b>选填</b> string(32)）：官方注明「二级商户收款必填」，属条件必填。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>电商平台 APPID（<c>sp_appid</c>，必填 string(32)）：申请商户功能时微信支付会配置绑定关系。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>二级商户 APPID（<c>sub_appid</c>，<b>选填</b> string(32)）：需电商平台侧有配置绑定关系才能传。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，<b>选填</b> string(32)）：原支付交易对应的微信订单号。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，<b>选填</b> string(32)）：原支付交易对应的商户订单号。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）：只能是数字、大小写字母 <c>-_|*@</c>，同一退款单号多次请求只退一笔。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>退款原因（<c>reason</c>，<b>选填</b> string(80)）：传入后会体现在下发给用户的退款消息中。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>订单金额信息（<c>amount</c>，必填）：含退款出资金额，见 <see cref="EcommerceRefundReqAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public EcommerceRefundReqAmount? Amount { get; set; }

    /// <summary>退款结果回调 URL（<c>notify_url</c>，<b>选填</b> string(256)）：须公网可访问、不能携带参数；传入则优先于商户平台配置的回调地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>退款出资商户（<c>refund_account</c>，<b>选填</b> string(32)）：见 <see cref="EcommerceRefundSources"/>；垫付值仅代表可用，实际出款以受理/查单为准。</summary>
    [JsonPropertyName("refund_account")]
    public string? RefundAccount { get; set; }

    /// <summary>资金账户（<c>funds_account</c>，<b>选填</b> string(32)）：见 <see cref="EcommerceRefundFundingAccounts"/>；与 <c>amount.from</c> 不可同时使用。</summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }
}

/// <summary>
/// 申请退款（二级商户身份）的订单金额入参（<c>amount</c>）。
/// </summary>
/// <remarks><b><c>refund</c> 与 <c>total</c> 必填、<c>from</c> 与 <c>currency</c> 选填</b>；币种官方目前只支持 <c>CNY</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundReqAmount
{
    /// <summary>退款金额（<c>refund</c>，必填 integer，<b>单位分</b>）：只能为整数，不能超过原订单支付金额。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>退款出资账户及金额（<c>from</c>，<b>选填</b>数组）：与 <c>funds_account</c> 互斥，账户类型不得重复。</summary>
    [JsonPropertyName("from")]
    public EcommerceRefundFunding[]? From { get; set; }

    /// <summary>原订单金额（<c>total</c>，必填 integer，<b>单位分</b>）：原支付交易的订单总金额。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>退款币种（<c>currency</c>，<b>选填</b> string(16)）：ISO 4217 三位代码，目前只支持 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>退款出资账户及金额（<c>amount.from[]</c>，申请退款·二级商户支）。</summary>
/// <remarks>
/// <b>本支两字段官方均标必填</b>，与补差回退支的同名结构（两字段均选填，见 <see cref="EcommerceSubsidyFunding"/>）
/// 必填性不同 ⇒ 分建类型；取值集共用 <see cref="EcommerceFundingAccountTypes"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundFunding
{
    /// <summary>出资账户类型（<c>account</c>，必填 string(32)）：<c>AVAILABLE</c> 可用余额 / <c>UNAVAILABLE</c> 不可用余额。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>出资金额（<c>amount</c>，必填 integer，<b>单位分</b>）：对应账户出资金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>
/// 申请退款受理应答·二级商户身份（19 行）。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>。
/// <b>受理应答没有 <c>status</c>、也没有 <c>transaction_id</c></b> ⇒
/// 收到本应答只表示「退款单已创建」，退款是否成功必须走查单（4012476908 / 4012476911）。
/// </para>
/// <para>
/// <b><c>refund_account</c> 在本应答是选填</b>：官方注明「申请退款时可能出现暂未扣除退款资金的情况，
/// 此时不返回本字段，需查单确认」⇒ 缺省不代表二级商户出资。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundApplyResponse : WechatPayResponse
{
    /// <summary>微信支付退款单号（<c>refund_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>退款创建时间（<c>create_time</c>，必填 string(64)）：退款受理时间，rfc3339 文本。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>退款金额信息（<c>amount</c>，必填）。</summary>
    [JsonPropertyName("amount")]
    public EcommerceRefundAmount? Amount { get; set; }

    /// <summary>优惠退款详情（<c>promotion_detail</c>，<b>选填</b>数组）。</summary>
    [JsonPropertyName("promotion_detail")]
    public EcommerceRefundPromotionDetail[]? PromotionDetail { get; set; }

    /// <summary>退款出资商户（<c>refund_account</c>，<b>选填</b> string(32)）：暂未扣除退款资金时不返回，见 <see cref="EcommerceRefundSources"/>。</summary>
    [JsonPropertyName("refund_account")]
    public string? RefundAccount { get; set; }
}

/// <summary>
/// 退款金额信息（应答侧，<b>二级商户支</b>形态：含出资账户与垫付金额）。
/// </summary>
/// <remarks>
/// <b>受理应答与查单应答的 <c>amount</c> 子表逐行相同</b>（2026-10-15 比对
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>）⇒ 共用一个类型。
/// <b><c>payer_refund</c> + <c>discount_refund</c> 与 <c>refund</c> 不是相等关系</b>：
/// 官方对 <c>payer_refund</c> 注明「不包含所有优惠券金额」，<c>discount_refund</c> 按比例退回。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundAmount
{
    /// <summary>退款金额（<c>refund</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>退款出资账户及金额（<c>from</c>，<b>选填</b>数组）：请求未指定出资账户时缺省。</summary>
    [JsonPropertyName("from")]
    public EcommerceRefundFunding[]? From { get; set; }

    /// <summary>用户退款金额（<c>payer_refund</c>，必填 integer，<b>单位分</b>）：退款给用户的金额，不含优惠券金额。</summary>
    [JsonPropertyName("payer_refund")]
    public long? PayerRefund { get; set; }

    /// <summary>优惠退款金额（<c>discount_refund</c>，<b>选填</b> integer，单位分）：原支付单的优惠按比例退款。</summary>
    [JsonPropertyName("discount_refund")]
    public long? DiscountRefund { get; set; }

    /// <summary>货币类型（<c>currency</c>，<b>选填</b> string(16)）：目前只支持 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>垫付金额（<c>advance</c>，<b>选填</b> integer，<b>单位分</b>）：电商平台垫付的金额。</summary>
    [JsonPropertyName("advance")]
    public long? Advance { get; set; }
}

/// <summary>
/// 优惠退款详情（<c>promotion_detail[]</c>）—— <b>本域退款族五支应答共用</b>。
/// </summary>
/// <remarks>
/// 2026-10-15 比对 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>：五行字段名、类型、必填性完全一致 ⇒ 共用。
/// <b>官方 <c>refund_amount</c> 的口径</b>：代金券退款金额 ≤ 退款金额，「退款金额 − 优惠退款金额 = 现金部分」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundPromotionDetail
{
    /// <summary>券 ID（<c>promotion_id</c>，必填 string(32)）：券或立减优惠 id。</summary>
    [JsonPropertyName("promotion_id")]
    public string? PromotionId { get; set; }

    /// <summary>优惠范围（<c>scope</c>，必填 string(32)）：见 <see cref="EcommercePromotionScopes"/>。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommercePromotionTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>优惠券面额（<c>amount</c>，必填 integer，<b>单位分</b>）：微信出资金额 + 商家出资金额 + 其他出资方金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>优惠退款金额（<c>refund_amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }
}

/// <summary>
/// 退款单查询应答·二级商户身份（<b>按 <c>refund_id</c> 查与按 <c>out_refund_no</c> 查两支的共用应答</b>，26 行）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476911"/> 的应答表 26 行<b>逐行相同</b>，
/// 差异只在定位键（<c>refund_id</c> vs <c>out_refund_no</c>）。
/// </para>
/// <para>
/// <b>比受理应答多出的字段</b>：<c>status</c>、<c>transaction_id</c>、<c>out_trade_no</c>、<c>channel</c>、
/// <c>user_received_account</c>、<c>success_time</c>、<c>funds_account</c> ⇒ 退款终态与资金去向只能从查单取。
/// </para>
/// <para>
/// <b><c>ABNORMAL</c> 的处置路径按官方指引是商户平台交易中心人工处理</b>（本身份支），
/// 与个人收款方支（走异常退款 API）不同。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRefundQueryResponse : WechatPayResponse
{
    /// <summary>微信支付退款单号（<c>refund_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>微信支付交易订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户原交易订单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>退款渠道（<c>channel</c>，<b>选填</b> string(16)）：见 <see cref="EcommerceRefundChannels"/>。</summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>退款入账账户（<c>user_received_account</c>，<b>选填</b> string(64)）：官方以「退款入账方」列举十余种形态，为展示用文本、非枚举。</summary>
    [JsonPropertyName("user_received_account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>退款成功时间（<c>success_time</c>，<b>选填</b> string(64)）：<c>status</c> 为 <c>SUCCESS</c> 时才返回；rfc3339 文本。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>退款创建时间（<c>create_time</c>，必填 string(64)）：退款受理时间，rfc3339 文本。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>退款状态（<c>status</c>，必填 string(16)）：见 <see cref="EcommerceRefundStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>退款金额信息（<c>amount</c>，必填）。</summary>
    [JsonPropertyName("amount")]
    public EcommerceRefundAmount? Amount { get; set; }

    /// <summary>营销详情（<c>promotion_detail</c>，<b>选填</b>数组）。</summary>
    [JsonPropertyName("promotion_detail")]
    public EcommerceRefundPromotionDetail[]? PromotionDetail { get; set; }

    /// <summary>退款出资商户（<c>refund_account</c>，<b>选填</b> string(32)）：见 <see cref="EcommerceRefundSources"/>。</summary>
    [JsonPropertyName("refund_account")]
    public string? RefundAccount { get; set; }

    /// <summary>资金账户（<c>funds_account</c>，<b>选填</b> string(32)）：应答侧词表见 <see cref="EcommerceRefundFundsAccountStates"/>。</summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }
}

/// <summary>
/// 申请退款·商品单个人收款方身份（<c>POST /v3/ecommerce/refunds/apply</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。与二级商户身份支共用路由、<b>不共用契约</b>。
/// </para>
/// <para>
/// <b>本页 <c>funds_account</c> 必填且官方只列 <c>UNSETTLED</c></b>（未结算资金）；
/// 二级商户支该字段选填且另有 <c>AVAILABLE</c> / <c>PREPAID</c> ⇒ 同名字段的必填性与取值集同时分叉。
/// </para>
/// <para>
/// <b>本页多出 <c>individual_auth_id</c> 与 <c>combine_transaction_id</c> 两个必填键</b>，
/// 且没有 <c>sub_mchid</c> / <c>sub_appid</c> / <c>refund_account</c> / <c>amount.from</c> 四项。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualRefundApplyRequest
{
    /// <summary>商品单个人收款方受理授权 ID（<c>individual_auth_id</c>，必填 string(32)）：原支付交易对应的个人收款方受理授权 ID。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>电商平台 APPID（<c>sp_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>交易单微信支付订单号（<c>combine_transaction_id</c>，必填 string(32)）：从支付通知或查询订单返回参数中获取。</summary>
    [JsonPropertyName("combine_transaction_id")]
    public string? CombineTransactionId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，<b>必填</b> string(32)）：与二级商户支（选填）不同。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）：同一退款单号多次请求只退一笔。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>退款原因（<c>reason</c>，<b>选填</b> string(80)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>订单金额信息（<c>amount</c>，必填）：本页该对象<b>没有</b> <c>from</c> 出资账户子表。</summary>
    [JsonPropertyName("amount")]
    public EcommerceIndividualRefundReqAmount? Amount { get; set; }

    /// <summary>退款结果回调 URL（<c>notify_url</c>，<b>选填</b> string(256)）：传入则优先于商户平台配置的回调地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>资金账户（<c>funds_account</c>，<b>必填</b>）：本页官方只提供 <c>UNSETTLED</c>（未结算资金）。</summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }
}

/// <summary>
/// 申请退款（个人收款方身份）的订单金额入参（<c>amount</c>）：官方类型名 <c>RefundReqAmount</c>，三行无 <c>from</c>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualRefundReqAmount
{
    /// <summary>退款金额（<c>refund</c>，必填 integer，<b>单位分</b>）：不能超过原订单支付金额。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>原订单金额（<c>total</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>退款币种（<c>currency</c>，<b>选填</b> string(16)）：目前只支持 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 申请退款受理应答·个人收款方身份（14 行）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>。
/// <b>同样没有 <c>status</c></b>，且金额子表比二级商户支少 <c>from</c> 与 <c>advance</c>（个人收款无垫付）⇒
/// 终态与资金去向须查单（4012650253 / 4012650314）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualRefundApplyResponse : WechatPayResponse
{
    /// <summary>微信支付退款单号（<c>refund_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>退款创建时间（<c>create_time</c>，必填 string(64)）：rfc3339 文本。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>退款金额信息（<c>amount</c>，必填）。</summary>
    [JsonPropertyName("amount")]
    public EcommerceIndividualRefundAmount? Amount { get; set; }

    /// <summary>优惠退款详情（<c>promotion_detail</c>，<b>选填</b>数组）。</summary>
    [JsonPropertyName("promotion_detail")]
    public EcommerceRefundPromotionDetail[]? PromotionDetail { get; set; }
}

/// <summary>
/// 退款金额信息（应答侧，<b>个人收款方支</b>形态：无出资账户、无垫付金额）。
/// </summary>
/// <remarks>
/// <b>受理应答（4012649457）与查单应答（4012650253 / 4012650314 / 4012649274）的 <c>amount</c> 子表逐行相同</b>
/// （四行 <c>refund</c> / <c>payer_refund</c> / <c>discount_refund</c> / <c>currency</c>）⇒ 共用一个类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualRefundAmount
{
    /// <summary>退款金额（<c>refund</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>用户退款金额（<c>payer_refund</c>，必填 integer，<b>单位分</b>）：不含优惠券金额。</summary>
    [JsonPropertyName("payer_refund")]
    public long? PayerRefund { get; set; }

    /// <summary>优惠退款金额（<c>discount_refund</c>，<b>选填</b> integer，单位分）。</summary>
    [JsonPropertyName("discount_refund")]
    public long? DiscountRefund { get; set; }

    /// <summary>退款币种（<c>currency</c>，<b>选填</b> string(16)）：目前只支持 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 退款单查询应答·个人收款方身份（21 行）—— <b>按 <c>refund_id</c> 查、按 <c>out_refund_no</c> 查、发起异常退款三支的共用应答</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 三页逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650314"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/> 的应答表<b>21 行逐行相同</b>
/// （含 <c>status</c> 行对 <c>ABNORMAL</c> 的处置指引文本亦一致指向异常退款 API）。
/// </para>
/// <para>
/// <b>与二级商户支查单应答（26 行）的差集恰是五行</b>：<c>amount.from</c>（含子表两行）、<c>amount.advance</c>、
/// <c>refund_account</c> ⇒ 个人收款方无垫付退款概念，故不建「一个超集类型」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualRefundQueryResponse : WechatPayResponse
{
    /// <summary>微信支付退款单号（<c>refund_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>微信支付交易订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户原交易订单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>退款渠道（<c>channel</c>，<b>选填</b> string(16)）：见 <see cref="EcommerceRefundChannels"/>。</summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>退款入账账户（<c>user_received_account</c>，<b>选填</b> string(64)）：展示用文本，非枚举。</summary>
    [JsonPropertyName("user_received_account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>退款成功时间（<c>success_time</c>，<b>选填</b> string(64)）：<c>status=SUCCESS</c> 时才返回；rfc3339 文本。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>退款创建时间（<c>create_time</c>，必填 string(64)）：rfc3339 文本。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>退款状态（<c>status</c>，必填 string(16)）：见 <see cref="EcommerceRefundStatuses"/>；本身份支的 <c>ABNORMAL</c> 指向异常退款 API。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>退款金额信息（<c>amount</c>，必填）。</summary>
    [JsonPropertyName("amount")]
    public EcommerceIndividualRefundAmount? Amount { get; set; }

    /// <summary>营销详情（<c>promotion_detail</c>，<b>选填</b>数组）。</summary>
    [JsonPropertyName("promotion_detail")]
    public EcommerceRefundPromotionDetail[]? PromotionDetail { get; set; }

    /// <summary>资金账户（<c>funds_account</c>，<b>选填</b>）：应答侧词表见 <see cref="EcommerceRefundFundsAccountStates"/>。</summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }
}

/// <summary>
/// 垫付回补请求（<c>POST /v3/ecommerce/refunds/{refund_id}/return-advance</c>）：官方入参只有 <c>sub_mchid</c> 一行。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476927"/>（2026-10-15 核验；支持商户：【平台商户】）。
/// 回补对象由路径 <c>refund_id</c> 指定且官方注明<b>必须是垫付退款的微信退款单</b> ⇒ 非垫付单回补会被拒。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceReturnAdvanceRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：微信支付分配给二级商户的商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}

/// <summary>
/// 垫付回补结果（<c>GET</c> 查询回补 与 <c>POST</c> 发起回补 <b>两支的共用应答</b>，9 行）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476916"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476927"/> 的应答表 9 行逐行相同 ⇒ 共用一个类型。
/// </para>
/// <para>
/// <b>两支的 <c>refund_id</c> 定位方式不同</b>：GET 走路径 <c>refund_id</c> + query <c>sub_mchid</c>，
/// POST 走路径 <c>refund_id</c> + body <c>sub_mchid</c> ⇒ 同一凭据两种传法，照页实现。
/// </para>
/// <para>
/// <b><c>payer_*</c> 是出款方、<c>payee_*</c> 是入账方</b>：回补方向与退款相反（资金回到垫付方），
/// 两对字段名是本页最易读反的地方。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceReturnAdvanceResponse : WechatPayResponse
{
    /// <summary>微信退款单号（<c>refund_id</c>，必填 string(32)）：必须是垫付退款的微信退款单。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>微信回补单号（<c>advance_return_id</c>，必填 string(32)）：微信支付生成的垫付回补操作单号。</summary>
    [JsonPropertyName("advance_return_id")]
    public string? AdvanceReturnId { get; set; }

    /// <summary>垫付回补金额（<c>return_amount</c>，必填 integer，<b>单位分</b>）：退款单对应的垫付退款金额。</summary>
    [JsonPropertyName("return_amount")]
    public long? ReturnAmount { get; set; }

    /// <summary>出款方商户号（<c>payer_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("payer_mchid")]
    public string? PayerMchId { get; set; }

    /// <summary>出款方账户（<c>payer_account</c>，必填 string(32)）：见 <see cref="EcommerceAdvanceAccountTypes"/>。</summary>
    [JsonPropertyName("payer_account")]
    public string? PayerAccount { get; set; }

    /// <summary>入账方商户号（<c>payee_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("payee_mchid")]
    public string? PayeeMchId { get; set; }

    /// <summary>入账方账户（<c>payee_account</c>，必填 string(32)）：见 <see cref="EcommerceAdvanceAccountTypes"/>。</summary>
    [JsonPropertyName("payee_account")]
    public string? PayeeAccount { get; set; }

    /// <summary>垫付回补结果（<c>result</c>，必填 string(32)）：见 <see cref="EcommerceReturnAdvanceResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>垫付回补完成时间（<c>success_time</c>，<b>选填</b> string(64)）：rfc3339 文本。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }
}

/// <summary>
/// 发起异常退款（<c>POST /v3/ecommerce/refunds/{refund_id}/apply-abnormal-refund</c>，个人收款方身份）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。用于退款状态为 <c>ABNORMAL</c>（原路退银行卡失败）的单据。
/// </para>
/// <para>
/// <b>本接口是 <c>Wechatpay-Serial</c> 必填的端点之一</b>：<c>bank_account</c>（string(1024)）与
/// <c>real_name</c>（string(1024)）必须以微信支付公钥（推荐）或平台证书公钥加密后传入。
/// 两个字段均属敏感个人信息，<b>不得进日志、遥测或异常消息</b>。
/// </para>
/// <para>
/// <b><c>type</c> 官方只提供一个取值 <c>USER_BANK_CARD</c></b>（原文「个人收款只支持退款至用户银行账户」）；
/// <c>bank_type</c> 在「退款至用户」时必填，且官方限定支持银行列表（借记卡）。
/// </para>
/// <para>
/// <b>与直连退款域的同名端点不是同一契约</b>：<c>/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund</c>
/// 属普通服务商（<c>4013080627</c>，已由 Refund 域承载），本支是收付通个人收款方专用路由。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceAbnormalRefundRequest
{
    /// <summary>商品单个人收款方受理授权 ID（<c>individual_auth_id</c>，必填 string(32)）：原支付交易对应的受理授权 ID。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）：商户系统内部唯一，同一退款单号多次请求只退一笔。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>异常退款处理方式（<c>type</c>，必填）：见 <see cref="EcommerceAbnormalRefundTypes"/>，官方只提供 <c>USER_BANK_CARD</c>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>开户银行（<c>bank_type</c>，<b>选填</b> string(16)）：退款至用户银行卡时<b>必填</b>；官方限定支持银行的借记卡。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>
    /// 收款银行卡号（<c>bank_account</c>，<b>选填</b> string(1024)）：<b>密文</b>，须以微信支付公钥（推荐）或平台证书公钥加密。
    /// 敏感字段，禁止写入日志、遥测或异常消息。
    /// </summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>
    /// 收款用户姓名（<c>real_name</c>，<b>选填</b> string(1024)）：<b>密文</b>，加密要求同 <see cref="BankAccount"/>。
    /// 敏感字段，禁止写入日志、遥测或异常消息。
    /// </summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }
}
