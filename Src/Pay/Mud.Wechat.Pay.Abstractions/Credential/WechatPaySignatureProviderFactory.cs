// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <see cref="IWechatPaySignatureProviderFactory"/> 默认实现：按 <c>MerchantKey</c> 缓存签名提供者。
/// </summary>
/// <remarks>
/// Singleton；RSA 私钥随缓存长期持有（商户私钥在进程生命周期内不变），
/// 因此<b>不</b>实现 <see cref="IDisposable"/> —— 由容器释放反而会在停机期提前拔掉仍在签名的密钥。
/// </remarks>
public sealed class WechatPaySignatureProviderFactory : IWechatPaySignatureProviderFactory
{
    private readonly IWechatPayMerchantCredentialProvider _credentials;
    private readonly IWechatPayPlatformCertificateStore _platformCertificates;
    private readonly ConcurrentDictionary<string, Task<IWechatPaySignatureProvider>> _byMerchantKey =
        new(StringComparer.Ordinal);

    /// <summary>
    /// 创建工厂。
    /// </summary>
    /// <param name="credentials">商户凭据取用器（经 <c>ISecretProvider</c> 取私钥）。</param>
    /// <param name="platformCertificates">平台证书存取端口（验签用；请求签名本身不用）。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    public WechatPaySignatureProviderFactory(
        IWechatPayMerchantCredentialProvider credentials,
        IWechatPayPlatformCertificateStore platformCertificates)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _platformCertificates = platformCertificates ?? throw new ArgumentNullException(nameof(platformCertificates));
    }

    /// <inheritdoc />
    public async Task<IWechatPaySignatureProvider> GetOrCreateAsync(
        WechatPayMerchantConfig merchant, CancellationToken cancellationToken = default)
    {
        if (merchant is null)
        {
            throw new ArgumentNullException(nameof(merchant));
        }

        var merchantKey = merchant.MerchantKey;

        if (!_byMerchantKey.TryGetValue(merchantKey, out var loadTask))
        {
            // 共享任务用 CancellationToken.None 承载（对齐企微授权编排的单飞形态）：
            // 若承载在首个调用者的 ct 上，一旦他取消，缓存里会留下**已取消的 Task**，
            // 此后所有调用者拿到的都是它 —— 一个旁观者的取消就把该商户打死了。
            loadTask = _byMerchantKey.GetOrAdd(merchantKey, _ => LoadAsync(merchant));
        }

        try
        {
            // 各调用者经 WaitAsync 独立取消：取消只断开本次等待，不牵连共享加载。
            return await loadTask.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // 仅当**共享任务本身**失败时才剔除缓存：密钥仓库的一次抖动若被缓存，
            // 该商户会永久不可用。调用者自己取消（任务仍健康）不得剔除，否则会误删有效缓存。
            if (loadTask.IsFaulted || loadTask.IsCanceled)
            {
                _byMerchantKey.TryRemove(
                    new KeyValuePair<string, Task<IWechatPaySignatureProvider>>(merchantKey, loadTask));
            }

            throw;
        }
    }

    private async Task<IWechatPaySignatureProvider> LoadAsync(WechatPayMerchantConfig merchant)
    {
        // 不传调用者的 ct：见上「共享任务用 None 承载」。
        var privateKey = await _credentials
            .LoadPrivateKeyAsync(merchant)
            .ConfigureAwait(false);

        return new WechatPaySignatureProvider(merchant.SerialNumber, privateKey, _platformCertificates);
    }
}
