// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 群聊会话信息（<c>/cgi-bin/appchat/get</c> 响应中的 chat_info 节点）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class AppChatInfo
{
    /// <summary>
    /// 获取或设置群聊唯一标志。
    /// </summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置群聊名。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置群主 id。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置群成员 id 列表。
    /// </summary>
    [JsonPropertyName("userlist")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置群聊类型：0 表示普通群聊，1 表示课程群聊。
    /// </summary>
    [JsonPropertyName("chat_type")]
    public int? ChatType { get; set; }
}
