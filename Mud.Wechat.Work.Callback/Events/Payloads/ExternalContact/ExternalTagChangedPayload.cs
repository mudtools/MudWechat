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
/// 企业客户标签变更事件载荷（<b>结构族</b>：官方事件键 <c>change_external_tag</c>，覆盖
/// <c>create</c> / <c>update</c> / <c>delete</c> / <c>shuffle</c> 四类 <c>ChangeType</c>；官方 92130/92277/96361）。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：本族 <c>ChangeType</c> 为裸 <c>create</c>/<c>update</c>/<c>delete</c>
/// （与客户群族同名），逐 <c>ChangeType</c> 键无法消歧 ⇒ 事件键为族事件值，
/// 具体类别由信封 <c>ChangeType</c> 判别。
/// </para>
/// <para>
/// <b>权限分层</b>：接收标签类事件需「企业客户权限→客户联系→管理企业客户标签」权限；
/// <see cref="StrategyId"/> 仅回调给「客户联系」应用。删除标签组时其下所有标签同时删除但<b>不会</b>逐一回调。
/// </para>
/// <para>
/// <b>重排事件（shuffle）</b>：<see cref="TagId"/> 为发生重排的标签组 id（为空表示全部标签组顺序变化）；
/// 收到后应尽快全量同步标签 order 值。注意重排报文的 <c>StrategyId</c> 为字符串形态（非数值），
/// 故本字段按文本承载。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalTag })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalTag })]
public sealed partial class ExternalTagChangedPayload : WechatCallbackPayload
{
    /// <summary>标签/标签组 id（官方 <c>Id</c>；重排事件为发生重排的标签组 id，为空表示全部标签组顺序变化）。</summary>
    [PayloadField("Id")]
    public string? TagId { get; set; }

    /// <summary>变更对象类型（官方 <c>TagType</c>；仅创建/变更/删除事件携带）：<c>tag</c>（标签）/ <c>tag_group</c>（标签组）。</summary>
    [PayloadField("TagType")]
    public string? TagType { get; set; }

    /// <summary>所属规则组 id（官方 <c>StrategyId</c>；官方重排报文为字符串形态 ⇒ 按文本承载，仅「客户联系」应用可收到）。</summary>
    [PayloadField("StrategyId")]
    public string? StrategyId { get; set; }
}
