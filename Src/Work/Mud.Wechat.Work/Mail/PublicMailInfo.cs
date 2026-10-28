// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 公共邮箱详细信息（官方公共邮箱对象结构：id / email / name + 成员、部门、标签、别名列表；
/// 获取公共邮箱详情响应元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class PublicMailInfo
{
    /// <summary>获取或设置公共邮箱 ID（官方 id）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置公共邮箱地址（官方 email）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>获取或设置公共邮箱名称（官方 name）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置有权限使用公共邮箱的成员 UserID 列表（官方 userid_list，<see cref="MailStringList"/>）。</summary>
    [JsonPropertyName("userid_list")]
    public MailStringList? UseridList { get; set; }

    /// <summary>获取或设置有权限使用公共邮箱的部门 ID 列表（官方 department_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("department_list")]
    public MailUintList? DepartmentList { get; set; }

    /// <summary>获取或设置有权限使用公共邮箱的标签 ID 列表（官方 tag_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("tag_list")]
    public MailUintList? TagList { get; set; }

    /// <summary>获取或设置公共邮箱的别名列表（官方 alias_list，<see cref="MailStringList"/>）。</summary>
    [JsonPropertyName("alias_list")]
    public MailStringList? AliasList { get; set; }
}
