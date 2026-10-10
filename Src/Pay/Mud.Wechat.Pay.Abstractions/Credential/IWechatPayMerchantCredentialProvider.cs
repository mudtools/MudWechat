// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 商户凭据取用端口：私钥与 APIv3 密钥**只经组件 <c>ISecretProvider</c> 取用**（方案 v2 §2.2 第 5 条）。
/// </summary>
/// <remarks>
/// <para>
/// 落位在 <b>Abstractions</b> 而非主包 <c>Mud.Wechat.Pay</c>：回调包 <c>Mud.Wechat.Pay.Callback</c>
/// 解密 <c>resource</c> 时同样要拿 APIv3 密钥，而 <c>Callback → Pay</c> 会把整个业务接口面、
/// DTO 源生成与 DI 装配拖进回调包（与 P0-b 把密码学下沉到 Abstractions 是同一理由）。
/// </para>
/// <para>
/// <b>返回值所有权</b>：<see cref="LoadPrivateKeyAsync"/> 返回的 <c>RSA</c> 实例<b>归调用方所有</b>，
/// 用完必须 <c>Dispose</c>（私钥材料驻留内存的时间窗由此收敛）。
/// </para>
/// <para>
/// <b>失败语义</b>：查无密钥名 / PEM 不可解析 / 密钥长度不符，一律抛 <see cref="InvalidOperationException"/>，
/// 且<b>异常消息绝不回显密钥内容</b>（PAY-B7；否则私钥头会随日志平台与 APM 二次扩散）。
/// </para>
/// </remarks>
public interface IWechatPayMerchantCredentialProvider
{
    /// <summary>
    /// 取用并解析商户 RSA-2048 私钥。
    /// </summary>
    /// <param name="config">商户配置（提供 <c>PrivateKeySecretName</c> 与商户键）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>已导入私钥的 <c>RSA</c> 实例（<b>调用方负责释放</b>）。</returns>
    /// <exception cref="InvalidOperationException">密钥查无、PEM 不可解析时抛出。</exception>
    Task<RSA> LoadPrivateKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取用 APIv3 密钥（<b>AEAD_AES_256_GCM</b> 与请求侧敏感字段加密所用，32 字节）。
    /// </summary>
    /// <param name="config">商户配置（提供 <c>ApiKeySecretName</c> 与商户键）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>长度恰为 32 字节的密钥材料。</returns>
    /// <exception cref="InvalidOperationException">密钥查无或长度不是 32 字节时抛出。</exception>
    Task<byte[]> LoadApiKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default);
}
