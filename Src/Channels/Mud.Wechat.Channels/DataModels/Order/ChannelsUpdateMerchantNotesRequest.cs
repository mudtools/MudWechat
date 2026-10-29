// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「订单主文档」DTO（order/get、order/list/get、order/search、order/price/update、
// order/address/update、order/addressmodify/*、order/merchantnotes/update、order/presentnote/add、
// order/presentsuborder/get、order/preshipmentchangesku/*、order/blindboxitem/set 等核心端点）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>修改订单备注（<c>order/merchantnotes/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUpdateMerchantNotesRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置备注内容（官方 <c>merchant_notes</c>，必填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string MerchantNotes { get; set; } = string.Empty;

    /// <summary>获取或设置备注标签颜色（官方 <c>merchant_notes_tag_color</c>，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("merchant_notes_tag_color")]
    public int? MerchantNotesTagColor { get; set; }
}
