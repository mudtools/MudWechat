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
/// 客户群变更事件载荷（<b>结构族</b>：官方事件键 <c>change_external_chat</c>，覆盖
/// <c>create</c> / <c>update</c> / <c>dismiss</c> 三类 <c>ChangeType</c>；官方 92130/92277/96361）。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：本族 <c>ChangeType</c> 为裸 <c>create</c>/<c>update</c>/<c>dismiss</c>
/// （与企业客户标签族同名），逐 <c>ChangeType</c> 键无法消歧 ⇒ 事件键为族事件值，
/// 具体类别由信封 <c>ChangeType</c> 判别。
/// </para>
/// <para>
/// <b>权限分层（处理器不得假设必有值）</b>：仅 <c>update</c> 携带成员/版本字段；接收本族事件需
/// 「客户联系-可调用接口的应用」（自建）或「客户联系→客户基础信息」权限（第三方/代开发）。
/// </para>
/// <para>
/// <b>版本号一致性技巧（官方建议）</b>：本地存储各群最新版本号，收到成员变动事件时比对
/// <see cref="LastMemberVersion"/> 与本地值 —— 一致则可信并更新；不一致说明回调丢失或乱序，
/// 应调「获取客户群详情」拉取最新数据，以低成本保证一致性。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 客户联系 事件回调（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277 客户联系 事件回调（第三方，套件信封）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96361">path 96361 客户联系 事件回调（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalChat })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalChat })]
public sealed partial class ExternalChatChangedPayload : WechatCallbackPayload
{
    /// <summary>客户群 id（官方 <c>ChatId</c>；三类事件均携带）。</summary>
    [PayloadField("ChatId")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 变更详情（官方 <c>UpdateDetail</c>；仅 <c>update</c> 携带）：
    /// <c>add_member</c>（入群）/ <c>del_member</c>（退群）/ <c>change_owner</c> / <c>change_name</c> / <c>change_notice</c>。
    /// </summary>
    [PayloadField("UpdateDetail")]
    public string? UpdateDetail { get; set; }

    /// <summary>入群方式（官方 <c>JoinScene</c>；仅成员变动时携带）：0 = 成员邀请入群（含邀请链接），3 = 扫群二维码入群。</summary>
    [PayloadField("JoinScene")]
    public long? JoinScene { get; set; }

    /// <summary>退群方式（官方 <c>QuitScene</c>；仅成员变动时携带）：0 = 自己退群，1 = 群主/群管理员移出。</summary>
    [PayloadField("QuitScene")]
    public long? QuitScene { get; set; }

    /// <summary>成员变更数量（官方 <c>MemChangeCnt</c>；仅成员变动时携带）。</summary>
    [PayloadField("MemChangeCnt")]
    public long? MemberChangeCount { get; set; }

    /// <summary>成员变更列表（官方 <c>MemChangeList/Item</c> 嵌套节点；仅成员变动时携带，无该节点时为空列表）。</summary>
    [PayloadField("MemChangeList", Format = PayloadFieldFormat.Items, ItemName = "Item")]
    public List<string> MemberChangeList { get; set; } = new List<string>();

    /// <summary>变更前的群成员版本号（官方 <c>LastMemVer</c>；仅成员变动时携带，用于回调丢失/乱序检测）。</summary>
    [PayloadField("LastMemVer")]
    public string? LastMemberVersion { get; set; }

    /// <summary>变更后的群成员版本号（官方 <c>CurMemVer</c>；仅成员变动时携带）。</summary>
    [PayloadField("CurMemVer")]
    public string? CurrentMemberVersion { get; set; }
}
