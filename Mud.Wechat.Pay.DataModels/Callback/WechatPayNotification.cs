// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Callback;

/// <summary>
/// 微信支付 APIv3 通知信封（回调 POST 报文体；<b>JSON</b>，与企微 / 公众号的 XML 信封完全不同）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：支付通知为通用信封，具体载荷由 <see cref="EventType"/> 决定
/// （交易成功 <c>TRANSACTION.SUCCESS</c>、退款结果 <c>REFUND.SUCCESS</c> 等）。
/// </para>
/// <para>
/// <b>⚠️ <c>event_type</c> 不足以唯一确定载荷</b>：官方把<b>分账动态通知</b>（分账 / 分账回退）
/// <b>也</b>标为 <c>TRANSACTION.SUCCESS</c>，与支付成功通知<b>同值</b>。可靠区分须结合
/// <c>resource.original_type</c>（分账为 <c>profitsharing</c>）—— 见
/// <see cref="WechatPayNotificationResource.OriginalType"/> 与
/// <see cref="WechatPayNotificationOriginalTypes"/>。
/// </para>
/// <para>
/// <b>字段名照官方原文</b>（<c>create_time</c> / <c>event_type</c> / <c>resource_type</c>）。
/// </para>
/// <para>
/// <b>验签在前、反序列化在后</b>：报文原文（原始字节）须先经
/// <c>WechatPaySignatureMessages.BuildVerifyMessage</c> + 平台证书验签，<b>通过后</b>才允许反序列化。
/// 先反序列化再验签会让攻击者用畸形 JSON 触发解析分支（且违反官方 FAQ 的「报文体必须是原始字节」要求）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayNotification
{
    /// <summary>通知 ID（<c>id</c>，string(36)），官方唯一标识，可用于幂等去重。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>通知创建时间（<c>create_time</c>，string(32)，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>通知类型（<c>resource_type</c>，string(32)），目前固定 <c>encrypt-resource</c>。</summary>
    [JsonPropertyName("resource_type")]
    public string? ResourceType { get; set; }

    /// <summary>
    /// 通知事件类型（<c>event_type</c>，string(32)），取值见 <see cref="WechatPayNotificationEventTypes"/>。
    /// </summary>
    /// <remarks>
    /// <b>不得单独用它选载荷</b>：<c>TRANSACTION.SUCCESS</c> 同时用于「支付成功」与「分账动态通知」，
    /// 须结合 <see cref="WechatPayNotificationResource.OriginalType"/> 判定。
    /// </remarks>
    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    /// <summary>通知简要说明（<c>summary</c>，string(64)）。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>通知资源数据（<c>resource</c>，必填），含 <c>AEAD_AES_256_GCM</c> 密文，见 <see cref="WechatPayNotificationResource"/>。</summary>
    [JsonPropertyName("resource")]
    public WechatPayNotificationResource? Resource { get; set; }
}

/// <summary>通知资源数据（<c>resource</c>；密文三元组 + 算法标识）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayNotificationResource
{
    /// <summary>原始类型（<c>original_type</c>，string(32)），如 <c>transaction</c> / <c>refund</c>。</summary>
    [JsonPropertyName("original_type")]
    public string? OriginalType { get; set; }

    /// <summary>加密算法（<c>algorithm</c>，string(32)），固定 <c>AEAD_AES_256_GCM</c>（其它取值一律拒绝）。</summary>
    [JsonPropertyName("algorithm")]
    public string? Algorithm { get; set; }

    /// <summary>密文（<c>ciphertext</c>，string，Base64）。<b>不得入日志 / 遥测 / 异常消息</b>（PAY-B7）。</summary>
    [JsonPropertyName("ciphertext")]
    public string? CipherText { get; set; }

    /// <summary>附加数据（<c>associated_data</c>，string(16)），参与 GCM 认证。</summary>
    [JsonPropertyName("associated_data")]
    public string? AssociatedData { get; set; }

    /// <summary>随机串（<c>nonce</c>，string(12)），即 GCM 的 12 字节 IV 的字符串表达。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}

/// <summary>
/// 通知事件类型（官方 <c>event_type</c> 取值）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 同名复用</b>：<see cref="TransactionSuccess"/> 同时是「支付成功」与「分账动态通知」
/// （分账 / 分账回退）的 <c>event_type</c> —— 官方原文如此。区分载荷必须再看
/// <see cref="WechatPayNotificationOriginalTypes"/>。
/// </para>
/// <para>取值来源：交易/退款通知页与本仓已核验的各回调页（2026-10-09）。</para>
/// </remarks>
public static class WechatPayNotificationEventTypes
{
    /// <summary>交易成功（官方 <c>TRANSACTION.SUCCESS</c>）；<b>分账动态通知也取此值</b>。</summary>
    public const string TransactionSuccess = "TRANSACTION.SUCCESS";

    /// <summary>退款成功（官方 <c>REFUND.SUCCESS</c>）。</summary>
    public const string RefundSuccess = "REFUND.SUCCESS";

    /// <summary>退款异常（官方 <c>REFUND.ABNORMAL</c>）。</summary>
    public const string RefundAbnormal = "REFUND.ABNORMAL";

    /// <summary>退款关闭（官方 <c>REFUND.CLOSED</c>）。</summary>
    public const string RefundClosed = "REFUND.CLOSED";
}

/// <summary>
/// 原始回调类型（官方 <c>resource.original_type</c> 取值）—— <b>载荷形态的真正判别式</b>。
/// </summary>
/// <remarks>
/// 官方原文：「加密前的对象类型」；分账动账通知的类型为 <c>profitsharing</c>。
/// 当 <c>event_type</c> 出现同名复用时（如 <c>TRANSACTION.SUCCESS</c>），本字段是<b>唯一可靠</b>的区分依据。
/// </remarks>
public static class WechatPayNotificationOriginalTypes
{
    /// <summary>交易（官方 <c>transaction</c>）。</summary>
    public const string Transaction = "transaction";

    /// <summary>退款（官方 <c>refund</c>）。</summary>
    public const string Refund = "refund";

    /// <summary>分账动账（官方 <c>profitsharing</c>）。</summary>
    public const string ProfitSharing = "profitsharing";
}
