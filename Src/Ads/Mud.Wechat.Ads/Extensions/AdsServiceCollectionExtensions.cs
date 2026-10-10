// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Extensions;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告业务接口注册入口（对齐 <c>AddWechatPayApi</c> / 企微 <c>AddWechatWorkServices</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>AddAdsApp</c> 的分工</b>：凭据底座（配置 → 多应用管理器 → OAuth 授权与令牌注入传输层）由
/// <c>Mud.Wechat.Ads.Abstractions</c> 的 <c>AddAdsApp</c> 装配，本入口只装配<b>业务接口</b>
/// （<see cref="AdsModule"/> 域注册）。两者必须<b>按此顺序</b>调用：本入口在 <c>Build()</c> 里校验
/// <c>IAdsAppManager</c> 已在服务集合中，缺失即抛出并点名修复方式，而不是等到首次调用才失败。
/// </para>
/// <para>
/// <b>入口名为何带 <c>Wechat</c> 前缀</b>：与 <c>AddWechatPayApi</c> 同一理由 —— 保持五条产品线
/// 「<c>AddWechat{线}Api</c> + <c>Add{线}App</c>」的可读并列，避免 <c>AddAdsApi</c> 与
/// 企微线 <c>WechatWorkServiceBuilder.AddPayApi()</c> 一类短名混列。
/// </para>
/// </remarks>
public static class AdsServiceCollectionExtensions
{
    /// <summary>按枚举注册广告业务接口。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">要注册的模块（如 <see cref="AdsModule.Advertiser"/>）。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddAdsApp</c>。</exception>
    public static IServiceCollection AddWechatAdsApi(
        this IServiceCollection services,
        params AdsModule[] modules)
    {
        if (modules == null)
        {
            throw new ArgumentNullException(nameof(modules));
        }

        var builder = new AdsServiceBuilder(services);
        builder.AddModules(modules);

        return builder.Build();
    }

    /// <summary>用建造者委托注册广告业务接口（可一次注册多个模块）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">建造者配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="configure"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddAdsApp</c>。</exception>
    /// <remarks>
    /// 典型用法：
    /// <code>
    /// services.AddAdsApp(configuration, "WechatAds")
    ///         .AddWechatAdsApi(b => b.AddAllApis());
    /// </code>
    /// </remarks>
    public static IServiceCollection AddWechatAdsApi(
        this IServiceCollection services,
        Action<AdsServiceBuilder> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new AdsServiceBuilder(services);
        configure(builder);

        return builder.Build();
    }
}
