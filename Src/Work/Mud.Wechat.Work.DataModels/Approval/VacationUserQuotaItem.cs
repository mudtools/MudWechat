// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 成员假期余额项（getuservacationquota 响应的 lists 元素）。
/// </summary>
/// <remarks>
/// <para>余额的时长单位都为秒：如果假期时间刻度为「按天」，需要除以 86400 得到真实假期余额天数；如果假期时间刻度为「按小时」，需要除以 3600 得到真实假期余额小时数。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class VacationUserQuotaItem
{
    /// <summary>
    /// 获取或设置假期id。
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// 获取或设置发放时长，单位为秒。
    /// </summary>
    [JsonPropertyName("assignduration")]
    public long? Assignduration { get; set; }

    /// <summary>
    /// 获取或设置使用时长，单位为秒。
    /// </summary>
    [JsonPropertyName("usedduration")]
    public long? Usedduration { get; set; }

    /// <summary>
    /// 获取或设置剩余时长，单位为秒。
    /// </summary>
    [JsonPropertyName("leftduration")]
    public long? Leftduration { get; set; }

    /// <summary>
    /// 获取或设置假期名称（官方字段名即为 vacationname）。
    /// </summary>
    [JsonPropertyName("vacationname")]
    public string? Vacationname { get; set; }

    /// <summary>
    /// 获取或设置假期的实际发放时长（通常在设置了按照实际工作时间发放假期后进行计算）。
    /// </summary>
    [JsonPropertyName("real_assignduration")]
    public long? RealAssignduration { get; set; }
}
