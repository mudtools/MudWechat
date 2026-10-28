// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>商户密钥抽象（承载 RSA / SM2 两种算法的私钥材料）。</summary>
/// <remarks>
/// <para><b>为何需要此抽象</b>：既有 <c>IWechatPayMerchantCredentialProvider.LoadPrivateKeyAsync</c> 返回 <c>RSA</c>，
/// 结构上无法承载 SM2 私钥（BCL <c>RSA</c> 只能装 RSA 曲线密钥）。国密链路要求同时支持 RSA-2048 与 SM2 两种签名算法，
/// 故需统一抽象。</para>
/// <para><b>使用方式</b>：调用方通过 <see cref="Algorithm"/> 判断密钥类型，再走对应分支处理。</para>
/// </remarks>
public sealed class WechatPayMerchantKey : IDisposable
{
    /// <summary>RSA 私钥（仅当 <see cref="Algorithm"/> = <c>SHA256-RSA2048</c> 时有值）。</summary>
    public RSA? RsaKey { get; }

    /// <summary>SM2 私钥字节（仅当 <see cref="Algorithm"/> = <c>SM2-WITH-SM3</c> 时有值；格式由实现方定义）。</summary>
    public byte[]? Sm2PrivateKey { get; }

    /// <summary>签名算法标记（决定使用哪条密钥路径）。</summary>
    public string Algorithm { get; }

    private WechatPayMerchantKey(RSA? rsaKey, byte[]? sm2PrivateKey, string algorithm)
    {
        // 二选一：要么有 RSA，要么有 SM2，不能同时为空或同时有值
        if ((rsaKey == null) == (sm2PrivateKey == null))
        {
            throw new ArgumentException("必须且只能提供一种密钥材料（RSA 或 SM2）。", nameof(algorithm));
        }

        RsaKey = rsaKey;
        Sm2PrivateKey = sm2PrivateKey;
        Algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
    }

    /// <summary>创建 RSA 密钥实例。</summary>
    public static WechatPayMerchantKey FromRsa(RSA rsaKey, string algorithm = "SHA256-RSA2048")
        => new(rsaKey ?? throw new ArgumentNullException(nameof(rsaKey)), null, algorithm);

    /// <summary>创建 SM2 密钥实例。</summary>
    /// <param name="sm2PrivateKey">SM2 私钥字节（格式由实现方定义，通常为 DER 编码或 PEM 解码后的原始字节）。</param>
    /// <param name="algorithm">算法标识（默认 <c>SM2-WITH-SM3</c>）。</param>
    public static WechatPayMerchantKey FromSm2(byte[] sm2PrivateKey, string algorithm = "SM2-WITH-SM3")
        => new(null, sm2PrivateKey ?? throw new ArgumentNullException(nameof(sm2PrivateKey)), algorithm);

    /// <summary>释放 RSA 密钥（SM2 字节数组无需特殊释放）。</summary>
    public void Dispose()
    {
        RsaKey?.Dispose();
    }
}
