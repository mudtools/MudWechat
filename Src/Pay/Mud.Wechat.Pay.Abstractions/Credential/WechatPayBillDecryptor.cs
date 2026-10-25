// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Abstractions.Crypto;
using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 账单解密编排器（<see cref="IWechatPayBillDecryptor"/> 的默认实现）：
/// 逐片完成「密钥解出 → GCM 解密 →（可选）gzip 解压 → 哈希校验」，再按序合并。
/// </summary>
/// <remarks>
/// <para><b>双算法矩阵全覆盖</b>：对称层 <c>AEAD_AES_256_GCM</c>（BCL <see cref="AesGcm"/>）与
/// <c>SM4_GCM</c>（经 <see cref="IWechatSmCryptoProvider"/> 端口）；密钥层 RSA 商户走
/// <see cref="RSA.Decrypt(byte[], RSAEncryptionPadding)"/>（OAEP-SHA1，官方口径）、SM2 商户走
/// <see cref="IWechatSmCryptoProvider.DecryptSm2"/>（C1C3C2_ASN1）。</para>
/// <para><b>SM 端口缺席即 fail-fast</b>：默认 <see cref="NullWechatSmCryptoProvider"/> 抛
/// <see cref="NotSupportedException"/>——绝不静默降级。宿主注册真实国密提供者后 SM4/SM2 链路方可使用。</para>
/// <para><b>缺片防护</b>：官方 <c>bill_sequence</c> 从 1 起 ⇒ 合并前校验序号<b>从 1 连续</b>，
/// 缺片（如只下载了 {1,3}）在编排层即失败，而不是静默拼出残缺账单。</para>
/// </remarks>
public sealed class WechatPayBillDecryptor : IWechatPayBillDecryptor
{
    private const int AesKeySizeBytes = 32;
    private const int Sm4KeySizeBytes = 16;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    private readonly IWechatSmCryptoProvider _smProvider;

    /// <summary>初始化账单解密编排器。</summary>
    /// <param name="smProvider">国密算法提供者（可空 ⇒ <see cref="NullWechatSmCryptoProvider"/>，SM2/SM4 链路 fail-fast）。</param>
    public WechatPayBillDecryptor(IWechatSmCryptoProvider? smProvider = null)
        => _smProvider = smProvider ?? new NullWechatSmCryptoProvider();

    /// <inheritdoc />
    public WechatPayBillDecryptResult Decrypt(WechatPayMerchantKey merchantKey, WechatPayBillDecryptRequest request)
    {
        if (merchantKey is null)
        {
            throw new ArgumentNullException(nameof(merchantKey));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var shards = request.Shards;
        if (shards is null || shards.Count == 0)
        {
            throw new InvalidOperationException("账单解密请求必须携带至少一个分片（WechatPayBillDecryptRequest.Shards）。");
        }

        if (request.Algorithm != AesGcmPayloadCodec.Algorithm && request.Algorithm != Sm4GcmPayloadCodec.Algorithm)
        {
            throw new InvalidOperationException(
                $"账单算法「{request.Algorithm}」不在支持面（{AesGcmPayloadCodec.Algorithm} / {Sm4GcmPayloadCodec.Algorithm}）。");
        }

        var ordered = shards.OrderBy(static s => s.Sequence).ToArray();
        ValidateSequences(ordered);

        // 对称密钥逐片解出（官方逐片自带 encrypt_key/nonce，即使同账单也可能逐片不同）。
        var plaintexts = new List<byte[]>(ordered.Length);
        foreach (var shard in ordered)
        {
            plaintexts.Add(DecryptShard(merchantKey, request.Algorithm, request.IsGzipCompressed, shard));
        }

        return new WechatPayBillDecryptResult(plaintexts);
    }

    /// <summary>解密单分片：密钥解出 → GCM → 解压 → 哈希校验。</summary>
    private byte[] DecryptShard(WechatPayMerchantKey merchantKey, string algorithm, bool isGzip, WechatPayBillShard shard)
    {
        var keyBytes = ResolveSymmetricKey(merchantKey, algorithm, shard.EncryptedKeyBase64, shard.Sequence);
        var nonce = ResolveNonce(shard.Nonce, shard.Sequence);
        var (ciphertext, tag) = SplitTag(shard.CipherBytes, shard.Sequence);
        var plaintext = algorithm == Sm4GcmPayloadCodec.Algorithm
            ? DecryptSm4Gcm(keyBytes, nonce, ciphertext, tag, shard.Sequence)
            : DecryptAesGcm(keyBytes, nonce, ciphertext, tag, shard.Sequence);

        if (isGzip)
        {
            plaintext = DecompressGzip(plaintext, shard.Sequence);
        }

        VerifyHash(plaintext, shard.HashType, shard.HashValue, shard.Sequence);
        return plaintext;
    }

    /// <summary>解出分片的对称密钥（RSA-OAEP-SHA1 / SM2 C1C3C2；解出结果为密钥字符串，UTF-8 编码后即密钥字节）。</summary>
    private byte[] ResolveSymmetricKey(WechatPayMerchantKey merchantKey, string algorithm, string? encryptedKeyBase64, int sequence)
    {
        var expectedKeyLength = algorithm == Sm4GcmPayloadCodec.Algorithm ? Sm4KeySizeBytes : AesKeySizeBytes;
        if (string.IsNullOrWhiteSpace(encryptedKeyBase64))
        {
            throw new InvalidOperationException($"分片 {sequence} 缺少 encrypt_key（官方逐片携带，不可共用）。");
        }

        string keyText;
        if (merchantKey.RsaKey is not null)
        {
            byte[] encrypted;
            try
            {
                encrypted = Convert.FromBase64String(encryptedKeyBase64);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException($"分片 {sequence} 的 encrypt_key 不是合法 Base64。", ex);
            }

            try
            {
                // 官方口径：encrypt_key 以商户证书公钥 RSA/ECB/OAEPWithSHA-1AndMGF1Padding 加密。
                keyText = Encoding.UTF8.GetString(merchantKey.RsaKey.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA1));
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException($"分片 {sequence} 的 encrypt_key 解密失败（私钥不匹配或密文损坏）。", ex);
            }
        }
        else if (merchantKey.Sm2PrivateKey is not null)
        {
            // 官方口径：国密商户的 encrypt_key 以 SM2 C1C3C2_ASN1 加密；由国密端口解出（未注册即 NotSupportedException）。
            keyText = _smProvider.DecryptSm2(merchantKey.Sm2PrivateKey, encryptedKeyBase64);
        }
        else
        {
            throw new InvalidOperationException(
                $"商户私钥材料既无 RSA 也无 SM2（Algorithm: {merchantKey.Algorithm}），无法解出账单对称密钥。");
        }

        var keyBytes = Encoding.UTF8.GetBytes(keyText);
        if (keyBytes.Length != expectedKeyLength)
        {
            throw new CryptographicException(
                $"分片 {sequence} 解出的对称密钥长度非法（期望 {expectedKeyLength} 字节，实际 {keyBytes.Length} 字节）——私钥或密文疑似不匹配。");
        }

        return keyBytes;
    }

    /// <summary>校验并解析 GCM nonce（UTF-8 编码后 12 字节）。</summary>
    private static byte[] ResolveNonce(string? nonce, int sequence)
    {
        if (string.IsNullOrEmpty(nonce))
        {
            throw new InvalidOperationException($"分片 {sequence} 缺少 nonce。");
        }

        var bytes = Encoding.UTF8.GetBytes(nonce);
        if (bytes.Length != NonceSizeBytes)
        {
            throw new CryptographicException($"分片 {sequence} 的 nonce 须为 {NonceSizeBytes} 字节（UTF-8），实际 {bytes.Length} 字节。");
        }

        return bytes;
    }

    /// <summary>切分「密文 + 末尾 16 字节 GCM 标签」。</summary>
    private static (byte[] Ciphertext, byte[] Tag) SplitTag(byte[]? cipherBytes, int sequence)
    {
        if (cipherBytes is null || cipherBytes.Length <= TagSizeBytes)
        {
            throw new CryptographicException(
                $"分片 {sequence} 的密文长度非法（须大于标签长度 {TagSizeBytes} 字节）——确认传入的是 download_url 的原始字节。");
        }

        var ciphertextLength = cipherBytes.Length - TagSizeBytes;
        var ciphertext = new byte[ciphertextLength];
        var tag = new byte[TagSizeBytes];
        Buffer.BlockCopy(cipherBytes, 0, ciphertext, 0, ciphertextLength);
        Buffer.BlockCopy(cipherBytes, ciphertextLength, tag, 0, TagSizeBytes);
        return (ciphertext, tag);
    }

    /// <summary>AES-256-GCM 解密（账单关联数据为空；认证失败抛 <see cref="CryptographicException"/>）。</summary>
    private static byte[] DecryptAesGcm(byte[] key, byte[] nonce, byte[] ciphertext, byte[] tag, int sequence)
    {
        var buffer = new byte[ciphertext.Length];
        try
        {
            using (var aes = CreateAesGcm(key))
            {
                aes.Decrypt(nonce, ciphertext, tag, buffer, Array.Empty<byte>());
            }
        }
        catch (CryptographicException ex)
        {
            CryptographicOperations.ZeroMemory(buffer);
            throw new CryptographicException($"分片 {sequence} 的 AES-256-GCM 解密失败（标签校验不过：密文或 nonce 损坏）。", ex);
        }
        catch (ArgumentException ex)
        {
            CryptographicOperations.ZeroMemory(buffer);
            throw new CryptographicException($"分片 {sequence} 的 AES-256-GCM 解密失败（参数非法）。", ex);
        }

        return buffer;
    }

    /// <summary>SM4-GCM 解密（经国密端口；tag 校验失败抛 <see cref="CryptographicException"/>）。</summary>
    private byte[] DecryptSm4Gcm(byte[] key, byte[] nonce, byte[] ciphertext, byte[] tag, int sequence)
    {
        if (!_smProvider.TryDecryptSm4Gcm(key, nonce, ciphertext, Array.Empty<byte>(), tag, out var plaintext)
            || plaintext is null)
        {
            throw new CryptographicException($"分片 {sequence} 的 SM4-GCM 解密失败（标签校验不过，或国密端口返回失败）。");
        }

        return plaintext;
    }

    /// <summary>gzip 解压（官方 <c>tar_type = GZIP</c> 时，分片明文为压缩包）。</summary>
    private static byte[] DecompressGzip(byte[] compressed, int sequence)
    {
        try
        {
            using var input = new MemoryStream(compressed);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            return output.ToArray();
        }
        catch (InvalidDataException ex)
        {
            throw new CryptographicException($"分片 {sequence} 的 gzip 解压失败——确认 IsGzipCompressed 与官方 tar_type 一致。", ex);
        }
    }

    /// <summary>完整性校验（<c>hash_type</c>/<c>hash_value</c> 缺失即跳过；SM3 经国密端口，SHA1 走 BCL）。</summary>
    private void VerifyHash(byte[] plaintext, string? hashType, string? hashValue, int sequence)
    {
        if (string.IsNullOrWhiteSpace(hashType) || string.IsNullOrWhiteSpace(hashValue))
        {
            return;
        }

        string actual;
        if (string.Equals(hashType, EcommerceBillHashTypes.Sha1, StringComparison.Ordinal))
        {
            using var sha1 = SHA1.Create();
            actual = ToHexLower(sha1.ComputeHash(plaintext));
        }
        else if (string.Equals(hashType, "SM3", StringComparison.Ordinal))
        {
            actual = _smProvider.Sm3DigestHex(plaintext);
        }
        else
        {
            throw new InvalidOperationException(
                $"分片 {sequence} 的 hash_type「{hashType}」不在支持面（当前：{EcommerceBillHashTypes.Sha1} / SM3）。");
        }

        // 官方哈希为十六进制小写；忽略大小写比较以容忍上游大小写口径。
        if (!string.Equals(actual, hashValue.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new CryptographicException(
                $"分片 {sequence} 的 {hashType} 完整性校验失败（期望 {hashValue}，实际 {actual}）——账单密文在传输或存储中损坏。");
        }
    }

    /// <summary>分片序号校验：必须从 1 起连续（官方 bill_sequence 语义），缺片在合并前即失败。</summary>
    private static void ValidateSequences(WechatPayBillShard[] ordered)
    {
        for (var i = 0; i < ordered.Length; i++)
        {
            if (ordered[i].Sequence != i + 1)
            {
                throw new InvalidOperationException(
                    $"分片序号须从 1 起连续（官方 bill_sequence 语义），实际在第 {i + 1} 位遇到序号 {ordered[i].Sequence}"
                    + "——疑似缺片，合并残缺账单不如显式失败。");
            }
        }
    }

    /// <summary>
    /// AesGcm 构造的 TFM 分支：net10 可显式声明标签长度；net6/net8 走旧构造
    /// （与 <see cref="AesGcmPayloadCodec"/> 的同源处理一致）。
    /// </summary>
    private static AesGcm CreateAesGcm(byte[] key)
#if NET9_0_OR_GREATER
        => new(key, TagSizeBytes);
#else
        => new(key);
#endif

    /// <summary>字节转小写十六进制（netstandard2.0 无 <c>Convert.ToHexString</c>，与回调密码学内核同源处理）。</summary>
    private static string ToHexLower(byte[] bytes)
    {
#if NET5_0_OR_GREATER
        return Convert.ToHexString(bytes).ToLowerInvariant();
#else
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
#endif
    }
}
