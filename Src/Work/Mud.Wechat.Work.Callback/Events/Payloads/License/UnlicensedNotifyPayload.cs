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
/// 接口调用许可「接口许可失效通知」事件载荷（<c>unlicensed_notify</c>，官方 path 97195，
/// 仅服务商代开发文档树提供——自建/第三方应用开发无对应事件回调）。
/// </summary>
/// <remarks>
/// <para>
/// <b>事件语义</b>：当许可账号失效（未激活或已过期）的企业成员访问应用或小程序时，
/// 企业微信提示用户联系服务商开通许可账号，并自动推送本事件。
/// <b>成员不在应用可见范围内则不回调</b>（官方权限限制，不得弱化）。
/// </para>
/// <para>
/// <b>事件不携带业务字段</b>：信封之外仅 <see cref="AgentId"/> 一个业务节点（与 <c>msgaudit_notify</c> 同形态）；
/// 触发成员取信封 <c>FromUserName</c>、归属企业取信封 <c>ToUserName</c>。
/// 服务商收到后应开通/激活许可账号。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在服务商代开发应用开发文档树提供该事件 ⇒ 仅代开发开放、应用数据通道承载
/// （<c>Event</c> 信封逐键自指）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97195">path 97195 接口许可失效通知（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.UnlicensedNotify })]
public sealed partial class UnlicensedNotifyPayload : WechatCallbackPayload
{
    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页查看；<c>Event</c> 信封即事件键，无 <c>ChangeType</c> 分组段）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
