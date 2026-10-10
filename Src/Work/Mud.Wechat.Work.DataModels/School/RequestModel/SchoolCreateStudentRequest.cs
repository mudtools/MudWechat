// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 创建学生请求体（<c>/cgi-bin/school/user/create_student</c>，家校沟通）。
/// <para>
/// 官方业务限制：参数字段超过长度限制时整个请求会被拦掉；
/// <c>department</c> 班级 id 列表不超过 20 个；
/// <c>to_invite</c> 仅验证的学校才能发起邀请。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolCreateStudentRequest
{
    /// <summary>
    /// 获取或设置学生 UserID（官方必填）。学校内必须唯一，可以与企业通讯录内成员 UserID 相同；
    /// 不区分大小写，长度为 1~64 个字节，只能由数字、字母和 "_-@." 四种字符组成，
    /// 且第一个字符必须是数字或字母。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserid { get; set; }

    /// <summary>
    /// 获取或设置学生手机号。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置是否发起邀请（默认为 true，仅验证的学校才能发起邀请）。
    /// </summary>
    [JsonPropertyName("to_invite")]
    public bool? ToInvite { get; set; }

    /// <summary>
    /// 获取或设置学生姓名（官方必填，长度为 1~32 个字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置学生所在的班级 id 列表（官方必填，不超过 20 个）。
    /// </summary>
    [JsonPropertyName("department")]
    public List<int>? Department { get; set; }
}
