// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 客服账号的接待人员信息（<c>/cgi-bin/kf/servicer/list</c> 的 <c>servicer_list</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfServicer
{
    /// <summary>
    /// 获取或设置接待人员 userid（按成员接待时返回）。
    /// <para>
    /// 第三方 / 代开发应用获取到的是密文 userid（即 open_userid）。
    /// </para>
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置接待状态（0 - 接待中，1 - 停止接待）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置「停止接待」的子类型（0 - 停止接待，1 - 暂时挂起）。
    /// </summary>
    [JsonPropertyName("stop_type")]
    public int? StopType { get; set; }

    /// <summary>
    /// 获取或设置接待人员部门的 id（按部门接待时返回）。
    /// </summary>
    [JsonPropertyName("department_id")]
    public int? DepartmentId { get; set; }
}
