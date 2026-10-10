// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 被动回复写出器：明文回复 XML 或加密回写（<c>&lt;xml&gt;&lt;Encrypt/&gt;&lt;MsgSignature/&gt;&lt;TimeStamp/&gt;&lt;Nonce/&gt;&lt;/xml&gt;</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约（F11）</b>：「如无特定要求，回复空串或者 success（无需加密）即可，其他回包内容需加密处理」——
/// 故本写出器<b>只</b>负责「有回复体」的场景；无回复由中间件回明文 <c>success</c>。
/// </para>
/// <para>
/// <b>签名基准（F10）</b>：<c>MsgSignature = sha1(sort(token, TimeStamp(回包), Nonce(回包), Encrypt(回包)))</c>。
/// 本实现以「请求的 timestamp / nonce 回填回包」（平台接受，且与企微智能机器人应答侧做法一致）。
/// </para>
/// </remarks>
internal static class MpCallbackReplyWriter
{
    /// <summary>无被动回复时的应答体（官方许可的明文形态，无需加密）。</summary>
    internal const string SuccessResponseBody = "success";

    /// <summary>纯文本应答的 Content-Type。</summary>
    internal const string PlainTextContentType = "text/plain; charset=utf-8";

    /// <summary>XML 应答的 Content-Type。</summary>
    internal const string XmlContentType = "text/xml; charset=utf-8";

    /// <summary>写出被动回复体（按当前安全模式决定是否加密）。</summary>
    /// <param name="app">命中应用的凭据。</param>
    /// <param name="mode">生效安全模式。</param>
    /// <param name="envelope">回调信封（回包收发方与时效参数取自本信封）。</param>
    /// <param name="reply">回复体。</param>
    /// <param name="now">当前时间（<c>CreateTime</c> 取值）。</param>
    /// <returns>应回写的应答体。</returns>
    internal static string WriteReply(
        MpAppCallbackOptions app,
        MpCallbackSecurityMode mode,
        MpCallbackEnvelope envelope,
        MpCallbackReply reply,
        DateTimeOffset now)
    {
        if (reply == null)
        {
            throw new ArgumentNullException(nameof(reply));
        }

        // 回包收发方与请求相反：回包的 ToUserName = 用户（请求 FromUserName），FromUserName = 公众号（请求 ToUserName）。
        var builder = new StringBuilder(256);
        builder.Append("<xml>");
        AppendElement(builder, "ToUserName", envelope.FromUserName);
        AppendElement(builder, "FromUserName", envelope.ToUserName);
        AppendElement(builder, "CreateTime", now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
        AppendElement(builder, "MsgType", reply.MsgType);

        foreach (var field in reply.Fields)
        {
            AppendElement(builder, field.Key, field.Value);
        }

        // 嵌套体（Image/Voice/Video/Music/Articles）由 Abstractions 的工厂方法预渲染（已转义）后原样拼入。
        if (!string.IsNullOrEmpty(reply.BodyXml))
        {
            builder.Append(reply.BodyXml);
        }

        builder.Append("</xml>");
        var plainXml = builder.ToString();

        // 明文模式：直接回明文 XML（平台按配置的明文模式解析）。
        if (mode == MpCallbackSecurityMode.Plain)
        {
            return plainXml;
        }

        // 安全/兼容模式：**只要回写体不是空串/success 就必须加密**（守卫 CB-MP-3 锁定）。
        var encrypt = WechatCallbackCrypto.Encrypt(app.PushEncodingAESKey, plainXml, app.AppId);
        var timeStamp = envelope.TimeStamp ?? string.Empty;
        var nonce = envelope.Nonce ?? string.Empty;
        var msgSignature = WechatCallbackCrypto.ComputeSignature(app.PushToken, timeStamp, nonce, encrypt);

        var encrypted = new StringBuilder(256);
        encrypted.Append("<xml>");
        AppendElement(encrypted, "Encrypt", encrypt);
        AppendElement(encrypted, "MsgSignature", msgSignature);
        AppendElement(encrypted, "TimeStamp", timeStamp);
        AppendElement(encrypted, "Nonce", nonce);
        encrypted.Append("</xml>");
        return encrypted.ToString();
    }

    private static void AppendElement(StringBuilder builder, string name, string? value)
    {
        builder.Append('<').Append(name).Append('>');
        AppendEscaped(builder, value);
        builder.Append("</").Append(name).Append('>');
    }

    private static void AppendEscaped(StringBuilder builder, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        foreach (var ch in value!)
        {
            switch (ch)
            {
                case '&':
                    builder.Append("&amp;");
                    break;
                case '<':
                    builder.Append("&lt;");
                    break;
                case '>':
                    builder.Append("&gt;");
                    break;
                case '"':
                    builder.Append("&quot;");
                    break;
                default:
                    builder.Append(ch);
                    break;
            }
        }
    }
}
