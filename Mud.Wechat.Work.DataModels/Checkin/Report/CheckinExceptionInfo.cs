// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡报表异常状态统计信息（日报 <c>exception_infos</c> / 月报 <c>exception_infos</c> 共用元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinExceptionInfo
{
    /// <summary>获取或设置异常类型：1 - 迟到；2 - 早退；3 - 缺卡；4 - 旷工；5 - 地点异常；6 - 设备异常。</summary>
    [JsonPropertyName("exception")]
    public int? Exception { get; set; }

    /// <summary>获取或设置异常次数（日报为当日此异常的次数；月报为统计周期内每日此异常次数之和）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置异常时长（秒；迟到/早退/旷工才有值；日报为当日时长，月报为统计周期内之和）。</summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}
