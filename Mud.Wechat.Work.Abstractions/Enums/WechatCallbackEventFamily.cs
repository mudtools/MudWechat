// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Enums;

/// <summary>
/// 企业微信回调事件族（把 <see cref="Mud.Wechat.Work.Abstractions.Callback.WechatCallbackEvent"/> 归入
/// 官方五类事件族，驱动「应用类型 × 回调通道」的开放面合法性闸）。
/// </summary>
/// <remarks>
/// <para>
/// 企业微信把回调拆成<b>套件指令回调 URL</b>（Suite 通道：suite_ticket / 授权通知 / 取消授权等）
/// 与<b>应用数据回调 URL</b>（App 通道：change_contact / batch_job_result / change_chain 等业务事件），
/// 两类 URL 承载的事件族互不重叠。事件族据此映射：
/// </para>
/// <list type="bullet">
/// <item><description><see cref="Authorization"/>：<c>InfoType</c> 非空（suite_ticket / create_auth / change_auth / cancel_auth / del_auth / reset_permanent_code）。</description></item>
/// <item><description><see cref="ContactChange"/>：<c>Event = change_contact</c>（通讯录成员/部门/标签变更）。</description></item>
/// <item><description><see cref="ChainChange"/>：<c>Event = change_chain</c>（上下游空间/分组/企业变更，仅自建应用可配置接收）。</description></item>
/// <item><description><see cref="BatchJob"/>：<c>Event = batch_job_result</c>（异步任务完成通知，通讯录 / 上下游双布局）。</description></item>
/// <item><description><see cref="Unknown"/>：无法判别（协议外报文），不拦截。</description></item>
/// </list>
/// <para>
/// 判别优先级与 <c>WechatCallbackEvent.EventTypeKey</c> 对齐：<c>InfoType</c> 优先，其次按 <c>Event</c> 值归类。
/// 合法性闸见 <c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>。
/// </para>
/// </remarks>
public enum WechatCallbackEventFamily
{
    /// <summary>无法判别的事件族（协议外报文，合法性闸不拦截）。</summary>
    Unknown = 0,

    /// <summary>授权族（InfoType 非空；仅第三方应用 / 服务商代开发的套件通道）。</summary>
    Authorization = 1,

    /// <summary>通讯录变更族（Event = change_contact；三类应用的应用数据通道）。</summary>
    ContactChange = 2,

    /// <summary>上下游变更族（Event = change_chain；仅自建应用的应用数据通道）。</summary>
    ChainChange = 3,

    /// <summary>异步任务族（Event = batch_job_result；应用数据通道）。</summary>
    BatchJob = 4,
}