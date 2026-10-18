// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;

namespace Mud.Wechat.Abstractions.Crypto;

/// <summary>SM4-GCM 密钥派生辅助类（静态方法，避免 ns2.0 DIM 限制）。</summary>
/// <remarks>
/// <para>官方规则：SM4 密钥 = SM3(APIv3 密钥) 摘要（256bit）取前 128bit（16 字节）。</para>
/// <para><see href="https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html">APIv3 国密指引</see></para>
/// </remarks>
public static class WechatSm4KeyDerivation
{
    /// <summary>SM4-GCM 密钥长度（字节）：16 字节（128 位）。</summary>
    public const int Sm4KeySizeBytes = 16;

    /// <summary>从 APIv3 密钥派生 SM4-GCM 密钥（SM3 摘要取前 16 字节）。</summary>
    /// <param name="provider">国密算法提供者（用于计算 SM3 摘要）。</param>
    /// <param name="apiV3Key">商户平台配置的 APIv3 密钥（UTF-8 字符串）。</param>
    /// <returns>16 字节 SM4 密钥。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> 或 <paramref name="apiV3Key"/> 为 null。</exception>
    /// <exception cref="ArgumentException"><paramref name="apiV3Key"/> 为空字符串。</exception>
    public static byte[] FromApiV3Key(IWechatSmCryptoProvider provider, string apiV3Key)
    {
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        if (apiV3Key == null) throw new ArgumentNullException(nameof(apiV3Key));
        if (apiV3Key.Length == 0) throw new ArgumentException("APIv3 密钥不能为空。", nameof(apiV3Key));

        var keyBytes = Encoding.UTF8.GetBytes(apiV3Key);
        var digest = provider.Sm3Digest(keyBytes);

        // 官方规则：取 SM3 摘要（32 字节）的前 16 字节作为 SM4 密钥
        var sm4Key = new byte[Sm4KeySizeBytes];
        Array.Copy(digest, 0, sm4Key, 0, Sm4KeySizeBytes);
        return sm4Key;
    }
}
