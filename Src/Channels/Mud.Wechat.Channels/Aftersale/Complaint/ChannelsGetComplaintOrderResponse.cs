// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「纠纷单」DTO（aftersale/getcomplaintorder、aftersale/addcomplaintmaterial、
// aftersale/addcomplaintproof、aftersale/syncworkorder）+ 纠纷单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>获取纠纷单详情（<c>aftersale/getcomplaintorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetComplaintOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置纠纷历史（官方 <c>history</c>）。</summary>
    [JsonPropertyName("history")]
    public List<ChannelsComplaintHistory>? History { get; set; }

    /// <summary>获取或设置纠纷单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsComplaintStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置虚拟号码信息（官方 <c>virtual_tel_num_info</c>，字段集与售后单共用）。</summary>
    [JsonPropertyName("virtual_tel_num_info")]
    public ChannelsVirtualTelNumInfo? VirtualTelNumInfo { get; set; }
}
