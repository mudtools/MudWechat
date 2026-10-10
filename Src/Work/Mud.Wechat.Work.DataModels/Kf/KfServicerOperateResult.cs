// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 接待人员添加 / 删除的单条操作结果（<c>/cgi-bin/kf/servicer/add</c> 与
/// <c>/cgi-bin/kf/servicer/del</c> 的 <c>result_list</c> 元素）。
/// <para>
/// 按成员操作时填充 <see cref="UserId"/>，按部门操作时填充 <see cref="DepartmentId"/>，
/// 二者互斥；逐条结果以 <see cref="ErrorCode"/> 判定（如重复添加 / 删除不存在的人员返回
/// 非零码或 ignored，不影响其它条目）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfServicerOperateResult
{
    /// <summary>
    /// 获取或设置接待人员的 userid（按成员操作时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置接待人员部门 id（按部门操作时返回）。
    /// </summary>
    [JsonPropertyName("department_id")]
    public int? DepartmentId { get; set; }

    /// <summary>
    /// 获取或设置该 userid / department_id 的操作结果码（0 表示成功）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrorCode { get; set; }

    /// <summary>
    /// 获取或设置该 userid / department_id 的操作结果描述（如 success / ignored）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }
}
