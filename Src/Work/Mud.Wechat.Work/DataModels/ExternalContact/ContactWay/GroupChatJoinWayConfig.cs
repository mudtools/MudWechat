// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 客户群进群方式配置详情（<c>join_way</c>，获取客户群进群方式配置响应）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class GroupChatJoinWayConfig
{
    /// <summary>
    /// 获取或设置进群方式的配置 id。
    /// </summary>
    [JsonPropertyName("config_id")]
    public string? ConfigId { get; set; }

    /// <summary>
    /// 获取或设置入群方式场景：1 - 群的小程序插件，2 - 群的二维码插件。
    /// </summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>
    /// 获取或设置进群方式的备注信息。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置群满后是否自动新建群：0 - 否，1 - 是。
    /// </summary>
    [JsonPropertyName("auto_create_room")]
    public int? AutoCreateRoom { get; set; }

    /// <summary>
    /// 获取或设置自动新建群的群名前缀。
    /// </summary>
    [JsonPropertyName("room_base_name")]
    public string? RoomBaseName { get; set; }

    /// <summary>
    /// 获取或设置自动新建群的起始序号。
    /// </summary>
    [JsonPropertyName("room_base_id")]
    public long? RoomBaseId { get; set; }

    /// <summary>
    /// 获取或设置使用该进群方式的客户群 ID 列表。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }

    /// <summary>
    /// 获取或设置联系我二维码链接（scene = 2）或小程序插件链接（scene = 1）。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>
    /// 获取或设置自定义入群渠道参数。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源（仅对「营销获客」应用生效）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }
}
