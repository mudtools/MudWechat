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
/// 部门变更事件载荷（<b>结构族</b>：覆盖 <c>create_party</c> / <c>update_party</c> / <c>delete_party</c>；官方 90971）。
/// </summary>
/// <remarks>
/// <para>
/// <c>update_party</c> 官方仅 <b>ParentId 变更</b>触发（<c>Name</c>/<c>Order</c> 变更不推送）；
/// <c>delete_party</c> 仅 <c>ParentId</c> 命中。字段是否齐全同样受<b>权限分层</b>约束
/// （2022-08-15 后新 URL 的部门事件仅回调 id 子集时不返回 <c>Name</c>）。
/// </para>
/// <para>三模式共用一份可空超集（ADR-14），不得按应用模式分叉。</para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class ContactPartyChangedPayload : WechatCallbackPayload
{
    /// <summary>部门 id（官方 <c>Id</c>）。</summary>
    [PayloadField("Id")]
    public string? PartyId { get; set; }

    /// <summary>部门名称（官方 <c>Name</c>；需通讯录部门权限）。</summary>
    [PayloadField("Name")]
    public string? Name { get; set; }

    /// <summary>父部门 id（官方 <c>ParentId</c>；根部门为 1）。</summary>
    [PayloadField("ParentId")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 在父部门中的次序值（官方 <c>Order</c>；越大越靠前；需通讯录部门权限，仅 <c>create_party</c> 命中）。
    /// </summary>
    [PayloadField("Order")]
    public long? Order { get; set; }
}
