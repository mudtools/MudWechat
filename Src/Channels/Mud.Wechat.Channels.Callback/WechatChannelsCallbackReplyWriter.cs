// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Encodings.Web;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Callback;

/// <summary>
/// 小店回调结构化应答回写器：把 <see cref="ChannelsCallbackReply"/> 的明文 JSON 加密并签名，
/// 组装官方回包信封 <c>{"Encrypt","MsgSignature","TimeStamp","Nonce"}</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方 dev_before/message_push（「其他回包内容需加密」）：Encrypt 为按本线 EncodingAESKey 的
/// AES-256-CBC 加密结果（复用 Core <see cref="WechatCallbackCrypto.Encrypt"/>，明文尾部 receiveid = 小店 AppID）；
/// MsgSignature = SHA1(token, TimeStamp, Nonce, Encrypt) 四参排序（复用 Core 四参 <c>ComputeSignature</c>）；
/// <b>Nonce 回填 URL 参数中的 nonce</b>，TimeStamp 取当前时间。
/// </para>
/// </remarks>
public static class WechatChannelsCallbackReplyWriter
{
    /// <summary>结构化回包的 Content-Type。</summary>
    public const string JsonContentType = "application/json; charset=utf-8";

    /// <summary>组装加密回包。</summary>
    /// <param name="app">回调凭据（PushToken / PushEncodingAESKey / AppId）。</param>
    /// <param name="reply">结构化应答（明文 JSON）。</param>
    /// <param name="urlNonce">回调 URL 查询参数中的 nonce（官方要求回包 Nonce 回填该值；为空时取时间戳兜底）。</param>
    /// <param name="utcNow">当前时间提供器。</param>
    /// <returns>加密回包 JSON 字符串。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> 或 <paramref name="reply"/> 为 <c>null</c>。</exception>
    public static string WriteReply(
        ChannelsAppCallbackOptions app,
        ChannelsCallbackReply reply,
        string? urlNonce,
        DateTimeOffset utcNow)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (reply == null)
        {
            throw new ArgumentNullException(nameof(reply));
        }

        var encrypt = WechatCallbackCrypto.Encrypt(app.PushEncodingAESKey, reply.Json, app.AppId);
        var timeStamp = utcNow.ToUnixTimeSeconds();
        var nonce = string.IsNullOrEmpty(urlNonce) ? timeStamp.ToString(System.Globalization.CultureInfo.InvariantCulture) : urlNonce!;
        var signature = WechatCallbackCrypto.ComputeSignature(
            app.PushToken, timeStamp.ToString(System.Globalization.CultureInfo.InvariantCulture), nonce, encrypt);

        // 回包信封以 Utf8JsonWriter 逐字段组合（全 TFM 可用、AOT 净零、无反射序列化；
        // 契约字段锁定由 CB-CH-10 加解密往返用例承担——生成上下文仅 net8+ 可用，故不经 context）。
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("Encrypt", encrypt);
            writer.WriteString("MsgSignature", signature);
            writer.WriteNumber("TimeStamp", timeStamp);
            writer.WriteString("Nonce", nonce);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
