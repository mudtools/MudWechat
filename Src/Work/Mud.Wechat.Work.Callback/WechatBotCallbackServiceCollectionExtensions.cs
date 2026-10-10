// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.Wechat.Work.Abstractions.Callback.Bots;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 智能机器人回调注册扩展：链式注册返回式处理器（形态对齐 <c>AddWechatCallback</c> 的建造者模型）。
/// </summary>
/// <remarks>
/// <para>
/// <b>无独立配置面</b>：智能机器人的回调凭据（PushToken / PushEncodingAESKey）与其它回调条目<b>同源</b>
/// —— 复用 <see cref="WechatCallbackOptions.Apps"/> 单字典，以 <c>Channel = Bot</c> 消歧
/// （字典键唯一性天然排除「同一键既是 App 又是 Bot」）。故本扩展只承载处理器注册，
/// 凭据配置仍经 <c>AddWechatCallback</c> 写入。
/// </para>
/// <para>
/// <b>前置依赖</b>：须先调用 <c>AddWechatCallback(...)</c>（接收器 / 分发器 / 注册表实例在彼处创建），
/// 否则 fail-fast 抛出明确错误，不静默降级。
/// </para>
/// </remarks>
public sealed class WechatBotCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly WechatBotHandlerRegistry _handlers;

    internal WechatBotCallbackServiceBuilder(IServiceCollection services, WechatBotHandlerRegistry handlers)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    /// <summary>
    /// 注册智能机器人回调处理器（返回式应答）。
    /// </summary>
    /// <typeparam name="THandler">处理器实现类型（<c>SupportedEventType</c> 须返回
    /// <c>WechatBotEventTypes</c> 常量或空串 = 兜底）。</typeparam>
    /// <param name="botKey">回调配置键（与 <see cref="WechatCallbackOptions.Apps"/> 字典键一致）；
    /// <c>null</c> = 全局（通配键）。</param>
    /// <returns>建造者实例（链式）。</returns>
    /// <remarks>处理器实例默认按 Transient 注册（<c>TryAdd</c> 语义：宿主可先行自行注册覆盖生命周期）。</remarks>
#if NET6_0_OR_GREATER
    public WechatBotCallbackServiceBuilder AddHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        string? botKey = null)
        where THandler : class, IWechatBotCallbackEventHandler
#else
    public WechatBotCallbackServiceBuilder AddHandler<THandler>(string? botKey = null)
        where THandler : class, IWechatBotCallbackEventHandler
#endif
    {
        _services.TryAddTransient<THandler>();
        _handlers.Register(
            botKey is { Length: > 0 } ? botKey : WechatCallbackOptions.WildcardAppKey,
            typeof(THandler));
        return this;
    }
}

/// <summary>智能机器人回调服务注册入口。</summary>
public static class WechatBotCallbackServiceCollectionExtensions
{
    /// <summary>
    /// 注册智能机器人回调处理器面（接收器 / 分发器 / 注册表在 <c>AddWechatCallback</c> 中装配）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>建造者（链式注册 <see cref="WechatBotCallbackServiceBuilder.AddHandler{THandler}"/>）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未先调用 <c>AddWechatCallback</c>（缺少智能机器人处理器注册表）。</exception>
    /// <remarks>
    /// 智能机器人回调的 URL 验证（GET echo）与 XML 侧共用 <c>UseWechatWebhook()</c> 中间件，
    /// 无需额外的管道扩展。
    /// </remarks>
    public static WechatBotCallbackServiceBuilder AddWechatBotCallback(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var registry = services
            .Where(d => d.ServiceType == typeof(WechatBotHandlerRegistry))
            .Select(d => d.ImplementationInstance)
            .OfType<WechatBotHandlerRegistry>()
            .FirstOrDefault();

        if (registry == null)
        {
            throw new InvalidOperationException(
                "未找到智能机器人回调处理器注册表。请先调用 AddWechatCallback(...) 完成回调服务装配，再调用 AddWechatBotCallback()。");
        }

        return new WechatBotCallbackServiceBuilder(services, registry);
    }
}
