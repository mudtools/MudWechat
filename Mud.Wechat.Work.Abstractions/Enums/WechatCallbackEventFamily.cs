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
/// <item><description><see cref="ExternalContactChange"/>：客户联系变更族（<c>change_external_contact</c> / <c>change_external_chat</c> / <c>change_external_tag</c>；官方 92130/92277/96361）。</description></item>
/// <item><description><see cref="CustomerAcquisition"/>：获客助手族（<c>customer_acquisition</c> / <c>customer_acquisition_permit_change</c>；官方 97299/97402/98958/99485）。</description></item>
/// <item><description><see cref="SecurityChange"/>：<c>Event = security</c>（安全管理族：域名IP变更等；官方 100080，仅自建应用可配置接收）。</description></item>
/// <item><description><see cref="KfEvent"/>：微信客服族（<c>kf_msg_or_event</c> / <c>kf_account_auth_change</c>；官方 94670/97712/94699/97302/96426/97713，三类应用）。</description></item>
/// <item><description><see cref="SchoolContactChange"/>：家校通讯录变更族（<c>change_school_contact</c> / <c>change_school_contact_batch</c>；官方 92032/92052/92050/92051/97281/96716/96717）。</description></item>
/// <item><description><see cref="Unknown"/>：无法判别（协议外报文），不拦截。</description></item>
/// </list>
/// <para>
/// 判别优先级与 <c>WechatCallbackEvent.EventTypeKey</c> 对齐：客户联系/获客族按「外层事件值」
/// （<c>Event</c> 优先、套件信封回退 <c>InfoType</c>）归类，其余授权族按 <c>InfoType</c>、业务族按 <c>Event</c> 值归类。
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

    /// <summary>
    /// 客户联系变更族（<c>change_external_contact</c> / <c>change_external_chat</c> / <c>change_external_tag</c>；
    /// 官方 92130 企业自建 / 92277 第三方 / 96361 服务商代开发）。
    /// </summary>
    /// <remarks>
    /// 官方开放面矩阵：企业自建与服务商代开发经<b>应用数据通道</b>（<c>Event</c> 信封），
    /// 第三方应用经<b>套件指令通道</b>（指令回调 URL，<c>InfoType</c> 信封，官方 92277）。
    /// </remarks>
    ExternalContactChange = 5,

    /// <summary>
    /// 获客助手族（<c>customer_acquisition</c> 及 <c>customer_acquisition_permit_change</c>；
    /// 官方 97299 企业自建 / 97402·99485 第三方 / 98958 服务商代开发）。
    /// </summary>
    /// <remarks>
    /// 官方开放面矩阵与客户联系变更族一致：自建/代开发走应用数据通道、第三方走套件指令通道
    /// （99485 的 <c>service_*</c> / <c>change_price</c> 组件事件仅第三方套件通道）。
    /// </remarks>
    CustomerAcquisition = 6,

    /// <summary>
    /// 安全事件族（<c>Event = security</c>；官方 100080 域名IP变更事件，具体类别看 <c>ChangeType</c>）。
    /// </summary>
    /// <remarks>
    /// 官方开放面：<b>仅企业自建应用</b>可配置接收（配置到「我的企业 - 设置 - 域名IP - 可调用API的应用」），
    /// 第三方 / 代开发应用暂不支持；经应用数据回调 URL（<c>Event</c> 信封）承载。
    /// </remarks>
    SecurityChange = 7,

    /// <summary>
    /// 微信客服族（<c>kf_msg_or_event</c> 新消息通知 / <c>kf_account_auth_change</c> 客服账号授权变更；
    /// 官方 94670/94699/96426 与 97712/97302/97713，三模式报文同构）。
    /// </summary>
    /// <remarks>
    /// 官方开放面：三类应用均可接收（自建配置到「微信客服-可调用接口的应用」；第三方/代开发需
    /// 「微信客服→管理账号、分配会话和收发消息」权限），经应用数据回调 URL（<c>Event</c> 信封）承载。
    /// </remarks>
    KfEvent = 8,

    /// <summary>
    /// 家校通讯录变更族（族事件值 <c>change_school_contact</c> 与 <c>change_school_contact_batch</c>；
    /// 官方 92032/92052 企业自建 / 92050/92051/97281 第三方 / 96716/96717 服务商代开发）。
    /// </summary>
    /// <remarks>
    /// 官方开放面矩阵：企业自建与服务商代开发经<b>应用数据通道</b>（<c>Event</c> 信封），
    /// 第三方应用经<b>套件指令通道</b>（指令回调 URL，<c>InfoType</c> 信封，官方 92050/92051/97281）。
    /// 成员事件的 <c>ChangeType</c> <c>subscribe</c>/<c>unsubscribe</c> 与消息与事件族（官方 90240）
    /// 的事件键同名，逐 <c>ChangeType</c> 键无法消歧 ⇒ 本族以族事件值为事件键（同客户联系/邮箱族）。
    /// </remarks>
    SchoolContactChange = 9,
}