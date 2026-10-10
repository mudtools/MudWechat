// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using Mud.HttpUtils.Payloads;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 事件键级载荷契约（v2.2 方案 ADR-15）：上游映射表 + 本仓库的两级开放面声明。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本类型</b>：上游 <see cref="IPayloadContractAccessor"/> 只承载「如何绑定字段」，
/// 不认识企业微信的事件语义（上游 §11.9/§11.10 边界：上游不引入 XML，也不引入企业微信语义）。
/// 「该事件键在哪些条件下合法」属<b>本仓库</b>知识，故由本记录承载。
/// </para>
/// <para>
/// <b>两级闸</b>：① 族前置条件（<see cref="RequiredEvent"/> / <see cref="RequiredFamily"/>）——
/// 防同名 <c>ChangeType</c> 跨族串门（行为保持矩阵 B4）；② 事件键级开放面
/// （<see cref="OpenSurfaces"/>）——补族级闸的精度缺口
/// （宿主注册新 <c>Event</c> 值会落 <c>Unknown</c> 族而被族闸放行）。
/// </para>
/// </remarks>
public sealed class WechatPayloadContract
{
    private readonly WechatOpenSurface[] _openSurfaces;

    private WechatPayloadContract(
        string eventTypeKey,
        IPayloadContractAccessor accessor,
        string? requiredEvent,
        WechatCallbackEventFamily? requiredFamily,
        WechatOpenSurface[] openSurfaces)
    {
        EventTypeKey = eventTypeKey;
        Accessor = accessor;
        RequiredEvent = requiredEvent;
        RequiredFamily = requiredFamily;
        _openSurfaces = openSurfaces;
    }

    /// <summary>事件类型键（= <c>WechatCallbackEvent.EventTypeKey</c>）。</summary>
    public string EventTypeKey { get; }

    /// <summary>上游字段映射契约（非泛型视图）。</summary>
    public IPayloadContractAccessor Accessor { get; }

    /// <summary>要求信封的<b>外层事件值</b>（<c>Event</c> 节点，套件信封为 <c>InfoType</c> 节点，
    /// 如 <c>change_contact</c> / <c>change_external_contact</c>）；<c>null</c> = 不校验。</summary>
    public string? RequiredEvent { get; }

    /// <summary>要求的事件族；<c>null</c> = 不校验。</summary>
    public WechatCallbackEventFamily? RequiredFamily { get; }

    /// <summary>
    /// 事件键级开放面声明：「（允许的应用模式集合, 要求的回调通道）」组合对集合，任一组命中即许可。
    /// </summary>
    /// <remarks>
    /// 显式声明形态（<see cref="CreateWithOpenSurface"/> / <see cref="CreateWithOpenSurfaces"/>）非空；
    /// 继承族默认形态（<see cref="Create"/>）为空列表 —— 事件键闸放行，由族级闸
    /// （<c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>）承担开放面判定。
    /// </remarks>
    public IReadOnlyList<WechatOpenSurface> OpenSurfaces => _openSurfaces;

    /// <summary>
    /// 报文是否满足本契约的<b>族前置条件</b>（B4：防同名 <c>ChangeType</c> 跨族串门）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="RequiredEvent"/> 比对的是<b>外层事件值</b>（<c>Event</c> 节点优先、套件信封回退
    /// <c>InfoType</c> 节点，与 <c>WechatCallbackEvent.EventTypeKey</c> 的口径一致）——
    /// 第三方应用的指令回调（套件信封）无 <c>Event</c> 节点，客户联系/获客族事件的外层事件值在
    /// <c>InfoType</c> 上。只依赖信封与事件族，<b>不需要</b>应用类型/通道 ⇒
    /// 读取器即可独立校验（纵深防御：分发器的事件键级闸之外，读取路径再判一次）。
    /// </para>
    /// </remarks>
    public bool MatchesEnvelope(WechatCallbackEvent evt)
    {
        if (evt == null)
            return false;

        if (RequiredEvent != null)
        {
            var outerEvent = evt.Event is { Length: > 0 } ? evt.Event : evt.InfoType;
            if (!string.Equals(outerEvent, RequiredEvent, StringComparison.Ordinal))
                return false;
        }

        if (RequiredFamily != null && evt.EventFamily != RequiredFamily.Value)
            return false;

        return true;
    }

    /// <summary>
    /// 事件键级闸：族前置条件 + 模式/通道声明的联合判定。
    /// </summary>
    /// <param name="evt">回调事件信封。</param>
    /// <param name="appType">当前回调条目的应用类型。</param>
    /// <param name="channel">当前回调条目的回调通道。</param>
    /// <returns><c>true</c> = 许可分发；<c>false</c> = 不适用于本回调条目（分发器返回 <c>Rejected</c> → 200 不重推）。</returns>
    public bool IsOpenFor(WechatCallbackEvent evt, WechatAppType appType, WechatCallbackChannel channel)
    {
        if (!MatchesEnvelope(evt))
            return false;

        // 开放面声明为空 ⇒ 继承族默认（由 WechatAppCallbackOptions.IsEventFamilyAllowed 兜底）。
        if (_openSurfaces.Length == 0)
            return true;

        var appTypeSet = ToSet(appType);
        foreach (var surface in _openSurfaces)
        {
            if (surface.RequiredChannel == channel && (surface.SupportedAppTypes & appTypeSet) != 0)
                return true;
        }

        return false;
    }

    /// <summary>把单一应用类型映射为集合位。</summary>
    public static WechatAppTypeSet ToSet(WechatAppType appType)
        => appType switch
        {
            WechatAppType.Internal => WechatAppTypeSet.Internal,
            WechatAppType.ThirdParty => WechatAppTypeSet.ThirdParty,
            WechatAppType.Provider => WechatAppTypeSet.Provider,
            _ => WechatAppTypeSet.None,
        };

    /// <summary>创建契约（开放面声明留空 = 继承族默认）。</summary>
    public static WechatPayloadContract Create(
        string eventTypeKey,
        IPayloadContractAccessor accessor,
        string? requiredEvent = null,
        WechatCallbackEventFamily? requiredFamily = null)
        => new(eventTypeKey, accessor, requiredEvent, requiredFamily, Array.Empty<WechatOpenSurface>());

    /// <summary>
    /// 创建契约并显式声明单一事件键级开放面（组合对便捷形态）。
    /// </summary>
    /// <remarks>
    /// <b>注册期 fail-fast（ADR-15）</b>：声明<b>不得宽于</b>事件族的官方默认（
    /// <see cref="WechatEventFamilyOpenSurface.ValidateNotWiderThanFamilyDefault"/>）。
    /// 这条校验防的是「宿主把仅自建的 <c>change_chain</c> 声明为三类应用全开放」——
    /// 那会让事件键闸形同虚设（与族级闸 <c>Unknown → true</c> 缺口同构的风险，只是挪到了声明面）。
    /// </remarks>
    /// <exception cref="ArgumentException">声明宽于官方族默认，或通道与官方不一致。</exception>
    public static WechatPayloadContract CreateWithOpenSurface(
        string eventTypeKey,
        IPayloadContractAccessor accessor,
        WechatAppTypeSet supportedAppTypes,
        WechatCallbackChannel requiredChannel,
        string? requiredEvent = null,
        WechatCallbackEventFamily? requiredFamily = null)
        => CreateWithOpenSurfaces(
            eventTypeKey,
            accessor,
            new[] { new WechatOpenSurface(supportedAppTypes, requiredChannel) },
            requiredEvent,
            requiredFamily);

    /// <summary>
    /// 创建契约并显式声明<b>多组</b>事件键级开放面（「（模式集合, 通道）」组合对）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何需要多组</b>：客户联系/获客族的官方接入方式按应用模式分通道 —— 自建/代开发经应用数据通道、
    /// 第三方经套件指令通道（官方 92130/92277/96361/97299/98958），单一组合对无法表达。
    /// 任一组命中即许可分发（<see cref="IsOpenFor"/>）。
    /// </para>
    /// <para>「不得宽于官方族默认」的注册期 fail-fast 语义与单组形态一致：声明的<b>每一组</b>
    /// 都必须被官方基线的某组合对覆盖（模式集合为其子集且通道一致）。</para>
    /// </remarks>
    /// <exception cref="ArgumentException">开放面为空/含 <see cref="WechatAppTypeSet.None"/>，
    /// 或声明宽于官方族默认，或通道与官方不一致。</exception>
    public static WechatPayloadContract CreateWithOpenSurfaces(
        string eventTypeKey,
        IPayloadContractAccessor accessor,
        WechatOpenSurface[] openSurfaces,
        string? requiredEvent = null,
        WechatCallbackEventFamily? requiredFamily = null)
    {
        if (openSurfaces == null || openSurfaces.Length == 0)
        {
            throw new ArgumentException(
                "事件键 " + eventTypeKey + " 的开放面声明不得为空（ADR-15：官方事件键必须显式声明开放面）。",
                nameof(openSurfaces));
        }

        foreach (var surface in openSurfaces)
        {
            if (surface.SupportedAppTypes == WechatAppTypeSet.None)
            {
                throw new ArgumentException(
                    "事件键 " + eventTypeKey + " 的开放面声明含 None（空模式集合）—— 该声明不可命中任何应用模式。",
                    nameof(openSurfaces));
            }
        }

        WechatEventFamilyOpenSurface.ValidateNotWiderThanFamilyDefault(
            eventTypeKey, requiredFamily, openSurfaces);

        return new WechatPayloadContract(
            eventTypeKey, accessor, requiredEvent, requiredFamily, openSurfaces);
    }
}
