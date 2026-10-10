// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 更新客户群进群方式配置请求体（<c>/cgi-bin/externalcontact/groupchat/update_join_way</c>）。
/// <para><see cref="ConfigId"/> / <see cref="Scene"/> / <see cref="ChatIdList"/> 为官方必填；
/// 该接口采用覆盖方式更新，传入字段将整体覆盖原配置；
/// <see cref="MarkSource"/> 只能由创建此二维码的应用更新。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class UpdateGroupChatJoinWayRequest
{
    /// <summary>
    /// 获取或设置企业联系方式的配置 id（官方必填）。
    /// </summary>
    [JsonPropertyName("config_id")]
    public string? ConfigId { get; set; }

    /// <summary>
    /// 获取或设置入群方式场景（官方必填）：1 - 群的小程序插件，2 - 群的二维码插件。
    /// </summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>
    /// 获取或设置进群方式的备注信息（最多 30 个字符，超长截断）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 获取或设置群满后是否自动新建群：0 - 否，1 - 是（默认为 1）。
    /// </summary>
    [JsonPropertyName("auto_create_room")]
    public int? AutoCreateRoom { get; set; }

    /// <summary>
    /// 获取或设置自动新建群的群名前缀（最长 40 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("room_base_name")]
    public string? RoomBaseName { get; set; }

    /// <summary>
    /// 获取或设置自动新建群的起始序号。
    /// </summary>
    [JsonPropertyName("room_base_id")]
    public long? RoomBaseId { get; set; }

    /// <summary>
    /// 获取或设置使用该进群方式的客户群 ID 列表（官方必填；最多支持 5 个）。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }

    /// <summary>
    /// 获取或设置自定义入群渠道参数（不超过 30 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源（默认为 true；仅对「营销获客」应用生效，且只能由创建此二维码的应用更新）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }
}
