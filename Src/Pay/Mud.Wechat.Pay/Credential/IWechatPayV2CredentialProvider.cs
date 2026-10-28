// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;
using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.V2.Credential;

/// <summary>
/// APIv2 专用凭据取用端口：APIv2 签名密钥与双向证书<b>只经组件 <c>ISecretProvider</c> 取用</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么另立端口而不扩 <c>IWechatPayMerchantCredentialProvider</c></b>：后者是 APIv3 线已发布的公开面，
/// 往接口上加成员对既有宿主即破坏性变更（其实现类须同批改）。APIv2 的两种材料（签名密钥串、PKCS#12 证书）
/// 与 APIv3 的（RSA 私钥、AEAD 密钥）语义完全不同、消费方也只在 V2 程序集内，落在 V2 侧是零影响的方向。
/// </para>
/// <para>
/// <b>密钥不同源</b>：<see cref="LoadApiV2KeyAsync"/> 取的是 <c>ApiV2KeySecretName</c>，
/// 与 APIv3 的 <c>ApiKeySecretName</c> 在商户平台<b>分别设置</b>；混用只会得到官方恒返回
/// <c>SIGNERROR</c> 且配置面无异常可循的故障，故两端口不提供「回退到另一个密钥」的便利。
/// </para>
/// <para>
/// <b>返回值所有权</b>：<see cref="LoadClientCertificateAsync"/> 返回的证书<b>由调用方缓存并负责释放</b>
/// （V2 安全通道按商户缓存一份，见 <c>WechatPayV2SecureChannelProvider</c>）。
/// </para>
/// <para>
/// <b>失败语义</b>：查无密钥名 / 内容不可解析 / 长度不符一律抛 <see cref="InvalidOperationException"/>，
/// 且异常消息<b>绝不回显密钥或证书材料</b>（与 APIv3 的 PAY-B7 同一红线）。
/// </para>
/// </remarks>
public interface IWechatPayV2CredentialProvider
{
    /// <summary>
    /// 取用 APIv2 签名密钥（MD5 / HMAC-SHA256 共用，官方要求 32 字符）。
    /// </summary>
    /// <param name="config">商户配置（提供 <c>ApiV2KeySecretName</c> 与商户键）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>密钥原文（用于拼签名串尾部的 <c>key=</c>）。</returns>
    /// <exception cref="InvalidOperationException">未配置密钥名、查无该名字或长度不是 32 字节时抛出。</exception>
    Task<string> LoadApiV2KeyAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取用并解析商户 API 证书（PKCS#12，<c>secapi/pay/*</c> 双向 TLS 用）。
    /// </summary>
    /// <param name="config">商户配置（提供 <c>ApiV2CertificateSecretName</c> 与可选的口令名）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>已加载私钥的证书实例（<b>调用方负责缓存与释放</b>）。</returns>
    /// <exception cref="InvalidOperationException">未配置、Base64 不可解析或 PKCS#12 口令错误时抛出。</exception>
    Task<X509Certificate2> LoadClientCertificateAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default);
}
