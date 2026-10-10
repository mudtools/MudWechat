// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 审批申请状态变化事件载荷（<c>sys_approval_change</c>，业务载荷在 <c>ApprovalInfo</c> 包装节点内；
/// 官方 path 91815 自建 · 92633 第三方 · 96508 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双布局（ScopeFallback）</b>：业务载荷全部在 <c>ApprovalInfo</c> 包装节点内
/// （<c>BatchJob</c> 同款三级作用域判定），本载荷字段声明均相对该容器。
/// 信封外无 <c>ChangeType</c>，<c>Event</c> 节点即事件键；信封 <c>FromUserName</c> 固定 <c>sys</c>；
/// 根级 <c>AgentID</c> 经信封 <c>evt.AgentID</c> 读取，不在载荷内映射（与 <c>open_approval_change</c> 同口径）。
/// </para>
/// <para>
/// <b>触发边界（官方原文）</b>：状态变化包括但不限于催办、撤销、同意、驳回、转审、添加备注等情况；
/// 类型经 <see cref="StatuChangeEvent"/> 判别。自建应用需配置到「审批-可调用接口的应用」；
/// 第三方应用在服务商后台配置<b>指令回调 URL</b> 接收。
/// </para>
/// <para>
/// <b>与旧式审批事件的关系</b>：<see cref="WechatCallbackEventTypes.OpenApprovalChange"/>（90240，
/// <see cref="ApprovalStatusChangedPayload"/>）是另一独立事件，本事件为其 OA 审批域的新式形态，二者键域互异。
/// </para>
/// <para>
/// <b>三模式说明（ADR-14）</b>：自建（91815）与代开发（96508）全文逐字一致；第三方（92633）报文结构
/// 同构（同一 <c>ApprovalInfo</c> 子树），仅文档层差异（参数表 <c>Approvor</c> 拼写 / <c>MediaId</c> 别名、
/// <c>StatuChangeEvent</c> 枚举缺 14/15）—— 建模取自建/代开发命名，处理器按 <c>evt.AppType</c> 分支语义。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/91815">path 91815 审批申请状态变化回调通知（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92633">path 92633（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96508">path 96508（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter), ScopeFallback = "ApprovalInfo")]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.SysApprovalChange })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.SysApprovalChange })]
public sealed partial class SysApprovalChangedPayload : WechatCallbackPayload
{
    /// <summary>审批编号（官方 <c>SpNoStr</c>，字符串类型；官方推荐以此代替 <c>SpNo</c>）。</summary>
    [PayloadField("SpNoStr")]
    public string? SpNoStr { get; set; }

    /// <summary>审批编号（官方 <c>SpNo</c>；官方注明「局校审批单不返回此字段，不推荐使用此字段」⇒ 处理器不得假设必有值）。</summary>
    [PayloadField("SpNo")]
    public string? SpNo { get; set; }

    /// <summary>审批申请类型名称（官方 <c>SpName</c>，即审批模板名称）。</summary>
    [PayloadField("SpName")]
    public string? SpName { get; set; }

    /// <summary>
    /// 申请单状态（官方 <c>SpStatus</c>，闭合值域）：1 审批中 / 2 已通过 / 3 已驳回 / 4 已撤销 /
    /// 6 通过后撤销 / 7 已删除 / 10 已支付。
    /// </summary>
    [PayloadField("SpStatus")]
    public long? SpStatus { get; set; }

    /// <summary>审批模板 id（官方 <c>TemplateId</c>）。</summary>
    [PayloadField("TemplateId")]
    public string? TemplateId { get; set; }

    /// <summary>审批申请提交时间（官方 <c>ApplyTime</c>，Unix 时间戳）。</summary>
    [PayloadField("ApplyTime")]
    public long? ApplyTime { get; set; }

    /// <summary>申请人信息（官方 <c>Applyer</c>：UserId + Party；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("Applyer")]
    public WechatCallbackOaApprovalApplyer? Applyer { get; set; }

    /// <summary>
    /// 审批流程信息列表（官方 <c>SpRecord</c>，「可能有多个审批节点」，ApprovalInfo 下平铺重复兄弟元素；
    /// 经 <see cref="WechatPayloadConverter.RepeatOaApprovalRecords"/> 读取；节点缺失 ⇒ 空列表）。
    /// </summary>
    [PayloadField("SpRecord", Method = nameof(WechatPayloadConverter.RepeatOaApprovalRecords))]
    public List<WechatCallbackOaApprovalRecord> SpRecords { get; set; } = new List<WechatCallbackOaApprovalRecord>();

    /// <summary>
    /// 抄送人列表（官方 <c>Notifyer</c>，「可能有多个抄送节点」，平铺重复兄弟元素；
    /// <b>官方拼写陷阱</b>：节点名 Notifyer 非 Notifier；节点缺失 ⇒ 空列表）。
    /// </summary>
    [PayloadField("Notifyer", Method = nameof(WechatPayloadConverter.RepeatOaApprovalNotifyers))]
    public List<WechatCallbackOaApprovalNotifyer> Notifyers { get; set; } = new List<WechatCallbackOaApprovalNotifyer>();

    /// <summary>
    /// 审批流程列表（官方 <c>ProcessList</c>，单节点容器，内含 <c>NodeList</c> 平铺重复兄弟元素；
    /// 节点缺失 ⇒ <c>null</c>）。
    /// </summary>
    [PayloadField("ProcessList")]
    public WechatCallbackOaProcessList? ProcessList { get; set; }

    /// <summary>
    /// 备注列表（官方 <c>Comments</c>，「可能有多个备注节点」，平铺重复兄弟元素；
    /// 经 <see cref="WechatPayloadConverter.RepeatOaApprovalComments"/> 读取；节点缺失 ⇒ 空列表）。
    /// </summary>
    [PayloadField("Comments", Method = nameof(WechatPayloadConverter.RepeatOaApprovalComments))]
    public List<WechatCallbackOaApprovalComment> Comments { get; set; } = new List<WechatCallbackOaApprovalComment>();

    /// <summary>
    /// 审批申请状态变化类型（官方 <c>StatuChangeEvent</c>，闭合值域）：1 提单 / 2 同意 / 3 驳回 / 4 转审 /
    /// 5 催办 / 6 撤销 / 8 通过后撤销 / 10 添加备注 / 11 回退给指定审批人 / 12 添加审批人 / 13 加签并同意 /
    /// 14 已办理 / 15 已转交（14/15 仅自建与代开发页文档列出，第三方页止于 13）。
    /// </summary>
    [PayloadField("StatuChangeEvent")]
    public long? StatuChangeEvent { get; set; }
}
