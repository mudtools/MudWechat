// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Certificates;

/// <summary>
/// 获取平台证书列表应答（微信支付 APIv3；<c>GET /v3/certificates</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>
/// （概览另有 <c>4024350132</c>；2026-10-09 逐字段核验）。支持商户类型：<b>普通商户</b>；
/// 频率限制：<b>单个商户号 1000 次/s</b>。
/// </para>
/// <para>
/// <b>证书为密文返回</b>：官方把平台证书 PEM <b>明文</b>经 <c>AEAD_AES_256_GCM</c>
/// （APIv3 密钥）加密后放在 <c>encrypt_certificate</c>，故消费侧必须先用
/// <c>WechatPayAesGcmCodec</c> 解密 <c>encrypt_certificate.ciphertext</c> 才能拿到 PEM。
/// </para>
/// <para>
/// <b>与「微信支付公钥」模式并存</b>：官方另有 <c>Wechatpay-Serial: PUB_KEY_ID_xxx</c> 公钥模式
/// （见设计方案 §2.5 待决项）。本 DTO 只覆盖<b>平台证书</b>模式；两种模式的取舍须在应答/回调验签侧统一决策。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Certificates")]
public class PlatformCertificatesResponse : WechatPayResponse
{
    /// <summary>平台证书列表（<c>data</c>，数组），见 <see cref="PlatformCertificate"/>。</summary>
    [JsonPropertyName("data")]
    public List<PlatformCertificate>? Data { get; set; }
}

/// <summary>平台证书条目（<c>data[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Certificates")]
public class PlatformCertificate
{
    /// <summary>证书序列号（<c>serial_no</c>，必填 string(32)），平台证书主键，用于匹配 <c>Wechatpay-Serial</c>。</summary>
    [JsonPropertyName("serial_no")]
    public string? SerialNumber { get; set; }

    /// <summary>证书启用时间（<c>effective_time</c>，选填，rfc3339 <b>秒级时间戳整数</b>）。</summary>
    [JsonPropertyName("effective_time")]
    public long? EffectiveTime { get; set; }

    /// <summary>证书弃用时间（<c>expire_time</c>，选填，rfc3339 <b>秒级时间戳整数</b>）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>加密的证书信息（<c>encrypt_certificate</c>），见 <see cref="PlatformCertificateCipher"/>。</summary>
    [JsonPropertyName("encrypt_certificate")]
    public PlatformCertificateCipher? EncryptCertificate { get; set; }
}

/// <summary>平台证书密文载体（<c>data[].encrypt_certificate</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Certificates")]
public class PlatformCertificateCipher
{
    /// <summary>加密算法（<c>algorithm</c>，必填 string(32)），目前仅支持 <c>AEAD_AES_256_GCM</c>。</summary>
    [JsonPropertyName("algorithm")]
    public string? Algorithm { get; set; }

    /// <summary>加密随机串（<c>nonce</c>，必填 string(16)），即 GCM 的 12 字节 IV 的 Base64 表达。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    /// <summary>附加数据（<c>associated_data</c>，选填 string(16)），固定 <c>certificate</c>。</summary>
    [JsonPropertyName("associated_data")]
    public string? AssociatedData { get; set; }

    /// <summary>证书内容密文（<c>ciphertext</c>，必填 string(1048576)），解密后为 PEM 格式平台证书明文。</summary>
    [JsonPropertyName("ciphertext")]
    public string? Ciphertext { get; set; }
}
