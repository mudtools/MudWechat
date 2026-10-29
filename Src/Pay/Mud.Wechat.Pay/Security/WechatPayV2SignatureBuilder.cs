// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Security;

/// <summary>
/// 微信支付 APIv2 签名器（MD5 / HMAC-SHA256 两种 <c>SIGN_TYPE</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方算法</b>（<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011934999"/>，签名算法总述）：
/// ① 参与签名参数 = 全部非空业务参数，<b>排除</b> <c>sign</c> 与空值（<c>null</c> / 空串）；
/// ② 按参数名 <b>ASCII 升序</b>排列后以 <c>k=v&amp;</c> 拼接，末尾追加 <c>key=APIv2密钥</c>；
/// ③ 取 <c>MD5</c> 或 <c>HMAC-SHA256</c> 摘要并<b>转大写十六进制</b>即 <c>sign</c>。</para>
/// <para><b>陷阱</b>：<c>key=</c> 追加的商户密钥<b>不参与排序</b>、<b>不出现在报文里</b>，
/// 但它是签名串的最后一节 —— 把它当成普通排序项会算出错误签名。
/// <c>HMAC-SHA256</c> 分支下密钥作 HMAC 的 key、<b>不</b>进消息串的说法是错误的，
/// 官方实现仍把 <c>key=...</c> 拼进消息再走 HMAC。</para>
/// <para><b>AOT 说明</b>：本类只吃 <see cref="IEnumerable{T}"/> 形态的扁平键值对，
/// 不涉反射，故可安全用于 Native AOT 严格模式。</para>
/// </remarks>
internal static class WechatPayV2SignatureBuilder
{
    /// <summary>APIv2 签名参数名（官方固定值，不参与排序且不出现在应答校验之外）。</summary>
    internal const string SignFieldName = "sign";

    /// <summary>签名字段名对应的取值参数（<c>sign_type</c>）。</summary>
    internal const string SignTypeFieldName = "sign_type";

    /// <summary>企业微信侧签名字段名（<c>workwx_sign</c>，官方固定值）。</summary>
    internal const string WorkWxSignFieldName = "workwx_sign";

    /// <summary>取 MD5 大写十六进制（供 <c>workwx_sign</c> 复用同一摘要口径）。</summary>
    internal static string Md5UpperHex(string message)
        => ToUpperHex(Mud.Wechat.Pay.Internal.PayCryptoCompat.Md5(Encoding.UTF8.GetBytes(message)));

    /// <summary>
    /// 构造参与签名的规范化串（不含 <c>key=</c> 尾巴）：ASCII 升序、排除 <c>sign</c> 与空值。
    /// </summary>
    /// <param name="fields">扁平键值对（键为官方参数名，值为已字符串化的字段值）。</param>
    /// <returns><c>k=v&amp;</c> 形态的待签名串，末尾自带一个 <c>&amp;</c> 以便直接接 <c>key=</c>。</returns>
    internal static string BuildCanonicalString(IEnumerable<KeyValuePair<string, string?>> fields)
        => BuildCanonicalString(fields, whitelist: null);

    /// <summary>
    /// 构造规范化串，但<b>只取白名单内的字段</b>（企业微信 <c>workwx_sign</c> 口径）。
    /// </summary>
    /// <param name="fields">扁平键值对。</param>
    /// <param name="whitelist">参与签名的字段名集合；<see langword="null"/> 表示「除 <c>sign</c> 外全部参与」。</param>
    /// <returns><c>k=v&amp;</c> 形态的待签名串。</returns>
    /// <remarks>
    /// 白名单是<b>官方固定字段集</b>而非「少签一点更安全」的可选项：企业红包与向员工付款两支的
    /// <c>workwx_sign</c> 各自规定了参与字段，多签或少签都会得到 <c>SIGNERROR</c>。
    /// </remarks>
    internal static string BuildCanonicalString(
        IEnumerable<KeyValuePair<string, string?>> fields, IReadOnlyCollection<string>? whitelist)
    {
        if (fields is null) { throw new ArgumentNullException(nameof(fields)); }

        var builder = new StringBuilder();
        foreach (var field in fields
                     .Where(f => !string.Equals(f.Key, SignFieldName, StringComparison.OrdinalIgnoreCase))
                     .Where(f => whitelist is null || whitelist.Contains(f.Key, StringComparer.OrdinalIgnoreCase))
                     .Where(f => !string.IsNullOrEmpty(f.Value))
                     .OrderBy(f => f.Key, StringComparer.Ordinal))
        {
            builder.Append(field.Key).Append('=').Append(field.Value).Append('&');
        }

        return builder.ToString();
    }

    /// <summary>
    /// 计算 <c>sign</c>。
    /// </summary>
    /// <param name="fields">扁平键值对。</param>
    /// <param name="apiKey">商户 APIv2 密钥（<c>api_key</c>，32 位）。</param>
    /// <param name="hmacSha256"><see langword="true"/> 走 <c>HMAC-SHA256</c>，<see langword="false"/> 走 <c>MD5</c>。</param>
    /// <returns>大写十六进制签名。</returns>
    internal static string Sign(IEnumerable<KeyValuePair<string, string?>> fields, string apiKey, bool hmacSha256)
    {
        // 不用 ArgumentException.ThrowIfNullOrEmpty（.NET 8 起才有，本线含 net6.0）。
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("APIv2 签名密钥不能为空。", nameof(apiKey));
        }

        // 官方串尾固定追加 key=<API 密钥>，该节不参与排序。
        var message = BuildCanonicalString(fields) + "key=" + apiKey;

        var hash = hmacSha256
            ? Mud.Wechat.Pay.Internal.PayCryptoCompat.HmacSha256(Encoding.UTF8.GetBytes(apiKey), Encoding.UTF8.GetBytes(message))
            : Mud.Wechat.Pay.Internal.PayCryptoCompat.Md5(Encoding.UTF8.GetBytes(message));

        return ToUpperHex(hash);
    }

    /// <summary>
    /// 校验应答签名（应答的 <c>return_code</c> 为 <c>SUCCESS</c> 时官方才保证 <c>sign</c> 可校验）。
    /// </summary>
    /// <param name="fields">应答扁平键值对（含 <c>sign</c>）。</param>
    /// <param name="apiKey">商户 APIv2 密钥。</param>
    /// <param name="hmacSha256">是否 <c>HMAC-SHA256</c>。</param>
    /// <returns>校验通过返回 <see langword="true"/>。</returns>
    internal static bool Verify(IEnumerable<KeyValuePair<string, string?>> fields, string apiKey, bool hmacSha256)
    {
        var list = fields.ToList();
        var actual = list.FirstOrDefault(f => string.Equals(f.Key, SignFieldName, StringComparison.OrdinalIgnoreCase)).Value;
        if (string.IsNullOrEmpty(actual))
        {
            return false;
        }

        return string.Equals(Sign(list, apiKey, hmacSha256), actual, StringComparison.OrdinalIgnoreCase);
    }

    private static string ToUpperHex(byte[] hash)
    {
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("X2"));
        }

        return builder.ToString();
    }
}
