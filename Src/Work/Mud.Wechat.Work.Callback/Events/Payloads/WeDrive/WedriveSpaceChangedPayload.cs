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
/// 微盘空间变更事件载荷（<b>结构族</b>：覆盖 <c>Event = wedrive_space_change</c> 的 3 个 <c>ChangeType</c>：
/// <c>dismiss_space</c> / <c>space_member_change</c> / <c>space_security_settings_change</c>；
/// 官方 97899/97901/97902/97903 自建 · 97973/97976/97977/97978 第三方 · 97933/97935/97936/97937 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族而非逐事件 DTO（ADR-1）</b>：各变更的报文结构同构（信封 + <see cref="SpaceIds"/>），
/// 具体类别由信封 <c>ChangeType</c> 判别。<see cref="SpaceIds"/> 为<b>根下重复同名兄弟元素</b>
/// （官方示例明示两个并列节点、参数表「空间ID列表」），经 <see cref="WechatPayloadConverter.RepeatSiblings"/> 读取；
/// 「空间变更」概述页的单节点形态同被覆盖（恰 1 个元素）。
/// </para>
/// <para>
/// <b>官方文档内部形态差异</b>：概述页（97899）示例 <c>FromUserName</c> 为 <c>sys</c>、
/// 解散空间等页为成员 UserID —— 信封语义按 <c>evt.FromUserName</c> 读取，处理器不得假设固定值。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97899">path 97899 空间变更事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97973">path 97973（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97933">path 97933（服务商代开发）</see>；
/// 解散空间 <see href="https://developer.work.weixin.qq.com/document/path/97901">path 97901</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97976">path 97976</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97935">path 97935</see>（代开发）；
/// 修改空间成员 <see href="https://developer.work.weixin.qq.com/document/path/97902">path 97902</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97977">path 97977</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97936">path 97936</see>（代开发）；
/// 修改空间安全设置 <see href="https://developer.work.weixin.qq.com/document/path/97903">path 97903</see>（自建）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97978">path 97978</see>（第三方）/
/// <see href="https://developer.work.weixin.qq.com/document/path/97937">path 97937</see>（代开发）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.WedriveSpaceChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.DismissSpace,
        WechatCallbackEventTypes.SpaceMemberChange,
        WechatCallbackEventTypes.SpaceSecuritySettingsChange })]
public sealed partial class WedriveSpaceChangedPayload : WechatCallbackPayload
{
    /// <summary>空间 id 列表（官方 <c>SpaceId</c>，根下重复同名兄弟元素；官方概述页亦有单节点形态，均被覆盖）。</summary>
    [PayloadField("SpaceId", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> SpaceIds { get; set; } = new List<string>();
}
