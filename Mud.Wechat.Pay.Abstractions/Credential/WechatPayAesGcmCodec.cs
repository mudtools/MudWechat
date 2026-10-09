// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// APIv3 <c>AEAD_AES_256_GCM</c> 载荷（nonce + ciphertext + tag），对应回调/平台证书下载报文里的
/// <c>resource</c> 三元组。
/// </summary>
public readonly struct WechatPayGcmPayload : IEquatable<WechatPayGcmPayload>
{
    /// <summary>初始化载荷。</summary>
    /// <param name="nonce">12 字节随机数。</param>
    /// <param name="ciphertext">密文（不含 tag）。</param>
    /// <param name="tag">16 字节认证标签。</param>
    public WechatPayGcmPayload(byte[] nonce, byte[] ciphertext, byte[] tag)
    {
        Nonce = nonce ?? throw new ArgumentNullException(nameof(nonce));
        Ciphertext = ciphertext ?? throw new ArgumentNullException(nameof(ciphertext));
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
    }

    /// <summary>12 字节 nonce。</summary>
    public byte[] Nonce { get; }

    /// <summary>密文字节（不含 tag）。</summary>
    public byte[] Ciphertext { get; }

    /// <summary>16 字节认证标签。</summary>
    public byte[] Tag { get; }

    /// <inheritdoc />
    public bool Equals(WechatPayGcmPayload other)
        => Nonce.AsSpan().SequenceEqual(other.Nonce)
           && Ciphertext.AsSpan().SequenceEqual(other.Ciphertext)
           && Tag.AsSpan().SequenceEqual(other.Tag);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WechatPayGcmPayload other && Equals(other);

    /// <inheritdoc />
    /// <remarks>
    /// 手写而非 <c>HashCode.AddBytes</c>：后者是 .NET 9+ 才有的 API，本包还覆盖 net6.0/net8.0。
    /// 采用「逐字节滚动」而非把数组**引用**喂给 <c>HashCode.Add</c>（后者只比引用，同内容不同实例会得到不同散列）。
    /// </remarks>
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            Accumulate(ref hash, Nonce);
            Accumulate(ref hash, Ciphertext);
            Accumulate(ref hash, Tag);
            return hash;
        }
    }

    private static void Accumulate(ref int hash, byte[] data)
    {
        foreach (var b in data)
        {
            hash = (hash * 31) + b;
        }
    }
}

/// <summary>
/// APIv3 对称加解密：<c>AEAD_AES_256_GCM</c>（PAY-B4）。
/// </summary>
/// <remarks>
/// <para>
/// <b>红线</b>：**禁止**把企微 XML 回调的「AES-CBC + 32 字节块 PKCS7」套到这里（方案 v2 §2.6、AGENTS §5.5）。
/// 两族算法的填充、块大小、认证语义全都不同；混用会在**编译期不报错**、运行期才炸，
/// 故同批新增了 <c>PayCallbackScaffoldContractGuards</c> 的源码级禁令。
/// </para>
/// <para>
/// <b>本类型不依赖 XML 与信封</b>：只做字节到字节的加解密，回调侧的 <c>resource</c> JSON 解析归
/// <c>Mud.Wechat.Pay.Callback</c>，端点侧的敏感字段加密归后续 P1 域。
/// </para>
/// <para>
/// 之所以能放本包：<c>AesGcm</c> 在 <c>netstandard2.0</c> **不存在**，故支付线四包统一去掉该 TFM
/// （AB-G8 锁定），这里天然只跑在 net6.0+ 上。
/// </para>
/// </remarks>
public static class WechatPayAesGcmCodec
{
    /// <summary>官方算法标识（回调 <c>resource.algorithm</c> 的合法取值）。</summary>
    public const string Algorithm = "AEAD_AES_256_GCM";

    /// <summary>APIv3 密钥长度：32 字节（256 位）。</summary>
    public const int KeySizeBytes = 32;

    /// <summary>GCM nonce 长度：12 字节。</summary>
    public const int NonceSizeBytes = 12;

    /// <summary>GCM 认证标签长度：16 字节。</summary>
    public const int TagSizeBytes = 16;

    /// <summary>
    /// 构造 <see cref="AesGcm"/>：<c>net9.0+</c> 起 <c>AesGcm(byte[])</c> 被标记过时（SYSLIB0053，
    /// 要求显式声明标签长度），故高版本走带标签长度的构造；<c>net6.0</c>/<c>net8.0</c> 无该重载，回退旧构造。
    /// </summary>
    /// <remarks>两分支语义<b>完全等价</b>：旧构造的标签长度恒为 16 字节，非本常量之外的值。</remarks>
    /// <param name="key">32 字节 APIv3 密钥。</param>
    /// <returns><see cref="AesGcm"/> 实例（调用方负责释放）。</returns>
    private static AesGcm CreateAesGcm(byte[] key)
#if NET9_0_OR_GREATER
        => new(key, TagSizeBytes);
#else
        => new(key);
#endif

    /// <summary>
    /// 加密。nonce 每次**密码学随机**生成，绝不复用（GCM 复用 nonce 会直接泄露明文异或值）。
    /// </summary>
    /// <param name="key">32 字节 APIv3 密钥（<b>不得</b>入日志，PAY-B7）。</param>
    /// <param name="plaintext">明文字节。</param>
    /// <param name="associatedData">关联数据（<c>resource.associated_data</c>）；<c>null</c>/<c>空</c> 表示无。</param>
    /// <returns>nonce + 密文 + tag。</returns>
    /// <exception cref="ArgumentNullException">任一必填参数为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> 长度不是 32 字节。</exception>
    public static WechatPayGcmPayload Encrypt(byte[] key, byte[] plaintext, byte[]? associatedData = null)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (plaintext is null)
        {
            throw new ArgumentNullException(nameof(plaintext));
        }

        if (key.Length != KeySizeBytes)
        {
            throw new ArgumentException($"APIv3 密钥必须为 {KeySizeBytes} 字节。", nameof(key));
        }

        var nonce = new byte[NonceSizeBytes];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSizeBytes];

        using (var aes = CreateAesGcm(key))
        {
            // 注：不要写 `associatedData ?? ReadOnlySpan<byte>.Empty` —— ?? 的右侧须能转成左侧类型
            // （byte[]），Span 不满足，会编译失败。先归一为 byte[]，再由隐式转换升为 ReadOnlySpan。
            aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData ?? Array.Empty<byte>());
        }

        return new WechatPayGcmPayload(nonce, ciphertext, tag);
    }

    /// <summary>
    /// 解密。**fail-closed 且不泄漏**（PAY-B4）：认证失败 / 长度非法 / 密钥错误一律返回 <c>false</c>、
    /// <paramref name="plaintext"/> 为 <c>null</c>，**绝不抛出**——
    /// 异常消息里会带上密文字节，等于把 <c>resource.ciphertext</c> 洒进日志与遥测。
    /// </summary>
    /// <param name="key">32 字节 APIv3 密钥。</param>
    /// <param name="nonce">12 字节 nonce。</param>
    /// <param name="ciphertext">密文字节（不含 tag）。</param>
    /// <param name="tag">16 字节认证标签。</param>
    /// <param name="associatedData">关联数据；<c>null</c>/<c>空</c> 表示无。</param>
    /// <param name="plaintext">成功时为明文字节；失败时恒为 <c>null</c>。</param>
    /// <returns>是否解密成功。</returns>
    public static bool TryDecrypt(
        byte[]? key,
        byte[]? nonce,
        byte[]? ciphertext,
        byte[]? tag,
        byte[]? associatedData,
        out byte[]? plaintext)
    {
        plaintext = null;

        if (key is null || key.Length != KeySizeBytes)
        {
            return false;
        }

        if (nonce is null || nonce.Length != NonceSizeBytes)
        {
            return false;
        }

        if (tag is null || tag.Length != TagSizeBytes)
        {
            return false;
        }

        if (ciphertext is null)
        {
            return false;
        }

        // 认证解密：先写入缓冲区，标签校验失败时该缓冲区内容不可信 ⇒ 立即清零后再返回。
        var buffer = new byte[ciphertext.Length];
        try
        {
            using (var aes = CreateAesGcm(key))
            {
                aes.Decrypt(nonce, ciphertext, tag, buffer, associatedData ?? Array.Empty<byte>());
            }
        }
        catch (CryptographicException)
        {
            CryptographicOperations.ZeroMemory(buffer);
            return false;
        }
        catch (ArgumentException)
        {
            CryptographicOperations.ZeroMemory(buffer);
            return false;
        }

        plaintext = buffer;
        return true;
    }

    /// <summary>
    /// 校验载荷三段长度是否合形状（**不**做解密），供回调侧在进入密码学之前先做廉价过滤。
    /// </summary>
    /// <param name="nonce">nonce。</param>
    /// <param name="ciphertext">密文。</param>
    /// <param name="tag">标签。</param>
    /// <returns>是否合形状。</returns>
    public static bool HasValidShape(byte[]? nonce, byte[]? ciphertext, byte[]? tag)
        => nonce is { Length: NonceSizeBytes }
           && ciphertext is not null
           && tag is { Length: TagSizeBytes };
}
