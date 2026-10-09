// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.HttpUtils;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Transport;

namespace Mud.Wechat.OpenPlatform.Extensions;

/// <summary>
/// 开放平台线（第三方平台）服务集合扩展入口。
/// </summary>
/// <remarks>
/// <para>
/// <b>本线<b>刻意不</b>依赖公众号 / 小程序线的任何装配</b>：第三方平台是独立产品线
/// （设计方案红线：独立令牌管理器、避免令牌串号）⇒ 宿主既可只装本线，
/// 也可与其它线并存而互不影响。
/// </para>
/// <para>
/// <b>SSRF 白名单零改动</b>：本线基址是 <c>api.weixin.qq.com</c>（后缀命中既有白名单
/// <c>weixin.qq.com</c>），因此<b>不需要</b>触碰进程级白名单
/// —— 与「发票文件下载需 <c>pay.wechatpay.cn</c> 因而须宿主显式决策」的情形形成对照。
/// </para>
/// </remarks>
public static class OpenPlatformServiceCollectionExtensions
{
    /// <summary>
    /// 注册第三方平台（component）凭证链接所需服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">平台配置（<c>component_appid</c> / <c>component_appsecret</c>）。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">配置缺少 appid / appsecret（<b>注册期</b> fail-fast）。</exception>
    /// <remarks>
    /// <para>
    /// <b>全部走 <c>TryAdd</c></b>：宿主预注册的实现优先（如把票据存储换成 Redis 分布式实现 ——
    /// 多实例部署<b>必须</b>这么做，否则推送只到达一台实例时其余实例取不到令牌）。
    /// </para>
    /// <para>
    /// <b>令牌提供者注册为单例</b>：它持有令牌缓存，多实例会让刷新频率乘以实例数。
    /// </para>
    /// </remarks>
    public static IServiceCollection AddOpenPlatform(
        this IServiceCollection services,
        Action<OpenPlatformAppConfig> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var config = new OpenPlatformAppConfig();
        configure(config);
        config.EnsureValid();

        // 命名客户端：组件 AddMudHttpClient 负责追踪 Handler 与连接期 SSRF 严格模式。
        // **不挂签名 Handler**：开放平台的 component 凭证是请求参数，不存在 APIv3 报文签名。
        // 重复调用 AddOpenPlatform 不会重复注册（幂等守卫与支付线同款）。
        if (services.All(static d => d.ServiceType != typeof(IWechatOpenPlatformHttpClient)))
        {
            services.AddMudHttpClient(
                OpenPlatformHttpClientNames.ClientName,
                static client => client.BaseAddress = new Uri(OpenPlatformHttpClientNames.BaseAddress));
        }

        services.TryAddSingleton<IWechatOpenPlatformHttpClient, WechatOpenPlatformHttpClient>();
        services.TryAddSingleton(config);
        services.TryAddSingleton<IOpenPlatformClock, SystemOpenPlatformClock>();
        services.TryAddSingleton<IComponentVerifyTicketStore, InMemoryComponentVerifyTicketStore>();
        services.TryAddSingleton<IComponentTokenProvider, ComponentTokenProvider>();

        // 票据推送接收器：凭证链的入口。宿主把「授权事件接收 URL」的 POST 转交它即可
        // （官方要求回 success，判定见 ComponentVerifyTicketReceiver.ShouldReturnSuccess）。
        services.TryAddSingleton<ComponentVerifyTicketReceiver>();

        // 授权流程（预授权码 / 换取授权信息 / 刷新授权方令牌）。
        services.TryAddSingleton<IComponentAuthorizationService, ComponentAuthorizationService>();

        return services;
    }
}
