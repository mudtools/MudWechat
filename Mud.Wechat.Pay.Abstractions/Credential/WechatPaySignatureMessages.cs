// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Security.Cryptography;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 微信支付 APIv3 签名串与 <c>Authorization</c> 头的**唯一组装点**（官方规范的机械落地）。
/// </summary>
/// <remarks>
/// <para>
/// 口径已与官方 SDK 源码逐字核对（<c>wechatpay-java</c> 的 <c>WechatPay2Credential#buildMessage</c>
/// 与 <c>WechatPay2Validator#validate</c>），**不是**凭印象实现：
/// </para>
/// <list type="number">
/// <item><b>请求签名串</b> = <c>HTTP方法\nURL\n时间戳\n随机串\n请求体\n</c>。
/// 其中 URL = <c>rawPath</c> +（存在查询时）<c>?rawQuery</c>；请求体为空取空串。
/// <b>行尾换行不可省</b>——少一个 <c>\n</c> 即线上 100% 验签失败。</item>
/// <item><b>验签串</b>（应答与回调**同一形态**）= <c>时间戳\n随机串\n报文体\n</c>，行尾同样带换行。
/// 报文体必须是**原始字节**，不得反序列化后重新序列化（官方 FAQ 明确的第一大失败原因）。</item>
/// </list>
/// <para>
/// 任一字节漂移都会让线上验签全量失败 ⇒ 本类是黄金向量守卫 <b>PAY-B2</b> 的被测对象；
/// 请勿为「可读性」改动换行位置、字段顺序或空 body 语义。
/// </para>
/// </remarks>
public static class WechatPaySignatureMessages
{
    /// <summary><c>Authorization</c> 头的 schema 前缀（官方固定值，勿改）。</summary>
    public const string AuthorizationSchema = "WECHATPAY2-SHA256-RSA2048";

    /// <summary>
    /// 签名算法标识（BCL 侧对应 <c>RSA.SignData</c> + <c>HashAlgorithmName.SHA256</c> + <c>RSASignaturePadding.Pkcs1</c>）。
    /// </summary>
    public const string SignatureAlgorithm = "SHA256-RSA2048";

    /// <summary>时间戳窗口默认半径（秒），与企微回调 <c>AllowClockSkewSeconds</c> 默认值同语义。</summary>
    public const int DefaultTimestampSkewSeconds = 300;

    /// <summary>换行符：官方签名串逐字节为 <c>\n</c>（U+000A），**不得**换成 <c>Environment.NewLine</c>。</summary>
    private const char NewLine = '\n';

    /// <summary>
    /// 组装签名用 URL：路径 +（可选）查询串。
    /// </summary>
    /// <param name="path">已编码的原始路径（raw path），如 <c>/v3/pay/transactions/jsapi</c>。</param>
    /// <param name="query">已编码的原始查询串（**不带**前导 <c>?</c>）；为 <c>null</c>/<c>空白</c> 时忽略。</param>
    /// <returns>签名用规范 URL。</returns>
    /// <remarks>必须用 raw 形态：任何再编码都会改变字节序列并导致验签失败。</remarks>
    public static string BuildCanonicalUrl(string path, string? query)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (string.IsNullOrEmpty(query))
        {
            return path;
        }

        return string.Concat(path, "?", query);
    }

    /// <summary>组装**请求**签名串：<c>方法\nURL\n时间戳\n随机串\n请求体\n</c>。</summary>
    /// <param name="httpMethod">大写 HTTP 方法（<c>GET</c>/<c>POST</c>/…）。</param>
    /// <param name="canonicalUrl">已由 <see cref="BuildCanonicalUrl"/> 组装的规范 URL。</param>
    /// <param name="timestamp">秒级 Unix 时间戳。</param>
    /// <param name="nonce">随机串（官方用 32 位）。</param>
    /// <param name="body">请求体原文；无请求体时传 <c>null</c> 或空串。</param>
    /// <returns>逐字节对齐官方规范的签名串。</returns>
    public static string BuildRequestMessage(
        string httpMethod,
        string canonicalUrl,
        long timestamp,
        string nonce,
        string? body)
    {
        if (httpMethod is null)
        {
            throw new ArgumentNullException(nameof(httpMethod));
        }

        if (canonicalUrl is null)
        {
            throw new ArgumentNullException(nameof(canonicalUrl));
        }

        if (nonce is null)
        {
            throw new ArgumentNullException(nameof(nonce));
        }

        return string.Concat(
            httpMethod, NewLine,
            canonicalUrl, NewLine,
            timestamp.ToString(CultureInfo.InvariantCulture), NewLine,
            nonce, NewLine,
            body ?? string.Empty, NewLine);
    }

    /// <summary>组装**验签**串（应答与回调同形态）：<c>时间戳\n随机串\n报文体\n</c>。</summary>
    /// <param name="timestamp"><c>Wechatpay-Timestamp</c> 原文（保持字符串，勿先转数值再格式化）。</param>
    /// <param name="nonce"><c>Wechatpay-Nonce</c> 原文。</param>
    /// <param name="body">**原始**报文体；缺失时传 <c>null</c>。</param>
    /// <returns>逐字节对齐官方规范的验签串。</returns>
    public static string BuildVerifyMessage(string timestamp, string nonce, string? body)
    {
        if (timestamp is null)
        {
            throw new ArgumentNullException(nameof(timestamp));
        }

        if (nonce is null)
        {
            throw new ArgumentNullException(nameof(nonce));
        }

        return string.Concat(timestamp, NewLine, nonce, NewLine, body ?? string.Empty, NewLine);
    }

    /// <summary>小程序调起支付的签名类型（官方 <c>signType</c>，<b>仅支持</b> <c>RSA</c>）。</summary>
    public const string MiniProgramPaySignType = "RSA";

    /// <summary>
    /// 组装**小程序调起支付**签名串：<c>appId\n时间戳\n随机串\npackage\n</c>。
    /// </summary>
    /// <param name="appId">调起支付小程序的 AppID（<b>必须</b>与下单时传入的一致，微信支付会校验一致性）。</param>
    /// <param name="timeStamp">秒级时间戳字符串（10 位；官方字段名<b>就是这个驼峰写法</b>）。</param>
    /// <param name="nonceStr">随机串（≤32 位）。</param>
    /// <param name="package">预支付交易会话标识，官方格式固定为 <c>prepay_id={prepay_id}</c>。</param>
    /// <returns>逐字节对齐官方规范的签名串。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方依据</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791898"/>（小程序调起支付）
    /// 明确列出参与签名的字段与<b>顺序</b>为 <c>appId</c> → <c>timeStamp</c> → <c>nonceStr</c> → <c>package</c>，
    /// 并指向「小程序调起支付签名」；该页未复述分隔符，而 APIv3 全部签名串共用
    /// <b>「每项后跟一个 <c>\n</c>（含末项）」</b> 的同一规则（与本类另两个组装方法一致）——
    /// 少一个 <c>\n</c> 即线上 100% 调起失败。
    /// </para>
    /// <para>
    /// <b>顺序不可改</b>：<c>signType</c> <b>不</b>参与签名（官方字段表中它只是前端参数），
    /// 而 <c>package</c> <b>必须</b>带 <c>prepay_id=</c> 前缀 —— 传裸 <c>prepay_id</c> 会签名不匹配。
    /// </para>
    /// </remarks>
    public static string BuildMiniProgramPaySignMessage(
        string appId, string timeStamp, string nonceStr, string package)
    {
        if (appId is null)
        {
            throw new ArgumentNullException(nameof(appId));
        }

        if (timeStamp is null)
        {
            throw new ArgumentNullException(nameof(timeStamp));
        }

        if (nonceStr is null)
        {
            throw new ArgumentNullException(nameof(nonceStr));
        }

        if (package is null)
        {
            throw new ArgumentNullException(nameof(package));
        }

        return string.Concat(appId, NewLine, timeStamp, NewLine, nonceStr, NewLine, package, NewLine);
    }

    /// <summary>组装 <c>package</c> 字段值：官方固定格式 <c>prepay_id={prepay_id}</c>。</summary>
    /// <param name="prepayId">下单接口返回的 <c>prepay_id</c>。</param>
    /// <returns><c>package</c> 字段值。</returns>
    /// <exception cref="ArgumentException"><paramref name="prepayId"/> 为 <c>null</c>/空白。</exception>
    public static string BuildPackageValue(string prepayId)
    {
        if (string.IsNullOrWhiteSpace(prepayId))
        {
            throw new ArgumentException("prepay_id 不可为空白。", nameof(prepayId));
        }

        return string.Concat("prepay_id=", prepayId);
    }

    /// <summary>组装 <c>Authorization</c> 头值。</summary>
    /// <remarks>
    /// 字段顺序照官方原文：<c>mchid</c> → <c>nonce_str</c> → <c>timestamp</c> → <c>serial_no</c> → <c>signature</c>。
    /// 头本身不参与签名，故顺序不影响验签结果，但仍照抄官方以免与抓包排障时对不上。
    /// </remarks>
    public static string BuildAuthorization(
        string mchid,
        string serialNo,
        long timestamp,
        string nonce,
        string signature)
    {
        if (mchid is null)
        {
            throw new ArgumentNullException(nameof(mchid));
        }

        if (serialNo is null)
        {
            throw new ArgumentNullException(nameof(serialNo));
        }

        if (nonce is null)
        {
            throw new ArgumentNullException(nameof(nonce));
        }

        if (signature is null)
        {
            throw new ArgumentNullException(nameof(signature));
        }

        return string.Concat(
            AuthorizationSchema,
            " mchid=\"", mchid,
            "\",nonce_str=\"", nonce,
            "\",timestamp=\"", timestamp.ToString(CultureInfo.InvariantCulture),
            "\",serial_no=\"", serialNo,
            "\",signature=\"", signature,
            "\"");
    }

    /// <summary>随机串字母表：官方请求 nonce 使用 <c>[a-zA-Z0-9]</c>。</summary>
    private const string NonceAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>生成请求用随机串长度（官方 32 位）。</summary>
    public const int NonceLength = 32;

    /// <summary>
    /// 生成请求签名用随机串（<c>nonce_str</c>，官方 32 位 <c>[a-zA-Z0-9]</c>）。
    /// </summary>
    /// <returns>随机串。</returns>
    /// <remarks>
    /// 用 <see cref="RandomNumberGenerator"/>（密码学随机）而非 <see cref="Random"/>：
    /// nonce 的不可预测性决定攻击者无法预知待签消息，且逐请求唯一性是防重放的第一道前提。
    /// </remarks>
    public static string CreateNonce()
    {
        Span<char> buffer = stackalloc char[NonceLength];
        Span<byte> random = stackalloc byte[NonceLength];
        RandomNumberGenerator.Fill(random);

        for (var i = 0; i < random.Length; i++)
        {
            // 取模：字母表 62 个字符可整除性偏差可忽略，且官方对 nonce 无均匀性要求（只需唯一 + 限长）。
            buffer[i] = NonceAlphabet[random[i] % NonceAlphabet.Length];
        }

        return new string(buffer);
    }
}
