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
    /// 通知事件类型（<c>event_type</c>，string(32)），如 <c>TRANSACTION.SUCCESS</c> /
    /// <c>REFUND.SUCCESS</c> / <c>REFUND.ABNORMAL</c> / <c>REFUND.CLOSED</c>。
    /// </summary>
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
