// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧打卡时间配置（获取员工打卡规则 / 获取企业所有打卡规则响应 <c>group.checkindate</c> 共用元素）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：获取企业所有打卡规则参数表将 <c>checkindate.checkintime</c> 类型标注为 <c>uint32</c>，但返回示例为对象数组（元素含 work_sec 等），实际形态是对象数组，本模型按对象数组承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinGroupCheckindate
{
    /// <summary>获取或设置工作日（固定时间上下班或自由上下班：1 到 6 分别表示星期一到星期六，0 表示星期日；开启大小周的固定上下班规则返回空或不返回；按班次上下班：表示拉取班次的日期）。</summary>
    [JsonPropertyName("workdays")]
    public List<int>? Workdays { get; set; }

    /// <summary>获取或设置工作日上下班打卡时间信息（官方参数表类型标注 uint32 系笔误，返回示例为对象数组）。</summary>
    [JsonPropertyName("checkintime")]
    public List<CheckinCheckintime>? Checkintime { get; set; }

    /// <summary>获取或设置下班是否不需要打卡（true 为下班不需要打卡，false 为下班需要打卡）。</summary>
    [JsonPropertyName("noneed_offwork")]
    public bool? NoneedOffwork { get; set; }

    /// <summary>获取或设置打卡时间限制（毫秒）。</summary>
    [JsonPropertyName("limit_aheadtime")]
    public int? LimitAheadtime { get; set; }

    /// <summary>获取或设置弹性时间（毫秒；只有 flex_on_duty_time、flex_off_duty_time 不生效时（值为 -1）才有意义）。</summary>
    [JsonPropertyName("flex_time")]
    public int? FlexTime { get; set; }

    /// <summary>获取或设置允许迟到时间（秒；官方类型标注 int32；值为 -1 使用 flex_time）。</summary>
    [JsonPropertyName("flex_on_duty_time")]
    public int? FlexOnDutyTime { get; set; }

    /// <summary>获取或设置允许早退时间（秒；官方类型标注 int32；值为 -1 使用 flex_time）。</summary>
    [JsonPropertyName("flex_off_duty_time")]
    public int? FlexOffDutyTime { get; set; }

    /// <summary>获取或设置大小周配置（固定上下班规则时有效；官方参数表列出但返回示例未出现该结构，仅开启大小周的固定上下班规则可能返回）。</summary>
    [JsonPropertyName("biweekly")]
    public CheckinBiweekly? Biweekly { get; set; }
}
