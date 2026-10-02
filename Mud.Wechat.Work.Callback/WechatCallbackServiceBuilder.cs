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
/// <b>无 <c>Build()</c> 步骤、无 Freeze</b>（v1.2 §5.6）：<paramref name="appKey"/> 传 <c>null</c>
/// 注册到通配键（全局生效，D11）；传显式应用键则仅该应用的回调事件路由到对应条目。
/// </para>
/// </remarks>
public sealed class WechatCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly WechatCallbackHandlerRegistry _handlers;
    private readonly WechatCallbackInterceptorRegistry _interceptors;

    internal WechatCallbackServiceBuilder(
        IServiceCollection services,
        WechatCallbackHandlerRegistry handlers,
        WechatCallbackInterceptorRegistry interceptors)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
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
