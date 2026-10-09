// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.OpenPlatform.Abstractions;

namespace Mud.Wechat.OpenPlatform;

/// <summary>票据推送的处置结果。</summary>
public enum ComponentTicketPushOutcome
{
    /// <summary>验签 + 解密 + 类型判定 + appid 比对全通过，票据<b>已写入</b>存储。</summary>
    Accepted,

    /// <summary>
    /// 验签与解密通过，但 <c>InfoType</c> 不是 <c>component_verify_ticket</c>
    /// （同一「授权事件接收 URL」还会推授权结果等事件）。
    /// </summary>
    /// <remarks>
    /// <b>本线目前只消费票据</b>：其余事件类型尚未建模 ⇒ 宿主须自行决定回 <c>success</c>（放弃该事件）
    /// 还是回非 <c>success</c>（让微信重试，直到后续步骤落地）。见
    /// <see cref="ComponentVerifyTicketReceiver.ShouldReturnSuccess"/> 的 remarks。
    /// </remarks>
    NotTicketPush,

    /// <summary>缺少 <c>msg_signature</c> / <c>timestamp</c> / <c>nonce</c> / 包体，或信封内没有 <c>Encrypt</c>。</summary>
    MalformedEnvelope,

    /// <summary>签名不匹配（Token 配置不符，或报文被篡改）。</summary>
    InvalidSignature,

    /// <summary>密文无法解密（编码错误、长度不合形、密钥不符等）。</summary>
    DecryptFailed,

    /// <summary>解密出的 <c>receiveId</c> 与本平台 appid 不一致（防跨平台重放）。</summary>
    AppIdMismatch,

    /// <summary>类型正确但 <c>ComponentVerifyTicket</c> 为空。</summary>
    MissingTicket,
}

/// <summary>
/// <c>component_verify_ticket</c> 推送的接收器（<b>本线凭证链的入口</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约（2026-10-09 核验）</b>：第三方平台创建审核通过后，微信服务器会向「授权事件接收 URL」
/// <b>每隔 10 分钟</b>以 <b>POST</b> 推送 <c>component_verify_ticket</c>；
/// 官方原文「接收 POST 请求后，只需直接返回字符串 <c>success</c>」；
/// 推送内容为<b>加密</b>信封（解密后为 XML：<c>AppId</c> / <c>CreateTime</c> / <c>InfoType</c> /
/// <c>ComponentVerifyTicket</c>）。来源：
/// <see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/Before_Develop/component_verify_ticket.html"/>。
/// </para>
/// <para>
/// <b>加解密与验签复用共享内核</b>：本类<b>不</b>另写一份 AES/签名实现，而是调用
/// <see cref="WechatCallbackCrypto"/>（与公众号 / 企微线同一份、同一套官方算法：
/// 4 参 <c>msg_signature</c> + AES-256-CBC/PKCS#7 + 明文 <c>random(16B)+len(4B)+msg+appid</c>）。
/// <b>安全算法绝不复制第二份</b> —— 任何一处漂移都会变成「验签通过但解出乱码」这类极难定位的缺陷。
/// </para>
/// <para>
/// <b>XML 解析安全</b>：用 <see cref="XDocument"/> 解析；.NET 默认 <c>DtdProcessing.Prohibit</c>，
/// 即<b>不</b>解析 DTD / 外部实体 ⇒ 天然免疫 XXE（无自定义 <c>XmlResolver</c>）。
/// </para>
/// </remarks>
public sealed class ComponentVerifyTicketReceiver
{
    /// <summary>票据推送的 <c>InfoType</c> 取值（官方原文 <c>component_verify_ticket</c>）。</summary>
    public const string TicketInfoType = "component_verify_ticket";

    /// <summary>官方要求返回的应答字符串。</summary>
    public const string SuccessResponse = "success";

    private readonly IComponentVerifyTicketStore _ticketStore;
    private readonly OpenPlatformAppConfig _config;

    /// <summary>创建接收器。</summary>
    /// <param name="ticketStore">票据存储（本类只负责<b>写入</b>；读取由令牌提供者负责）。</param>
    /// <param name="config">平台配置（须含 Token 与 43 位 EncodingAesKey）。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">配置缺少推送凭据。</exception>
    public ComponentVerifyTicketReceiver(IComponentVerifyTicketStore ticketStore, OpenPlatformAppConfig config)
    {
        _ticketStore = ticketStore ?? throw new ArgumentNullException(nameof(ticketStore));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _config.EnsureValid();
    }

    /// <summary>
    /// 处置一次推送。
    /// </summary>
    /// <param name="msgSignature">URL 查询参数 <c>msg_signature</c>。</param>
    /// <param name="timestamp">URL 查询参数 <c>timestamp</c>。</param>
    /// <param name="nonce">URL 查询参数 <c>nonce</c>。</param>
    /// <param name="body">请求体（加密信封 XML）。</param>
    /// <returns>处置结果；仅 <see cref="ComponentTicketPushOutcome.Accepted"/> 时票据被写入。</returns>
    /// <remarks>
    /// <b>判定顺序是安全相关的</b>：先验签、再解密、再比对 appid，最后才看类型与票据 ——
    /// 任何一步失败都<b>不写</b>存储。若把写入提前到验签之前，一个伪造报文就能覆盖真票据
    /// （等于让攻击者关掉整条凭证链）。
    /// </remarks>
    public ComponentTicketPushOutcome Receive(
        string? msgSignature,
        string? timestamp,
        string? nonce,
        string? body)
    {
        if (string.IsNullOrWhiteSpace(msgSignature)
            || string.IsNullOrWhiteSpace(timestamp)
            || string.IsNullOrWhiteSpace(nonce)
            || string.IsNullOrWhiteSpace(body))
        {
            return ComponentTicketPushOutcome.MalformedEnvelope;
        }

        var encrypt = ReadElement(body!, "Encrypt");
        if (string.IsNullOrWhiteSpace(encrypt))
        {
            return ComponentTicketPushOutcome.MalformedEnvelope;
        }

        // ① 验签（4 参形态：含 Encrypt 参与项）—— 失败即返回，绝不进入解密与写入。
        if (!WechatCallbackCrypto.VerifySignature(_config.Token!, timestamp!, nonce!, encrypt!, msgSignature!))
        {
            return ComponentTicketPushOutcome.InvalidSignature;
        }

        // ② 解密。捕获面刻意收窄：只接「密文本身有问题」这一类异常，其余（如 OOM、取消）不吞。
        //
        // ⚠️ **必须包含 <see cref="WechatCallbackException"/>**：共享内核以该**领域异常**表达
        // 「密文长度非法」等前置校验失败（本轮实测踩到：漏了它，一个畸形密文会以异常形式
        // 直接穿透接收器，而不是被归一为 DecryptFailed 结果）。
        // 复用共享算法的同时，也**必须接住它的异常契约** —— 这是跨线复用的常见集成盲点。
        string plaintext;
        string receiveId;
        try
        {
            plaintext = WechatCallbackCrypto.Decrypt(_config.EncodingAesKey!, encrypt!, out receiveId);
        }
        catch (Exception ex) when (
            ex is WechatCallbackException or CryptographicException or FormatException or ArgumentException)
        {
            return ComponentTicketPushOutcome.DecryptFailed;
        }

        // ③ appid 比对：防跨平台重放（同一条密文被搬到另一个平台的接收 URL 上）。
        if (!string.Equals(receiveId, _config.ComponentAppId, StringComparison.Ordinal))
        {
            return ComponentTicketPushOutcome.AppIdMismatch;
        }

        // ④ 类型判定：同一接收 URL 也会推授权结果等事件，本线只消费票据。
        var infoType = ReadElement(plaintext, "InfoType");
        if (!string.Equals(infoType, TicketInfoType, StringComparison.Ordinal))
        {
            return ComponentTicketPushOutcome.NotTicketPush;
        }

        // ⑤ 取票据并写入（唯一写点）。
        var ticket = ReadElement(plaintext, "ComponentVerifyTicket");
        if (string.IsNullOrWhiteSpace(ticket))
        {
            return ComponentTicketPushOutcome.MissingTicket;
        }

        _ticketStore.Set(ticket);
        return ComponentTicketPushOutcome.Accepted;
    }

    /// <summary>
    /// 该结果是否应当向微信回 <c>success</c>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>只有 <see cref="ComponentTicketPushOutcome.Accepted"/> 才回 <c>success</c></b>：
    /// 验签/解密失败若也回 <c>success</c>，等于告诉微信「一切正常」—— 真票据丢失、攻击尝试
    /// 都会在监控上<b>静默消失</b>（微信不再重试，我方也没有任何告警线索）。
    /// </para>
    /// <para>
    /// <b><see cref="ComponentTicketPushOutcome.NotTicketPush"/> 刻意不自动回 success</b>：
    /// 本线尚未建模授权结果事件，自动回 <c>success</c> 会让这些事件被<b>静默丢弃</b>。
    /// 宿主应显式选择：回 <c>success</c>（放弃）或回非 <c>success</c>（让微信重试，
    /// 直到本线落地授权结果事件处理）。
    /// </para>
    /// </remarks>
    public static bool ShouldReturnSuccess(ComponentTicketPushOutcome outcome)
        => outcome == ComponentTicketPushOutcome.Accepted;

    /// <summary>读取 XML 中某元素的文本（解析失败 / 元素缺失返回 <c>null</c>）。</summary>
    /// <remarks>
    /// <b>CDATA 已由 XML 解析器处理</b>：官方推送的 <c>Encrypt</c> 常以 <c>&lt;![CDATA[...]]&gt;</c> 包裹，
    /// <see cref="XElement.Value"/> 会给出其内容（无需自行剥 CDATA）。
    /// </remarks>
    private static string? ReadElement(string xml, string elementName)
    {
        try
        {
            var root = XDocument.Parse(xml).Root;
            return root?.Element(elementName)?.Value;
        }
        catch (XmlException)
        {
            return null;
        }
    }
}
