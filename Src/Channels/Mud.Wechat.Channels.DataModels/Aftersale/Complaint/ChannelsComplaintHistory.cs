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

/// <summary>纠纷历史（官方 <c>getcomplaintorder.history</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsComplaintHistory
{
    /// <summary>获取或设置历史操作类型（官方 <c>item_type</c>，枚举值见官方文档）。</summary>
    [JsonPropertyName("item_type")]
    public int? ItemType { get; set; }

    /// <summary>获取或设置操作时间（官方 <c>time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>获取或设置相关文本内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置相关图片 media_id 列表（官方 <c>media_id_list</c>，此字段已废弃，请用 <see cref="MediaInfos"/>）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<string>? MediaIdList { get; set; }

    /// <summary>获取或设置售后类型（官方 <c>after_sale_type</c>）：1 仅退款；2 退货退款。</summary>
    [JsonPropertyName("after_sale_type")]
    public int? AfterSaleType { get; set; }

    /// <summary>获取或设置售后原因（官方 <c>after_sale_reason</c>，见 获取全量售后原因）。</summary>
    [JsonPropertyName("after_sale_reason")]
    public int? AfterSaleReason { get; set; }

    /// <summary>获取或设置多媒体信息列表（官方 <c>media_infos</c>）。</summary>
    [JsonPropertyName("media_infos")]
    public List<ChannelsMediaInfo>? MediaInfos { get; set; }
}
