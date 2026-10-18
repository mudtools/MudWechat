// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Demo.Shared;

namespace Mud.Wechat.Demo.SelfTest;

/// <summary>
/// 智能机器人 JSON 回调自测：GET 回显、POST 三类事件的应答形态、应答外壳契约、四道闸门、
/// 形态闸（415/405/400）、通道互斥与台账共二十三断言。
/// </summary>
/// <remarks>
/// <para>
/// <b>与前四条线的回调最不一样的一处</b>：机器人通道是<b>返回式</b>处理器 —— 应答不是明文 <c>success</c>
/// 而是<b>加密 JSON 外壳</b> <c>{encrypt, msgsignature, timestamp, nonce}</c>，且明文本身可能是
/// 「欢迎语消息」也可能是「空包 <c>{}</c>」。因此本探针必须把应答<b>解密回来</b>断言，
/// 只看 200 完全无法区分「回了消息」与「回了空包」。
/// </para>
/// <para>
/// <b>四处与 XML 侧的形态差异</b>（都落在断言面上）：
/// ① GET 走的是 XML 接收器（官方 101033 与 90968 同协议）⇒ 回显<b>解密明文</b>而非 echostr 原文；
/// ② 明文 <c>receiveid</c> 恒为<b>空串</b>（官方 101033）⇒ 不存在「跨属主」这一道闸，
///    取而代之的是「receiveid 非空即拒」（本探针反向构造）；
/// ③ 报文判别子只有顶层 <c>msgtype</c>（<c>event</c> = 事件族，其余 = 消息族），无 <c>Encrypt</c> 大写字母形态；
/// ④ 未命中处理器 / 软超时都回 <c>200</c> + 空包（不是 XML 侧的 503），差别只在 Demo 台账。
/// </para>
/// <para>
/// <b>报文由 SDK 自身的密码学内核构造</b>（<see cref="WechatCallbackCrypto"/>，Core 叶层），
/// 明文 JSON 的字段名照官方原文写字面量（<c>msgid</c> / <c>chattype</c> / <c>eventtype</c>）——
/// 探针与 DTO 各自独立，才测得出契约漂移。
/// </para>
/// </remarks>
internal static class BotCallbackProbe
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    /// <summary>Demo 已接线的消息族键（<c>WechatBotAgentHandler</c> + <c>TextEventObserver</c>）。</summary>
    private const string TextMsg = "text";

    /// <summary>Demo 已接线的事件族键（<c>EnterChatWelcomeHandler</c>，官方唯一允许被动回复 text 的事件）。</summary>
    private const string EnterChat = "enter_chat";

    /// <summary>Demo 未精确接线的事件族键（落兜底处理器；官方约束「仅支持回复空包」）。</summary>
    private const string FeedbackEvent = "feedback_event";

    /// <summary>兜底处理器写进台账的应答形态标记（与 <c>RecordOnlyFallbackHandler</c> 文案一致）。</summary>
    private const string FallbackMarker = "空包 {}";

    public static async Task<IReadOnlyList<SelfTestAssertion>> RunAsync(SelfTestOptions options)
    {
        string token;
        string aesKey;
        try
        {
            var env = DemoEnv.RequireAll("WORK_BOT_PUSH_TOKEN", "WORK_BOT_PUSH_AES_KEY");
            token = env["WORK_BOT_PUSH_TOKEN"];
            aesKey = env["WORK_BOT_PUSH_AES_KEY"];
        }
        catch (InvalidOperationException ex)
        {
            return new[] { new SelfTestAssertion("机器人回调凭据读取", false, ex.Message) };
        }

        if (aesKey.Length != 43)
        {
            return new[]
            {
                new SelfTestAssertion(
                    "EncodingAESKey 形态", false,
                    $"长度为 {aesKey.Length}，官方要求 43 位 Base64 字符（机器人通道强制安全模式）。"),
            };
        }

        var url = new Uri(options.BaseUrl, options.Route);
        var results = new List<SelfTestAssertion>
        {
            await EchoAsync(url, token, aesKey).ConfigureAwait(false),
            await EchoRejectedAsync(url, token, aesKey, tamperSignature: true).ConfigureAwait(false),
            await EchoRejectedAsync(url, token, aesKey, tamperSignature: false).ConfigureAwait(false),
        };

        // —— 三个合法事件：一次生成各自的 timestamp/nonce/密文，指纹互不干扰 ——
        var textId = NewMsgId(TextMsg);
        var (textQuery, textBody) = BuildPackage(token, aesKey, BuildTextPlain(textId));
        results.Add(await PostForReplyAsync(
            url, textQuery, textBody, aesKey,
            $"POST {TextMsg} 消息（期望 200 + 加密空包：回合排后台，HTTP 侧不阻塞）",
            expectPlain: "{}", requireEnvelope: true).ConfigureAwait(false));
        results.Add(await LedgerContainsAsync(options.BaseUrl, textId,
            $"台账出现本次 text 事件的 msgid（证明处理器真的被调用，而非只过了闸门）（{textId}）").ConfigureAwait(false));

        var enterId = NewMsgId(EnterChat);
        var (enterQuery, enterBody) = BuildPackage(token, aesKey, BuildEventPlain(EnterChat, enterId));
        results.Add(await PostForReplyAsync(
            url, enterQuery, enterBody, aesKey,
            $"POST {EnterChat}（期望 200 + 应答明文是 text 消息：官方唯一允许被动回复 text 的事件）",
            expectPlain: null, expectContains: "\"msgtype\":\"text\"").ConfigureAwait(false));
        results.Add(await LedgerContainsAsync(options.BaseUrl, EnterChat,
            "台账含 enter_chat 记录（欢迎语处理器接住了事件）").ConfigureAwait(false));

        var feedbackId = NewMsgId(FeedbackEvent);
        var (feedbackQuery, feedbackBody) = BuildPackage(token, aesKey, BuildEventPlain(FeedbackEvent, feedbackId));
        results.Add(await PostForReplyAsync(
            url, feedbackQuery, feedbackBody, aesKey,
            $"POST {FeedbackEvent}（期望 200 + 加密空包：官方 101027 该事件仅支持回复空包）",
            expectPlain: "{}", requireEnvelope: true).ConfigureAwait(false));
        results.Add(await LedgerContainsAsync(options.BaseUrl, FallbackMarker,
            "台账含兜底记录（未精确接线的事件被兜底处理器记账，而非静默丢弃）").ConfigureAwait(false));

        // —— 四道闸门 ——
        results.Add(await PostAsync(url, textQuery, textBody, "同一报文重放（期望 403，指纹闸 fail-closed）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));
        results.Add(await PostAsync(url, TamperSignature(textQuery), textBody, "篡改 msg_signature（期望 403）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        var staleTimestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString(CultureInfo.InvariantCulture);
        var staleNonce = Guid.NewGuid().ToString("N");
        var staleEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTextPlain(NewMsgId(TextMsg)), string.Empty);
        var staleSignature = WechatCallbackCrypto.ComputeSignature(token, staleTimestamp, staleNonce, staleEncrypt);
        results.Add(await PostAsync(
            url,
            $"msg_signature={Uri.EscapeDataString(staleSignature)}&timestamp={staleTimestamp}&nonce={staleNonce}",
            BuildEnvelope(staleEncrypt),
            "timestamp 超出 ±300s 窗口（期望 403，验签通过仍拒）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        // 缺签名：合法报文只剥掉 msg_signature 参数（其余三项仍成套 ⇒ 打的是「参数存在性」这一道闸）。
        var missingSignature = BuildPackage(token, aesKey, BuildTextPlain(NewMsgId(TextMsg)));
        results.Add(await PostAsync(
            url, RemoveSignature(missingSignature.Query), missingSignature.Body,
            "缺少 msg_signature 参数（期望 403）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        var noEncryptId = NewMsgId(TextMsg);
        var (noEncryptQuery, _) = BuildPackage(token, aesKey, BuildTextPlain(noEncryptId));
        results.Add(await PostAsync(url, noEncryptQuery, "{\"msgid\":\"x\"}",
            "体缺 encrypt 字段（期望 403，JSON 侧的「报文非法」）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        // receiveid 非空：签名与密文都合法，只是明文尾部写了「另一个属主」——Bot 通道据此拒收。
        var foreignTimestamp = Now();
        var foreignNonce = Guid.NewGuid().ToString("N");
        var foreignEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTextPlain(NewMsgId(TextMsg)), "wxForeignOwner");
        var foreignSignature = WechatCallbackCrypto.ComputeSignature(token, foreignTimestamp, foreignNonce, foreignEncrypt);
        results.Add(await PostAsync(
            url,
            $"msg_signature={Uri.EscapeDataString(foreignSignature)}&timestamp={foreignTimestamp}&nonce={foreignNonce}",
            BuildEnvelope(foreignEncrypt),
            "明文 receiveid 非空（期望 403，官方 101033 机器人场景恒为空串）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        // 明文可解但非官方报文（缺 msgid）：接收器硬拒（官方 101463 以 msgid 排重）。
        var (noMsgIdQuery, noMsgIdBody) = BuildPackage(token, aesKey, "{\"msgtype\":\"text\",\"text\":{\"content\":\"hi\"}}");
        results.Add(await PostAsync(url, noMsgIdQuery, noMsgIdBody,
            "明文缺 msgid（期望 403，非可识别的官方报文）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        // —— 形态闸与互斥 ——
        results.Add(await PostAsync(url, textQuery, textBody,
            "Content-Type 既非 xml 也非 json（期望 415，不进接收链）",
            HttpStatusCode.UnsupportedMediaType, null, "text/plain").ConfigureAwait(false));

        results.Add(await MethodAsync(url, HttpMethod.Put, "PUT 方法（期望 405）", HttpStatusCode.MethodNotAllowed)
            .ConfigureAwait(false));

        results.Add(await PostAsync(url, textQuery, string.Empty,
            "空请求体 + JSON Content-Type（期望 400，机器人侧的空体单独成态）",
            HttpStatusCode.BadRequest, null).ConfigureAwait(false));

        // 同一 Apps 字典里的 Channel=App 条目（通配键）：JSON 报文只能由 Bot 条目接住。
        results.Add(await PostAsync(
            new Uri(options.BaseUrl, "/wechat"), textQuery, textBody,
            "POST JSON 到通配键路由 /wechat（Channel=App，期望 403：两条通道报文形态互斥）",
            HttpStatusCode.Forbidden, null).ConfigureAwait(false));

        results.Add(await HealthAsync(options.BaseUrl, options.HealthFragment).ConfigureAwait(false));
        return results;
    }

    /// <summary>
    /// GET URL 验证：期望 200 且响应体等于<b>解密后的明文</b>。
    /// 机器人 GET 复用 XML 接收器（<c>msg_signature</c> 四参含 echostr），
    /// 与小店/公众号那两条「回显 echostr 密文原文」的线不同 —— 回显形态照官方 90930，探针据实断言。
    /// </summary>
    private static async Task<SelfTestAssertion> EchoAsync(Uri url, string token, string aesKey)
    {
        var echoPlain = Guid.NewGuid().ToString("N");
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, echoPlain, string.Empty);
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, echoEncrypt);

        var target = new Uri(
            url,
            $"?msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}" +
            $"&echostr={Uri.EscapeDataString(echoEncrypt)}");

        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK
                         && string.Equals(text, echoPlain, StringComparison.Ordinal);

            return new SelfTestAssertion(
                "GET URL 验证回显 echostr 的解密明文（四参 msg_signature）",
                passed,
                passed
                    ? "实际 200，响应体等于 echostr 解密明文（机器人 GET 复用 XML 接收器）。"
                    : $"实际 {(int)response.StatusCode}，响应体「{Truncate(text)}」（期望解密明文 {Truncate(echoPlain)}）。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(EchoAsync), target, ex);
        }
    }

    /// <summary>GET 拒绝态：<paramref name="tamperSignature"/> 决定打的是「签名」还是「时效」这一道闸。</summary>
    private static async Task<SelfTestAssertion> EchoRejectedAsync(
        Uri url, string token, string aesKey, bool tamperSignature)
    {
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, Guid.NewGuid().ToString("N"), string.Empty);
        var timestamp = tamperSignature
            ? Now()
            : (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString(CultureInfo.InvariantCulture);
        var nonce = Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, echoEncrypt);
        if (tamperSignature)
        {
            signature = FlipFirst(signature);
        }

        var target = new Uri(
            url,
            $"?msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}" +
            $"&echostr={Uri.EscapeDataString(echoEncrypt)}");
        var name = tamperSignature
            ? "GET 篡改 msg_signature（期望 403 空体）"
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

    /// <summary>
    /// POST 并校验<b>应答明文</b>：解开应答外壳的 <c>encrypt</c> 后与期望比对，
    /// 顺带断言外壳契约（字段名 <c>msgsignature</c> 无下划线、<c>nonce</c> 复用回调 URL 值）。
    /// </summary>
    private static async Task<SelfTestAssertion> PostForReplyAsync(
        Uri url,
        string query,
        string body,
        string aesKey,
        string name,
        string? expectPlain,
        string? expectContains = null,
        bool requireEnvelope = false)
    {
        var target = new Uri(url, "?" + query);
        var requestNonce = NonceOf(query);

        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await Http.PostAsync(target, content).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new SelfTestAssertion(
                    name, false, $"实际 {(int)response.StatusCode}（期望 200），响应体「{Truncate(text)}」。");
            }

            string? envelopeEncrypt;
            string? msgsignature;
            string? envelopeNonce;
            try
            {
                using var document = JsonDocument.Parse(text);
                envelopeEncrypt = ReadString(document.RootElement, "encrypt");
                msgsignature = ReadString(document.RootElement, "msgsignature");
                envelopeNonce = ReadString(document.RootElement, "nonce");
            }
            catch (JsonException)
            {
                return new SelfTestAssertion(
                    name, false, $"应答不是 JSON 外壳（机器人通道不得回明文 success）：「{Truncate(text)}」。");
            }

            if (envelopeEncrypt == null)
            {
                return new SelfTestAssertion(name, false, $"应答外壳缺 encrypt：「{Truncate(text)}」。");
            }

            if (requireEnvelope && (msgsignature == null || envelopeNonce != requestNonce))
            {
                return new SelfTestAssertion(
                    name, false,
                    $"应答外壳契约不符：msgsignature={(msgsignature == null ? "缺失" : "存在")}、" +
                    $"nonce「{Truncate(envelopeNonce ?? string.Empty)}」（期望复用回调 URL 的 nonce）。");
            }

            var plain = WechatCallbackCrypto.Decrypt(aesKey, envelopeEncrypt, out _);
            var plainPassed = expectPlain != null
                ? string.Equals(plain, expectPlain, StringComparison.Ordinal)
                : plain.Contains(expectContains!, StringComparison.Ordinal);

            return new SelfTestAssertion(
                name,
                plainPassed,
                plainPassed
                    ? $"实际 200，应答解密明文「{Truncate(plain)}」。"
                    : $"应答解密明文「{Truncate(plain)}」（期望 {((expectPlain != null) ? "逐字等于 " + expectPlain : "包含 " + expectContains)})。");
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
        string? contentType = null)
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

            return new SelfTestAssertion(
                name,
                response.StatusCode == expected,
                $"实际 {(int)response.StatusCode}（期望 {(int)expected}），响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, url, ex);
        }
    }

    /// <summary>读 Demo 机器人台账端点，断言其响应体包含指定片段（台账只含非敏感字段，可安全回显）。</summary>
    private static async Task<SelfTestAssertion> LedgerContainsAsync(Uri baseUrl, string fragment, string name)
    {
        var target = new Uri(baseUrl, "/api/bot-events?take=200");
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

    /// <summary>健康检查：确认探针打的是机器人 Demo（防止端口撞车时把别的 Demo 当成本线结果）。</summary>
    private static async Task<SelfTestAssertion> HealthAsync(Uri baseUrl, string? healthFragment)
    {
        // 多线共存宿主的 /healthz 标识是宿主名而非 Demo 名 ⇒ 由 --health-fragment 显式给出，默认仍是 ai-agent Demo。
        var expected = healthFragment ?? "ai-agent";
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

    /// <summary>生成一组合法查询串 + 报文体（timestamp/nonce/密文一次成型，重放断言复用同一组）。</summary>
    private static (string Query, string Body) BuildPackage(string token, string aesKey, string plainJson)
    {
        var timestamp = Now();
        var nonce = Guid.NewGuid().ToString("N");
        var encrypt = WechatCallbackCrypto.Encrypt(aesKey, plainJson, string.Empty);
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        return ($"msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}",
                BuildEnvelope(encrypt));
    }

    /// <summary>
    /// 文本消息明文（字段名照官方 100719 原文，全小写）：<b>刻意不带 response_url</b> ——
    /// 机器人主动回复要有它才能发出去，缺它时 SDK 处理器只告警并回空包，
    /// 探针因此不会向官方地址发任何真实请求（离线自测的硬要求）。
    /// </summary>
    private static string BuildTextPlain(string msgId)
        => "{\"msgid\":\"" + msgId + "\""
           + ",\"aibotid\":\"aibotSelfTest\""
           + ",\"chattype\":\"single\""
           + ",\"from\":{\"corpid\":\"wwSelfTestCorp\",\"userid\":\"selfTestUser\"}"
           + ",\"msgtype\":\"text\""
           + ",\"text\":{\"content\":\"自测文本消息\"}"
           + "}";

    /// <summary>事件族明文：顶层 <c>msgtype=event</c> + <c>event.eventtype</c>（判别子即事件键来源）。</summary>
    private static string BuildEventPlain(string eventType, string msgId)
        => "{\"msgid\":\"" + msgId + "\""
           + ",\"create_time\":" + Now() +
           ",\"aibotid\":\"aibotSelfTest\""
           + ",\"chattype\":\"single\""
           + ",\"from\":{\"corpid\":\"wwSelfTestCorp\",\"userid\":\"selfTestUser\"}"
           + ",\"msgtype\":\"event\""
           + ",\"event\":{\"eventtype\":\"" + eventType + "\"}"
           + "}";

    /// <summary>请求信封（官方 JSON 形态）：只有 <c>encrypt</c> 参与验签。</summary>
    private static string BuildEnvelope(string encrypt) => "{\"encrypt\":\"" + encrypt + "\"}";

    private static string NewMsgId(string eventKey)
        => $"selftest-{eventKey}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds():x}-{Guid.NewGuid():N}";

    /// <summary>剥掉 <c>msg_signature</c> 参数（保留其余参数原样，用于「缺参数」闸门）。</summary>
    private static string RemoveSignature(string query)
    {
        const string key = "msg_signature=";
        var start = query.IndexOf(key, StringComparison.Ordinal);
        if (start < 0)
        {
            return query;
        }

        var end = query.IndexOf('&', start);
        return end < 0
            ? query.Substring(0, start).TrimEnd('&')
            : query.Substring(0, start) + query.Substring(end + 1);
    }

    private static string NonceOf(string query)
    {
        const string key = "nonce=";
        var start = query.IndexOf(key, StringComparison.Ordinal);
        if (start < 0)
        {
            return string.Empty;
        }

        var rest = query.Substring(start + key.Length);
        var amp = rest.IndexOf('&');
        return amp < 0 ? rest : rest.Substring(0, amp);
    }

    private static string? ReadString(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

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
            $"请求 {url} 失败：{ex.GetType().Name} {ex.Message}。请确认机器人 Demo 已以 callback 形态启动（默认 http://localhost:56564）。");

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
