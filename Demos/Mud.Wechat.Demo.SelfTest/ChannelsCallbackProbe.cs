// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Text;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Demo.Shared;

namespace Mud.Wechat.Demo.SelfTest;

/// <summary>
/// 微信小店 JSON 回调自测：GET 回显、POST 类型化载荷消费、重放/篡改/超窗/错属主四类闸门、
/// 兜底记录、形态闸（415/405）与路由未配置共十七条断言。
/// </summary>
/// <remarks>
/// <para>
/// 报文由 <see cref="WechatCallbackCrypto"/>（SDK 自身的密码学内核，Core 叶层）构造，
/// 因此自测通过即同时证明「Demo 侧回调配置 + SDK 接收链」两端一致；
/// <b>不引用任何产品线主包</b>，明文 JSON 里的字段名照官方原文写字面量（<c>ProductSpuUpdate</c> /
/// <c>product_id</c> / <c>status</c>）—— 探针与 DTO 各自独立，才测得出契约漂移。
/// </para>
/// <para>
/// <b>与公众号 XML 通道的四处形态差异</b>（都是本探针要覆盖的断言面）：
/// ① 信封是 JSON（<c>{"ToUserName":"gh_*","Encrypt":"..."}</c>），Content-Type 必须是 JSON，否则 415；
/// ② 官方<b>强制安全模式</b>，没有「明文分支」⇒ 不存在「安全模式拒收明文」这条断言，
///    取而代之的是「体不是合法 JSON / 缺 Encrypt 字段」→ 403；
/// ③ 无被动回复语义 ⇒ 成功应答恒为明文 <c>success</c>（<c>text/plain</c>），不是加密回包；
/// ④ receiveid 校验基准是小店 AppID，且 <c>AppId</c> 属回调<b>必填</b>配置（缺即 Validate 拒）。
/// </para>
/// <para>
/// <b>三态口径</b>：闸门拒绝 = <c>403 + 空体</c>（官方语义上等同「未收到」⇒ 会重推，但同指纹再推仍 403）；
/// 正常 = <c>200 + success</c>；「无人接线」不算拒绝 —— 分发器只记 Warning，应答照样 success，
/// 故这类事件必须靠 <c>/api/callback-events</c> 台账判定，光看 200 会误读成「处理好了」。
/// </para>
/// </remarks>
internal static class ChannelsCallbackProbe
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    /// <summary>Demo 已接线的类型化事件键（<c>ProductSpuUpdateHandler</c>）。</summary>
    private const string WiredEvent = "product_spu_update";

    /// <summary>Demo 未接线的官方事件键（用于验证兜底记录器真的接住了未命中事件）。</summary>
    private const string UnwiredEvent = "set_shop_nickname";

    /// <summary>Demo 台账里兜底记录的固定标记（与 <c>FallbackEventRecorder</c> 的 detail 文案一致）。</summary>
    private const string FallbackMarker = "兜底记录";

    /// <summary>事件主体里的商品 ID 前缀；每次运行拼上秒级时间戳，避免命中上一次运行的台账残留。</summary>
    private const string ProductIdPrefix = "98000000";

    public static async Task<IReadOnlyList<SelfTestAssertion>> RunAsync(SelfTestOptions options)
    {
        string token;
        string aesKey;
        string appId;
        try
        {
            var env = DemoEnv.RequireAll("CHANNELS_CALLBACK_TOKEN", "CHANNELS_CALLBACK_AES_KEY", "CHANNELS_APP_ID");
            token = env["CHANNELS_CALLBACK_TOKEN"];
            aesKey = env["CHANNELS_CALLBACK_AES_KEY"];
            appId = env["CHANNELS_APP_ID"];
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
                    $"长度为 {aesKey.Length}，官方要求 43 位 Base64 字符（小店强制安全模式）。"),
            };
        }

        var url = new Uri(options.BaseUrl, options.Route);
        var results = new List<SelfTestAssertion>
        {
            await EchoAsync(url, token, aesKey, appId).ConfigureAwait(false),
            await EchoRejectedAsync(
                url, token, aesKey, appId, tamperSignature: true).ConfigureAwait(false),
            await EchoRejectedAsync(
                url, token, aesKey, appId, tamperSignature: false).ConfigureAwait(false),
        };

        // 合法报文：timestamp/nonce/密文一次生成，重放断言复用同一组 ⇒ 指纹恒定，才测得出第二道闸。
        var productId = ProductIdPrefix + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var encrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildPlainJson(WiredEvent, appId, productId), appId);
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        var query = $"msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}";
        var body = BuildEnvelope(encrypt);

        results.Add(await PostAsync(
            url, query, body, $"POST {WiredEvent}（期望 200 + 明文 success）",
            HttpStatusCode.OK, "success", null).ConfigureAwait(false));
        results.Add(await LedgerContainsAsync(
            options.BaseUrl, productId,
            $"台账出现本次推送的 product_id（证明类型化载荷真被读出）（{productId}）").ConfigureAwait(false));
        results.Add(await PostAsync(
            url, query, body, "同一报文重放（期望 403，指纹闸 fail-closed）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));
        results.Add(await PostAsync(
            url, TamperSignature(query), body, "篡改 msg_signature（期望 403）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        // 换属主：签名与密文都合法，只是明文尾部 appid 属于「另一家小店」。
        var foreignAppId = "wxSelfTestForeignShop";
        var foreignTimestamp = Now();
        var foreignNonce = Guid.NewGuid().ToString("N");
        var foreignEncrypt = WechatCallbackCrypto.Encrypt(
            aesKey, BuildPlainJson(WiredEvent, foreignAppId, productId), foreignAppId);
        var foreignSignature = WechatCallbackCrypto.ComputeSignature(token, foreignTimestamp, foreignNonce, foreignEncrypt);
        results.Add(await PostAsync(
            url,
            $"msg_signature={Uri.EscapeDataString(foreignSignature)}&timestamp={foreignTimestamp}&nonce={foreignNonce}",
            BuildEnvelope(foreignEncrypt), "receiveid 非本小店 AppId（期望 403，跨属主拒收）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        // 超窗报文：签名按超窗 timestamp 计算 ⇒ 验签通过、时效闸拒绝（证明两道闸是独立的）。
        var staleTimestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString(CultureInfo.InvariantCulture);
        var staleNonce = Guid.NewGuid().ToString("N");
        var staleEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildPlainJson(WiredEvent, appId, productId), appId);
        var staleSignature = WechatCallbackCrypto.ComputeSignature(token, staleTimestamp, staleNonce, staleEncrypt);
        results.Add(await PostAsync(
            url,
            $"msg_signature={Uri.EscapeDataString(staleSignature)}&timestamp={staleTimestamp}&nonce={staleNonce}",
            BuildEnvelope(staleEncrypt), "timestamp 超出 ±300s 窗口（期望 403，验签通过仍拒）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        results.Add(await PostAsync(
            url, $"timestamp={timestamp}&nonce={nonce}", body, "缺少 msg_signature 参数（期望 403）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        results.Add(await PostAsync(
            url, query, "{\"ToUserName\":\"gh_x\"}", "体缺 Encrypt 字段（期望 403，JSON 侧的「报文非法」）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        // 未接线事件：应答仍是 200 success（官方语义 = 已收到），差别只在台账标记。
        var unwiredId = ProductIdPrefix + "unwired";
        var unwiredTimestamp = Now();
        var unwiredNonce = Guid.NewGuid().ToString("N");
        var unwiredEncrypt = WechatCallbackCrypto.Encrypt(
            aesKey, BuildPlainJson(UnwiredEvent, appId, unwiredId), appId);
        var unwiredSignature = WechatCallbackCrypto.ComputeSignature(token, unwiredTimestamp, unwiredNonce, unwiredEncrypt);
        results.Add(await PostAsync(
            url,
            $"msg_signature={Uri.EscapeDataString(unwiredSignature)}&timestamp={unwiredTimestamp}&nonce={unwiredNonce}",
            BuildEnvelope(unwiredEncrypt), $"POST 未接线事件 {UnwiredEvent}（期望 200 + success：无人处理 ≠ 拒绝）",
            HttpStatusCode.OK, "success", null).ConfigureAwait(false));
        results.Add(await LedgerContainsAsync(
            options.BaseUrl, FallbackMarker,
            "台账含兜底记录（未接线事件被兜底记录器记账，而非被静默丢弃）").ConfigureAwait(false));

        results.Add(await PostAsync(
            url, query, body, "Content-Type 非 JSON（期望 415，不进接收链）",
            HttpStatusCode.UnsupportedMediaType, null, "application/xml").ConfigureAwait(false));

        results.Add(await MethodAsync(url, HttpMethod.Put, "PUT 方法（期望 405）", HttpStatusCode.MethodNotAllowed)
            .ConfigureAwait(false));

        results.Add(await PostAsync(
            new Uri(options.BaseUrl, "/channels/not-configured"), query, body,
            "路由 AppKey 未配置回调凭据（期望 403，UnknownReceiver）",
            HttpStatusCode.Forbidden, null, null).ConfigureAwait(false));

        results.Add(await HealthAsync(options.BaseUrl, options.HealthFragment).ConfigureAwait(false));
        return results;
    }

    /// <summary>
    /// GET URL 验证：期望 200 且响应体<b>逐字等于 echostr 密文原文</b>（与公众号同协议，不解密）。
    /// 小店 GET 用<b>三参</b> signature（token + timestamp + nonce，不含 echostr）。
    /// </summary>
    private static async Task<SelfTestAssertion> EchoAsync(Uri url, string token, string aesKey, string appId)
    {
        var echoPlain = Guid.NewGuid().ToString("N");
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, echoPlain, appId);
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
                    ? "实际 200，响应体与 echostr 密文逐字一致（GET 不消费指纹，重复保存配置仍成功）。"
                    : $"实际 {(int)response.StatusCode}，响应体「{Truncate(text)}」（期望 echostr 密文原文 {Truncate(echoEncrypt)}）。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(EchoAsync), url, ex);
        }
    }

    /// <summary>GET 拒绝态：<paramref name="tamperSignature"/> 决定打的是「签名」还是「时效」这一道闸。</summary>
    private static async Task<SelfTestAssertion> EchoRejectedAsync(
        Uri url, string token, string aesKey, string appId, bool tamperSignature)
    {
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, Guid.NewGuid().ToString("N"), appId);
        var timestamp = tamperSignature
            ? Now()
            : (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString(CultureInfo.InvariantCulture);
        var nonce = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce);
        if (tamperSignature)
        {
            signature = FlipFirst(signature);
        }

        var target = new Uri(
            url,
            $"?signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}" +
            $"&echostr={Uri.EscapeDataString(echoEncrypt)}");
        var name = tamperSignature
            ? "GET 篡改 signature（期望 403 空体）"
            : "GET timestamp 超出时效窗口（期望 403 空体）";

        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.Forbidden && text.Length == 0;

            return new SelfTestAssertion(
                name,
                passed,
                passed
                    ? "实际 403 且响应体为空（拒绝不给「验签是否通过」的探测面）。"
                    : $"实际 {(int)response.StatusCode}，响应体「{Truncate(text)}」（期望 403 + 空体）。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, target, ex);
        }
    }

    private static async Task<SelfTestAssertion> PostAsync(
        Uri url,
        string query,
        string body,
        string name,
        HttpStatusCode expected,
        string? expectedBody,
        string? contentType)
    {
        var target = new Uri(url, "?" + query);
        try
        {
            using var content = new StringContent(body, Encoding.UTF8, contentType ?? "application/json");
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

    /// <summary>非 POST/GET 方法：期望 405（中间件在验签之前就拒，故不需要合法报文）。</summary>
    private static async Task<SelfTestAssertion> MethodAsync(
        Uri url, HttpMethod method, string name, HttpStatusCode expected)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            };
            using var response = await Http.SendAsync(request).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == expected;

            return new SelfTestAssertion(
                name,
                passed,
                $"实际 {(int)response.StatusCode}（期望 {(int)expected}），响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, url, ex);
        }
    }

    /// <summary>读 Demo 台账端点，断言其响应体包含指定片段（台账只含非敏感字段，可安全回显）。</summary>
    private static async Task<SelfTestAssertion> LedgerContainsAsync(Uri baseUrl, string fragment, string name)
    {
        var target = new Uri(baseUrl, "/api/callback-events?take=200");
        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK && text.Contains(fragment, StringComparison.Ordinal);

            return new SelfTestAssertion(
                name,
                passed,
                passed
                    ? $"台账命中片段「{Truncate(fragment)}」。"
                    : $"实际 {(int)response.StatusCode}，台账未含片段「{Truncate(fragment)}」，内容「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, target, ex);
        }
    }

    /// <summary>健康检查：确认探针打的是小店 Demo（防止端口撞车时把别的 Demo 当成本线结果）。</summary>
    private static async Task<SelfTestAssertion> HealthAsync(Uri baseUrl, string? healthFragment)
    {
        // 多线共存宿主的 /healthz 标识是宿主名而非线名 ⇒ 由 --health-fragment 显式给出，默认仍是本线专属 Demo。
        var expected = healthFragment ?? "channels";
        var target = new Uri(baseUrl, "/healthz");
        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK
                         && text.Contains($"\"{expected}\"", StringComparison.Ordinal);

            return new SelfTestAssertion(
                $"GET /healthz 标识为 {expected} 宿主",
                passed,
                passed ? "健康检查确认目标宿主与探针目标一致。" : $"实际 {(int)response.StatusCode}，内容「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(HealthAsync), target, ex);
        }
    }

    /// <summary>
    /// 解密明文 JSON：字段名照官方原文（顶层 <c>PascalCase</c>、事件主体内 <c>snake_case</c>）。
    /// <c>MsgType</c> 恒为 <c>event</c>；缺 <c>Event</c> 的明文在接收器即拒收。
    /// </summary>
    private static string BuildPlainJson(string eventKey, string appId, string productId)
        => "{\"ToUserName\":\"" + appId + "\""
           + ",\"FromUserName\":\"gh_selftest\""
           + ",\"CreateTime\":" + Now() +
           ",\"MsgType\":\"event\""
           + ",\"Event\":\"" + eventKey + "\""
           + ",\"ProductSpuUpdate\":{\"product_id\":\"" + productId + "\",\"status\":0,\"product_type\":1}"
           + "}";

    /// <summary>推送信封（官方 JSON 形态，非 XML）：只有 <c>Encrypt</c> 参与验签。</summary>
    private static string BuildEnvelope(string encrypt)
        => "{\"ToUserName\":\"gh_selftest\",\"Encrypt\":\"" + encrypt + "\"}";

    /// <summary>翻转签名首字符（保持长度与十六进制形态，只让比对失败）。</summary>
    private static string TamperSignature(string query)
    {
        const string key = "msg_signature=";
        var start = query.IndexOf(key, StringComparison.Ordinal);
        if (start < 0)
        {
            return query;
        }

        var first = query[start + key.Length];
        var flipped = first == '0' ? '1' : '0';
        return $"{query[..(start + key.Length)]}{flipped}{query[(start + key.Length + 1)..]}";
    }

    private static string FlipFirst(string value)
    {
        if (value.Length == 0)
        {
            return value;
        }

        var first = value[0];
        return (first == '0' ? '1' : '0') + value.Substring(1);
    }

    private static string Now()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

    private static SelfTestAssertion ConnectionFailure(string name, Uri url, Exception ex)
        => new(
            name,
            false,
            $"请求 {url} 失败：{ex.GetType().Name} {ex.Message}。请确认小店 Demo 已启动（默认 http://localhost:56562）。");

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
