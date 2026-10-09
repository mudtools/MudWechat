// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <see cref="IWechatPayMerchantContext"/> 的默认实现：基于 <see cref="AsyncLocal{T}"/> 的环境商户选择。
/// </summary>
/// <remarks>
/// Singleton 注册（环境值本身按异步流隔离，跨线程/跨 scope 各自独立，不会互相污染）。
/// </remarks>
public sealed class WechatPayMerchantContext : IWechatPayMerchantContext
{
    private readonly IWechatPayMerchantManager _manager;

    /// <summary>
    /// 当前商户键；<c>null</c> 表示未开作用域（回落默认商户规则）。
    /// </summary>
    private readonly AsyncLocal<string?> _currentMerchantKey = new();

    /// <summary>
    /// 创建环境商户上下文。
    /// </summary>
    /// <param name="manager">多商户管理器（经 DI 解析，宿主预注册者按契约胜出）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="manager"/> 为 <c>null</c>。</exception>
    public WechatPayMerchantContext(IWechatPayMerchantManager manager)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    /// <inheritdoc />
    public WechatPayMerchantConfig ResolveCurrent()
    {
        var scopedKey = _currentMerchantKey.Value;
        if (!string.IsNullOrEmpty(scopedKey))
        {
            // GetMerchant 对未注册键 fail-fast（带已注册键清单），比这里重复一次校验更清楚。
            return _manager.GetMerchant(scopedKey);
        }

        var merchants = _manager.Merchants;
        if (merchants.Count == 1)
        {
            // 单商户 = 默认商户：最常见的宿主形态零样板。
            return merchants[0];
        }

        if (merchants.Count == 0)
        {
            throw new InvalidOperationException(
                "未注册任何微信支付商户。请先调用 AddPayApp(configuration, \"WechatPayMerchants\") " +
                "或 AddPayApp(config => ...) 注册至少一个商户。");
        }

        // 多商户且未指定 —— 静默挑第一个会让多商户宿主拿错私钥签名（签名有效但落到错误商户），
        // 属于最难排查的一类故障，因此 fail-fast 并点名处置方式。
        throw new InvalidOperationException(
            $"已注册 {merchants.Count} 个微信支付商户，无法判定当前应当使用哪一个。" +
            "请用 using (merchantContext.UseMerchant(merchantKey)) { ... } 包裹本次调用，" +
            $"或只注册一个商户。已注册商户：{string.Join(", ", merchants.Select(static m => m.MerchantKey))}。");
    }

    /// <inheritdoc />
    public IDisposable UseMerchant(string merchantKey)
    {
        if (string.IsNullOrWhiteSpace(merchantKey))
        {
            throw new ArgumentException("商户键不能为空白。", nameof(merchantKey));
        }

        // 进入作用域前先解析：未注册的商户键立即失败，不留到真正签名时才暴露。
        _ = _manager.GetMerchant(merchantKey);

        var previous = _currentMerchantKey.Value;
        _currentMerchantKey.Value = merchantKey;

        return new MerchantScope(this, previous);
    }

    private void Restore(string? previousMerchantKey) => _currentMerchantKey.Value = previousMerchantKey;

    /// <summary>
    /// 一次性商户作用域：释放时<b>还原上一层</b>（嵌套安全）且幂等。
    /// </summary>
    private sealed class MerchantScope : IDisposable
    {
        private readonly WechatPayMerchantContext _context;
        private readonly string? _previousMerchantKey;
        private int _disposed;

        public MerchantScope(WechatPayMerchantContext context, string? previousMerchantKey)
        {
            _context = context;
            _previousMerchantKey = previousMerchantKey;
        }

        public void Dispose()
        {
            // 幂等：重复 Dispose 不得把外层已还原的值再覆盖一次。
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _context.Restore(_previousMerchantKey);
        }
    }
}
