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

/// <summary>推送的处置结果分类。</summary>
public enum ComponentTicketPushOutcome
{
    /// <summary>票据推送：验签 + 解密 + 类型 + appid 全通过，票据<b>已写入</b>存储。</summary>
    Accepted,

    /// <summary>授权变更事件（授权成功 / 更新授权 / 取消授权）：事件已解析并随结果返回。</summary>
    AuthorizerEvent,

    /// <summary>未知 <c>InfoType</c>（本线尚未建模的类型，含官方未来新增的事件）。</summary>
    UnknownInfoType,

    /// <summary>缺少 <c>msg_signature</c> / <c>timestamp</c> / <c>nonce</c> / 包体，或信封内没有 <c>Encrypt</c>。</summary>
    MalformedEnvelope,

    /// <summary>签名不匹配（Token 配置不符，或报文被篡改）。</summary>
    InvalidSignature,

    /// <summary>密文无法解密（编码错误、长度不合形、密钥不符等）。</summary>
    DecryptFailed,

    /// <summary>解密出的 <c>receiveId</c> 与本平台 appid 不一致（防跨平台重放）。</summary>
    AppIdMismatch,

    /// <summary>类型为票据推送但 <c>ComponentVerifyTicket</c> 为空。</summary>
    MissingTicket,
}

/// <summary>一次推送的处置结果。</summary>
public sealed class ComponentPushResult
{
    internal ComponentPushResult(
        ComponentTicketPushOutcome outcome,
        string? infoType = null,
        ComponentAuthorizerEvent? authorizerEvent = null)
    {
        Outcome = outcome;
        InfoType = infoType;
        AuthorizerEvent = authorizerEvent;
    }

    /// <summary>结果分类。</summary>
    public ComponentTicketPushOutcome Outcome { get; }

    /// <summary>解密后的 <c>InfoType</c>（仅在验签解密通过后才有值）。</summary>
    public string? InfoType { get; }

    /// <summary>授权变更事件（仅 <see cref="ComponentTicketPushOutcome.AuthorizerEvent"/> 时非空）。</summary>
    public ComponentAuthorizerEvent? AuthorizerEvent { get; }

    /// <summary>是否应向微信回 <c>success</c>（判定见 <see cref="ComponentVerifyTicketReceiver.ShouldReturnSuccess"/>）。</summary>
    public bool ShouldReturnSuccess => ComponentVerifyTicketReceiver.ShouldReturnSuccess(Outcome);
}

/// <summary>
/// 「授权事件接收 URL」推送的接收器（<b>本线凭证链与授权链的共同入口</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约（2026-10-09 核验，两页）</b>：同一个接收 URL 会收到两类推送 ——
/// ① 《验证票据》：每 10 分钟推 <c>component_verify_ticket</c>；
/// ② 《授权变更通知推送》：用户授权 / 取消授权 / 更新授权后推
/// <c>authorized</c> / <c>unauthorized</c> / <c>updateauthorized</c>。
/// 两页均要求「接收 POST 请求后，只需直接返回字符串 <c>success</c>」。
/// </para>
/// <para>
/// <b>加解密与验签复用共享内核</b>（<see cref="WechatCallbackCrypto"/>，与公众号 / 企微线同一份），
/// 不另写第二份安全算法；并<b>接住其领域异常</b> <see cref="WechatCallbackException"/>
/// （共享算法以它表达「密文长度非法」等前置校验失败 —— 漏接会让畸形密文以异常穿透接收器）。
/// </para>
/// <para>
/// <b>XML 解析安全</b>：<see cref="XDocument"/> 解析，.NET 默认 <c>DtdProcessing.Prohibit</c>
/// ⇒ 不解析 DTD / 外部实体，天然免疫 XXE。
/// </para>
/// </remarks>
public sealed class ComponentVerifyTicketReceiver
{
    /// <summary>票据推送的 <c>InfoType</c>（官方原文；等价于 <see cref="ComponentPushInfoTypes.ComponentVerifyTicket"/>）。</summary>
    public const string TicketInfoType = ComponentPushInfoTypes.ComponentVerifyTicket;

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
    /// 处置一次推送（票据推送或授权变更推送）。
    /// </summary>
    /// <param name="msgSignature">URL 查询参数 <c>msg_signature</c>。</param>
    /// <param name="timestamp">URL 查询参数 <c>timestamp</c>。</param>
    /// <param name="nonce">URL 查询参数 <c>nonce</c>。</param>
    /// <param name="body">请求体（加密信封 XML）。</param>
    /// <returns>处置结果；票据推送成功时票据已写入存储，授权变更事件随结果返回。</returns>
    /// <remarks>
    /// <b>判定顺序是安全相关的</b>：先验签、再解密、再比对 appid，最后才看类型并<b>分流</b> ——
    /// 任何一步失败都<b>不写</b>存储、也<b>不</b>返回事件。若把写入/返回提前到验签之前，
    /// 一个伪造报文就能覆盖真票据或伪造一次「授权成功」（等于让攻击者拿到授权码）。
    /// </remarks>
    public ComponentPushResult Receive(
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
            return new ComponentPushResult(ComponentTicketPushOutcome.MalformedEnvelope);
        }

        var envelope = Parse(body!);
        var encrypt = ReadElement(envelope, ComponentPushXmlElements.Encrypt);
        if (string.IsNullOrWhiteSpace(encrypt))
        {
            return new ComponentPushResult(ComponentTicketPushOutcome.MalformedEnvelope);
        }

        // ① 验签（4 参形态：含 Encrypt 参与项）—— 失败即返回，绝不进入解密 / 写入 / 事件返回。
        if (!WechatCallbackCrypto.VerifySignature(_config.Token!, timestamp!, nonce!, encrypt!, msgSignature!))
        {
            return new ComponentPushResult(ComponentTicketPushOutcome.InvalidSignature);
        }

        // ② 解密。捕获面刻意收窄，但**必须**含共享内核的领域异常（见类注释）。
        string plaintext;
        string receiveId;
        try
        {
            plaintext = WechatCallbackCrypto.Decrypt(_config.EncodingAesKey!, encrypt!, out receiveId);
        }
        catch (Exception ex) when (
            ex is WechatCallbackException or CryptographicException or FormatException or ArgumentException)
        {
            return new ComponentPushResult(ComponentTicketPushOutcome.DecryptFailed);
        }

        // ③ appid 比对：防跨平台重放。
        if (!string.Equals(receiveId, _config.ComponentAppId, StringComparison.Ordinal))
        {
            return new ComponentPushResult(ComponentTicketPushOutcome.AppIdMismatch);
        }

        var payload = Parse(plaintext);
        var infoType = ReadElement(payload, ComponentPushXmlElements.InfoType);

        // ④ 分流：票据 或 授权变更事件 或 未知类型。
        if (string.Equals(infoType, ComponentPushInfoTypes.ComponentVerifyTicket, StringComparison.Ordinal))
        {
            var ticket = ReadElement(payload, ComponentPushXmlElements.ComponentVerifyTicket);
            if (string.IsNullOrWhiteSpace(ticket))
            {
                return new ComponentPushResult(
                    ComponentTicketPushOutcome.MissingTicket, ComponentPushInfoTypes.ComponentVerifyTicket);
            }

            _ticketStore.Set(ticket);
            return new ComponentPushResult(
                ComponentTicketPushOutcome.Accepted, ComponentPushInfoTypes.ComponentVerifyTicket);
        }

        if (IsAuthorizerEvent(infoType))
        {
            return new ComponentPushResult(
                ComponentTicketPushOutcome.AuthorizerEvent,
                infoType,
                new ComponentAuthorizerEvent(
                    infoType,
                    ReadElement(payload, ComponentPushXmlElements.AuthorizerAppId),
                    ReadElement(payload, ComponentPushXmlElements.AuthorizationCode),
                    ReadElement(payload, ComponentPushXmlElements.AuthorizationCodeExpiredTime),
                    ReadElement(payload, ComponentPushXmlElements.PreAuthCode),
                    ReadElement(payload, ComponentPushXmlElements.CreateTime)));
        }

        return new ComponentPushResult(ComponentTicketPushOutcome.UnknownInfoType, infoType);
    }

    /// <summary>
    /// 该结果是否应当向微信回 <c>success</c>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>只有「已被本线完整消化」的两类结果才回 <c>success</c></b>：
    /// <see cref="ComponentTicketPushOutcome.Accepted"/>（票据已落存储）与
    /// <see cref="ComponentTicketPushOutcome.AuthorizerEvent"/>（事件已随返回值交付给调用方）。
    /// 验签/解密失败若也回 <c>success</c>，等于告诉微信「一切正常」—— 真票据丢失、攻击尝试
    /// 都会在监控上<b>静默消失</b>（微信不再重试，我方也无告警线索）。
    /// </para>
    /// <para>
    /// <b>⚠️ 本判定在上一步曾是「授权类事件一律不回 success」</b>：当时本线尚未建模授权变更事件，
    /// 自动回 <c>success</c> 会让这些事件被静默丢弃 —— 那个结论在<b>未建模时是正确的</b>。
    /// 本步已把三类事件解析并<b>同步</b>随返回值交付 ⇒ 同批解除该处置（与守卫「事实变化则同批改判」同款纪律）。
    /// 保留下来的保守面是 <see cref="ComponentTicketPushOutcome.UnknownInfoType"/>：<b>未知类型仍不回</b>
    /// <c>success</c>，让宿主在监控上看得见（微信会重试若干次后放弃）。
    /// </para>
    /// </remarks>
    public static bool ShouldReturnSuccess(ComponentTicketPushOutcome outcome)
        => outcome is ComponentTicketPushOutcome.Accepted or ComponentTicketPushOutcome.AuthorizerEvent;

    /// <summary>是否为授权变更事件（三类）。</summary>
    private static bool IsAuthorizerEvent(string? infoType)
        => string.Equals(infoType, ComponentPushInfoTypes.Authorized, StringComparison.Ordinal)
           || string.Equals(infoType, ComponentPushInfoTypes.UpdateAuthorized, StringComparison.Ordinal)
           || string.Equals(infoType, ComponentPushInfoTypes.Unauthorized, StringComparison.Ordinal);

    /// <summary>解析 XML（失败返回 <c>null</c>，不抛出）。</summary>
    private static XElement? Parse(string xml)
    {
        try
        {
            return XDocument.Parse(xml).Root;
        }
        catch (XmlException)
        {
            return null;
        }
    }

    /// <summary>读取元素文本（根为 <c>null</c> / 元素缺失时返回 <c>null</c>）。</summary>
    /// <remarks>CDATA 由 XML 解析器处理，<see cref="XElement.Value"/> 直接给出内容。</remarks>
    private static string? ReadElement(XElement? root, string elementName)
        => root?.Element(elementName)?.Value;
}
