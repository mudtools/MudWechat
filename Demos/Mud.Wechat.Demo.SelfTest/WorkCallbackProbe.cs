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
/// 企业微信 XML 回调自测：GET URL 验证、POST 事件、同报文重放、篡改签名四条断言。
/// </summary>
/// <remarks>
/// 报文由 <see cref="WechatCallbackCrypto"/>（SDK 自身的密码学内核）构造，
/// 因此自测通过即同时证明「Demo 侧配置 + SDK 接收链」两端一致；不引入任何独立实现的加解密代码。
/// </remarks>
internal static class WorkCallbackProbe
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    public static async Task<IReadOnlyList<SelfTestAssertion>> RunAsync(SelfTestOptions options)
    {
        string token;
        string aesKey;
        string corpId;
        try
        {
            var env = DemoEnv.RequireAll("WORK_CALLBACK_TOKEN", "WORK_CALLBACK_AES_KEY", "WORK_CORP_ID");
            token = env["WORK_CALLBACK_TOKEN"];
            aesKey = env["WORK_CALLBACK_AES_KEY"];
            corpId = env["WORK_CORP_ID"];
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
            await EchoAsync(url, token, aesKey, corpId).ConfigureAwait(false),
        };

        // POST 与重放共用同一组 timestamp/nonce/密文：指纹恒定，才能断言第二道闸 fail-closed。
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var nonce = Guid.NewGuid().ToString("N");
        var encrypt = WechatCallbackCrypto.Encrypt(aesKey, BuildCreateUserXml(corpId), corpId);
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encrypt);
        var query = $"msg_signature={Uri.EscapeDataString(signature)}&timestamp={timestamp}&nonce={nonce}";
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt><AgentID><![CDATA[1]]></AgentID></xml>";

        results.Add(await PostEventAsync(url, query, body).ConfigureAwait(false));
        results.Add(await ReplayAsync(url, query, body).ConfigureAwait(false));
        results.Add(await TamperedSignatureAsync(url, query, body).ConfigureAwait(false));
        return results;
    }

    /// <summary>GET URL 验证：期望 200 且回显体等于加密前的 echostr 明文。</summary>
    private static async Task<SelfTestAssertion> EchoAsync(Uri url, string token, string aesKey, string corpId)
    {
        var echoPlain = Guid.NewGuid().ToString("N");
        var echoEncrypt = WechatCallbackCrypto.Encrypt(aesKey, echoPlain, corpId);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
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
            var passed = response.StatusCode == HttpStatusCode.OK && text == echoPlain;

            return new SelfTestAssertion(
                "GET URL 验证回显 echostr",
                passed,
                $"实际 {(int)response.StatusCode}，响应体 {(passed ? "与明文一致" : $"为「{Truncate(text)}」（期望 {echoPlain}）")}");
        }
        catch (Exception ex)
        {
            return ConnectionFailure(nameof(EchoAsync), url, ex);
        }
    }

    /// <summary>POST <c>create_user</c> 事件：期望 200 且响应体为 <c>success</c>。</summary>
    private static Task<SelfTestAssertion> PostEventAsync(Uri url, string query, string body)
        => PostAsync(url, query, body, "POST create_user 事件（期望 200 success）", HttpStatusCode.OK, "success");

    /// <summary>同一报文重放：期望 403（一次性指纹闸，fail-closed 为正确行为）。</summary>
    private static Task<SelfTestAssertion> ReplayAsync(Uri url, string query, string body)
        => PostAsync(url, query, body, "同一报文重放（期望 403，指纹闸 fail-closed）", HttpStatusCode.Forbidden, null);

    /// <summary>篡改签名：期望 403（验签失败，且不消耗指纹）。</summary>
    private static Task<SelfTestAssertion> TamperedSignatureAsync(Uri url, string query, string body)
    {
        var tampered = query.Replace("msg_signature=", "msg_signature=0", StringComparison.Ordinal);
        return PostAsync(url, tampered, body, "篡改 msg_signature（期望 403）", HttpStatusCode.Forbidden, null);
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

    /// <summary>
    /// 通讯录新增成员明文（官方 90970 结构）：外层 <c>Event = change_contact</c> + <c>ChangeType = create_user</c>
    /// ⇒ 事件键为 <c>create_user</c>；UserId 每次运行取新值，避免命中上一次的业务幂等键。
    /// </summary>
    private static string BuildCreateUserXml(string corpId)
        => "<xml>"
           + $"<ToUserName><![CDATA[{corpId}]]></ToUserName>"
           + "<FromUserName><![CDATA[sys]]></FromUserName>"
           + $"<CreateTime>{DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)}</CreateTime>"
           + "<MsgType><![CDATA[event]]></MsgType>"
           + "<Event><![CDATA[change_contact]]></Event>"
           + "<ChangeType><![CDATA[create_user]]></ChangeType>"
           + $"<UserID><![CDATA[selftest_{Guid.NewGuid().ToString("N")}]]></UserID>"
           + "<Name><![CDATA[SelfTest 成员]]></Name>"
           + "<Department><![CDATA[1,2]]></Department>"
           + "<MainDepartment>1</MainDepartment>"
           + "<IsLeaderInDept><![CDATA[1,0]]></IsLeaderInDept>"
           + "<Status>1</Status>"
           + "<AgentID>1</AgentID>"
           + "</xml>";

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
