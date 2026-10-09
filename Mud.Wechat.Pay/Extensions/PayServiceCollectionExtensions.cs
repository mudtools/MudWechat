// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Extensions;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付业务接口注册入口（对齐企微 <c>AddWechatWorkServices</c> / 公众号同名入口）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>AddPayApp</c> 的分工</b>：凭据底座（配置 → 多商户管理器 → 签名传输层）由
/// <c>Mud.Wechat.Pay.Abstractions</c> 的 <c>AddPayApp</c> 装配，本入口只装配<b>业务接口</b>
/// （<see cref="PayModule"/> 域注册）。两者可各自单独调用：
/// 回调包只需前者即可验签解密，不必拉起全部业务接口客户端。
/// </para>
/// <para>
/// <b>入口名为何是 <c>AddWechatPayApi</c> 而非 <c>AddPayApi</c></b>：
/// 本仓已有「企业支付」产品线占用 <c>WechatWorkServiceBuilder.AddPayApi()</c>（README 已记为企业支付），
/// 若此处再叫 <c>AddPayApi()</c>，公开面会出现两个同名不同义的方法。
/// 统一为 <c>AddWechatPay…</c> 前缀后与 <c>AddPayApp</c> 并列可读、且跨产品线永不错认。
/// </para>
/// <para>
/// <b>调用顺序</b>：<c>AddWechatPayApi</c> 内部会校验 <c>IWechatPayMerchantManager</c> 已注册，
/// 故 <c>AddPayApp</c> 必须先调用（否则立即抛出并点名修复方式，而非等到首次交易）。
/// </para>
/// </remarks>
public static class PayServiceCollectionExtensions
{
    /// <summary>按枚举注册微信支付业务接口。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">要注册的模块（如 <see cref="PayModule.Transactions"/>）。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddPayApp</c>。</exception>
    public static IServiceCollection AddWechatPayApi(
        this IServiceCollection services,
        params PayModule[] modules)
    {
        if (modules == null)
        {
            throw new ArgumentNullException(nameof(modules));
        }

        var builder = new PayServiceBuilder(services);
        builder.AddModules(modules);

        return builder.Build();
    }

    /// <summary>用建造者委托注册微信支付业务接口（可一次注册多个模块）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">建造者配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="configure"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先调用 <c>AddPayApp</c>。</exception>
    /// <remarks>
    /// 典型用法：
    /// <code>
    /// services.AddPayApp(configuration, "WechatPayMerchants")
    ///         .AddWechatPayApi(b => b.AddAllApis());
    /// </code>
    /// </remarks>
    public static IServiceCollection AddWechatPayApi(
        this IServiceCollection services,
        Action<PayServiceBuilder> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = new PayServiceBuilder(services);
        configure(builder);

        return builder.Build();
    }
}
