// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 获取审批数据（旧）请求体（<c>/cgi-bin/corp/getapprovaldata</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：endtime 需大于 starttime，同时起始时间跨度不要超过 30 天；一次请求返回的审批记录上限是 100 条，超过 100 条记录请使用 next_spnum 进行分页拉取；无法获取管理员已删除的审批单数据。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class GetApprovalDataRequest
{
    /// <summary>
    /// 获取或设置获取审批记录的开始时间（Unix时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("starttime")]
    public long? Starttime { get; set; }

    /// <summary>
    /// 获取或设置获取审批记录的结束时间（Unix时间戳，官方必填）。
    /// </summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }

    /// <summary>
    /// 获取或设置第一个拉取的审批单号，不填从该时间段的第一个审批单拉取。
    /// </summary>
    [JsonPropertyName("next_spnum")]
    public long? NextSpnum { get; set; }
}
