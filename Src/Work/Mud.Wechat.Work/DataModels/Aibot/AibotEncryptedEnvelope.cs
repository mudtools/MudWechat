// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人加密报文外壳（官方 101033：回调密文 <c>{ "encrypt": "msg_encrypt" }</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 请求侧（企微 → 开发者）：HTTP POST 包体为 <c>{"encrypt":"..."}</c>（<b>JSON</b>，区别于应用回调的加密 XML）；
/// 解密后即为 <see cref="AibotMessageCallback"/> / <see cref="AibotEventCallback"/> 的明文 JSON。
/// </para>
/// <para>
/// 应答侧（开发者 → 企微）字段名与验签字段名<b>不同</b>：应答为
/// <c>{ "encrypt", "msgsignature", "timestamp", "nonce" }</c> ——
/// <c>msgsignature</c> <b>无下划线</b>（请求/回调侧为 <c>msg_signature</c> 带下划线）；
/// <c>nonce</c> 必须使用回调 URL 中的 nonce，<c>timestamp</c> 为秒级时间戳。
/// </para>
/// <para>
/// 应答侧字段名由本类型固化（<see cref="Msgsignature"/> 的 <c>[JsonPropertyName]</c> 是唯一权威），
/// 防止「顺手统一为 <c>msg_signature</c>」导致企微验签失败。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotEncryptedEnvelope
{
    /// <summary>加密后的消息内容（Base64）。</summary>
    [JsonPropertyName("encrypt")]
    public string? Encrypt { get; set; }

    /// <summary>消息签名（<b>应答侧字段名 <c>msgsignature</c>，无下划线</b>；请求侧查询参数名为 <c>msg_signature</c>）。</summary>
    [JsonPropertyName("msgsignature")]
    public string? Msgsignature { get; set; }

    /// <summary>时间戳（应答侧为秒级时间戳）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>随机数（应答侧必须使用回调 URL 中的 nonce）。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
