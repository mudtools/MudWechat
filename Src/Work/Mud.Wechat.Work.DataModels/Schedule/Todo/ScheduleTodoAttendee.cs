// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 待办参与人（获取待办详情响应 attendees 元素，及更新待办状态请求 attendees 元素；更新侧最多支持 20 个参与人）。
/// </summary>
/// <remarks>
/// <para>更新待办状态请求传入 attendees 时，<see cref="Userid"/> 与 <see cref="Status"/> 均官方必填。</para>
/// <para>
/// 官方文档对更新待办状态的参数表仅列出 <c>status</c> 的 0/1 两种取值，
/// 但请求示例中出现值 2，两种形态不一致，本模型以整数承载以兼容两种形态（以参数表为准）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class ScheduleTodoAttendee
{
    /// <summary>获取或设置待办参与人 ID（更新请求传入 attendees 时官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置参与人的待办状态：0 - 完成；1 - 进行中（更新请求传入 attendees 时官方必填）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
