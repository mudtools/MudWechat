// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡月报加班标准信息（<c>overwork.standards</c> / <c>ov_time.standards</c> 共用元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinOverworkStandard
{
    /// <summary>获取或设置加班标准名称。</summary>
    [JsonPropertyName("standard_name")]
    public string? StandardName { get; set; }

    /// <summary>获取或设置加班标准类型。</summary>
    [JsonPropertyName("standard_type")]
    public int? StandardType { get; set; }

    /// <summary>获取或设置申请加班时长（秒）。</summary>
    [JsonPropertyName("apply_duration")]
    public int? ApplyDuration { get; set; }

    /// <summary>获取或设置实际加班时长（秒）。</summary>
    [JsonPropertyName("actual_duration")]
    public int? ActualDuration { get; set; }

    /// <summary>获取或设置是否异常：0 - 正常；1 - 异常。</summary>
    [JsonPropertyName("exception_flag")]
    public int? ExceptionFlag { get; set; }
}
