// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 售后族（售后单更新 / 纠纷单更新 / 保障单更新）。
// 事件键：channels_ec_aftersale_update、channels_ec_complaint_update、channels_ec_guarantee_update。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>售后单更新事件主体（官方 <c>finder_shop_aftersale_status_update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsAftersaleStatusInfo
{
    /// <summary>
    /// 获取或设置售后单状态（官方 <c>status</c>，字符串枚举）：USER_CANCELD 用户取消申请；
    /// MERCHANT_PROCESSING 商家受理中；MERCHANT_REJECT_REFUND 商家拒绝退款；MERCHANT_REJECT_RETURN 商家拒绝退货退款；
    /// USER_WAIT_RETURN 待买家退货；RETURN_CLOSED 退货退款关闭；MERCHANT_WAIT_RECEIPT 待商家收货；
    /// MERCHANT_OVERDUE_REFUND 商家逾期未退款；MERCHANT_REFUND_SUCCESS 退款完成；MERCHANT_RETURN_SUCCESS 退货退款完成；
    /// PLATFORM_REFUNDING 平台退款中；PLATFORM_REFUND_FAIL 平台退款失败；USER_WAIT_CONFIRM 待用户确认；
    /// MERCHANT_REFUND_RETRY_FAIL 商家打款失败，客服关闭售后；MERCHANT_FAIL 售后关闭；
    /// USER_WAIT_CONFIRM_UPDATE 待用户处理商家协商；USER_WAIT_HANDLE_MERCHANT_AFTER_SALE 待用户处理商家代发起的售后申请；
    /// WAIT_PACKAGE_INTERCEPT 物流线上拦截中；MERCHANT_REJECT_EXCHANGE 商家拒绝换货；MERCHANT_REJECT_RESHIP 商家拒绝发货；
    /// USER_WAIT_RECEIPT 待用户收货；MERCHANT_EXCHANGE_SUCCESS 换货完成；PLATFORM_PROCESSING 平台审核中。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置小店售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
