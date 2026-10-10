// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡报表假期信息（第三方日报 <c>holiday_infos</c> 与第三方月报 <c>baseinfo.holiday_infos</c> 共用元素）。
/// </summary>
/// <remarks>
/// <para>与自建/代开发文档口径的假期信息（<see cref="CheckinHolidayInfo"/>，sp_number 为审批单 id 字符串 + 多语言摘要结构）不同构。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinHolidayInfo
{
    /// <summary>获取或设置假期名称。</summary>
    [JsonPropertyName("sp_item")]
    public string? SpItem { get; set; }

    /// <summary>获取或设置假期时长（单位为小时，数字形态）。</summary>
    [JsonPropertyName("sp_number")]
    public int? SpNumber { get; set; }

    /// <summary>获取或设置假期日期（Unix 时间戳）。</summary>
    [JsonPropertyName("sp_date")]
    public long? SpDate { get; set; }

    /// <summary>获取或设置时段信息。</summary>
    [JsonPropertyName("sp_time")]
    public List<ThirdPartyCheckinSpTime>? SpTime { get; set; }
}
