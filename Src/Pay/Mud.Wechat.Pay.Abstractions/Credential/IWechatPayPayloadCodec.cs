// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>支付载荷编解码端口（按 <c>resource.algorithm</c> 分派 AES-GCM / SM4-GCM）。</summary>
/// <remarks>
/// <para><b>职责边界</b>：此端口负责「支付报文载荷层」的加解密，与叶层 <c>IWechatSmCryptoProvider</c>（国密原语层）分层不同。
/// <c>Sm4GcmPayloadCodec</c> 实现委托前者，<c>AesGcmPayloadCodec</c> 实现为纯 AES。</para>
/// <para><b>消费点</b>：<c>WechatPayCallbackReceiver</c>（回调通知解密）、<c>WechatPayPlatformCertificateRefresher</c>（平台证书下载 cipher 解密）、
/// <c>IWechatPayEcommerceBillService</c>（账单下载解密）。</para>
/// </remarks>
public interface IWechatPayPayloadCodec
{
    /// <summary>算法标识（<c>AEAD_AES_256_GCM</c> / <c>SM4_GCM</c>）。</summary>
    string Algorithm { get; }

    /// <summary>
    /// 解密官方载荷（<c>resource</c> 三元组：nonce + ciphertext + tag）。
    /// </summary>
    /// <param name="apiV3Key">APIv3 密钥（32 字节；SM4-GCM 时会派生为 16 字节）。</param>
    /// <param name="nonce">随机数（AES-GCM 12 字节，SM4-GCM 12 字节）。</param>
    /// <param name="ciphertext">密文。</param>
    /// <param name="associatedData">附加认证数据（通常为 <c>resource.ciphertext</c> 字段名或空）。</param>
    /// <param name="tag">认证标签（16 字节）。</param>
    /// <returns>解密后的明文字节。</returns>
    /// <exception cref="CryptographicException">解密失败或 tag 校验失败时抛出。</exception>
    byte[] DecryptOfficialPayload(byte[] apiV3Key, byte[] nonce, byte[] ciphertext, byte[] associatedData, byte[] tag);
}
