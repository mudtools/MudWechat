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
/// 会话内容存档「客户同意存档事件」载荷（<b>结构族</b>：覆盖官方事件键
/// <c>chat_archive_audit_approved_single</c>（单聊）/ <c>chat_archive_audit_approved_room</c>（群聊）两键；
/// 服务商侧事件，经<b>指令回调 URL</b>（官方「系统事件接收 URL」）以套件信封推送，
/// 外层事件值在 <c>InfoType</c> 节点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>事件语义</b>：客户（外部联系人）确认同意进行聊天内容存档后推送本事件，服务商/企业据此方可拉取
/// 该会话的存档内容（配合 <c>MsgAudit</c> 域的 <c>check_single_agree</c> / <c>check_room_agree</c> 核验同意状态，
/// 以及「获取会话内容」拉取正文）。本事件<b>不携带消息内容</b>。
/// </para>
/// <para>
/// <b>为何一个载荷覆盖两键</b>：两键的官方报文骨架同一，差异仅一个专属字段 —— 群聊变体携带
/// <see cref="ChatId"/>，单聊变体不携带。故以一份可空超集承载（ADR-14），<b>缺失字段为
/// <see langword="null"/></b>，处理器不得假设必有值。
/// </para>
/// <para>
/// <b>不入载荷的字段</b>：<c>SuiteId</c> 与 <c>AuthCorpId</c> 由信封
/// <c>WechatCallbackEvent.SuiteId</c> / <c>WechatCallbackEvent.AuthCorpId</c> 承载（本族报文含该两节点，
/// 无须 <c>FromUserName</c> 兜底）；<c>InfoType</c> 即事件键；<c>TimeStamp</c> 属信封字段（ADR-9）。
/// </para>
/// <para>
/// <b>开放面</b>：本族走套件信封（<c>InfoType</c> 非空）⇒ 事件族为
/// <see cref="WechatCallbackEventFamily.Authorization"/>，开放面声明为族默认
/// 「第三方 | 代开发 × 套件指令通道」（不宽于默认，组合根期 fail-fast 校验）。
/// <b>自建应用不开放</b>：官方自建文档树内本事件属「获取会话内容」的配套服务商通知。
/// </para>
/// <para>
/// <b>待官方逐页核验</b>：官方文档站为 SPA、正文当前不可达，本载荷的<b>事件键与字段名以本地 SKIT 源码
/// （<c>SKIT.FlurlHttpClient.Wechat.Work/Events/Service/ChatArchive/ChatArchiveAuditApprovedSingleEvent.cs</c>）
/// 为对齐依据</b>；官方 URL 存在分歧（SKIT 引 path/99532，补齐方案引 path/101385），
/// 恢复可达后须逐页核验并同步 <c>WechatCallbackContractGuards</c>（CB2/CB4/CB4b/CB4c/CB4d）计数与键集。
/// </para>
/// <para>
/// <b>官方文档</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/101385">path 101385 客户同意进行聊天内容存档事件回调</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/99532">path 99532 同名页（SKIT 所引）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Authorization,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[]
    {
        WechatCallbackEventTypes.ChatArchiveAuditApprovedSingle,
        WechatCallbackEventTypes.ChatArchiveAuditApprovedRoom,
    })]
public sealed partial class ChatArchiveAuditApprovedPayload : WechatCallbackPayload
{
    /// <summary>
    /// 企业服务人员的 OpenUserId（官方 <c>OpenUserID</c>，<b>大小写照官方原文</b>：<c>ID</c> 全大写）。
    /// </summary>
    [PayloadField("OpenUserID")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 外部联系人（客户）UserId（官方 <c>ExternalUserID</c>）—— 同意的主体，单聊与群聊变体均携带。
    /// </summary>
    [PayloadField("ExternalUserID")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 客户群 ID（官方 <c>ChatId</c>）：<b>仅群聊变体（chat_archive_audit_approved_room）携带</b>，
    /// 单聊变体缺失为 <see langword="null"/>。
    /// </summary>
    [PayloadField("ChatId")]
    public string? ChatId { get; set; }
}
