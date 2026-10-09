// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 平台证书**进程内**缓存（<see cref="IWechatPayPlatformCertificateStore"/> 的默认实现）。
/// </summary>
/// <remarks>
/// <para>
/// 多实例部署须换分布式实现（走宿主前置 <c>TryAdd</c> 覆盖，与四个存储端口同款纪律）——
/// 否则每个实例各自拉证书，序列号视图不一致会在轮换瞬间产生**间歇性**验签失败，极难排障。
/// </para>
/// <para><b>本类持有证书所有权</b>：<see cref="Set"/> 后由缓存负责，<see cref="Dispose"/> 时统一释放。</para>
/// </remarks>
public sealed class WechatPayPlatformCertificateCache : IWechatPayPlatformCertificateStore, IDisposable
{
    private readonly Dictionary<string, X509Certificate2> _bySerial =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _sync = new();
    private bool _disposed;

    /// <summary>当前缓存的证书数（诊断用）。</summary>
    public int Count
    {
        get
        {
            lock (_sync)
            {
                return _bySerial.Count;
            }
        }
    }

    /// <summary>登记（或覆盖）一本平台证书。</summary>
    /// <param name="serialNumber">平台证书序列号。</param>
    /// <param name="certificate">证书实例；**所有权随之转移给本缓存**。</param>
    /// <exception cref="ArgumentException"><paramref name="serialNumber"/> 为 <c>null</c>/空白。</exception>
    /// <exception cref="ObjectDisposedException">实例已释放。</exception>
    /// <remarks>同序列号重复登记时，旧实例会被释放（覆盖语义）。</remarks>
    public void Set(string serialNumber, X509Certificate2 certificate)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            throw new ArgumentException("平台证书序列号不可为空白。", nameof(serialNumber));
        }

        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        X509Certificate2? previous = null;
        lock (_sync)
        {
            // 注：不用 ObjectDisposedException.ThrowIf —— 那是 .NET 8+ API，本包还覆盖 net6.0。
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WechatPayPlatformCertificateCache));
            }

            previous = _bySerial.TryGetValue(serialNumber, out var existing) ? existing : null;
            _bySerial[serialNumber] = certificate;
        }

        previous?.Dispose();
    }

    /// <inheritdoc />
    /// <remarks>未命中（含序列号为 <c>null</c>/空白）一律返回 <c>false</c>，保持验签 fail-closed。</remarks>
    public bool TryGetCertificate(string? serialNumber, out X509Certificate2? certificate)
    {
        certificate = null;
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return false;
        }

        lock (_sync)
        {
            if (_disposed)
            {
                return false;
            }

            if (_bySerial.TryGetValue(serialNumber, out var found))
            {
                certificate = found;
                return true;
            }
        }

        return false;
    }

    /// <summary>移除并释放指定序列号的证书。</summary>
    /// <param name="serialNumber">平台证书序列号。</param>
    /// <returns>是否命中。</returns>
    public bool Remove(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return false;
        }

        X509Certificate2? removed = null;
        lock (_sync)
        {
            if (_bySerial.TryGetValue(serialNumber, out var found))
            {
                _bySerial.Remove(serialNumber);
                removed = found;
            }
        }

        if (removed is null)
        {
            return false;
        }

        removed.Dispose();
        return true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        List<X509Certificate2> all;
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            all = _bySerial.Values.ToList();
            _bySerial.Clear();
        }

        foreach (var cert in all)
        {
            cert.Dispose();
        }
    }
}
