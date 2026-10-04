// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 更新邮件群组请求体（<c>/cgi-bin/exmail/group/update</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class UpdateMailGroupRequest
{
    /// <summary>获取或设置邮件群组 ID，邮箱格式（官方必填）。</summary>
    [JsonPropertyName("groupid")]
    public string? Groupid { get; set; }

    /// <summary>
    /// 获取或设置邮件群组名称（官方选填）。
    /// <para>不能与其他群组重名，长度限定 200 字节。</para>
    /// </summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>
    /// 获取或设置群组内成员邮箱地址（官方 email_list，<see cref="MailStringList"/>；读取成员的 biz_mail 字段）。
    /// </summary>
    /// <remarks>
    /// <para>官方更新语义：不传则不变，传空则清空；成员由 email_list、group_list、department_list、
    /// tag_list 共同组成，不允许全部清空。</para>
    /// </remarks>
    [JsonPropertyName("email_list")]
    public MailStringList? EmailList { get; set; }

    /// <summary>获取或设置群组内包含的标签 ID（官方 tag_list，<see cref="MailUintList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("tag_list")]
    public MailUintList? TagList { get; set; }

    /// <summary>获取或设置群组内包含的部门 ID（官方 department_list，<see cref="MailUintList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("department_list")]
    public MailUintList? DepartmentList { get; set; }

    /// <summary>获取或设置群组内包含的群组邮箱 ID（官方 group_list，<see cref="MailStringList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("group_list")]
    public MailStringList? GroupList { get; set; }

    /// <summary>
    /// 获取或设置群组使用权限（官方 allow_type）：
    /// 0 - 企业成员，1 - 任何人，2 - 组内成员，3 - 自定义成员；若不需更新则不传入。
    /// </summary>
    /// <remarks>
    /// <para>官方业务限制：当值为 0、1、2 时，不得传入 allow_emaillist、allow_departmentlist、allow_taglist；
    /// 当值为 3 时，必须传入三者至少一项。</para>
    /// </remarks>
    [JsonPropertyName("allow_type")]
    public long? AllowType { get; set; }

    /// <summary>获取或设置允许使用群组群发的成员邮箱地址（官方 allow_emaillist，<see cref="MailStringList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("allow_emaillist")]
    public MailStringList? AllowEmaillist { get; set; }

    /// <summary>获取或设置允许使用群组群发的部门 ID（官方 allow_departmentlist，<see cref="MailUintList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("allow_departmentlist")]
    public MailUintList? AllowDepartmentlist { get; set; }

    /// <summary>获取或设置允许使用群组群发的标签 ID（官方 allow_taglist，<see cref="MailUintList"/>；不传则不变，传空为清空）。</summary>
    [JsonPropertyName("allow_taglist")]
    public MailUintList? AllowTaglist { get; set; }
}
