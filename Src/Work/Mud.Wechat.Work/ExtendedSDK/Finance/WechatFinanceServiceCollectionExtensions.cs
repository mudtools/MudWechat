// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 会话内容存档 C SDK 的 DI 注册入口（<c>AddWechatFinanceSdk</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>WechatModule</c> / <c>Add{域}Api()</c> 零交叉</b>：本域是进程内原生封装，没有 HTTP 端点，
/// 因此不注册任何 <c>[HttpClientApi]</c> 客户端、不进模块枚举、不参与令牌链路（守卫 FIN-B6）。
/// 注册产物只有两项：配置面 <see cref="WechatFinanceOptions"/> 与工厂
/// <see cref="IWechatWorkFinanceClientFactory"/>。
/// </para>
/// <para>
/// <b>不注册 <c>IWechatWorkFinanceClient</c> 本身</b>：客户端按机器人键定位，而「用哪个机器人」是调用期
/// 事实而非装配期事实。把它做成 Singleton 会强迫配置里只有一个机器人。
/// </para>
/// <para>
/// <b>前置依赖 <c>ISecretProvider</c></b>：存档 <c>secret</c>、代理口令、RSA 私钥一律经该端口取用
/// （组件 <c>Mud.HttpUtils</c> 的密钥治理面）。SDK <b>不</b>抢占注册默认实现 —— 与支付侧同一原则，
/// 密钥治理权在宿主。未注册时在本入口<b>首次解析工厂</b>时给出点名错误，而非留下一句难懂的 DI 缺失异常。
/// </para>
/// <para>
/// <b>出网不由本 SDK 管控</b>：本域不走组件 HTTP 管线 ⇒ 不登记 SSRF 白名单、不受
/// <c>WechatApiHosts</c> 与 <c>AllowCustomBaseUrl</c> 约束，代理与直连都由原生库自行发起。
/// 这是与其余各线实质不同的安全边界：出网管控须落在宿主进程级网络策略上（配置里的
/// <c>ProxyAddress</c> 只影响原生库，不构成防线）。
/// </para>
/// </remarks>
public static class WechatFinanceServiceCollectionExtensions
{
    /// <summary>
    /// 从配置节注册会话存档 SDK（惰性 <c>Configure(o =&gt; section.Bind(o))</c>，走源生成绑定器）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名，默认 <see cref="WechatFinanceOptions.DefaultSectionName"/>。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configuration"/> 为 <c>null</c> 时抛出。</exception>
    public static IServiceCollection AddWechatFinanceSdk(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = WechatFinanceOptions.DefaultSectionName)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var section = configuration.GetSection(sectionName);
        return AddWechatFinanceSdkCore(services, options => section.Bind(options));
    }

    /// <summary>
    /// 用代码注册会话存档 SDK（多个机器人在 <see cref="WechatFinanceOptions.Robots"/> 里逐键给）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> 为 <c>null</c> 时抛出。</exception>
    public static IServiceCollection AddWechatFinanceSdk(
        this IServiceCollection services,
        Action<WechatFinanceOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        return AddWechatFinanceSdkCore(services, configure);
    }

    /// <summary>注册核心：配置面（含启动期校验）+ 工厂（按机器人键单飞装配）。</summary>
    private static IServiceCollection AddWechatFinanceSdkCore(
        IServiceCollection services, Action<WechatFinanceOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // 配置绑定走源生成路径（AGENTS 红线：不得用 Configure<T>(IConfiguration) 反射重载）；
        // 校验挂在 Options 管道上，首次取值即失败 —— 工厂构造期还会再显式校验一次。
        services.AddOptions<WechatFinanceOptions>()
            .Configure(configure)
            .Validate(
                static options =>
                {
                    options.Validate();
                    return true;
                },
                "会话存档配置校验失败（详见 WechatFinanceOptions.Validate）。");

        services.TryAddSingleton<IWechatWorkFinanceClientFactory>(static sp =>
        {
            var secrets = sp.GetService<ISecretProvider>();
            if (secrets == null)
            {
                // 点名错误：默认 DI 缺失异常只报接口名，宿主很难一眼看出「是谁要求注册的」。
                throw new InvalidOperationException(
                    "未注册 ISecretProvider（组件 Mud.HttpUtils 的密钥端口）。" +
                    "会话存档的 secret、代理口令与 RSA 私钥一律经该端口取用，" +
                    "请先由宿主注册自身实现：services.AddSingleton<ISecretProvider>(...)，" +
                    "再调用 AddWechatFinanceSdk(...)。");
            }

            return new WechatFinanceClientFactory(
                sp.GetRequiredService<IOptions<WechatFinanceOptions>>(), secrets);
        });

        return services;
    }
}
