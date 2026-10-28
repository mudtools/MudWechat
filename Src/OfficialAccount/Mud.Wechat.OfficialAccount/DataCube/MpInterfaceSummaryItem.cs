// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 被动回复概要/分布数据条目（官方 <c>getinterfacesummary</c> 的 list 元素；hour 端点多 ref_hour
/// ⇒ 超集共用本 DTO）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpInterfaceSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置数据的小时（官方 <c>ref_hour</c>；<b>仅分时端点返回</b>，日数据端点为 null）。</summary>
    [JsonPropertyName("ref_hour")]
    public int? RefHour { get; set; }

    /// <summary>获取或设置被动回复用户消息的次数（官方 <c>callback_count</c>）。</summary>
    [JsonPropertyName("callback_count")]
    public int? CallbackCount { get; set; }

    /// <summary>获取或设置被动回复的失败次数（官方 <c>fail_count</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }

    /// <summary>获取或设置总耗时（官方 <c>total_time_cost</c>；除以 callback_count 即平均耗时）。</summary>
    [JsonPropertyName("total_time_cost")]
    public int? TotalTimeCost { get; set; }

    /// <summary>获取或设置最大耗时（官方 <c>max_time_cost</c>）。</summary>
    [JsonPropertyName("max_time_cost")]
    public int? MaxTimeCost { get; set; }
}
