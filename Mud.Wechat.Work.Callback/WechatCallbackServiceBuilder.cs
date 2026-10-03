// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// 回调服务建造者（对齐 <c>FeishuWebhookServiceBuilder</c> 的极简 API；v1 方案 §5.8）：
/// <c>AddWechatCallback(...)</c> 返回本建造者，链式注册类型化处理器与拦截器。
/// </summary>
/// <remarks>
/// <para>
/// 注册<b>即时生效</b>（组合根期直接写入注册表单例并 <c>TryAddTransient</c>），
/// <b>无 <c>Build()</c> 步骤、无 Freeze</b>（v1.2 §5.6）：<c>appKey</c> 传 <c>null</c>
/// 注册到通配键（全局生效，D11）；传显式应用键则仅该应用的回调事件路由到对应条目。
/// </para>
/// </remarks>
public sealed class WechatCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly WechatCallbackHandlerRegistry _handlers;
    private readonly WechatCallbackInterceptorRegistry _interceptors;
    private readonly IWechatPayloadContractRegistry _payloadContracts;

    internal WechatCallbackServiceBuilder(
        IServiceCollection services,
        WechatCallbackHandlerRegistry handlers,
        WechatCallbackInterceptorRegistry interceptors,
        IWechatPayloadContractRegistry payloadContracts)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
        _payloadContracts = payloadContracts ?? throw new ArgumentNullException(nameof(payloadContracts));
    }

    /// <summary>
    /// 登记事件键契约（<b>宿主扩展点</b>：供官方未覆盖的事件族或宿主私有事件键使用）。
    /// </summary>
    /// <typeparam name="TPayload">载荷类型（须为 <c>partial</c> 并标注 <c>[PayloadContract]</c>，
    /// 或在手写链形态下自行提供 <c>PayloadFieldMap</c> 静态成员）。</typeparam>
    /// <param name="eventTypeKey">事件类型键（宿主自有常量亦可）。</param>
    /// <param name="map">载荷的字段映射表（在具体类型处做静态成员访问；<b>无反射</b>）。</param>
    /// <returns>建造者实例（链式）。</returns>
    /// <remarks>
    /// <para>
    /// <b>为何不提供 <c>AddPayload&lt;TPayload&gt;(key)</c> 单参重载</b>：C# 禁止在泛型上下文访问
    /// 类型参数的静态成员（<c>TPayload.PayloadFieldMap</c> 是 CS0712），
    /// 而 <c>static abstract</c> 需 net7+ 运行时支持（本包含 <c>netstandard2.0</c>）。
    /// ⇒ 改为由调用点在<b>具体类型</b>处书写，既合法又保持编译期完全类型化。
    /// </para>
    /// <para>
    /// 契约默认<b>继承族级开放面</b>（<c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>）。
    /// 官方已文档化的事件族若需显式声明，请用 <see cref="AddPayloadWithOpenSurface{TPayload}"/>。
    /// </para>
    /// </remarks>
    public WechatCallbackServiceBuilder AddPayload<TPayload>(
        string eventTypeKey, IPayloadFieldMap<TPayload> map)
        where TPayload : class
    {
        if (string.IsNullOrEmpty(eventTypeKey))
            throw new ArgumentException("事件键不得为空。", nameof(eventTypeKey));

        if (map == null)
            throw new ArgumentNullException(nameof(map));

        if (map is not IPayloadContractAccessor accessor)
        {
            throw new ArgumentException(
                "映射表 " + map.GetType().FullName + " 未实现 " + nameof(IPayloadContractAccessor) +
                "；请使用上游 PayloadFieldMap<T> 构建映射表。", nameof(map));
        }

        _payloadContracts.Register(
            WechatPayloadContract.Create(eventTypeKey, accessor));
        return this;
    }

    /// <summary>
    /// 登记事件键契约并<b>显式声明事件键级开放面</b>（ADR-15）。
    /// </summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="map">载荷的字段映射表。</param>
    /// <param name="supportedAppTypes">允许的应用模式集合。</param>
    /// <param name="requiredChannel">要求的回调通道。</param>
    /// <param name="requiredEvent">要求信封的 <c>Event</c> 值（防同名 <c>ChangeType</c> 跨族串门）；可空。</param>
    /// <param name="requiredFamily">要求的事件族；可空。</param>
    /// <returns>建造者实例（链式）。</returns>
    public WechatCallbackServiceBuilder AddPayloadWithOpenSurface<TPayload>(
        string eventTypeKey,
        IPayloadFieldMap<TPayload> map,
        WechatAppTypeSet supportedAppTypes,
        WechatCallbackChannel requiredChannel,
        string? requiredEvent = null,
        WechatCallbackEventFamily? requiredFamily = null)
        where TPayload : class
    {
        return AddPayloadWithOpenSurfaces(
            eventTypeKey, map,
            new[] { new WechatOpenSurface(supportedAppTypes, requiredChannel) },
            requiredEvent, requiredFamily);
    }

    /// <summary>
    /// 登记事件键契约并<b>显式声明多组事件键级开放面</b>（「（模式集合, 通道）」组合对，ADR-15）。
    /// </summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="map">载荷的字段映射表。</param>
    /// <param name="openSurfaces">开放面组合对集合（任一组命中即许可分发；每组都不得宽于官方族默认）。</param>
    /// <param name="requiredEvent">要求信封的外层事件值（<c>Event</c> 节点，套件信封为 <c>InfoType</c> 节点）；可空。</param>
    /// <param name="requiredFamily">要求的事件族；可空。</param>
    /// <returns>建造者实例（链式）。</returns>
    /// <remarks>
    /// 官方部分事件族的接入方式按应用模式分通道（如客户联系/获客助手族：自建·代开发经应用数据通道、
    /// 第三方经套件指令通道）—— 单一「模式集合 × 通道」无法表达，需按组合对声明。
    /// </remarks>
    public WechatCallbackServiceBuilder AddPayloadWithOpenSurfaces<TPayload>(
        string eventTypeKey,
        IPayloadFieldMap<TPayload> map,
        WechatOpenSurface[] openSurfaces,
        string? requiredEvent = null,
        WechatCallbackEventFamily? requiredFamily = null)
        where TPayload : class
    {
        if (map is not IPayloadContractAccessor accessor)
        {
            throw new ArgumentException(
                "映射表 " + map.GetType().FullName + " 未实现 " + nameof(IPayloadContractAccessor) + "。", nameof(map));
        }

        _payloadContracts.Register(WechatPayloadContract.CreateWithOpenSurfaces(
            eventTypeKey, accessor, openSurfaces, requiredEvent, requiredFamily));
        return this;
    }

    /// <summary>
    /// 注册类型化事件处理器。
    /// </summary>
    /// <typeparam name="THandler">处理器实现类型（须在 <see cref="IWechatCallbackEventHandler.SupportedEventType"/>
    /// 返回 <see cref="WechatCallbackEventTypes"/> 常量或空串=兜底）。</typeparam>
    /// <param name="appKey">应用键；<c>null</c> = 全局（通配键）。</param>
    /// <returns>建造者实例（链式）。</returns>
    /// <remarks>
    /// 处理器实例默认按 Transient 注册（<c>TryAdd</c> 语义：宿主可先行自行注册覆盖生命周期），
    /// 分发器在请求 scope 内解析。
    /// <paramref name="appKey"/> 传 <c>null</c> 注册到通配键（全局生效，D11）。
    /// </remarks>
#if NET6_0_OR_GREATER
    public WechatCallbackServiceBuilder AddHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        string? appKey = null)
        where THandler : class, IWechatCallbackEventHandler
#else
    public WechatCallbackServiceBuilder AddHandler<THandler>(string? appKey = null)
        where THandler : class, IWechatCallbackEventHandler
#endif
    {
        _services.TryAddTransient<THandler>();
        _handlers.Register(appKey is { Length: > 0 } ? appKey : WechatCallbackOptions.WildcardAppKey, typeof(THandler));
        return this;
    }

    /// <summary>
    /// 注册事件拦截器。
    /// </summary>
    /// <typeparam name="TInterceptor">拦截器实现类型。</typeparam>
    /// <param name="appKey">应用键；<c>null</c> = 全局（通配键）。</param>
    /// <returns>建造者实例（链式）。</returns>
#if NET6_0_OR_GREATER
    public WechatCallbackServiceBuilder AddInterceptor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TInterceptor>(
        string? appKey = null)
        where TInterceptor : class, IWechatCallbackEventInterceptor
#else
    public WechatCallbackServiceBuilder AddInterceptor<TInterceptor>(string? appKey = null)
        where TInterceptor : class, IWechatCallbackEventInterceptor
#endif
    {
        _services.TryAddTransient<TInterceptor>();
        _interceptors.Register(appKey is { Length: > 0 } ? appKey : WechatCallbackOptions.WildcardAppKey, typeof(TInterceptor));
        return this;
    }
}
