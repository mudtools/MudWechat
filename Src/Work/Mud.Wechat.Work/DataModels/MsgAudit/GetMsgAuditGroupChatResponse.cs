// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取会话内容存档内部群信息响应体（<c>/cgi-bin/msgaudit/groupchat/get</c>）。
/// <para>官方业务限制：仅支持查询内部群；错误码 301052 表示会话存档已过期。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class GetMsgAuditGroupChatResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置 roomid 对应的群名称。
    /// </summary>
    [JsonPropertyName("roomname")]
    public string? RoomName { get; set; }

    /// <summary>
    /// 获取或设置 roomid 对应的群创建者（userid）。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置 roomid 对应的群创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("room_create_time")]
    public long? RoomCreateTime { get; set; }

    /// <summary>
    /// 获取或设置 roomid 对应的群公告。
    /// </summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }

    /// <summary>
    /// 获取或设置 roomid 对应的群成员列表（见 <see cref="MsgAuditGroupChatMember"/>）。
    /// </summary>
    [JsonPropertyName("members")]
    public List<MsgAuditGroupChatMember>? Members { get; set; }
}

/// <summary>
/// 内部群群成员信息（<see cref="GetMsgAuditGroupChatResponse.Members"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class MsgAuditGroupChatMember
{
    /// <summary>
    /// 获取或设置群成员的 id（userid）。
    /// </summary>
    [JsonPropertyName("memberid")]
    public string? MemberId { get; set; }

    /// <summary>
    /// 获取或设置群成员的入群时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("jointime")]
    public long? JoinTime { get; set; }
}
