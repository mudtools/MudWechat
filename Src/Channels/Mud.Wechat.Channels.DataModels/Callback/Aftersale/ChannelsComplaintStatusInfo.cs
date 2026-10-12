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

/// <summary>纠纷单状态事件主体（官方 <c>finder_shop_complaint</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsComplaintStatusInfo
{
    /// <summary>
    /// 获取或设置纠纷单状态（官方 <c>complaint_status</c>，数字枚举）：100 待商家处理纠纷；101 待客服处理；
    /// 102 取消客服介入；103 客服处理中；104 待用户补充凭证；105 用户已补充凭证；106 待商家补充凭证；
    /// 107 商家已补充凭证；108 待双方补充凭证；109 双方补充凭证超时；110 待商家确认；111 商家申诉中；
    /// 112 调解完成；113 客服核实；114 重新退款中；115 退款核实完成；116 调解关闭；
    /// 305 用户补充凭证超时；307 商家补充凭证超时。
    /// </summary>
    [JsonPropertyName("complaint_status")]
    public long? ComplaintStatus { get; set; }

    /// <summary>获取或设置小店售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>）。</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>获取或设置小店订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
