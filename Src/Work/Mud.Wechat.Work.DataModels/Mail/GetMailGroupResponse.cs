// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 获取邮件群组详情响应体（<c>/cgi-bin/exmail/group/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class GetMailGroupResponse : WechatWorkResponse
{
    /// <summary>获取或设置邮件群组 ID，邮箱格式（官方 groupid）。</summary>
    [JsonPropertyName("groupid")]
    public string? Groupid { get; set; }

    /// <summary>获取或设置邮件群组名称（官方 groupname）。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置群组内成员邮箱地址（官方 email_list，<see cref="MailStringList"/>）。</summary>
    [JsonPropertyName("email_list")]
    public MailStringList? EmailList { get; set; }

    /// <summary>获取或设置群组内包含的标签 ID（官方 tag_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("tag_list")]
    public MailUintList? TagList { get; set; }

    /// <summary>获取或设置群组内包含的部门 ID（官方 department_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("department_list")]
    public MailUintList? DepartmentList { get; set; }

    /// <summary>获取或设置群组内包含的群组邮箱 ID（官方 group_list，<see cref="MailStringList"/>）。</summary>
    [JsonPropertyName("group_list")]
    public MailStringList? GroupList { get; set; }

    /// <summary>
    /// 获取或设置群组使用权限（官方 allow_type）：
    /// 0 - 企业成员，1 - 任何人，2 - 组内成员，3 - 自定义成员。
    /// </summary>
    [JsonPropertyName("allow_type")]
    public long? AllowType { get; set; }

    /// <summary>获取或设置允许使用群组群发的成员邮箱地址（官方 allow_emaillist，<see cref="MailStringList"/>）。</summary>
    [JsonPropertyName("allow_emaillist")]
    public MailStringList? AllowEmaillist { get; set; }

    /// <summary>获取或设置允许使用群组群发的部门 ID（官方 allow_departmentlist，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("allow_departmentlist")]
    public MailUintList? AllowDepartmentlist { get; set; }

    /// <summary>获取或设置允许使用群组群发的标签 ID（官方 allow_taglist，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("allow_taglist")]
    public MailUintList? AllowTaglist { get; set; }
}
