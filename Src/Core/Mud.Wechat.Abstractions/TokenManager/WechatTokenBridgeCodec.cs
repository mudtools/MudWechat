// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.TokenManager;

/// <summary>
/// 令牌持久化桥接编解码器（对齐 <c>FeishuTokenBridgeCodec</c>）：
/// <c>CredentialToken</c> 与存储字符串 <c>{expireMs}|{token}</c> 的互转。
/// </summary>
/// <remarks>
/// <b>为何属于公用层</b>：编码格式是「持久层契约」——同一份存储实例（含 M1 的 Redis 实现）
/// 可能同时承载多条产品线的令牌条目，解码规则必须单一，否则跨产品线读穿透会静默失败。
/// </remarks>
internal static class WechatTokenBridgeCodec
{
    private const char Separator = '|';

    /// <summary>写穿方向：CredentialToken → 存储字符串（{expireMs}|{token} 格式）。</summary>
    /// <param name="accessToken">访问令牌。</param>
    /// <param name="expireMs">绝对过期毫秒时间戳。</param>
    /// <returns>持久化字符串。</returns>
    public static string EncodeToken(string? accessToken, long expireMs)
        => expireMs.ToString(System.Globalization.CultureInfo.InvariantCulture) + Separator + (accessToken ?? string.Empty);

    /// <summary>读穿透方向：存储字符串 → (accessToken, expireMs)。解码失败返回空令牌。</summary>
    /// <param name="stored">持久化字符串。</param>
    /// <returns>令牌与绝对过期毫秒时间戳。</returns>
    public static (string? AccessToken, long ExpireTimestampMs) DecodeToken(string? stored)
    {
        if (string.IsNullOrEmpty(stored))
        {
            return (null, 0);
        }

        var separatorIndex = stored!.IndexOf(Separator);
        if (separatorIndex <= 0
            || !long.TryParse(
                stored.Substring(0, separatorIndex),
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var expireMs)
            || expireMs <= 0)
        {
            return (null, 0);
        }

        return (stored.Substring(separatorIndex + 1), expireMs);
    }

    /// <summary>由过期毫秒时间戳计算剩余有效秒数。</summary>
    /// <param name="expireMs">绝对过期毫秒时间戳。</param>
    /// <returns>剩余秒数（已过期返回 0）。</returns>
    public static long RemainingSeconds(long expireMs)
    {
        var remaining = expireMs - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return remaining > 0 ? remaining / 1000L : 0;
    }
}
