// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 获取企业群发成员执行结果响应体（<c>/cgi-bin/externalcontact/get_groupmsg_send_result</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GetGroupMsgSendResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置群成员发送结果列表。
    /// </summary>
    [JsonPropertyName("send_list")]
    public List<GroupMsgSendResultItem>? SendList { get; set; }

    /// <summary>
    /// 获取或设置分页游标（用于查询下一个分页，无更多数据时为空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// 群成员发送结果项（获取企业群发成员执行结果响应中 <c>send_list[]</c> 的元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgSendResultItem
{
    /// <summary>
    /// 获取或设置外部联系人的 userid（群发到客户时返回；群发到客户群时不返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置外部客户群的 id（群发到客户群时返回；群发到客户时不返回）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置企业服务人员的 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置发送状态：0 - 未发送，1 - 已发送，2 - 因客户不是好友导致发送失败，
    /// 3 - 因客户已收到其他群发消息导致发送失败。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置发送时间（Unix 时间戳，秒；状态为 1（已发送）时返回）。
    /// </summary>
    [JsonPropertyName("send_time")]
    public long? SendTime { get; set; }
}
