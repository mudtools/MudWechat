// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 发送普通邮件请求体（<c>/cgi-bin/exmail/app/compose_send</c>），
/// 同时作为发送日程/会议邮件请求体（<see cref="MailSendScheduleRequest"/> / <see cref="MailSendMeetingRequest"/>）
/// 的公共基类——三种邮件官方共用同一路由，公共字段收敛于本基类
/// （形态对齐 Message 域 <c>MessageSendRequest</c> 同路由多请求基类）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailSendRequest
{
    /// <summary>获取或设置收件人（官方必填，<see cref="MailRecipient"/>；to.emails 与 to.userids 至少传一个）。</summary>
    [JsonPropertyName("to")]
    public MailRecipient? To { get; set; }

    /// <summary>获取或设置抄送人（官方选填，<see cref="MailRecipient"/>）。</summary>
    [JsonPropertyName("cc")]
    public MailRecipient? Cc { get; set; }

    /// <summary>获取或设置密送人（官方选填，<see cref="MailRecipient"/>）。</summary>
    [JsonPropertyName("bcc")]
    public MailRecipient? Bcc { get; set; }

    /// <summary>
    /// 获取或设置邮件标题（官方必填）。
    /// <para>发送日程邮件时同时也是日程标题；发送会议邮件时同时也是会议标题。</para>
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// 获取或设置邮件正文（官方必填）。
    /// <para>发送日程邮件时同时也是日程描述；发送会议邮件时同时也是会议描述。</para>
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置附件列表（官方选填，<see cref="MailAttachment"/>）。</summary>
    [JsonPropertyName("attachment_list")]
    public List<MailAttachment>? AttachmentList { get; set; }

    /// <summary>获取或设置正文内容类型：html、text（官方选填，默认 html）。</summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    /// <summary>
    /// 获取或设置是否开启 id 转译：0 - 否（默认），1 - 是。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方注明仅第三方应用需要用到，企业自建应用可忽略；目前仅 subject、content、
    /// attachment_list[].file_name 字段支持转译。模板语法（照官方原文）：
    /// <c>$departmentName=DEPARTMENT_ID$</c>、<c>$userName=USERID$</c>、<c>$userAlias=USERID$</c>、
    /// <c>$userAliasOrName=USERID$</c>；模板语法无效或 userid / 部门 id 无效时保留原样不替换；
    /// userAlias 无别名时保留原样；userAliasOrName 别名优先于姓名。
    /// </para>
    /// </remarks>
    [JsonPropertyName("enable_id_trans")]
    public int? EnableIdTrans { get; set; }
}
