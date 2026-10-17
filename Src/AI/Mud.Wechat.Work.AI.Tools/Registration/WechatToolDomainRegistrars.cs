// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Mud.Wechat.Work.AI.Tools.Registration;

/// <summary>
/// 域注册器的聚合点：生成物的 DI 装配代码经 <see cref="Add"/> 交入「按需构造的注册器工厂」，
/// 注册表在首次解析时按序汇总。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么用工厂委托而不是直接实例</b>：注册器构造需要执行器实例，而执行器构造又需要 SDK 客户端
/// （其可用性取决于宿主注册了哪些业务域）——「软缺席」语义要求「拿不到依赖 ⇒ 该域工具不进注册表」，
/// 只能在解析期判定，不能在装配期判定。
/// </para>
/// <para>
/// <b>重复装配会显式报错</b>：同一容器重复调用 <see cref="Add"/> 会得到两个域注册器，而工具名重复即失败
/// （见 <see cref="WechatToolRegistry.Register"/>），不会静默叠加。
/// </para>
/// </remarks>
public static class WechatToolDomainRegistrars
{
    /// <summary>登记一个域注册器工厂。</summary>
    /// <param name="services">服务集合（不得为 <see langword="null"/>）。</param>
    /// <param name="factory">工厂（返回 <see langword="null"/> 表示该域本次不参与装配，即软缺席）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="factory"/> 为 <see langword="null"/>。</exception>
    public static IServiceCollection Add(
        IServiceCollection services,
        Func<IServiceProvider, IWechatToolDomainRegistrar?> factory)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        services.AddSingleton(new WechatToolDomainRegistrarFactory(factory));
        return services;
    }

    /// <summary>按注册顺序构造工具注册表。</summary>
    /// <param name="provider">服务提供者（不得为 <see langword="null"/>）。</param>
    /// <returns>已装入全部可用域工具的表。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> 为 <see langword="null"/>。</exception>
    public static WechatToolRegistry Build(IServiceProvider provider)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var registry = new WechatToolRegistry();
        foreach (var factory in provider.GetServices<WechatToolDomainRegistrarFactory>())
        {
            factory.Create(provider)?.Register(registry);
        }

        return registry;
    }

    /// <summary>注册器工厂的 DI 载体（产品内部类型，宿主不直接消费）。</summary>
    private sealed class WechatToolDomainRegistrarFactory
    {
        private readonly Func<IServiceProvider, IWechatToolDomainRegistrar?> _factory;

        public WechatToolDomainRegistrarFactory(Func<IServiceProvider, IWechatToolDomainRegistrar?> factory)
            => _factory = factory;

        public IWechatToolDomainRegistrar? Create(IServiceProvider provider) => _factory(provider);
    }
}
