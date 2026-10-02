// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 上下游空间变更事件（<c>change_chain</c> + <c>create_chain</c>/<c>update_chain</c>/<c>delete_chain</c>；
/// 官方 95796）。
/// </summary>
/// <remarks>
/// <para>
/// <b>开放面</b>：仅自建应用可配置接收（配置到「上下游-可调用接口的应用」并开启「上下游变更回调」），
/// 第三方/代开发暂不支持；<b>由上下游系统应用触发的变更不回调</b>。
/// </para>
/// <para>
/// 与成员/部门域「逐事件 DTO」不同，上下游 9 类变更的官方报文结构仅三族
/// （空间 = <see cref="ChainId"/>；分组 = <see cref="ChainId"/> + GroupIds 列表；企业 = <see cref="ChainId"/> + CorpIds 列表），
/// 同族内字段完全一致，故按<b>结构族</b>建 DTO（具体变更类别经
/// <see cref="Mud.Wechat.Work.Abstractions.Callback.WechatCallbackEvent.ChangeType"/> 判别），避免 9 份样板。
/// </para>
/// </remarks>
public class ChainChangedEvent
{
    /// <summary>上下游空间 id（ChainId 节点）。</summary>
    public string? ChainId { get; set; }
}

/// <summary>
/// 上下游分组变更事件（<c>change_chain</c> + <c>create_group</c>/<c>update_group</c>/<c>delete_group</c>；
/// 官方 95796）。
/// </summary>
public class ChainGroupChangedEvent
{
    /// <summary>上下游空间 id（ChainId 节点）。</summary>
    public string? ChainId { get; set; }

    /// <summary>变更涉及的分组 id 列表（GroupIds/GroupId 嵌套节点；报文无该节点时为空列表）。</summary>
    public List<string> GroupIds { get; set; } = new();
}

/// <summary>
/// 上下游企业变更事件（<c>change_chain</c> + <c>corp_join</c>/<c>update_corp</c>/<c>remove_corp</c>；
/// 官方 95796）。
/// </summary>
/// <remarks><c>corp_join</c> 仅对已加入上下游的企业产生事件；<c>update_corp</c> 在变更企业分组时触发。</remarks>
public class ChainCorpChangedEvent
{
    /// <summary>上下游空间 id（ChainId 节点）。</summary>
    public string? ChainId { get; set; }

    /// <summary>变更涉及的企业 id 列表（CorpIds/CorpId 嵌套节点；报文无该节点时为空列表）。</summary>
    public List<string> CorpIds { get; set; } = new();
}
