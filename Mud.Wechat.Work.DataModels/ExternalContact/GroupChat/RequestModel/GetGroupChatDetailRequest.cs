// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

/// <summary>
/// 获取客户群详情请求体（<c>/cgi-bin/externalcontact/groupchat/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupChat")]
public class GetGroupChatDetailRequest
{
    /// <summary>
    /// 获取或设置客户群 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置是否需要返回群成员的名字（<c>group_chat.member_list.name</c>）：
    /// 0 - 不返回；1 - 返回。默认不返回。
    /// </summary>
    [JsonPropertyName("need_name")]
    public int? NeedName { get; set; }
}
