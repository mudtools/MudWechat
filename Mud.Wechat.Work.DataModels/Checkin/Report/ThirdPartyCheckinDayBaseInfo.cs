// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡日报基础信息（<c>datas.baseinfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>record_type</c> 已被官方标注废弃（「因业务调整，请改用 checkin_recordtype 字段判断打卡记录类型」）；
/// <c>dep_name</c> 返回的是 JSON 序列化后的字符串（如 <c>"[\"技术部\",\"产品部\"]"</c>），
/// 参数表「多个部门使用;分隔」的说法与示例不一致，以示例的 JSON 字符串形态为准；
/// <c>acctivity_name</c> 官方即此拼写（少了一个 t），照抄勿改。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinDayBaseInfo
{
    /// <summary>获取或设置日期（Unix 时间戳）。</summary>
    [JsonPropertyName("date")]
    public long? Date { get; set; }

    /// <summary>获取或设置打卡记录类型：0 - 上下班打卡；1 - 外出打卡（官方已标注废弃，请改用 checkin_recordtype 判断）。</summary>
    [JsonPropertyName("record_type")]
    public int? RecordType { get; set; }

    /// <summary>获取或设置打卡记录类型：0 - 上下班打卡；1 - 外出打卡（官方推荐使用本字段判断）。</summary>
    [JsonPropertyName("checkin_recordtype")]
    public int? CheckinRecordtype { get; set; }

    /// <summary>获取或设置打卡人员姓名。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置打卡人员所在部门（返回为 JSON 序列化后的字符串，如 "[\"技术部\",\"产品部\"]"）。</summary>
    [JsonPropertyName("dep_name")]
    public string? DepName { get; set; }

    /// <summary>获取或设置活动名称（正常上下班打卡该字段为空串；官方拼写 acctivity_name 照抄）。</summary>
    [JsonPropertyName("acctivity_name")]
    public string? AcctivityName { get; set; }

    /// <summary>获取或设置当日打卡总次数。</summary>
    [JsonPropertyName("checkin_count")]
    public int? CheckinCount { get; set; }

    /// <summary>获取或设置固定上班总时长（秒）。</summary>
    [JsonPropertyName("regular_work_sec")]
    public int? RegularWorkSec { get; set; }

    /// <summary>获取或设置标准上班总时长（秒）。</summary>
    [JsonPropertyName("standard_work_sec")]
    public int? StandardWorkSec { get; set; }

    /// <summary>获取或设置当日异常时长（秒，比如迟到、早退等的总秒数）。</summary>
    [JsonPropertyName("exception_duration")]
    public int? ExceptionDuration { get; set; }
}
