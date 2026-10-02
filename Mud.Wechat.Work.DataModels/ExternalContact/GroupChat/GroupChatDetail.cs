// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

/// <summary>
/// 客户群详情（<c>group_chat</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupChat")]
public class GroupChatDetail
{
    /// <summary>
    /// 获取或设置客户群 ID。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置群名。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置群主 ID。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置群的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置群公告。
    /// </summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }

    /// <summary>
    /// 获取或设置群成员列表。
    /// </summary>
    [JsonPropertyName("member_list")]
    public List<GroupChatMember>? MemberList { get; set; }

    /// <summary>
    /// 获取或设置群管理员列表。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<GroupChatAdmin>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置当前群成员版本号（可配合客户群变更事件减少主动调用本接口的次数）。
    /// </summary>
    [JsonPropertyName("member_version")]
    public string? MemberVersion { get; set; }
}
