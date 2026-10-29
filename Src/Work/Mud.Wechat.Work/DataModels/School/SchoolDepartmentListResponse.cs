// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取部门列表响应体（<c>/cgi-bin/school/department/list</c>，家校沟通）。
/// <para>
/// 官方形态：不传 id 参数时默认获取全量组织架构，传 id 时获取指定部门及其下的子部门。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolDepartmentListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置部门列表数据。
    /// </summary>
    [JsonPropertyName("departments")]
    public List<SchoolDepartmentInfo>? Departments { get; set; }
}
