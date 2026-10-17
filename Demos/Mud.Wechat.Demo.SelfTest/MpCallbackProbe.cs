// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Demo.Shared;

namespace Mud.Wechat.Demo.SelfTest;

/// <summary>
/// 公众号 XML 回调自测：GET 回显、POST 被动回复、同报文重放、篡改签名、安全模式拒收明文五条断言。
/// </summary>
/// <remarks>
/// <para>
/// 报文由 <see cref="WechatCallbackCrypto"/>（SDK 自身的密码学内核）构造，因此自测通过即同时证明
/// 「Demo 侧配置 + SDK 接收链」两端一致；不引入任何独立实现的加解密代码。
/// </para>
/// <para>
/// <b>与企微线的三处形态差异</b>（各自都是本探针要覆盖的断言面）：
/// ① GET 验签是<b>三参</b> <c>signature</c>（不含 echostr），且响应体为 <b>echostr 原文（密文）</b>而非解密明文；
/// ② POST 密文必须用<b>四参</b> <c>msg_signature</c>，官方明文警告「不要使用 signature 验证」⇒ 缺该参数即 403；
/// ③ receiveid 取公众号 <c>AppId</c>（企微自建应用为 <c>CorpId</c>）。
/// </para>
/// </remarks>
internal static class MpCallbackProbe
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    /// <summary>命中 Demo 关键字回复的正文（<c>WelcomeReplyHandler</c> 判据为包含「帮助」）。</summary>
    private const string HelpKeyword = "帮助";

    /// <summary>被动回复明文里的预期片段（用「回复文案」而非整句，避免改文案即误报链路故障）。</summary>
    private const string ExpectedReplyFragment = "关键字回复";

    public static async Task<IReadOnlyList<SelfTestAssertion>> RunAsync(SelfTestOptions options)
    {
        string token;
        string aesKey;
        string appId;
        try
        {
            var env = DemoEnv.RequireAll("MP_CALLBACK_TOKEN", "MP_CALLBACK_AES_KEY", "MP_APP_ID");
            token = env["MP_CALLBACK_TOKEN"];
            aesKey = env["MP_CALLBACK_AES_KEY"];
            appId = env["MP_APP_ID"];
        }
        catch (InvalidOperationException ex)
        {
            return new[] { new SelfTestAssertion("回调凭据读取", false, ex.Message) };
        }

        if (aesKey.Length != 43)
        {
            return new[]
            {
                new SelfTestAssertion(
                    "EncodingAESKey 形态", false,
                    $"长度为 {aesKey.Length}，官方要求 43 位 Base64 字符。"),
            };
        }

        var url = new Uri(options.BaseUrl, options.Route);
        var results = new List<SelfTestAssertion>
        {
            await EchoAsync(url, token, aesKey, appId).ConfigureAwait(false),
        };

        // POST 与重放共用同一组 timestamp/nonce/密文：指纹恒定，才能断言第二道闸 fail-closed。
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var encrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTextXml(appId), appId);
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        var query = $"msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}"
                    + "&encrypt_type=aes";
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

        results.Add(await PostReplyAsync(url, query, body, aesKey, appId).ConfigureAwait(false));
        results.Add(await PostAsync(
            url, query, body, "同一报文重放（期望 403，指纹闸 fail-closed）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));
        results.Add(await TamperedSignatureAsync(url, query, body).ConfigureAwait(false));
        results.Add(await PlainTextRejectedAsync(url, token, appId).ConfigureAwait(false));
        return results;
    }

    /// <summary>
    /// GET URL 验证：期望 200 且回显体<b>等于 echostr 密文原文</b>。
    /// 公众号侧的契约是「原样返回 echostr 参数内容」，与企微侧「返回解密明文」不同 —— 断言写反即说明实现漂移。
    /// </summary>
    private static async Task<SelfTestAssertion> EchoAsync(Uri url, string token, string aesKey, string appId)
    {
        var echoPlain = Guid.NewGuid().ToString("N");
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, echoPlain, appId);

        // 三参签名：token / timestamp / nonce，**不含** echostr。
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce);

        var target = new Uri(
            url,
            $"?signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}" +
            $"&echostr={Uri.EscapeDataString(echoEncrypt)}");

        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK
                         && string.Equals(text, echoEncrypt, StringComparison.Ordinal);

            return new SelfTestAssertion(
                "GET URL 验证回显 echostr 原文（三参 signature）",
                passed,
                passed
                    ? "实际 200，响应体与 echostr 密文逐字一致。"
                    : $"实际 {(int)response.StatusCode}，响应体「{Truncate(text)}」（期望 echostr 密文原文 {Truncate(echoEncrypt)}）。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(EchoAsync), url, ex);
        }
    }

    /// <summary>
    /// POST 文本消息（正文含「帮助」）：期望 200 且回包为<b>加密 XML</b>，
    /// 解密后 receiveid = AppId、明文含被动回复文案 —— 一并验证「密文进、密文出」的整条回复链。
    /// </summary>
    private static async Task<SelfTestAssertion> PostReplyAsync(
        Uri url, string query, string body, string aesKey, string appId)
    {
        var target = new Uri(url, "?" + query);
        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/xml");
            using var response = await Http.PostAsync(target, content).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new SelfTestAssertion(
                    "POST 文本消息触发被动回复（期望 200 加密回包）",
                    false,
                    $"实际 {(int)response.StatusCode}（期望 200），响应体「{Truncate(text)}」。");
            }

            return new SelfTestAssertion(
                "POST 文本消息触发被动回复（期望 200 加密回包）",
                TryReadReply(text, aesKey, appId, out var reason),
                reason);
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(PostReplyAsync), target, ex);
        }
    }

    /// <summary>篡改 msg_signature：期望 403（验签失败，且不消耗指纹）。</summary>
    private static async Task<SelfTestAssertion> TamperedSignatureAsync(Uri url, string query, string body)
    {
        var tampered = query.Replace("msg_signature=", "msg_signature=0", StringComparison.Ordinal);
        return await PostAsync(
            url, tampered, body, "篡改 msg_signature（期望 403）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false);
    }

    /// <summary>
    /// 安全模式下推明文报文（带合法三参 <c>signature</c>、无 <c>msg_signature</c>）：期望 403。
    /// 这是 CB-MP-2 的运行期反证 —— 密文分支<b>不得</b>回落 <c>signature</c>，明文分支在安全模式一律拒收。
    /// </summary>
    private static async Task<SelfTestAssertion> PlainTextRejectedAsync(Uri url, string token, string appId)
    {
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce);
        var query = $"signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}";

        return await PostAsync(
            url, query, BuildTextXml(appId), "安全模式拒收明文报文（期望 403，不得回落三参 signature）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false);
    }

    private static async Task<SelfTestAssertion> PostAsync(
        Uri url, string query, string body, string name, HttpStatusCode expected, string? expectedBody)
    {
        var target = new Uri(url, "?" + query);
        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/xml");
            using var response = await Http.PostAsync(target, content).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var bodyMatches = expectedBody == null || string.Equals(text, expectedBody, StringComparison.Ordinal);
            var passed = response.StatusCode == expected && bodyMatches;

            return new SelfTestAssertion(
                name,
                passed,
                $"实际 {(int)response.StatusCode}（期望 {(int)expected}），响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, target, ex);
        }
    }

    /// <summary>解析加密回包并校验 receiveid 与回复文案（失败原因写进断言详情，便于区分「没回包」与「回错内容」）。</summary>
    private static bool TryReadReply(string responseText, string aesKey, string appId, out string reason)
    {
        XElement? root;
        try
        {
            root = XDocument.Parse(responseText).Root;
        }
        catch (Exception ex) when (ex is XmlException or FormatException)
        {
            reason = $"回包非 XML（安全模式下有回复必须加密）：{ex.GetType().Name}，原文「{Truncate(responseText)}」。";
            return false;
        }

        var encrypt = root?.Element("Encrypt")?.Value;
        if (string.IsNullOrEmpty(encrypt))
        {
            reason = $"回包缺少 Encrypt 节点，原文「{Truncate(responseText)}」。";
            return false;
        }

        string plaintext;
        try
        {
            plaintext = WechatCallbackCrypto.Decrypt(aesKey, encrypt!, out var receiveId);
            if (!string.Equals(receiveId, appId, StringComparison.Ordinal))
            {
                reason = $"回包解密成功但 receiveid 为「{Truncate(receiveId)}」，期望 AppId「{Truncate(appId)}」。";
                return false;
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            reason = $"回包解密失败：{ex.GetType().Name} {ex.Message}";
            return false;
        }

        var passed = plaintext.Contains(ExpectedReplyFragment, StringComparison.Ordinal);
        reason = passed
            ? $"回包解密后的明文回复「{Truncate(plaintext)}」。"
            : $"回包解密后的明文为「{Truncate(plaintext)}」，未含预期片段「{ExpectedReplyFragment}」。";
        return passed;
    }

    /// <summary>
    /// 文本消息明文（官方 17888 结构）：<c>ToUserName</c> = 公众号 AppId、<c>FromUserName</c> = 用户 openid。
    /// MsgId 每次运行取新值，避免命中 Demo 侧的被动回复排重键。
    /// </summary>
    private static string BuildTextXml(string appId)
        => "<xml>"
           + $"<ToUserName><![CDATA[{appId}]]></ToUserName>"
           + $"<FromUserName><![CDATA[oSelfTest{Guid.NewGuid().ToString("N").Substring(0, 16)}]]></FromUserName>"
           + $"<CreateTime>{Now()}</CreateTime>"
           + "<MsgType><![CDATA[text]]></MsgType>"
           + $"<Content><![CDATA[{HelpKeyword}]]></Content>"
           // 官方 MsgId 为 64 位整数字符串；Demo 排重键取该值，故这里保持数字形态（秒 + 随机后缀 = 每次运行唯一）。
           + $"<MsgId>{Now()}{RandomNumberGenerator.GetInt32(100, 999)}</MsgId>"
           + "</xml>";

    private static string Now(IFormatProvider? provider = null)
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(provider ?? CultureInfo.InvariantCulture);

    private static SelfTestAssertion ConnectionFailure(string name, Uri url, Exception ex)
        => new(
            name,
            false,
            $"请求 {url} 失败：{ex.GetType().Name} {ex.Message}。请确认目标 Demo 已启动、地址与路由正确。");

    private static async Task<string> SafeReadAsync(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return $"<响应体读取失败：{ex.GetType().Name}>";
        }
    }

    private static string Truncate(string value)
        => value.Length <= 120 ? value : value.Substring(0, 120) + "…";
}
