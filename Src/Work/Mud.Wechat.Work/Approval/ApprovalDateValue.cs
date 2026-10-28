// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 日期/日期+时间控件值（Date）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalDateValue
{
    /// <summary>
    /// 获取或设置时间展示类型：day-日期；hour-日期+时间（和对应模板控件属性一致）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置时间戳，在此填写日期/日期+时间控件的选择值，以此为准。
    /// </summary>
    /// <remarks>
    /// <para>官方请求与响应示例均按字符串传输（如 "1569859200"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("s_timestamp")]
    public string? Timestamp { get; set; }

    /// <summary>
    /// 获取或设置时区信息；只有在非UTC+8的情况下会返回（仅获取审批申请详情响应侧）。
    /// </summary>
    [JsonPropertyName("timezone_info")]
    public ApprovalTimezoneInfo? TimezoneInfo { get; set; }
}
