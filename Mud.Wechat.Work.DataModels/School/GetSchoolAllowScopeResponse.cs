// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取可使用的家长范围响应体（<c>/cgi-bin/school/agent/get_allow_scope</c>）。
/// <para>
/// 官方业务限制：该范围只能由学校的系统管理员在「管理端-家校沟通-配置」配置；
/// 应用只能给该列表下的家长发送「学校通知」。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class GetSchoolAllowScopeResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置可在微信「学校通知-学校应用」使用该应用的家长范围（见 <see cref="SchoolAllowScope"/>）。
    /// </summary>
    [JsonPropertyName("allow_scope")]
    public SchoolAllowScope? AllowScope { get; set; }
}

/// <summary>
/// 可使用应用的家长范围（<see cref="GetSchoolAllowScopeResponse.AllowScope"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAllowScope
{
    /// <summary>
    /// 获取或设置学生范围（见 <see cref="SchoolAllowScopeStudents"/>）。
    /// </summary>
    [JsonPropertyName("students")]
    public SchoolAllowScopeStudents? Students { get; set; }

    /// <summary>
    /// 获取或设置部门（班级）范围（见 <see cref="SchoolAllowScopeDepartments"/>）。
    /// </summary>
    [JsonPropertyName("departments")]
    public SchoolAllowScopeDepartments? Departments { get; set; }
}

/// <summary>
/// 可使用应用的学生列表（<see cref="SchoolAllowScope.Students"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAllowScopeStudents
{
    /// <summary>
    /// 获取或设置家长可在微信「学校通知-学校应用」使用该应用的学生列表。
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }
}

/// <summary>
/// 可使用应用的部门列表（<see cref="SchoolAllowScope.Departments"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAllowScopeDepartments
{
    /// <summary>
    /// 获取或设置家长可在微信「学校通知-学校应用」使用该应用的部门（班级）列表。
    /// </summary>
    [JsonPropertyName("partyid")]
    public List<long>? Partyid { get; set; }
}
