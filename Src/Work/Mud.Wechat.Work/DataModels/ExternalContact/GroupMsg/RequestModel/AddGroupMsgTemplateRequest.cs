// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 创建企业群发请求体（<c>/cgi-bin/externalcontact/add_msg_template</c>）。
/// <para>
/// <see cref="Text"/> 与 <see cref="Attachments"/> 不能同时为空；
/// 客户群发（chat_type = single）时 <see cref="Sender"/>、<see cref="ExternalUserid"/>、<see cref="TagFilter"/>
/// 不可同时为空，指定 <see cref="ExternalUserid"/> 后 <see cref="TagFilter"/> 不生效；
/// 每位客户或客户群每月最多接收的群发条数为当月天数，超过上限则无法再收到群发。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class AddGroupMsgTemplateRequest
{
    /// <summary>
    /// 获取或设置群发任务类型：single - 发给客户（默认），group - 发给客户群。
    /// </summary>
    [JsonPropertyName("chat_type")]
    public string? ChatType { get; set; }

    /// <summary>
    /// 获取或设置客户的 external_userid 列表（仅 chat_type = single 有效；单次最多 1 万个客户）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public List<string>? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置客户群 id 列表（仅 chat_type = group 有效；单次最多 2000 个群；
    /// 需企业微信 4.1.10 及以上终端）。
    /// </summary>
    [JsonPropertyName("chat_id_list")]
    public List<string>? ChatIdList { get; set; }

    /// <summary>
    /// 获取或设置客户标签过滤条件（指定 external_userid 后不生效）。
    /// </summary>
    [JsonPropertyName("tag_filter")]
    public GroupMsgTagFilter? TagFilter { get; set; }

    /// <summary>
    /// 获取或设置发送群发的成员 userid（发送给客户群时必填；仅提供 sender 时相当于选取该成员所有客户）。
    /// </summary>
    [JsonPropertyName("sender")]
    public string? Sender { get; set; }

    /// <summary>
    /// 获取或设置是否允许成员重新选择待发送客户（默认为 false；仅客户群发场景生效）。
    /// </summary>
    [JsonPropertyName("allow_select")]
    public bool? AllowSelect { get; set; }

    /// <summary>
    /// 获取或设置文本消息（最长 4000 字节；与附件不能同时为空）。
    /// </summary>
    [JsonPropertyName("text")]
    public GroupMsgTextContent? Text { get; set; }

    /// <summary>
    /// 获取或设置附件列表（最多 9 个；各字段须与 msgtype 一致，否则报错）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<GroupMsgAttachment>? Attachments { get; set; }
}
