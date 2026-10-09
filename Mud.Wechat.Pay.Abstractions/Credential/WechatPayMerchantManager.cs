// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <see cref="IWechatPayMerchantManager"/> 的进程内实现：注册期校验、去重，运行期只读查询。
/// </summary>
/// <remarks>
/// <b>构造期 fail-fast</b>：全部商户在构造时完成 <see cref="WechatPayMerchantConfig.Validate"/> 与
/// 重复键检测。支付<b>没有 errcode 令牌自愈</b>（无 <c>access_token</c>），坏配置若拖到首次真实交易才暴露，
/// 症状是「下单 5xx + 官方侧无对应单据」，排查成本极高；启动期即抛能把问题钉在部署环节。
/// </remarks>
public sealed class WechatPayMerchantManager : IWechatPayMerchantManager
{
    private readonly Dictionary<string, WechatPayMerchantConfig> _merchants;
    private readonly IReadOnlyList<WechatPayMerchantConfig> _ordered;

    /// <summary>创建多商户基座。</summary>
    /// <param name="merchants">商户配置集合（顺序保留，便于诊断输出与默认商户推断）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="merchants"/> 为 <c>null</c> 时抛出。</exception>
    /// <exception cref="InvalidOperationException">存在非法配置或重复商户键时抛出。</exception>
    public WechatPayMerchantManager(IEnumerable<WechatPayMerchantConfig> merchants)
    {
        if (merchants == null) throw new ArgumentNullException(nameof(merchants));

        var ordered = new List<WechatPayMerchantConfig>();
        var map = new Dictionary<string, WechatPayMerchantConfig>(StringComparer.Ordinal);

        foreach (var config in merchants)
        {
            if (config == null)
            {
                throw new InvalidOperationException("商户配置集合中存在 null 项。");
            }

            // 先校验再取键：MerchantKey 由字段推导，字段非法时键本身也没意义。
            config.Validate();

            var key = config.MerchantKey;

            if (map.TryGetValue(key, out var existing))
            {
                // 重复键 = 两份凭据争同一槽位，后注册者静默覆盖前者 ⇒ 收款落到错误商户。
                // 这是资金侧错误，必须在启动期即失败。
                throw new InvalidOperationException(
                    $"商户键 {key} 重复注册" +
                    $"（SerialNumber {existing.SerialNumber} 与 {config.SerialNumber}）；" +
                    "同一商户只能注册一份配置。");
            }

            map.Add(key, config);
            ordered.Add(config);
        }

        if (ordered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要注册一个微信支付商户。请调用 AddPayApp(configuration, \"WechatPayMerchants\") " +
                "或 AddPayApp(config) / AddPayApp(List<WechatPayMerchantConfig>)。");
        }

        _merchants = map;
        _ordered = ordered;
    }

    /// <inheritdoc />
    public IReadOnlyList<WechatPayMerchantConfig> Merchants => _ordered;

    /// <inheritdoc />
    public bool TryGetMerchant(string merchantKey, out WechatPayMerchantConfig? config)
    {
        if (string.IsNullOrWhiteSpace(merchantKey))
        {
            config = null;
            return false;
        }

        return _merchants.TryGetValue(merchantKey, out config);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 本方法<b>刻意不走</b> <see cref="TryGetMerchant"/> 的字典键：它按官方字段本身匹配，
    /// 因此对 <c>MerchantKey</c> 的拼接格式零依赖（格式日后若变，回调侧不受影响）。
    /// 商户数量级为个位到几十，线性扫描的代价可忽略，换来的是「少一处拼接规则」。
    /// </remarks>
    public bool TryGetByOfficialIds(
        string? mchId, string? spMchId, string? subMchId,
        out WechatPayMerchantConfig? config)
    {
        var hasSp = !string.IsNullOrWhiteSpace(spMchId);
        var hasSub = !string.IsNullOrWhiteSpace(subMchId);

        // 服务商两个字段只出现其一 ⇒ 畸形报文。**不猜**：直接不命中，由调用方告警。
        // 在此「猜一个商户」等于把通知算到错误的收款主体上 —— 资金侧错误。
        if (hasSp != hasSub)
        {
            config = null;
            return false;
        }

        foreach (var merchant in _ordered)
        {
            if (hasSp)
            {
                // 服务商：子商户号跨服务商可重复，必须 sp_mchid + sub_mchid 复合才唯一。
                if (string.Equals(merchant.SpMchId, spMchId, StringComparison.Ordinal) &&
                    string.Equals(merchant.SubMchId, subMchId, StringComparison.Ordinal))
                {
                    config = merchant;
                    return true;
                }
            }
            else if (string.Equals(merchant.MchId, mchId, StringComparison.Ordinal))
            {
                config = merchant;
                return true;
            }
        }

        config = null;
        return false;
    }

    /// <inheritdoc />
    public WechatPayMerchantConfig GetMerchant(string merchantKey)
    {
        if (string.IsNullOrWhiteSpace(merchantKey))
        {
            throw new InvalidOperationException("商户键不能为空。");
        }

        if (_merchants.TryGetValue(merchantKey, out var config))
        {
            return config;
        }

        throw new InvalidOperationException(
            $"未注册的商户键 {merchantKey}。已注册：{string.Join(", ", _merchants.Keys)}。");
    }
}
