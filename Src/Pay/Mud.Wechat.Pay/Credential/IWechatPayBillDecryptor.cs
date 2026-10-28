// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 微信支付账单文件解密编排器：把「RSA/SM2 解出对称密钥 → GCM 逐片解密 →（可选）gzip 解压 →
/// 哈希完整性校验 → 按序合并」收进 SDK 一次调用。
/// </summary>
/// <remarks>
/// <para><b>为什么要有编排器</b>：官方账单体系的密文由三层拼成——<c>encrypt_key</c>（商户证书公钥加密）、
/// 分片密文（末尾 16 字节 GCM 标签、关联数据为空）、分片哈希（对解密解压后的原始账单）。调用方自拼
/// 极易漏掉 SM4-GCM 分支或哈希校验（总案 D3，追踪号 MASTER-REMEDIATION-2026-10-24）。</para>
/// <para><b>官方算法口径</b>（SKIT <c>WechatTenpayClientResponseDecryptionExtensions</c> 与官方账单解密说明同源）：
/// RSA 商户的 <c>encrypt_key</c> 以 <c>RSA/ECB/OAEPWithSHA-1AndMGF1Padding</c> 加密；SM2 商户以 SM2
/// <c>C1C3C2_ASN1</c> 加密；解出的对称密钥为字符串（AES 32 字节 / SM4 16 字节）；分片 GCM 的关联数据为空。</para>
/// <para><b>不代理下载</b>：本编排器只解密；<c>download_url</c> 的时效（5 分钟）与下载由
/// <c>IWechatPayBillDownloadService</c> 与调用方承担。</para>
/// </remarks>
public interface IWechatPayBillDecryptor
{
    /// <summary>解密并合并一份账单的全部分片。</summary>
    /// <param name="merchantKey">商户私钥材料（经 <c>IWechatPayMerchantCredentialProvider.LoadPrivateKeyAsync</c> 取得；<b>所有权归调用方</b>，本编排器不释放）。</param>
    /// <param name="request">解密请求（算法 + 全部分片 + gzip 标记）。</param>
    /// <returns>解密结果（逐片明文 + 按序合并的完整账单）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="merchantKey"/> 或 <paramref name="request"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="InvalidOperationException">算法取值未知、私钥材料与算法不匹配、分片序号不从 1 起连续、或 SM2 私钥遇 SM 端口未注册。</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">密钥解出失败、GCM 标签校验失败或密文形态非法。</exception>
    WechatPayBillDecryptResult Decrypt(WechatPayMerchantKey merchantKey, WechatPayBillDecryptRequest request);
}
