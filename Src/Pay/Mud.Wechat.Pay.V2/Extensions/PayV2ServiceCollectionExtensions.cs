// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.Extensions;

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 <b>APIv2</b> 业务接口注册入口（与 APIv3 的 <c>AddWechatPayApi</c> 并列、互不牵连）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>AddPayApp</c> 的分工</b>：商户凭据底座（配置 → 多商户管理器 → 上下文）由
/// <c>Mud.Wechat.Pay.Abstractions</c> 的 <c>AddPayApp</c> 装配，且<b>两条线共用</b>；
/// 本入口只装配 APIv2 的业务接口与其专属传输面（XML 序列化器、签名 Handler、证书通道）。
/// </para>
/// <para>
/// <b>调用顺序</b>：<see cref="AddWechatPayV2Api(IServiceCollection, Action{PayV2ServiceBuilder})"/> 在
/// <see cref="PayV2ServiceBuilder.Build"/> 期校验 <c>IWechatPayMerchantManager</c> 已注册
/// ⇒ <c>AddPayApp</c> 必须先调用，否则注册期立即抛出并点名修复方式，而不是拖到首次下单。
/// </para>
/// <para>
/// <b>典型用法</b>：
/// <code>
/// services.AddPayApp(configuration, "WechatPayMerchants")
///         .AddWechatPayApi(b => b.AddModules(PayModule.Transactions))
///         .AddWechatPayV2Api(b => b.AddAllApis());
/// </code>
/// 两条线可只装其一：只接 APIv2 的老商户不必装 APIv3 域，反之亦然。
/// </para>
/// </remarks>
public static class PayV2ServiceCollectionExtensions
{
    /// <summary>按枚举注册微信支付 APIv2 业务接口。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">要注册的模块（如 <see cref="PayV2Module.BasePayV2"/>）。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddPayApp</c>。</exception>
    public static IServiceCollection AddWechatPayV2Api(
        this IServiceCollection services,
        params PayV2Module[] modules)
    {
        if (modules == null)
        {
            throw new ArgumentNullException(nameof(modules));
        }

        var builder = new PayV2ServiceBuilder(services);
        builder.AddModules(modules);

        return builder.Build();
    }

    /// <summary>用建造者委托注册微信支付 APIv2 业务接口。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">建造者配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="configure"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddPayApp</c>。</exception>
    public static IServiceCollection AddWechatPayV2Api(
        this IServiceCollection services,
        Action<PayV2ServiceBuilder> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new PayV2ServiceBuilder(services);
        configure(builder);

        return builder.Build();
    }
}
