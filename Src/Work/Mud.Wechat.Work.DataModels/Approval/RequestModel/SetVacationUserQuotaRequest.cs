// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 修改成员假期余额请求体（<c>/cgi-bin/oa/vacation/setoneuserquota</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class SetVacationUserQuotaRequest
{
    /// <summary>
    /// 获取或设置需要修改假期余额的成员的userid（官方必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置假期id（官方必填）。
    /// </summary>
    [JsonPropertyName("vacation_id")]
    public int? VacationId { get; set; }

    /// <summary>
    /// 获取或设置的假期余额，单位为秒（官方必填）。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：不能大于 1000 天或 24000 小时；当假期时间刻度为按小时请假时，必须为 360 的整倍数（即 0.1 小时整倍数）；按天请假时，必须为 8640 的整倍数（即 0.1 天整倍数）。</para>
    /// </remarks>
    [JsonPropertyName("leftduration")]
    public long? Leftduration { get; set; }

    /// <summary>
    /// 获取或设置假期时间刻度：0-按天请假；1-按小时请假（官方必填；主要用于校验，必须等于企业假期管理配置中设置的假期时间刻度类型）。
    /// </summary>
    [JsonPropertyName("time_attr")]
    public int? TimeAttr { get; set; }

    /// <summary>
    /// 获取或设置修改备注，用于显示在假期余额的修改记录当中，可对修改行为作说明，不超过 200 字符。
    /// </summary>
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }
}
