// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 创建/更新模板的假勤控件配置（config.attendance 请求侧，外出/出差/加班组件）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSettingAttendanceConfig
{
    /// <summary>
    /// 获取或设置假勤组件类型：3-出差；4-外出；5-加班。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置假勤组件时间选择范围属性。
    /// </summary>
    [JsonPropertyName("date_range")]
    public ApprovalTemplateDateRangeConfig? DateRange { get; set; }
}
