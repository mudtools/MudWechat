// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 家长的孩子信息条目（<c>children</c> 数组元素，读取学生或家长 / 获取部门家长详情响应）。
/// <para>
/// 覆盖两个官方响应结构：<c>school/user/get</c> 的 <c>parent.children[]</c>（无 name）与
/// <c>school/user/list_parent</c> 的 <c>parents[].children[]</c>（含学生姓名 name），
/// 以可空超集承载两处差异。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolChildInfo
{
    /// <summary>
    /// 获取或设置学生的 userid。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserid { get; set; }

    /// <summary>
    /// 获取或设置家长与孩子的关系。
    /// </summary>
    [JsonPropertyName("relation")]
    public string? Relation { get; set; }

    /// <summary>
    /// 获取或设置学生姓名（仅获取部门家长详情响应返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
