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
/// 日历变更事件载荷（<b>结构族</b>：覆盖 <c>delete_calendar</c> / <c>modify_calendar</c>；
/// 官方 97728/97730 自建 · 97806/97808 第三方 · 97771/97772 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：两事件的官方报文字段集合一致（信封 + <c>CalId</c>），
/// 具体类别由信封 <c>Event</c> 值判别（本族无 <c>ChangeType</c> 分组段，<c>Event</c> 节点即事件键）。
/// 与日程类事件（<see cref="ScheduleChangedPayload"/>，多 <c>ScheduleId</c> 节点）字段集不同，故分立两个载荷。
/// </para>
/// <para>
/// <b>触发边界</b>：仅 API 创建的日历产生事件（由日历管理员删除/修改触发）。
/// </para>
/// <para>
/// <b>三模式无关性（ADR-14）</b>：三份文档（自建/第三方/代开发）报文逐字一致，差异只是「值是否出现」。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97728">path 97728 删除日历事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97806">path 97806（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97771">path 97771（服务商代开发）</see>；
/// 修改日历 <see href="https://developer.work.weixin.qq.com/document/path/97730">path 97730</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97808">path 97808</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97772">path 97772</see>（代开发）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.DeleteCalendar,
        WechatCallbackEventTypes.ModifyCalendar })]
public sealed partial class CalendarChangedPayload : WechatCallbackPayload
{
    /// <summary>日历 ID（官方 <c>CalId</c>）。</summary>
    [PayloadField("CalId")]
    public string? CalId { get; set; }
}
