// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Globalization;
using System.Text.Json;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 智能机器人被动回复应答组装器（官方 101033）：明文 JSON → 加密 → 签名 → 应答外壳。
/// </summary>
/// <remarks>
/// <para>
/// 这是「加密被动回复应答通道」的唯一生产调用点 —— <c>WechatCallbackCrypto.Encrypt</c> 在此由
/// 「零生产调用」转为实际启用（XML 侧被动回复仍属延后能力，不因本能力提前）。
/// </para>
/// <para>
/// <b>应答外壳字段名与请求侧不同</b>：应答为 <c>{ encrypt, msgsignature, timestamp, nonce }</c>
/// —— <c>msgsignature</c> <b>无下划线</b>（请求查询参数为 <c>msg_signature</c>）；
/// <c>nonce</c> 必须复用回调 URL 中的 nonce，<c>timestamp</c> 为秒级。
/// 字段名由 <see cref="AibotEncryptedEnvelope"/> 的 <c>[JsonPropertyName]</c> 唯一固化。
/// </para>
/// <para>
/// <b>空包语义</b>：<paramref name="reply"/> 为 <c>null</c> 时应答 <c>{}</c>（空 JSON 对象）。
/// 官方 101033 允许「直接回复空包」（<c>feedback_event</c> 更明确「仅支持回复空包」）；
/// 采用 <c>{}</c> 而非零字节明文，兼顾「空包」与「应答壳必须可被 JSON 解析」两种解读。
/// </para>
/// </remarks>
internal static class WechatBotReplyWriter
{
    /// <summary>应答 Content-Type（官方 101033：应答为 JSON 密文包）。</summary>
    internal const string ResponseContentType = "application/json; charset=utf-8";

    /// <summary>空包明文（无应答 / <c>feedback_event</c> / 未匹配处理器时使用）。</summary>
    internal const string EmptyReplyJson = "{}";

    /// <summary>
    /// 组装被动回复应答 JSON（加密 + 签名）。
    /// </summary>
    /// <param name="app">命中的回调凭据（提供 PushToken 与 PushEncodingAESKey）。</param>
    /// <param name="reply">处理器应答；<c>null</c> = 空包。</param>
    /// <param name="nonce">回调 URL 中的 nonce（官方要求应答 nonce 必须使用回调 URL 的 nonce）。</param>
    /// <returns>应答 JSON 字符串。</returns>
    /// <exception cref="InvalidOperationException">应答形态不被 HTTP 被动回复支持（见 <see cref="Mud.Wechat.Work.Abstractions.Callback.Bots.WechatBotReplySupport"/>）。</exception>
    internal static string WriteReply(WechatAppCallbackOptions app, AibotMessage? reply, string? nonce)
    {
        var plainJson = SerializeReply(reply);
        var encrypted = WechatCallbackCrypto.Encrypt(app.PushEncodingAESKey, plainJson, string.Empty);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signature = WechatCallbackCrypto.ComputeSignature(
            app.PushToken,
            timestamp.ToString(CultureInfo.InvariantCulture),
            nonce ?? string.Empty,
            encrypted);

        var envelope = new AibotEncryptedEnvelope
        {
            Encrypt = encrypted,
            Msgsignature = signature,
            Timestamp = timestamp,
            Nonce = nonce,
        };

        return JsonSerializer.Serialize(envelope, AibotJsonContext.Default.AibotEncryptedEnvelope);
    }

    /// <summary>序列化应答明文（并执行 HTTP 被动回复支持面校验）。</summary>
    private static string SerializeReply(AibotMessage? reply)
    {
        if (reply == null)
        {
            return EmptyReplyJson;
        }

        // 支持面校验：非法形态在此 fail-fast，避免静默发给企微后被丢弃
        //（官方模板卡片事件与欢迎语只推一次，静默失败无法补救）。
        Mud.Wechat.Work.Abstractions.Callback.Bots.WechatBotReplySupport.ValidateHttpPassiveReply(reply);
        return JsonSerializer.Serialize(reply, AibotJsonContext.Default.AibotMessage);
    }
}
#endif
