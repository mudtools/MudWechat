// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 按商户产出 <see cref="IWechatPaySignatureProvider"/> 的工厂（<b>带缓存</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要工厂且必须缓存</b>：<see cref="WechatPaySignatureProvider"/> 绑定<b>单一商户</b>的
/// RSA 私钥与证书序列号（多商户宿主下每笔请求可能用不同商户），而
/// <see cref="IWechatPayMerchantCredentialProvider.LoadPrivateKeyAsync"/> <b>无缓存</b> ——
/// 每次调用都要打一次 <c>ISecretProvider</c> 并重新解析 PEM。若传输层逐请求调用，
/// 等于给每个支付请求加一次密钥仓库往返，因此由本工厂按 <c>MerchantKey</c> 缓存。
/// </para>
/// <para>
/// <b>私钥不进日志</b>（PAY-B7）：本工厂只持有 RSA 实例，不提供任何导出/回显入口。
/// </para>
/// </remarks>
public interface IWechatPaySignatureProviderFactory
{
    /// <summary>
    /// 取得（必要时首次加载）指定商户的签名提供者。
    /// </summary>
    /// <param name="merchant">商户配置（提供 <c>PrivateKeySecretName</c> 与证书序列号）。</param>
    /// <param name="cancellationToken">取消控制（<b>仅</b> 取消本次调用者的等待，不影响共享加载任务）。</param>
    /// <returns>该商户的签名提供者。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="merchant"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">商户未配置证书序列号，或密钥取用失败。</exception>
    Task<IWechatPaySignatureProvider> GetOrCreateAsync(
        WechatPayMerchantConfig merchant, CancellationToken cancellationToken = default);
}
