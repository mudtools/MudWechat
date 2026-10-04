// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 创建公共邮箱请求体（<c>/cgi-bin/exmail/publicmail/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class CreatePublicMailRequest
{
    /// <summary>获取或设置公共邮箱地址（官方必填）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 获取或设置公共邮箱名称（官方必填）。
    /// <para>不多于 64 个字符或 32 个汉字，不得与其他公共邮箱重名。</para>
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置有权限使用公共邮箱的成员 UserID 列表（官方 userid_list，<see cref="MailStringList"/>）。
    /// </summary>
    /// <remarks><para>官方业务限制：userid_list、department_list、tag_list 不能同时为空。</para></remarks>
    [JsonPropertyName("userid_list")]
    public MailStringList? UseridList { get; set; }

    /// <summary>获取或设置有权限使用公共邮箱的部门 ID 列表（官方 department_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("department_list")]
    public MailUintList? DepartmentList { get; set; }

    /// <summary>获取或设置有权限使用公共邮箱的标签 ID 列表（官方 tag_list，<see cref="MailUintList"/>）。</summary>
    [JsonPropertyName("tag_list")]
    public MailUintList? TagList { get; set; }

    /// <summary>
    /// 获取或设置是否创建客户端专用密码（官方 create_auth_code）：0 - 否（默认），1 - 是。
    /// </summary>
    /// <remarks>
    /// <para>官方业务限制：一个公共邮箱通过 API 接口创建的客户端专用密码不能超过 10 个（不包括已删除的）；
    /// 密码只显示一次，请妥善存储。</para>
    /// </remarks>
    [JsonPropertyName("create_auth_code")]
    public long? CreateAuthCode { get; set; }

    /// <summary>获取或设置创建客户端专用密码的备注信息（官方选填，<see cref="PublicMailAuthCodeInfo"/>；仅当 create_auth_code=1 时有效）。</summary>
    [JsonPropertyName("auth_code_info")]
    public PublicMailAuthCodeInfo? AuthCodeInfo { get; set; }
}
