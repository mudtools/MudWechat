// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则响应侧特殊日期（<c>group.spe_workdays</c> / <c>group.spe_offdays</c> 共用元素形态）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：获取员工打卡规则参数表将 <c>spe_workdays.checkintime</c> 类型标注为 <c>string</c>，返回示例为对象数组（获取企业所有打卡规则同构字段标注 <c>obj[]</c>），本模型按对象数组承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinSpeDay
{
    /// <summary>获取或设置特殊日期时间戳。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>获取或设置特殊日期备注。</summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>获取或设置特殊日期打卡时间配置（特殊非工作日为空数组；官方参数表类型标注存在 string/obj[] 两种口径，返回示例为对象数组）。</summary>
    [JsonPropertyName("checkintime")]
    public List<CheckinCheckintime>? Checkintime { get; set; }
}
