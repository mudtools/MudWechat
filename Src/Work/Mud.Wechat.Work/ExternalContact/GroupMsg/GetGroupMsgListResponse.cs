// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 获取群发记录列表响应体（<c>/cgi-bin/externalcontact/get_groupmsg_list_v2</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GetGroupMsgListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置群发记录列表。
    /// </summary>
    [JsonPropertyName("group_msg_list")]
    public List<GroupMsgRecord>? GroupMsgList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（用于查询下一个分页，无更多数据时为空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 群发记录（<c>group_msg_list[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgRecord
{
    /// <summary>
    /// 获取或设置群发消息 id（可用于查询执行结果）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>
    /// 获取或设置群发创建者的 userid（通过 API 接口创建的群发不返回该字段）。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置群发的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置群发来源：0 - 企业创建，1 - 个人创建。
    /// </summary>
    [JsonPropertyName("create_type")]
    public int? CreateType { get; set; }

    /// <summary>
    /// 获取或设置群发的文本内容。
    /// </summary>
    [JsonPropertyName("text")]
    public GroupMsgTextContent? Text { get; set; }

    /// <summary>
    /// 获取或设置群发的附件列表。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<GroupMsgAttachment>? Attachments { get; set; }
}
