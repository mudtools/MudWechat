// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 订单详情（官方 <c>Res.order</c>，23 字段）。
/// </summary>
/// <remarks>
/// <para><b>金额单位是「分」但类型不统一</b>：<c>order_fee</c> / <c>paid_fee</c> / <c>refund_fee</c> / <c>left_fee</c> / <c>platform_fee_fen</c> / <c>cps_fee_fen</c> 全部 <c>number</c>（分），而提现侧的 <c>withdraw_amount</c> 是「元」为单位的 <c>string</c> ⇒ 跨域对账须先换算，不得直接相加。</para>
/// <para><b><c>sett_time</c> 的判据是「大于 0」</b>（官方原文）而非非空 ⇒ 未结算时官方回 0，不是缺字段。</para>
/// <para><b><c>platform_fee_fen</c> / <c>cps_fee_fen</c> 仅在 <c>sett_state = 2</c> 时有值</b>（官方原文）⇒ 分成核算必须先看结算状态，否则把 0 误读成「无服务费」。</para>
/// <para><b>敏感值</b>：<c>token</c> 是下单时米大师返回的凭证 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayOrderInfo
{
    /// <summary>order_id（官方类型 <c>string</c>）：订单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：创建时间</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>update_time（官方类型 <c>number</c>）：更新时间</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：当前状态</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>biz_type（官方类型 <c>number</c>）：业务类型0-短剧</summary>
    [JsonPropertyName("biz_type")]
    public long? BizType { get; set; }

    /// <summary>order_fee（官方类型 <c>number</c>）：订单金额，单位分</summary>
    [JsonPropertyName("order_fee")]
    public long? OrderFee { get; set; }

    /// <summary>coupon_fee（官方类型 <c>number</c>）：订单优惠金额，单位分(暂无此字段)</summary>
    [JsonPropertyName("coupon_fee")]
    public long? CouponFee { get; set; }

    /// <summary>paid_fee（官方类型 <c>number</c>）：用户支付金额</summary>
    [JsonPropertyName("paid_fee")]
    public long? PaidFee { get; set; }

    /// <summary>order_type（官方类型 <c>number</c>）：订单类型</summary>
    [JsonPropertyName("order_type")]
    public long? OrderType { get; set; }

    /// <summary>refund_fee（官方类型 <c>number</c>）：当类型为退款单时表示退款金额，单位分</summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }

    /// <summary>paid_time（官方类型 <c>number</c>）：支付/退款时间，unix秒级时间戳</summary>
    [JsonPropertyName("paid_time")]
    public long? PaidTime { get; set; }

    /// <summary>provide_time（官方类型 <c>number</c>）：发货时间</summary>
    [JsonPropertyName("provide_time")]
    public long? ProvideTime { get; set; }

    /// <summary>biz_meta（官方类型 <c>string</c>）：订单创建时传的信息</summary>
    [JsonPropertyName("biz_meta")]
    public string? BizMeta { get; set; }

    /// <summary>env_type（官方类型 <c>number</c>）：环境类型1-现网 2-沙箱</summary>
    [JsonPropertyName("env_type")]
    public long? EnvType { get; set; }

    /// <summary>token（官方类型 <c>string</c>）：下单时米大师返回的token</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>left_fee（官方类型 <c>number</c>）：支付单类型时表示此单经过退款还剩余的金额，单位分</summary>
    [JsonPropertyName("left_fee")]
    public long? LeftFee { get; set; }

    /// <summary>wx_order_id（官方类型 <c>string</c>）：微信内部单号</summary>
    [JsonPropertyName("wx_order_id")]
    public string? WxOrderId { get; set; }

    /// <summary>channel_order_id（官方类型 <c>string</c>）：渠道单号，为用户微信支付详情页面上的商户单号</summary>
    [JsonPropertyName("channel_order_id")]
    public string? ChannelOrderId { get; set; }

    /// <summary>wxpay_order_id（官方类型 <c>string</c>）：微信支付交易单号，为用户微信支付详情页面上的交易单号</summary>
    [JsonPropertyName("wxpay_order_id")]
    public string? WxpayOrderId { get; set; }

    /// <summary>sett_time（官方类型 <c>number</c>）：结算时间的秒级时间戳，大于0表示结算成功</summary>
    [JsonPropertyName("sett_time")]
    public long? SettTime { get; set; }

    /// <summary>sett_state（官方类型 <c>number</c>）：结算状态0-未开始结算 1-结算中 2-结算成功 3-待结算（与0相同）</summary>
    [JsonPropertyName("sett_state")]
    public long? SettState { get; set; }

    /// <summary>platform_fee_fen（官方类型 <c>number</c>）：虚拟支付技术服务费，单位为分；sett_state = 2时返回</summary>
    [JsonPropertyName("platform_fee_fen")]
    public long? PlatformFeeFen { get; set; }

    /// <summary>cps_fee_fen（官方类型 <c>number</c>）：公众号、视频号平台的cps服务费，单位为分；sett_state = 2时返回</summary>
    [JsonPropertyName("cps_fee_fen")]
    public long? CpsFeeFen { get; set; }
}
