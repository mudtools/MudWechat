// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Security;

/// <summary>
/// APIv2 报文<b>签名 / 验签</b>的统一判定：把「算法怎么选」这一条规则收在一处，
/// 供出站签名 Handler 与双向证书通道（<c>secapi</c> 侧手动收发）共用。
/// </summary>
/// <remarks>
/// <para>
/// <b>请求侧</b>：算法由报文里的 <c>sign_type</c> 决定，缺省即 <c>MD5</c>（官方默认）。
/// </para>
/// <para>
/// <b>应答侧</b>：官方<b>应答体不带</b> <c>sign_type</c>，而算法实际由请求侧选定 ⇒
/// 「这条 sign 该按哪种算法重算」在报文内不可判。此处按<b>两种都试、任一命中即通过</b>处理：
/// 验签的性质是「只有持有密钥者能造出这条 sign」，双试不削弱它（伪造仍需密钥），
/// 却能避免「商户用 HMAC-SHA256 下单 ⇒ 应答验签必败」这种自伤。
/// 应答若自带 <c>sign_type</c> 则严格按其声明判定。
/// </para>
/// </remarks>
internal static class WechatPayV2Signer
{
    /// <summary>HMAC-SHA256 的官方取值（大小写不敏感比对）。</summary>
    internal const string HmacSha256SignType = "HMAC-SHA256";

    /// <summary>按报文 <c>sign_type</c> 判签名算法。</summary>
    internal static bool UsesHmacSha256(IEnumerable<KeyValuePair<string, string?>> fields)
        => fields.Any(static f =>
            string.Equals(f.Key, WechatPayV2SignatureBuilder.SignTypeFieldName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(f.Value, HmacSha256SignType, StringComparison.OrdinalIgnoreCase));

    /// <summary>对字段表签名（不含 <c>sign</c> 自身，空值不参与）。</summary>
    internal static string Sign(IReadOnlyList<KeyValuePair<string, string?>> fields, string apiKey)
        => WechatPayV2SignatureBuilder.Sign(fields, apiKey, UsesHmacSha256(fields));

    /// <summary>校验字段表中的 <c>sign</c>；应答未声明算法时两种都试（见类注释）。</summary>
    internal static bool Verify(IReadOnlyList<KeyValuePair<string, string?>> fields, string apiKey)
    {
        var declared = fields.FirstOrDefault(static f =>
            string.Equals(f.Key, WechatPayV2SignatureBuilder.SignTypeFieldName, StringComparison.OrdinalIgnoreCase))
            .Value;

        return string.IsNullOrEmpty(declared)
            ? WechatPayV2SignatureBuilder.Verify(fields, apiKey, hmacSha256: false)
              || WechatPayV2SignatureBuilder.Verify(fields, apiKey, hmacSha256: true)
            : WechatPayV2SignatureBuilder.Verify(fields, apiKey, UsesHmacSha256(fields));
    }

    /// <summary>字段表中是否携带 <c>sign</c>。</summary>
    internal static bool HasSignature(IEnumerable<KeyValuePair<string, string?>> fields)
        => fields.Any(static f =>
            string.Equals(f.Key, WechatPayV2SignatureBuilder.SignFieldName, StringComparison.OrdinalIgnoreCase));
}
