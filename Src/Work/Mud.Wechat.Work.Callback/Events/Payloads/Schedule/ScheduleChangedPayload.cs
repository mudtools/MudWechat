// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 日程变更事件载荷（<b>结构族</b>：覆盖 <c>modify_schedule</c> / <c>delete_schedule</c> / <c>respond_schedule</c>；
/// 官方 97731/97732/98111 自建 · 97809/97810/98099 第三方 · 97773/97774/98110 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：三事件的官方报文字段集合一致（信封 + <c>CalId</c> + <c>ScheduleId</c>），
/// 具体类别由信封 <c>Event</c> 值判别（本族无 <c>ChangeType</c> 分组段，<c>Event</c> 节点即事件键）。
/// 与日历类事件（<see cref="CalendarChangedPayload"/>，仅 <c>CalId</c>）字段集不同，故分立两个载荷。
/// </para>
/// <para>
/// <b>触发边界</b>：仅 API 创建的日程产生事件；回执事件（<c>respond_schedule</c>）在参与人进行
/// 回执操作（接受、待定、拒绝）时触发——回执结果不在推送报文节点中，须业务侧按成员与状态另行对齐。
/// </para>
/// <para>
/// <b>信封语义差异</b>：修改/删除日程的 <c>FromUserName</c> 为「成员UserID」；
/// 回执事件为「进行回执操作的企业成员UserID」（信封字段，处理器可据 <c>evt.Event</c> 判别语义）。
/// </para>
/// <para>
/// <b>三模式无关性（ADR-14）</b>：三份文档（自建/第三方/代开发）报文逐字一致，差异只是「值是否出现」。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97731">path 97731 修改日程事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97809">path 97809（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97773">path 97773（服务商代开发）</see>；
/// 删除日程 <see href="https://developer.work.weixin.qq.com/document/path/97732">path 97732</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97810">path 97810</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97774">path 97774</see>（代开发）；
/// 日程回执 <see href="https://developer.work.weixin.qq.com/document/path/98111">path 98111</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98099">path 98099</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/98110">path 98110</see>（代开发）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.ModifySchedule,
        WechatCallbackEventTypes.DeleteSchedule,
        WechatCallbackEventTypes.RespondSchedule })]
public sealed partial class ScheduleChangedPayload : WechatCallbackPayload
{
    /// <summary>日历 ID（官方 <c>CalId</c>）。</summary>
    [PayloadField("CalId")]
    public string? CalId { get; set; }

    /// <summary>日程 ID（官方 <c>ScheduleId</c>）。</summary>
    [PayloadField("ScheduleId")]
    public string? ScheduleId { get; set; }
}
