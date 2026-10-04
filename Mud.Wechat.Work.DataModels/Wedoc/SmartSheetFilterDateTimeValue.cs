// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 过滤条件的日期值（官方 FilterDataTimeValue）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetFilterDateTimeValue
{
    /// <summary>
    /// 获取或设置日期类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>DATE_TIME_TYPE_DETAIL_DATE</c> 具体时间、<c>DATE_TIME_TYPE_TODAY</c> 今天、<c>DATE_TIME_TYPE_TOMORROW</c> 明天、<c>DATE_TIME_TYPE_YESTERDAY</c> 昨天、<c>DATE_TIME_TYPE_CURRENT_WEEK</c> 本周、<c>DATE_TIME_TYPE_LAST_WEEK</c> 上周、<c>DATE_TIME_TYPE_CURRENT_MONTH</c> 本月、<c>DATE_TIME_TYPE_THE_PAST_7_DAYS</c> 过去 7 天内、<c>DATE_TIME_TYPE_THE_NEXT_7_DAYS</c> 接下来 7 天内、<c>DATE_TIME_TYPE_LAST_MONTH</c> 上月、<c>DATE_TIME_TYPE_THE_PAST_30_DAYS</c> 过去 30 天内、<c>DATE_TIME_TYPE_THE_NEXT_30_DAYS</c> 接下来 30 天内。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置具体日期值（官方 <c>value</c>，必填），<c>type</c> 为具体日期（<c>DATE_TIME_TYPE_DETAIL_DATE</c>）时使用。
    /// 官方示例以 Unix 毫秒时间戳字符串承载（如 <c>"1747152000000"</c>，即 2025 年 5 月 14 日）。
    /// </summary>
    [JsonPropertyName("value")]
    public List<string>? Value { get; set; }
}
