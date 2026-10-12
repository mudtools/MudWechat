// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 结构化回包加密信封（官方 dev_before/message_push「其他回包内容需加密」的回包格式）。
/// </summary>
/// <remarks>由 <c>WechatChannelsCallbackReplyWriter</c> 组装；字段名照官方原文（PascalCase）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsCallbackReplyEnvelope
{
    /// <summary>获取或设置 AES-256-CBC 加密的应答密文（Base64）。</summary>
    [JsonPropertyName("Encrypt")]
    public string Encrypt { get; set; } = string.Empty;

    /// <summary>获取或设置回包签名（SHA1(token, TimeStamp, Nonce, Encrypt) 四参排序）。</summary>
    [JsonPropertyName("MsgSignature")]
    public string MsgSignature { get; set; } = string.Empty;

    /// <summary>获取或设置回包时间戳（秒）。</summary>
    [JsonPropertyName("TimeStamp")]
    public long TimeStamp { get; set; }

    /// <summary>获取或设置随机数（官方要求回填回调 URL 参数中的 nonce）。</summary>
    [JsonPropertyName("Nonce")]
    public string Nonce { get; set; } = string.Empty;
}
