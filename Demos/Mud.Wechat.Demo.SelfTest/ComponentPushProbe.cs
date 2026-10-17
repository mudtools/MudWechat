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
/// 开放平台「授权事件接收 URL」自测：票据、篡改签名、时间戳越界、跨平台重放、授权/取消授权事件、未知类型六类处置。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号 / 企微两通道形态不同的三点</b>：
/// ① 本线没有回调中间件，Demo 把原始请求体直接转交接收器 ⇒ 信封只需 <c>Encrypt</c>（企微的
/// <c>ToUserName/AgentID</c>、公众号的 <c>MsgType</c> 分支在这里都不存在）；
/// ② 失败<b>不回 4xx</b>：官方契约只区分「回 success / 不回 success」，Demo 以响应体
/// <c>success</c> / <c>fail</c> 表达处置分类，故本探针断言的是<b>响应体</b>而非状态码；
/// ③ <c>receiveid</c> 恒为<b>第三方平台 appid</b>（不是授权方 appid、也不是 CorpId）⇒ 拿授权方
/// appid 加密的报文应被判 <c>AppIdMismatch</c>，这正是防跨平台重放的那道闸。
/// </para>
/// <para>
/// <b>元素名照录官方原文、勿「规范」拼写</b>：授权方字段是 <c>AuthorizerAppid</c>（结尾小写 d），
/// 与 JSON 接口的 <c>authorizer_appid</c> 不同批拼写。这里用字面量而非 SDK 常量，
/// 是本工具的既有纪律：SelfTest 只依赖叶层密码学内核，不引用任何产品线主包。
/// </para>
/// </remarks>
internal static class ComponentPushProbe
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    public static async Task<IReadOnlyList<SelfTestAssertion>> RunAsync(SelfTestOptions options)
    {
        string token;
        string aesKey;
        string componentAppId;
        try
        {
            var env = DemoEnv.RequireAll(
                "OPENPLATFORM_TOKEN", "OPENPLATFORM_AES_KEY", "OPENPLATFORM_COMPONENT_APP_ID");
            token = env["OPENPLATFORM_TOKEN"];
            aesKey = env["OPENPLATFORM_AES_KEY"];
            componentAppId = env["OPENPLATFORM_COMPONENT_APP_ID"];
        }
        catch (InvalidOperationException ex)
        {
            return new[] { new SelfTestAssertion("推送凭据读取", false, ex.Message) };
        }

        if (aesKey.Length != 43)
        {
            return new[]
            {
                new SelfTestAssertion(
                    "EncodingAESKey 形态", false,
                    $"长度为 {aesKey.Length}，官方要求 43 位 Base64 字符（Demo 启动期同样 fail-fast）。"),
            };
        }

        var url = new Uri(options.BaseUrl, options.Route);
        var results = new List<SelfTestAssertion>();

        // ① 合法票据推送：期望响应体逐字 "success"，且票据真的落进 Demo 的存储（下一断言查）。
        var ticket = $"ticket@@@selftest{Guid.NewGuid():N}";
        results.Add(await PushAsync(
            url, token, aesKey, componentAppId,
            BuildTicketPlainXml(componentAppId, ticket), "合法票据推送（期望 success）"));

        results.Add(await TicketStoredAsync(options.BaseUrl, ticket).ConfigureAwait(false));

        // ② 篡改签名：验签失败即返回，绝不解密、绝不写存储 ⇒ 回非 success 让微信重试。
        var ticketEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTicketPlainXml(componentAppId, ticket), componentAppId);
        results.Add(await PushWithQueryAsync(
            url,
            TamperSignature(BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), ticketEncrypt)),
            BuildEnvelope(ticketEncrypt),
            "篡改 msg_signature（期望 fail）",
            "fail").ConfigureAwait(false));

        // ③ 时间戳越界（签名按越界值算，故验签是<b>通过</b>的 —— 只有时效闸能拒它）：期望 fail。
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString(CultureInfo.InvariantCulture);
        var staleEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTicketPlainXml(componentAppId, ticket), componentAppId);
        results.Add(await PushWithQueryAsync(
            url,
            BuildQuery(token, stale, Guid.NewGuid().ToString("N"), staleEncrypt),
            BuildEnvelope(staleEncrypt),
            "时间戳越界 600 秒（期望 fail，验签通过但时效窗拒绝）",
            "fail").ConfigureAwait(false));

        // ④ receiveid 用别的 appid：验签与解密都过，appid 比对必须拦住（跨平台重放）。
        var foreignId = "wx" + new string('f', Math.Max(0, componentAppId.Length - 2));
        var foreignEncrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildTicketPlainXml(foreignId, ticket), foreignId);
        results.Add(await PushWithQueryAsync(
            url,
            BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), foreignEncrypt),
            BuildEnvelope(foreignEncrypt),
            "receiveid 非本平台 appid（期望 fail）",
            "fail").ConfigureAwait(false));

        // ⑤ 授权事件：带授权码 ⇒ 回 success 且 Demo 台账出现「未消费授权码」（原文全程不回显）。
        var authorizerAppId = "wx" + new string('a', Math.Max(0, componentAppId.Length - 2));
        var authorizedPlain = "<xml>"
            + Cdata("AppId", componentAppId)
            + Cdata("CreateTime", Now())
            + Cdata("InfoType", "authorized")
            + Cdata("AuthorizerAppid", authorizerAppId)
            + Cdata("AuthorizationCode", $"selftest-code-{Guid.NewGuid():N}")
            + Cdata("AuthorizationCodeExpiredTime", (DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10)).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
            + Cdata("PreAuthCode", "selftest-preauth")
            + "</xml>";
        var authorizedEncrypt = WechatCallbackCrypto.Encrypt(aesKey, authorizedPlain, componentAppId);
        var authorizedQuery = BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), authorizedEncrypt);
        var authorizedEnvelope = BuildEnvelope(authorizedEncrypt);
        results.Add(await PushWithQueryAsync(
            url, authorizedQuery, authorizedEnvelope, "authorized 事件（期望 success）", "success")
            .ConfigureAwait(false));
        results.Add(await PendingAuthCodeAsync(options.BaseUrl, true, "authorized 事件后台账有待消费授权码").ConfigureAwait(false));

        // ⑤.1 消费一次（换取端点把「取出」当作「消费」，与官方的一次性语义一致；
        //       假凭据下这一步必然 502 ⇒ 只断言「走到了换取路径」，不判官方结果）。
        results.Add(await ConsumeAsync(options.BaseUrl).ConfigureAwait(false));

        // ⑤.2 微信「没收到 success 就重试」= 同一条报文再推一次。已消费的码**不得**重新入台账，
        //       否则下一次换取会凭空撞 40029，而 Demo 看起来什么都没做错。
        results.Add(await PushWithQueryAsync(
            url, authorizedQuery, authorizedEnvelope, "同一 authorized 报文重放（仍回 success）", "success")
            .ConfigureAwait(false));
        results.Add(await PendingAuthCodeAsync(options.BaseUrl, false, "重放不会把已消费的授权码装回台账").ConfigureAwait(false));

        // ⑥ 取消授权同一 appid：官方此时不带授权码 ⇒ Demo 必须清掉那条旧码，
        //    否则演示者会拿一个官方已作废的码去换令牌（表现为 40029，却找不到原因）。
        //    先用一条**新码**重新装满台账，这条断言才是真的在测「清」而不是撞上一段的空台账。
        var rearmCode = Guid.NewGuid().ToString("N");
        var rearmedXml = "<xml>"
            + Cdata("AppId", componentAppId)
            + Cdata("CreateTime", Now())
            + Cdata("InfoType", "authorized")
            + Cdata("AuthorizerAppid", authorizerAppId)
            + Cdata("AuthorizationCode", $"selftest-code-{rearmCode}")
            + Cdata("AuthorizationCodeExpiredTime", (DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10)).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
            + Cdata("PreAuthCode", "selftest-preauth-rearm")
            + "</xml>";
        var rearmedEncrypt = WechatCallbackCrypto.Encrypt(aesKey, rearmedXml, componentAppId);
        results.Add(await PushWithQueryAsync(
            url,
            BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), rearmedEncrypt),
            BuildEnvelope(rearmedEncrypt),
            "新的 authorized 事件（重装载台账，期望 success）",
            "success").ConfigureAwait(false));
        results.Add(await PendingAuthCodeAsync(options.BaseUrl, true, "重装载后台账再次有待消费授权码").ConfigureAwait(false));

        var unauthorizedPlain = "<xml>"
            + Cdata("AppId", componentAppId)
            + Cdata("CreateTime", Now())
            + Cdata("InfoType", "unauthorized")
            + Cdata("AuthorizerAppid", authorizerAppId)
            + "</xml>";
        var unauthorizedEncrypt = WechatCallbackCrypto.Encrypt(aesKey, unauthorizedPlain, componentAppId);
        results.Add(await PushWithQueryAsync(
            url,
            BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), unauthorizedEncrypt),
            BuildEnvelope(unauthorizedEncrypt),
            "unauthorized 事件（期望 success）",
            "success").ConfigureAwait(false));
        results.Add(await PendingAuthCodeAsync(options.BaseUrl, false, "取消授权后台账不再留旧授权码").ConfigureAwait(false));

        // ⑦ 未知 InfoType：本线未建模 ⇒ 刻意<b>不</b>回 success（让丢失在监控上可见）。
        var unknownPlain = "<xml>"
            + Cdata("AppId", componentAppId)
            + Cdata("CreateTime", Now())
            + Cdata("InfoType", "selftest_unknown_info_type")
            + "</xml>";
        var unknownEncrypt = WechatCallbackCrypto.Encrypt(aesKey, unknownPlain, componentAppId);
        results.Add(await PushWithQueryAsync(
            url,
            BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), unknownEncrypt),
            BuildEnvelope(unknownEncrypt),
            "未知 InfoType（期望 fail，不得静默 success）",
            "fail").ConfigureAwait(false));

        return results;
    }

    /// <summary>构造合法四参签名并 POST 一次票据推送。</summary>
    private static async Task<SelfTestAssertion> PushAsync(
        Uri url, string token, string aesKey, string receiveId, string plainXml, string name)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(aesKey, plainXml, receiveId);
        return await PushWithQueryAsync(
            url, BuildQuery(token, Now(), Guid.NewGuid().ToString("N"), encrypt), BuildEnvelope(encrypt), name, "success")
            .ConfigureAwait(false);
    }

    /// <summary>POST 一条已组装好的报文，断言响应体等于期望值（本线以响应体表达处置，状态码恒 200）。</summary>
    private static async Task<SelfTestAssertion> PushWithQueryAsync(
        Uri url, string query, string body, string name, string expectedBody)
    {
        var target = new Uri(url, "?" + query);
        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/xml");
            using var response = await Http.PostAsync(target, content).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK
                         && string.Equals(text, expectedBody, StringComparison.Ordinal);

            return new SelfTestAssertion(
                name,
                passed,
                $"实际 {(int)response.StatusCode} / 响应体「{Truncate(text)}」，期望 200 /「{expectedBody}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, target, ex);
        }
    }

    /// <summary>读 Demo 的票据快照端点：证明票据确实<b>落库</b>而不只是回了 success。</summary>
    private static async Task<SelfTestAssertion> TicketStoredAsync(Uri baseUrl, string pushedTicket)
    {
        var target = new Uri(baseUrl, "/api/ticket");
        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);

            // /api/ticket 只回长度与指纹（票据原文不回显），故这里按「hasTicket=true」判定；
            // pushedTicket 用于失败时提示实际推送值（本工具的自测值，非真实凭据）。
            var passed = response.StatusCode == HttpStatusCode.OK
                         && text.Contains("\"hasTicket\":true", StringComparison.Ordinal);

            return new SelfTestAssertion(
                "票据落库可查（GET /api/ticket 回 hasTicket=true）",
                passed,
                passed
                    ? $"实际 {Truncate(text)}；推送值 {Truncate(pushedTicket)}（仅自测构造值）。"
                    : $"实际 {(int)response.StatusCode}，响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure("票据落库可查（GET /api/ticket 回 hasTicket=true）", target, ex);
        }
    }

    /// <summary>
    /// 触发一次「消费台账里那条授权码」：只断言请求走到了换取路径（200 或 502），
    /// 假凭据下官方必然拒绝 ⇒ 官方结果不在本探针的断言面内，本探针只验 Demo 的台账语义。
    /// </summary>
    private static async Task<SelfTestAssertion> ConsumeAsync(Uri baseUrl)
    {
        var target = new Uri(baseUrl, "/api/auth/exchange-latest");
        try
        {
            using var response = await Http.PostAsync(target, new StringContent("")).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode is HttpStatusCode.OK or HttpStatusCode.BadGateway;

            return new SelfTestAssertion(
                "消费授权码（POST /api/auth/exchange-latest 走到换取路径）",
                passed,
                $"实际 {(int)response.StatusCode}（期望 200 或 502），响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure("消费授权码（POST /api/auth/exchange-latest 走到换取路径）", target, ex);
        }
    }

    /// <summary>读 Demo 台账的「是否存在未消费授权码」标志（同样不回显原文）。</summary>
    private static async Task<SelfTestAssertion> PendingAuthCodeAsync(Uri baseUrl, bool expected, string name)
    {
        var target = new Uri(baseUrl, "/api/push-events");
        var expectedText = expected ? "true" : "false";
        try
        {
            using var response = await Http.GetAsync(target).ConfigureAwait(false);
            var text = await SafeReadAsync(response).ConfigureAwait(false);
            var passed = response.StatusCode == HttpStatusCode.OK
                         && text.Contains($"\"pendingAuthCodeAvailable\":{expectedText}", StringComparison.Ordinal);

            return new SelfTestAssertion(
                name,
                passed,
                $"实际 {(int)response.StatusCode}，期望 pendingAuthCodeAvailable={expectedText}，响应体「{Truncate(text)}」。");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(name, target, ex);
        }
    }

    private static string BuildEnvelope(string encrypt)
        => $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

    private static string BuildTicketPlainXml(string appId, string ticket)
        => "<xml>"
           + Cdata("AppId", appId)
           + Cdata("CreateTime", Now())
           + Cdata("InfoType", "component_verify_ticket")
           + Cdata("ComponentVerifyTicket", ticket)
           + "</xml>";

    private static string Cdata(string element, string value)
        => $"<{element}><![CDATA[{value}]]></{element}>";

    /// <summary>四参签名（<c>Encrypt</c> 为参与项）—— 与公众号/企微侧同一份内核，参数名却是本线官方的 <c>msg_signature</c>。</summary>
    private static string BuildQuery(string token, string timestamp, string nonce, string encrypt)
    {
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        return $"msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}";
    }

    private static string Now()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

    /// <summary>把 <c>msg_signature</c> 的首字符改掉（十六进制签名改一位必不匹配，等价于「报文被篡改」）。</summary>
    private static string TamperSignature(string query)
    {
        const string Key = "msg_signature=";
        var start = query.IndexOf(Key, StringComparison.Ordinal) + Key.Length;
        var first = query[start] == '0' ? '1' : '0';
        return $"{query[..start]}{first}{query[(start + 1)..]}";
    }

    private static SelfTestAssertion ConnectionFailure(string name, Uri url, Exception ex)
        => new(
            name,
            false,
            $"请求 {url} 失败：{ex.GetType().Name} {ex.Message}。请确认开放平台 Demo 已启动、地址与路由正确。");

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
