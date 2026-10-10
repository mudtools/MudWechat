// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

/// <summary>
/// 敏感词规则的使用范围（<c>applicable_range</c> / <c>add_applicable_range</c> / <c>remove_applicable_range</c>）。
/// <para>成员与部门不可同时为空；成员与部门须在应用可见范围内；
/// 单次传入的成员 / 部门各最多 1000 个节点，规则内成员 userid 总数上限 10000 个（超过建议使用部门 id）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "InterceptRule")]
public class InterceptRuleRange
{
    /// <summary>
    /// 获取或设置使用范围的成员 userid 列表（须在应用可见范围内；单次最多 1000 个节点）。
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置使用范围的部门 id 列表（须在应用可见范围内；单次最多 1000 个节点）。
    /// </summary>
    [JsonPropertyName("department_list")]
    public List<long>? DepartmentList { get; set; }
}
