// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 时间戳窗口闸：支付回调/应答三道闸中的**第 ② 道**（方案 v2 §2.6，PAY-B3）。
/// </summary>
/// <remarks>
/// <para>
/// 为什么必须有：只验签不校验时效时，攻击者截获**一次**合法通知即可**无限重放**
/// ——签名在证书有效期内恒真。故「验签通过」**不足以**放行。
/// </para>
/// <para>
/// 语义与企微回调 <c>AllowClockSkewSeconds</c>（±300s）一致，便于宿主统一理解；
/// 但本闸**独立于**抗重放指纹（第 ③ 道），两道各司其职、不得互相替代。
/// </para>
/// <para><b>fail-closed</b>：缺失 / 非数字 / 负值 / 超窗 —— 任何一种都不得放行，且**不得**放宽为「解析失败即跳过」。</para>
/// </remarks>
public static class WechatPayTimestampGate
{
    /// <summary>时间戳校验结论（枚举而非异常消息 ⇒ 天然不含报文内容，满足 PAY-B7）。</summary>
    public enum Verdict
    {
        /// <summary>在窗口内，可通过。</summary>
        Valid = 0,

        /// <summary>缺失或空白。</summary>
        Missing = 1,

        /// <summary>非十进制整数，或为负值。</summary>
        NotNumeric = 2,

        /// <summary>超出允许的时间偏差窗口。</summary>
        OutOfWindow = 3,
    }

    /// <summary>校验时间戳是否落在允许窗口内。</summary>
    /// <param name="timestamp">时间戳原文（<c>Wechatpay-Timestamp</c>，保持字符串传入）。</param>
    /// <param name="nowEpochSeconds">当前秒级 Unix 时间戳。</param>
    /// <param name="allowedSkewSeconds">允许偏差（秒），默认 <see cref="WechatPaySignatureMessages.DefaultTimestampSkewSeconds"/>。</param>
    /// <returns>校验结论。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nowEpochSeconds"/> 为负，或 <paramref name="allowedSkewSeconds"/> 为负。</exception>
    public static Verdict Validate(
        string? timestamp,
        long nowEpochSeconds,
        int allowedSkewSeconds = WechatPaySignatureMessages.DefaultTimestampSkewSeconds)
    {
        if (nowEpochSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nowEpochSeconds), "当前时间戳不可为负。");
        }

        if (allowedSkewSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(allowedSkewSeconds), "允许偏差不可为负。");
        }

        if (string.IsNullOrWhiteSpace(timestamp))
        {
            return Verdict.Missing;
        }

        // NumberStyles.None ⇒ 不接受符号、千分位与空白：任何非纯数字形态一律 NotNumeric。
        if (!long.TryParse(timestamp, System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out var seconds))
        {
            return Verdict.NotNumeric;
        }

        if (seconds < 0)
        {
            return Verdict.NotNumeric;
        }

        // 双方均非负 ⇒ 差值不可能溢出（先判大小再相减，避免 now - long.MinValue 回绕）。
        long delta = nowEpochSeconds >= seconds ? nowEpochSeconds - seconds : seconds - nowEpochSeconds;
        return delta <= allowedSkewSeconds ? Verdict.Valid : Verdict.OutOfWindow;
    }

    /// <summary>便捷判定：<see cref="Verdict.Valid"/> 即通过。</summary>
    /// <param name="timestamp">时间戳原文。</param>
    /// <param name="nowEpochSeconds">当前秒级 Unix 时间戳。</param>
    /// <param name="allowedSkewSeconds">允许偏差（秒）。</param>
    /// <returns>是否可放行。</returns>
    public static bool IsAccepted(
        string? timestamp,
        long nowEpochSeconds,
        int allowedSkewSeconds = WechatPaySignatureMessages.DefaultTimestampSkewSeconds)
        => Validate(timestamp, nowEpochSeconds, allowedSkewSeconds) == Verdict.Valid;
}
