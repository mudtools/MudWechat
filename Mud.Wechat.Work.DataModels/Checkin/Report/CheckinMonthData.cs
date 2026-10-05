// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡月报数据（获取打卡月报数据响应 <c>datas</c> 元素，自建应用与服务商代开发文档口径）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinMonthData
{
    /// <summary>获取或设置基础信息。</summary>
    [JsonPropertyName("base_info")]
    public CheckinMonthBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置汇总信息。</summary>
    [JsonPropertyName("summary_info")]
    public CheckinMonthSummaryInfo? SummaryInfo { get; set; }

    /// <summary>获取或设置异常状态统计信息。</summary>
    [JsonPropertyName("exception_infos")]
    public List<CheckinExceptionInfo>? ExceptionInfos { get; set; }

    /// <summary>获取或设置假勤统计信息。</summary>
    [JsonPropertyName("sp_items")]
    public List<CheckinSpItem>? SpItems { get; set; }

    /// <summary>获取或设置加班情况。</summary>
    [JsonPropertyName("overwork_info")]
    public CheckinMonthOverworkInfo? OverworkInfo { get; set; }
}
