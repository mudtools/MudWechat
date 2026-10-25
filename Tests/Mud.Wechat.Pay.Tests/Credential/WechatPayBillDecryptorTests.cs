// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Abstractions.Crypto;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.DataModels.Ecommerce;
using Moq;

namespace Mud.Wechat.Pay.Tests.Credential;

/// <summary>
/// 账单解密编排器测试（总案 D3，追踪号 MASTER-REMEDIATION-2026-10-24）：
/// AES/RSA 链路用<b>真实密码学</b>回环（BCL 内建）；SM2/SM4 链路用<b>回环 mock</b> 验证编排层的
/// 参数传递与结果传播——SM4/SM2 原语本身是宿主国密端口的契约（<see cref="IWechatSmCryptoProvider"/>），
/// 不属 SDK 测试面。官方口径（SKIT 同源）：RSA-OAEP-SHA1 解 encrypt_key、分片末尾 16 字节为 GCM 标签、
/// 关联数据为空、bill_sequence 从 1 起连续。
/// </summary>
public class WechatPayBillDecryptorTests
{
    private const string AesKeyText = "0123456789abcdef0123456789abcdef"; // 32 字节（AES-256）
    private const string Sm4KeyText = "0123456789abcdef"; // 16 字节（SM4）
    private const string NonceText = "123456789012"; // 12 字节

    private readonly WechatPayBillDecryptor _decryptor = new();

    // ---------- 正向：RSA + AES-256-GCM（真实回环） ----------

    [Fact]
    public void Decrypt_ShouldReturnMergedBill_WhenSingleRsaAesShard()
    {
        var plaintext = Encoding.UTF8.GetBytes("交易日,账单文本,金额\r\n2026-10-24,测试商户,100.00\r\n");
        var (key, shard) = BuildRsaAesShard(plaintext, sequence: 1);

        var result = _decryptor.Decrypt(key, SingleRequest(shard));

        result.MergedBill.Should().Equal(plaintext, "单分片账单的合并结果即该分片明文");
        result.ShardPlaintexts.Should().HaveCount(1);
    }

    [Fact]
    public void Decrypt_ShouldMergeInSequenceOrder_WhenShardsPassedOutOfOrder()
    {
        var first = Encoding.UTF8.GetBytes("第一片数据\r\n");
        var second = Encoding.UTF8.GetBytes("第二片数据\r\n");
        using var rsa = RSA.Create(2048); // 同一账单的分片共用同一商户证书 ⇒ 共用同一把 RSA 私钥
        var (_, shard1) = BuildRsaAesShard(rsa, first, sequence: 1);
        var (_, shard2) = BuildRsaAesShard(rsa, second, sequence: 2);

        var result = _decryptor.Decrypt(WechatPayMerchantKey.FromRsa(rsa), new WechatPayBillDecryptRequest
        {
            Algorithm = EcommerceBillAlgorithms.AeadAes256Gcm,
            Shards = new[] { shard2, shard1 }, // 故意乱序
        });

        result.MergedBill.Should().Equal(first.Concat(second).ToArray(), "合并须按 bill_sequence 升序而非入参顺序");
    }

    [Fact]
    public void Decrypt_ShouldDecompressGzip_WhenFlagSet()
    {
        var original = Encoding.UTF8.GetBytes("gzip 后的账单内容\r\n");
        var (key, shard) = BuildRsaAesShard(Compress(original), sequence: 1);

        var result = _decryptor.Decrypt(key, new WechatPayBillDecryptRequest
        {
            Algorithm = EcommerceBillAlgorithms.AeadAes256Gcm,
            IsGzipCompressed = true, // 官方 tar_type = GZIP
            Shards = new[] { shard },
        });

        result.MergedBill.Should().Equal(original, "tar_type=GZIP 时解密后逐片解压");
    }

    [Fact]
    public void Decrypt_ShouldVerifySha1Hash_WhenHashValueProvided()
    {
        var plaintext = Encoding.UTF8.GetBytes("带哈希校验的账单\r\n");
        var (key, shard) = BuildRsaAesShard(plaintext, sequence: 1);
        shard.HashType = EcommerceBillHashTypes.Sha1;
        shard.HashValue = Sha1Hex(plaintext);

        var act = () => _decryptor.Decrypt(key, SingleRequest(shard));

        act.Should().NotThrow("哈希值与解密解压后的明文一致时校验通过");
    }

    // ---------- 正向：SM2 + SM4（回环 mock，验证编排参数传递） ----------

    [Fact]
    public void Decrypt_ShouldDelegateToSmProvider_WhenSm2MerchantAndSm4Algorithm()
    {
        var smProvider = new Mock<IWechatSmCryptoProvider>();
        var plaintext = Encoding.UTF8.GetBytes("国密账单内容\r\n");
        var sm2KeyBytes = new byte[] { 0xAB, 0xCD, 0xEF, 0x01 };
        byte[]? capturedKey = null;
        byte[]? capturedNonce = null;
        byte[]? capturedAad = null;

        smProvider.Setup(p => p.DecryptSm2(It.IsAny<byte[]>(), It.IsAny<string>()))
            .Returns(Sm4KeyText);
        smProvider.Setup(p => p.TryDecryptSm4Gcm(
                It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<byte[]>(), out It.Ref<byte[]>.IsAny))
            .Callback(new TryDecryptSm4GcmCallback((byte[] key, byte[] nonce, byte[] cipher, byte[] aad, byte[] tag, out byte[] plain) =>
            {
                capturedKey = key;
                capturedNonce = nonce;
                capturedAad = aad;
                // 用 AES-128-GCM 以相同入参做真实 GCM 解密，锁定编排层交给端口的参数形态。
#pragma warning disable SYSLIB0053 // 测试固定 net8.0 单 TFM；带标签长度的构造在 net9+ 才有（与源工程 AesGcmPayloadCodec 的分支同源）
                using var aes = new AesGcm(key);
#pragma warning restore SYSLIB0053
                plain = new byte[cipher.Length];
                aes.Decrypt(nonce, cipher, tag, plain, aad);
            }))
            .Returns(true);

        var merchantKey = WechatPayMerchantKey.FromSm2(sm2KeyBytes, "SM2-WITH-SM3");
        var request = new WechatPayBillDecryptRequest
        {
            Algorithm = EcommerceBillAlgorithms.Sm4Gcm,
            Shards = new[] { BuildSmShard(smProvider.Object, plaintext, sequence: 1) },
        };

        var result = new WechatPayBillDecryptor(smProvider.Object).Decrypt(merchantKey, request);

        result.MergedBill.Should().Equal(plaintext, "编排层按端口的解密结果合并");
        capturedKey.Should().Equal(Encoding.UTF8.GetBytes(Sm4KeyText), "SM2 解出的 16 字节密钥应原样交给端口");
        capturedNonce.Should().Equal(Encoding.UTF8.GetBytes(NonceText), "nonce 以 UTF-8 编码为 12 字节交给端口");
        capturedAad.Should().BeEmpty("账单分片的 GCM 关联数据为空（官方口径）");
        smProvider.Verify(p => p.DecryptSm2(sm2KeyBytes, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Decrypt_ShouldFailFast_WhenSmProviderMissingAndSm4Requested()
    {
        var merchantKey = WechatPayMerchantKey.FromSm2(new byte[] { 0x01 }, "SM2-WITH-SM3");
        var request = new WechatPayBillDecryptRequest
        {
            Algorithm = EcommerceBillAlgorithms.Sm4Gcm,
            Shards = new[] { new WechatPayBillShard { Sequence = 1, EncryptedKeyBase64 = "AAAA", Nonce = NonceText, CipherBytes = new byte[64] } },
        };

        // 默认构造器无国密端口（NullWechatSmCryptoProvider）⇒ SM2 解密 fail-fast，绝不静默降级。
        var act = () => _decryptor.Decrypt(merchantKey, request);

        act.Should().Throw<NotSupportedException>().WithMessage("*IWechatSmCryptoProvider*");
    }

    // ---------- 负向：请求形态与完整性 ----------

    [Fact]
    public void Decrypt_ShouldThrow_WhenSequenceNotContiguousFromOne()
    {
        var (key, shard1) = BuildRsaAesShard(Encoding.UTF8.GetBytes("第一片\r\n"), sequence: 1);
        var (_, shard3) = BuildRsaAesShard(Encoding.UTF8.GetBytes("第三片\r\n"), sequence: 3);

        var act = () => _decryptor.Decrypt(key, new WechatPayBillDecryptRequest
        {
            Algorithm = EcommerceBillAlgorithms.AeadAes256Gcm,
            Shards = new[] { shard1, shard3 }, // 缺第 2 片
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*bill_sequence*", "缺片在合并前显式失败，不产出残缺账单");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenSha1HashMismatched()
    {
        var plaintext = Encoding.UTF8.GetBytes("将被篡改哈希的账单\r\n");
        var (key, shard) = BuildRsaAesShard(plaintext, sequence: 1);
        shard.HashType = EcommerceBillHashTypes.Sha1;
        shard.HashValue = new string('0', 40);

        var act = () => _decryptor.Decrypt(key, SingleRequest(shard));

        act.Should().Throw<CryptographicException>().WithMessage("*完整性校验失败*");
    }

    [Fact]
    public void Decrypt_ShouldThrowCryptographicException_WhenTagTampered()
    {
        var (key, shard) = BuildRsaAesShard(Encoding.UTF8.GetBytes("账单内容\r\n"), sequence: 1);
        shard.CipherBytes[^1] ^= 0xFF; // 破坏末尾标签

        var act = () => _decryptor.Decrypt(key, SingleRequest(shard));

        act.Should().Throw<CryptographicException>().WithMessage("*AES-256-GCM 解密失败*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenAlgorithmUnknown()
    {
        var (key, shard) = BuildRsaAesShard(Encoding.UTF8.GetBytes("账单内容\r\n"), sequence: 1);

        var act = () => _decryptor.Decrypt(key, new WechatPayBillDecryptRequest
        {
            Algorithm = "DES_CBC",
            Shards = new[] { shard },
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*算法*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenEncryptedKeyMissing()
    {
        var (key, shard) = BuildRsaAesShard(Encoding.UTF8.GetBytes("账单内容\r\n"), sequence: 1);
        shard.EncryptedKeyBase64 = null;

        var act = () => _decryptor.Decrypt(key, SingleRequest(shard));

        act.Should().Throw<InvalidOperationException>().WithMessage("*encrypt_key*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenCipherTooShort()
    {
        var (key, shard) = BuildRsaAesShard(Encoding.UTF8.GetBytes("账单内容\r\n"), sequence: 1);
        shard.CipherBytes = new byte[8]; // 不足 16 字节标签长度

        var act = () => _decryptor.Decrypt(key, SingleRequest(shard));

        act.Should().Throw<CryptographicException>().WithMessage("*密文长度非法*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenShardsEmpty()
    {
        var key = WechatPayMerchantKey.FromRsa(RSA.Create(2048));

        var act = () => _decryptor.Decrypt(key, new WechatPayBillDecryptRequest { Shards = Array.Empty<WechatPayBillShard>() });

        act.Should().Throw<InvalidOperationException>().WithMessage("*至少一个分片*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenMerchantKeyNull()
    {
        var act = () => _decryptor.Decrypt(null!, new WechatPayBillDecryptRequest());

        act.Should().Throw<ArgumentNullException>();
    }

    // ---------- 构造辅助 ----------

    private static WechatPayBillDecryptRequest SingleRequest(WechatPayBillShard shard) => new()
    {
        Algorithm = EcommerceBillAlgorithms.AeadAes256Gcm,
        Shards = new[] { shard },
    };

    /// <summary>单分片便捷形态：自建一把 RSA-2048（返回的 Key 持有它，测试期间保持存活）。</summary>
    private static (WechatPayMerchantKey Key, WechatPayBillShard Shard) BuildRsaAesShard(byte[] plaintext, int sequence)
    {
        var rsa = RSA.Create(2048);
        var (key, shard) = BuildRsaAesShard(rsa, plaintext, sequence);
        return (WechatPayMerchantKey.FromRsa(rsa), shard);
    }

    /// <summary>用真实 RSA-2048 + AES-256-GCM 构造一片「RSA 商户 + AES 账单」密文（官方形态：OAEP-SHA1 + 末尾 16 字节标签）。传入共享的 RSA 以覆盖多分片场景。</summary>
    private static (WechatPayMerchantKey Key, WechatPayBillShard Shard) BuildRsaAesShard(RSA rsa, byte[] plaintext, int sequence)
    {
        var encryptedKey = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(AesKeyText), RSAEncryptionPadding.OaepSHA1));
        var nonce = Encoding.UTF8.GetBytes(NonceText);

        byte[] cipherWithTag;
#pragma warning disable SYSLIB0053 // 测试固定 net8.0 单 TFM；带标签长度的构造在 net9+ 才有（与源工程 AesGcmPayloadCodec 的分支同源）
        using (var aes = new AesGcm(Encoding.UTF8.GetBytes(AesKeyText)))
#pragma warning restore SYSLIB0053
        {
            var cipher = new byte[plaintext.Length];
            var tag = new byte[16];
            aes.Encrypt(nonce, plaintext, cipher, tag, Array.Empty<byte>());
            cipherWithTag = cipher.Concat(tag).ToArray();
        }

        return (
            WechatPayMerchantKey.FromRsa(rsa),
            new WechatPayBillShard
            {
                Sequence = sequence,
                EncryptedKeyBase64 = encryptedKey,
                Nonce = NonceText,
                CipherBytes = cipherWithTag,
            });
    }

    /// <summary>构造一片「SM2 商户 + SM4 账单」密文（明文以 AES-128-GCM 本地加密，令编排层 mock 的解密侧可逆）。</summary>
    private static WechatPayBillShard BuildSmShard(IWechatSmCryptoProvider smProvider, byte[] plaintext, int sequence)
    {
        byte[] tag = new byte[16];
        byte[] cipher = new byte[plaintext.Length];
#pragma warning disable SYSLIB0053 // 同上：net8.0 无带标签长度构造
        using (var aes = new AesGcm(Encoding.UTF8.GetBytes(Sm4KeyText)))
#pragma warning restore SYSLIB0053
        {
            aes.Encrypt(Encoding.UTF8.GetBytes(NonceText), plaintext, cipher, tag, Array.Empty<byte>());
        }

        return new WechatPayBillShard
        {
            Sequence = sequence,
            // SM2 商户的 encrypt_key 由端口 DecryptSm2 解出（mock 恒返 Sm4KeyText），密文内容不影响断言。
            EncryptedKeyBase64 = Convert.ToBase64String(new byte[] { 0x00, 0x01 }),
            Nonce = NonceText,
            CipherBytes = cipher.Concat(tag).ToArray(),
        };
    }

    private static byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionLevel.SmallestSize))
        {
            gzip.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    private static string Sha1Hex(byte[] data)
    {
        using var sha1 = SHA1.Create();
        return Convert.ToHexString(sha1.ComputeHash(data)).ToLowerInvariant();
    }

    /// <summary>Moq 对带 <c>out</c> 参数方法的回调委托（AGENTS.md §7：out 参数用自定义 delegate）。</summary>
    private delegate void TryDecryptSm4GcmCallback(byte[] key, byte[] nonce, byte[] cipher, byte[] associatedData, byte[] tag, out byte[] plain);
}
