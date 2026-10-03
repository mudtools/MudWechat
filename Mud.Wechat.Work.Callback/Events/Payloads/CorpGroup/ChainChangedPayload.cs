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
/// 上下游变更事件载荷（<b>结构族</b>：覆盖 <c>change_chain</c> 的全部 9 个 <c>ChangeType</c>；官方 95796）。
/// </summary>
/// <remarks>
/// <para>
/// <b>1 型覆盖 9 键</b>：官方 9 类变更的报文结构仅三族且同构 ——
/// 空间族（<c>create_chain</c>/<c>update_chain</c>/<c>delete_chain</c>）仅 <c>ChainId</c>；
/// 分组族（<c>create_group</c>/<c>update_group</c>/<c>delete_group</c>）带 <see cref="GroupIds"/>；
/// 企业族（<c>corp_join</c>/<c>update_corp</c>/<c>remove_corp</c>）带 <see cref="CorpIds"/>。
/// 具体类别由信封 <c>ChangeType</c> 判别，故 3 个旧 DTO 进一步收敛为 1 个载荷。
/// </para>
/// <para>
/// <b>官方开放面</b>：仅<b>企业自建应用</b>可配置接收（配置到「上下游-可调用接口的应用」并开启
/// 「上下游变更回调」）；第三方/代开发暂不支持；<b>由上下游系统应用触发的变更不回调</b>。
/// 该约束由事件键级开放面声明承载（<c>SupportedAppTypes = Internal</c>，见 <c>OfficialPayloadContracts</c>）。
/// </para>
/// <para>
/// <c>corp_join</c> 仅对已加入上下游的企业产生事件；<c>update_corp</c> 在变更企业分组时触发。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.ChangeChain,
    RequiredFamily = WechatCallbackEventFamily.ChainChange,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.CreateChain,
        WechatCallbackEventTypes.UpdateChain,
        WechatCallbackEventTypes.DeleteChain,
        WechatCallbackEventTypes.CreateGroup,
        WechatCallbackEventTypes.UpdateGroup,
        WechatCallbackEventTypes.DeleteGroup,
        WechatCallbackEventTypes.CorpJoin,
        WechatCallbackEventTypes.UpdateCorp,
        WechatCallbackEventTypes.RemoveCorp })]
public sealed partial class ChainChangedPayload : WechatCallbackPayload
{
    /// <summary>上下游空间 id（官方 <c>ChainId</c>）。</summary>
    [PayloadField("ChainId")]
    public string? ChainId { get; set; }

    /// <summary>
    /// 变更涉及的分组 id 列表（官方 <c>GroupIds/GroupId</c> 嵌套节点；
    /// 仅分组族事件命中，其余为空列表）。
    /// </summary>
    [PayloadField("GroupIds", Format = PayloadFieldFormat.Items, ItemName = "GroupId")]
    public List<string> GroupIds { get; set; } = new List<string>();

    /// <summary>
    /// 变更涉及的企业 id 列表（官方 <c>CorpIds/CorpId</c> 嵌套节点；
    /// 仅企业族事件命中，其余为空列表）。
    /// </summary>
    [PayloadField("CorpIds", Format = PayloadFieldFormat.Items, ItemName = "CorpId")]
    public List<string> CorpIds { get; set; } = new List<string>();
}
