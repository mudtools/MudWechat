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

/// <summary>保障单状态事件主体（官方 <c>finder_shop_guarantee_status_update</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsGuaranteeStatusInfo
{
    /// <summary>
    /// 获取或设置保障单状态（官方 <c>status</c>，字符串枚举）：STATUS_WAIT_MERCHANT_HANDLE 等待商家处理；
    /// STATUS_WAIT_PLATFORM_HANDLE 等待平台处理；STATUS_WAIT_USER_CONFIRM 等待用户确认；
    /// STATUS_WAIT_MERCHANT_PROOF 等待商家举证；STATUS_WAIT_USER_PROOF 等待用户举证；
    /// STATUS_WAIT_BOTH_PROOF 等待双方举证；STATUS_WAIT_OP_COMFIRM 等待 OP 确认（官方拼写 COMFIRM 照抄）；
    /// STATUS_WAIT_PAYSCORE_DONE 等待支付分付款；STATUS_NO_NEED_PAY 无需赔付；STATUS_PAYING 赔付中；
    /// STATUS_PAY_BLOCK 假一赔四金额异常，待确认；STATUS_PAY_SUCC 赔付成功；STATUS_PAY_FAIL 赔付失败；
    /// STATUS_USER_CANCEL 用户取消申请。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置小店保障单号（官方 <c>guarantee_order_id</c>）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public string? GuaranteeOrderId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
