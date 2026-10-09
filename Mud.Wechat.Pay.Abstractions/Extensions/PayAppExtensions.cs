// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.HttpUtils;
using Mud.Wechat.Abstractions.TokenManager;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Abstractions.Configuration;
using Mud.Wechat.Pay.Abstractions.Transport;

namespace Mud.Wechat.Pay.Abstractions.Extensions;

/// <summary>
/// 微信支付商户底座注册入口（对齐企微 <c>AddWechatApp</c> / 公众号 <c>AddMpApp</c> 的命名与三重载形态）。
/// </summary>
/// <remarks>
/// <para>
/// 典型用法：
/// <code>
/// services.AddSingleton&lt;ISecretProvider&gt;(new MyVaultSecretProvider());
/// services.AddPayApp(configuration, "WechatPayMerchants");
/// </code>
/// </para>
/// <para>
/// <b>与 <c>AddPayApi()</c> 的分工</b>（P1-a 落地）：<c>AddPayApp</c> 只装配<b>商户凭据底座</b>
/// （配置 → 多商户管理器 → 凭据取用器）；<c>AddPayApi()</c> 负责<b>业务接口</b>（<c>PayModule</c> 域注册）。
/// 两者可各自单独调用 —— 回调包只需前者即可验签解密，不必拉起全部业务接口客户端。
/// </para>
/// <para>
/// <b>不注册默认 <c>ISecretProvider</c></b>：该端口属组件 <c>Mud.HttpUtils</c>，密钥治理权在宿主；
/// SDK 抢占注册会削弱宿主的密钥策略（与「SDK 不抢占组件 <c>IExceptionRedactor</c>」同一原则）。
/// 宿主未注册时，本入口在<b>首次解析凭据取用器</b>时给出点名错误，而非留下难懂的 DI 缺失异常。
/// </para>
/// </remarks>
public static class PayAppExtensions
{
    /// <summary>默认配置节名。</summary>
    public const string DefaultSectionName = "WechatPayMerchants";

    /// <summary>
    /// 从配置节注册微信支付商户底座（绑定 <c>List&lt;WechatPayMerchantConfig&gt;</c>）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 <c>WechatPayMerchants</c>。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <remarks>绑定走<b>源生成器</b>（<c>EnableConfigurationBindingGenerator</c>），非反射绑定。</remarks>
    public static IServiceCollection AddPayApp(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSectionName)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(sectionName);
        var configs = new List<WechatPayMerchantConfig>();

        // 源生成配置绑定器：显式 new + Bind，避免 ConfigurationBinder 的反射路径。
        section.Bind(configs);

        return services.AddPayInfrastructure(configs);
    }

    /// <summary>
    /// 用代码注册单个商户（多个请用 <see cref="AddPayApp(IServiceCollection, List{WechatPayMerchantConfig})"/>）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">商户配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddPayApp(
        this IServiceCollection services,
        Action<WechatPayMerchantConfig> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var config = new WechatPayMerchantConfig();
        configure(config);

        return services.AddPayInfrastructure(new List<WechatPayMerchantConfig> { config });
    }

    /// <summary>
    /// 一次性注册多个商户。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configs">商户配置集合。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddPayApp(
        this IServiceCollection services,
        List<WechatPayMerchantConfig> configs)
    {
        if (configs == null) throw new ArgumentNullException(nameof(configs));

        return services.AddPayInfrastructure(configs);
    }

    /// <summary>
    /// 装配商户凭据底座：多商户管理器 + 环境商户上下文 + 凭据取用器 + 签名工厂 + 传输层签名（均为 Singleton）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么传输层签名放这里</b>：签名要的商户私钥与序列号本就出自本底座，拆开注册只会让
    /// 「只装了凭据、忘了签名」这类半装配状态成为可能（到了真实下单才暴露）。命名客户端是<b>惰性</b>的 ——
    /// 只调用 <c>AddPayApp</c> 而不发支付请求（如仅用回调包验签）不会真的建连。
    /// </para>
    /// <para>
    /// <b>TryAdd 语义</b>：宿主可预注册自己的 <see cref="IWechatPayMerchantManager"/>（如需要从配置中心
    /// 动态增删商户），预注册者按契约胜出，本方法不会覆盖。
    /// </para>
    /// </remarks>
    internal static IServiceCollection AddPayInfrastructure(
        this IServiceCollection services, List<WechatPayMerchantConfig> configs)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // SSRF 白名单（进程级全局态）必须在这里登记：支付线有意不走 [Token]，
        // 因而不会触发 AddWechatTokenRecovery，纯支付宿主若不登记会让**每笔请求**被
        // 组件的连接期 SSRF 严格模式拦下（实测缺陷，见 WechatPayTransportPipelineTests）。
        // 经公用层窄入口登记 ⇒ 不违反 AB-G4「产品线不得自行登记白名单」的单点约束，
        // 且白名单数组零改动（PAY-B8 / AB-G9）。
        services.AddWechatApiHosts();

        // 立刻构造一次：把「配置非法 / 重复商户键 / 空集合」钉在注册期而非首次交易期。
        // 支付无 errcode 令牌自愈，坏配置拖到真实下单才暴露的排查成本极高。
        var manager = new WechatPayMerchantManager(configs);

        services.TryAddSingleton<IWechatPayMerchantManager>(manager);

        // 环境商户上下文：经 DI 解析管理器（而非捕获上面的局部 manager），
        // 否则宿主预注册了自己的 IWechatPayMerchantManager 时，上下文会读到另一份商户表。
        services.TryAddSingleton<IWechatPayMerchantContext>(
            static sp => new WechatPayMerchantContext(
                sp.GetRequiredService<IWechatPayMerchantManager>()));

        services.TryAddSingleton<IWechatPayMerchantCredentialProvider>(
            static sp =>
            {
                var secrets = sp.GetService<ISecretProvider>();
                if (secrets == null)
                {
                    // 点名错误：默认 DI 缺失异常只报接口名，宿主很难一眼看出「是谁要求注册的」。
                    throw new InvalidOperationException(
                        "未注册 ISecretProvider（组件 Mud.HttpUtils 的密钥端口）。" +
                        "微信支付的商户私钥与 APIv3 密钥一律经该端口取用，" +
                        "请先由宿主注册自身实现：services.AddSingleton<ISecretProvider>(...)，" +
                        "再调用 AddPayApp(...)。");
                }

                return new WechatPayMerchantCredentialProvider(secrets);
            });

        // 平台证书缓存：请求签名只用商户私钥，验签（回调 / 应答验签）才需要平台证书；
        // 这里给出默认进程内实现，宿主可预注册自己的（如需跨实例共享）。
        services.TryAddSingleton<IWechatPayPlatformCertificateStore>(
            static _ => new WechatPayPlatformCertificateCache());

        services.TryAddSingleton<IWechatPaySignatureProviderFactory>(
            static sp => new WechatPaySignatureProviderFactory(
                sp.GetRequiredService<IWechatPayMerchantCredentialProvider>(),
                sp.GetRequiredService<IWechatPayPlatformCertificateStore>()));

        // 传输层签名：命名客户端（组件 AddMudHttpClient 负责追踪 Handler 与连接期 SSRF 严格模式）
        // + 支付签名 Handler。重复 AddPayApp 不会重复挂 Handler —— Handler 对已带 Authorization 的请求幂等，
        // 且 TryAddSingleton 保证 IWechatPayHttpClient 单实例。
        if (services.All(static d => d.ServiceType != typeof(IWechatPayHttpClient)))
        {
            // AddHttpMessageHandler<T> 是**从 DI 解析 T**（不是 new），漏注册会在**首次请求**才抛
            // 「No service for type ... has been registered」—— 已由 WechatPayTransportPipelineTests 锁定。
            services.TryAddSingleton<WechatPayAuthorizationHandler>();

            services.AddMudHttpClient(
                WechatPayHttpClientNames.ClientName,
                static client => client.BaseAddress = new Uri(WechatPayHttpClientNames.BaseAddress))
                .AddHttpMessageHandler<WechatPayAuthorizationHandler>();
        }

        services.TryAddSingleton<IWechatPayHttpClient, WechatPayHttpClient>();

        return services;
    }
}
