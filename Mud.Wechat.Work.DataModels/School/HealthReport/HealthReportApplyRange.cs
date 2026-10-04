// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 健康上报任务适用范围（<c>job_info.apply_range</c> 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportApplyRange
{
    /// <summary>
    /// 获取或设置适用范围的成员列表。
    /// <para>任务类型为 1 时为企业成员 userid，任务类型为 2 时为学生的 userid。</para>
    /// </summary>
    [JsonPropertyName("userids")]
    public List<string>? UserIds { get; set; }

    /// <summary>
    /// 获取或设置适用范围的部门列表。
    /// <para>任务类型为 1 时为企业通讯录部门 id，任务类型为 2 时为学校通讯录部门 id。</para>
    /// </summary>
    [JsonPropertyName("partyids")]
    public List<long>? PartyIds { get; set; }
}
