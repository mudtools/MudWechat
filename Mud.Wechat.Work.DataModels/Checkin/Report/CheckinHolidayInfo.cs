// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报假勤相关信息（<c>datas.holiday_infos</c> 元素，自建/代开发文档口径；第三方文档页为另一套结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinHolidayInfo
{
    /// <summary>获取或设置假勤申请 id（即当日关联的假勤审批单 id）。</summary>
    [JsonPropertyName("sp_number")]
    public string? SpNumber { get; set; }

    /// <summary>获取或设置假勤信息摘要 - 标题信息。</summary>
    [JsonPropertyName("sp_title")]
    public CheckinLangText? SpTitle { get; set; }

    /// <summary>获取或设置假勤信息摘要 - 描述信息。</summary>
    [JsonPropertyName("sp_description")]
    public CheckinLangText? SpDescription { get; set; }
}
