// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Security;

/// <summary>
/// 企业微信侧签名 <c>workwx_sign</c> 的判定器（发放企业红包、向员工付款两支写入端点专用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>sign</c> 的三处不同</b>（官方「企业红包签名算法」页，2026-10-18 逐页核验）：
/// ① <b>密钥</b>不是商户 APIv2 密钥，而是<b>请求里 <c>agentid</c> 对应的企业微信自建应用 Secret</b>
/// （历史上使用系统应用「企业支付」Secret 的商户可继续沿用）；
/// ② <b>算法是 MD5</b>（不是 HMAC-SHA256，也不随 <c>sign_type</c> 变化）；
/// ③ 串尾固定追加的是 <c>secret=&lt;Secret&gt;</c>（字面量 <c>secret=</c>），
/// 而非 <c>sign</c> 用的 <c>key=&lt;APIv2 密钥&gt;</c>。排序与空值剔除规则与 <c>sign</c> 完全一致。
/// </para>
/// <para>
/// <b>参与字段是官方固定白名单</b>（见 <see cref="SendRedPackWhitelist"/> 与
/// <see cref="PayPocketWhitelist"/>）：<c>agentid</c>、<c>wishing</c>、<c>remark</c> 等均<b>不参与</b>。
/// 白名单做成常量而非「调用方传什么签什么」，是因为传错字段的症状是官方 SIGNERROR，
/// 而排查方向（少签/多签）在报文里完全看不出来。
/// </para>
/// <para>
/// <b>顺序不可颠倒</b>：官方明确「除 <c>sign</c> 外所有字段都参与签名，<b>包括 <c>workwx_sign</c></b>」⇒
/// 必须先算 <c>workwx_sign</c> 入表、再算 <c>sign</c>。反过来产出的 <c>sign</c> 必然对不上。
/// </para>
/// <para>
/// <b>不出密钥</b>（PAY-B7 同线红线）：本类只吃已取到的 Secret，异常文本由调用方构造且不得含 Secret 值；
/// 白名单字段值若含用户身份（如 <c>re_openid</c>），同样不得进日志。
/// </para>
/// </remarks>
internal static class WechatPayV2WorkWxSigner
{
    /// <summary>发放企业红包（<c>/mmpaymkttransfers/sendworkwxredpack</c>）的官方固定白名单。</summary>
    internal static readonly string[] SendRedPackWhitelist =
    {
        "act_name",
        "mch_billno",
        "mch_id",
        "nonce_str",
        "re_openid",
        "total_amount",
        "wxappid",
    };

    /// <summary>向员工付款（<c>/promotion/paywwsptrans2pocket</c>）的官方固定白名单。</summary>
    internal static readonly string[] PayPocketWhitelist =
    {
        "amount",
        "appid",
        "desc",
        "mch_id",
        "nonce_str",
        "openid",
        "partner_trade_no",
        "ww_msg_type",
    };

    /// <summary>白名单内字段在请求里缺失或为空时的错误口径：只报字段名，不报值。</summary>
    internal const string MissingWhitelistFieldMessage =
        "参与 workwx_sign 的官方白名单字段缺失或为空 ⇒ 请补全后再发起（白名单字段少一个即 SIGNERROR）。";

    /// <summary>
    /// 计算 <c>workwx_sign</c>。
    /// </summary>
    /// <param name="fields">请求字段表（可含 <c>sign</c> 占位，不参与）。</param>
    /// <param name="whitelist">该端点的官方固定白名单，取 <see cref="SendRedPackWhitelist"/> 或 <see cref="PayPocketWhitelist"/>。</param>
    /// <param name="secret"><c>agentid</c> 对应的企业微信自建应用 Secret。</param>
    /// <returns>大写十六进制 MD5 签名。</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="secret"/> 为空，或白名单字段在字段表里<b>一个都没出现</b>
    /// （序列化漏字段与漏签名产出的报文同样会被拒，故在此 fail-fast 而不是发出去换一次 SIGNERROR）。
    /// </exception>
    internal static string Sign(
        IReadOnlyList<KeyValuePair<string, string?>> fields,
        IReadOnlyCollection<string> whitelist,
        string secret)
    {
        if (fields is null) { throw new ArgumentNullException(nameof(fields)); }
        if (whitelist is null) { throw new ArgumentNullException(nameof(whitelist)); }

        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("企业微信应用 Secret 不能为空。", nameof(secret));
        }

        var stringA = WechatPayV2SignatureBuilder.BuildCanonicalString(fields, whitelist);
        if (stringA.Length == 0)
        {
            throw new ArgumentException(MissingWhitelistFieldMessage, nameof(fields));
        }

        // 官方串尾是 secret=<Secret>（不是 key=），照抄这一处差异是 workwx_sign 最常见的踩坑点。
        return WechatPayV2SignatureBuilder.Md5UpperHex(stringA + "secret=" + secret);
    }

    /// <summary>字段表中是否已带 <c>workwx_sign</c>（用于幂等放行，与 <c>sign</c> 同一口径）。</summary>
    internal static bool HasWorkWxSignature(IEnumerable<KeyValuePair<string, string?>> fields)
        => fields.Any(static f =>
            string.Equals(f.Key, WechatPayV2SignatureBuilder.WorkWxSignFieldName, StringComparison.OrdinalIgnoreCase));
}
