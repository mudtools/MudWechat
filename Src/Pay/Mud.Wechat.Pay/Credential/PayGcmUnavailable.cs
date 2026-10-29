// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NETSTANDARD2_0

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <c>netstandard2.0</c> 的 AES-GCM 占位实现：<c>AesGcm</c> 自 <c>netstandard2.1</c> / <c>netcoreapp3.0</c>
/// 起才进 BCL，官方也没有 downlevel 回填包（<c>Microsoft.Bcl.Cryptography</c> 只补 X509 / KDF 面）。
/// </summary>
/// <remarks>
/// <para>
/// 本类型只负责让 APIv3 加解密端口在 <c>netstandard2.0</c> 下<b>可编译</b>；任何真实加解密一律
/// <b>fail-fast</b> —— <b>不</b>静默降级为 AES-CBC、<b>不</b>自研 GCM、<b>不</b>引入第三方密码学库。
/// </para>
/// <para>
/// 因此该目标框架下的可用面 = APIv2（MD5 / HMAC-SHA256）、签名验签、DTO、配置与模块注册；
/// 需要 GCM 的 APIv3 路径（回调 resource 解密、账单解密、平台证书解密）会抛
/// <see cref="PlatformNotSupportedException"/> 并指明改用 net6.0+ 宿主。
/// </para>
/// </remarks>
internal sealed class PayGcmUnavailable : IDisposable
{
    private const string UnsupportedMessage =
        "netstandard2.0 不含 AesGcm：微信支付 APIv3 的 AEAD_AES_256_GCM 加解密需要 net6.0 及以上目标框架；"
        + "请把宿主升级到 net6.0+，或对 APIv2 之外的能力改用 net6.0+ 版本的 Mud.Wechat.Pay。";

    internal PayGcmUnavailable(byte[] key)
    {
        _ = key;
        throw new PlatformNotSupportedException(UnsupportedMessage);
    }

    internal void Encrypt(byte[] nonce, byte[] plaintext, byte[] ciphertext, byte[] tag, byte[] associatedData)
    {
        _ = nonce;
        _ = plaintext;
        _ = ciphertext;
        _ = tag;
        _ = associatedData;
        throw new PlatformNotSupportedException(UnsupportedMessage);
    }

    internal void Decrypt(byte[] nonce, byte[] ciphertext, byte[] tag, byte[] plaintext, byte[] associatedData)
    {
        _ = nonce;
        _ = ciphertext;
        _ = tag;
        _ = plaintext;
        _ = associatedData;
        throw new PlatformNotSupportedException(UnsupportedMessage);
    }

    public void Dispose()
    {
    }
}

#endif
