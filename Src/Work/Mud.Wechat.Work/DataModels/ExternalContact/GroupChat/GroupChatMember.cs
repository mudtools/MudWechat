// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

/// <summary>
/// 客户群群成员（<c>group_chat.member_list[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupChat")]
public class GroupChatMember
{
    /// <summary>
    /// 获取或设置群成员 id。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置成员类型：1 - 企业成员；2 - 外部联系人。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置外部联系人在微信开放平台的唯一身份标识（微信 unionid）。
    /// 仅当群成员类型是微信用户且企业绑定了微信开发者 ID 时返回；第三方不可获取，
    /// 上游企业不可获取下游企业客户的 unionid 字段。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 获取或设置入群时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("join_time")]
    public long? JoinTime { get; set; }

    /// <summary>
    /// 获取或设置入群方式：1 - 由群成员邀请入群（直接邀请入群）；
    /// 2 - 由群成员邀请入群（通过邀请链接入群）；3 - 通过扫描群二维码入群。
    /// </summary>
    [JsonPropertyName("join_scene")]
    public int? JoinScene { get; set; }

    /// <summary>
    /// 获取或设置邀请者（目前仅当是由本企业内部成员邀请入群时会返回该值）。
    /// </summary>
    [JsonPropertyName("invitor")]
    public GroupChatInvitor? Invitor { get; set; }

    /// <summary>
    /// 获取或设置在群里的昵称。
    /// </summary>
    [JsonPropertyName("group_nickname")]
    public string? GroupNickname { get; set; }

    /// <summary>
    /// 获取或设置成员名字（仅当请求 <c>need_name = 1</c> 时返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
