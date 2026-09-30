// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 令牌持久化桥接编解码器（对齐 FeishuTokenBridgeCodec）：
/// CredentialToken 与存储三元组 <c>{expireMs}|{token}</c> 的互转。
/// </summary>
internal static class WechatTokenBridgeCodec
{
    private const char Separator = '|';

    /// <summary>写穿方向：CredentialToken → 存储字符串（{expireMs}|{token} 格式）。</summary>
    public static string EncodeToken(string? accessToken, long expireMs)
        => expireMs.ToString(CultureInfo.InvariantCulture) + Separator + (accessToken ?? string.Empty);

    /// <summary>读穿透方向：存储字符串 → (accessToken, expireMs)。解码失败返回空令牌。</summary>
    public static (string? AccessToken, long ExpireTimestampMs) DecodeToken(string? stored)
    {
        if (string.IsNullOrEmpty(stored))
        {
            return (null, 0);
        }

        var separatorIndex = stored.IndexOf(Separator);
        if (separatorIndex <= 0
            || !long.TryParse(stored.Substring(0, separatorIndex), NumberStyles.Integer, CultureInfo.InvariantCulture, out var expireMs)
            || expireMs <= 0)
        {
            return (null, 0);
        }

        return (stored.Substring(separatorIndex + 1), expireMs);
    }

    /// <summary>由过期毫秒时间戳计算剩余有效秒数（永不超过 2^31-1）。</summary>
    public static long RemainingSeconds(long expireMs)
    {
        var remaining = expireMs - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return remaining > 0 ? remaining / 1000L : 0;
    }
}
