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
/// （<see cref="SupportedAppTypes"/> / <see cref="RequiredChannel"/>）——补族级闸的精度缺口
/// （宿主注册新 <c>Event</c> 值会落 <c>Unknown</c> 族而被族闸放行）。
/// 两者为 <see langword="null"/> 时表示「继承族默认」。
/// </para>
/// </remarks>
public sealed class WechatPayloadContract
{
    private WechatPayloadContract(
        string eventTypeKey,
        IPayloadContractAccessor accessor,
        string? requiredEvent,
        WechatCallbackEventFamily? requiredFamily,
        WechatAppTypeSet? supportedAppTypes,
        WechatCallbackChannel? requiredChannel)
    {
        EventTypeKey = eventTypeKey;
        Accessor = accessor;
        RequiredEvent = requiredEvent;
        RequiredFamily = requiredFamily;
        SupportedAppTypes = supportedAppTypes;
        RequiredChannel = requiredChannel;
    }

    /// <summary>事件类型键（= <c>WechatCallbackEvent.EventTypeKey</c>）。</summary>
    public string EventTypeKey { get; }

    /// <summary>上游字段映射契约（非泛型视图）。</summary>
    public IPayloadContractAccessor Accessor { get; }

    /// <summary>要求信封的 <c>Event</c> 值（如 <c>change_contact</c> / <c>change_chain</c>）；<c>null</c> = 不校验。</summary>
    public string? RequiredEvent { get; }

    /// <summary>要求的事件族；<c>null</c> = 不校验。</summary>
    public WechatCallbackEventFamily? RequiredFamily { get; }

    /// <summary>事件键级开放面：允许的应用模式集合；<c>null</c> = 继承族默认。</summary>
    public WechatAppTypeSet? SupportedAppTypes { get; }

    /// <summary>事件键级开放面：要求的回调通道；<c>null</c> = 继承族默认。</summary>
    public WechatCallbackChannel? RequiredChannel { get; }

    /// <summary>
    /// 事件键级闸：族前置条件 + 模式/通道声明的联合判定。
    /// </summary>
    /// <param name="evt">回调事件信封。</param>
    /// <param name="appType">当前回调条目的应用类型。</param>
    /// <param name="channel">当前回调条目的回调通道。</param>
    /// <returns><c>true</c> = 许可分发；<c>false</c> = 不适用于本回调条目（分发器返回 <c>Rejected</c> → 200 不重推）。</returns>
    /// <summary>
    /// 报文是否满足本契约的<b>族前置条件</b>（B4：防同名 <c>ChangeType</c> 跨族串门）。
    /// </summary>
    /// <remarks>
    /// 只依赖信封（<c>Event</c> 值与事件族），<b>不需要</b>应用类型/通道 ⇒
    /// 读取器即可独立校验（纵深防御：分发器的事件键级闸之外，读取路径再判一次）。
    /// </remarks>
    public bool MatchesEnvelope(WechatCallbackEvent evt)
    {
        if (evt == null)
            return false;

        if (RequiredEvent != null && !string.Equals(evt.Event, RequiredEvent, StringComparison.Ordinal))
            return false;

        if (RequiredFamily != null && evt.EventFamily != RequiredFamily.Value)
            return false;

        return true;
    }

    public bool IsOpenFor(WechatCallbackEvent evt, WechatAppType appType, WechatCallbackChannel channel)
    {
        if (!MatchesEnvelope(evt))
            return false;

        // 事件键级开放面声明（null ⇒ 继承族默认，由 WechatAppCallbackOptions.IsEventFamilyAllowed 兜底）
        if (RequiredChannel != null && channel != RequiredChannel.Value)
            return false;

        if (SupportedAppTypes != null && (SupportedAppTypes.Value & ToSet(appType)) == 0)
            return false;

        return true;
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
        => new(eventTypeKey, accessor, requiredEvent, requiredFamily, null, null);

    /// <summary>
    /// 创建契约并显式声明事件键级开放面（官方 17 键须显式声明，守卫 CB4c 断言）。
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
    {
        WechatEventFamilyOpenSurface.ValidateNotWiderThanFamilyDefault(
            eventTypeKey, requiredFamily, supportedAppTypes, requiredChannel);

        return new WechatPayloadContract(
            eventTypeKey, accessor, requiredEvent, requiredFamily, supportedAppTypes, requiredChannel);
    }
}
