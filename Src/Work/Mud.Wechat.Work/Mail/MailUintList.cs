// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 邮件管理端官方 <c>{ "list": [整数数组] }</c> 包装结构（整数 ID 列表；
/// 邮件群组与公共邮箱的标签、部门 ID 列表共用）。
/// </summary>
/// <remarks>
/// <para>官方对邮件群组与公共邮箱的标签/部门 ID 列表均以
/// <c>{ "list": [...] }</c> 包装传输（如 tag_list / department_list / allow_taglist / allow_departmentlist）；
/// 更新语义遵循官方约定：不传则保持不变，传空结构或空数组则清空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailUintList
{
    /// <summary>获取或设置整数 ID 列表（官方 list）。</summary>
    [JsonPropertyName("list")]
    public List<long>? List { get; set; }
}
