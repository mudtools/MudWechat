// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡月报加班信息（<c>baseinfo.overwork</c> 加班口径 / <c>baseinfo.ov_time</c> 加班审批口径共用元素形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinOverwork
{
    /// <summary>获取或设置加班总时长（秒）。</summary>
    [JsonPropertyName("work_sec")]
    public int? WorkSec { get; set; }

    /// <summary>获取或设置加班天数。</summary>
    [JsonPropertyName("days_cnt")]
    public int? DaysCnt { get; set; }

    /// <summary>获取或设置加班异常天数（官方参数表写作「overwork excepion_days_cnt」（字段名间有空格），返回示例键为 excepion_days_cnt，官方拼写少了一个 t，照抄勿改）。</summary>
    [JsonPropertyName("excepion_days_cnt")]
    public int? ExcepionDaysCnt { get; set; }

    /// <summary>获取或设置加班记录类型：0 - 正常；1 - 其他。</summary>
    [JsonPropertyName("record_type")]
    public int? RecordType { get; set; }

    /// <summary>获取或设置加班标准信息列表。</summary>
    [JsonPropertyName("standards")]
    public List<ThirdPartyCheckinOverworkStandard>? Standards { get; set; }
}
