// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 账单文件的<b>单个密文分片</b>（解密入参，与官方 <c>download_bill_list[]</c> 元素 / 直连账单下载报文对应）。
/// </summary>
/// <remarks>
/// <para><b>字段与官方来源的对应</b>：<see cref="EncryptedKeyBase64"/> ← <c>encrypt_key</c>、
/// <see cref="Nonce"/> ← <c>nonce</c>、<see cref="HashType"/>/<see cref="HashValue"/> ← <c>hash_type</c>/<c>hash_value</c>、
/// <see cref="Sequence"/> ← <c>bill_sequence</c>（电商二级商户资金账单逐片携带）；直连账单的 <c>encrypt_key</c>/<c>nonce</c>
/// 在下载报文体的 JSON 段内，宿主解析后同样填入本类型。</para>
/// <para><b>密文分片形态</b>：<see cref="CipherBytes"/> 是从 <c>download_url</c> 下载到的<b>原始字节</b>——
/// 末尾 16 字节为 GCM 认证标签，编排层负责切分；前置 JSON 段（直连形态）须由宿主先行剥离。</para>
/// </remarks>
public sealed class WechatPayBillShard
{
    /// <summary>
    /// 账单文件序号（官方 <c>bill_sequence</c>，<b>从 1 起</b>）：合并顺序依据；编排层校验序号从 1 连续（防缺片静默拼错账）。
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>加密密钥（官方 <c>encrypt_key</c>）：以商户证书公钥加密后 Base64；须以商户私钥解出对称密钥。</summary>
    public string? EncryptedKeyBase64 { get; set; }

    /// <summary>随机字符串（官方 <c>nonce</c>）：GCM 随机数（UTF-8 编码后 12 字节）。</summary>
    public string? Nonce { get; set; }

    /// <summary>分片密文（<c>download_url</c> 的原始下载字节；末尾 16 字节为 GCM 认证标签）。</summary>
    public byte[]? CipherBytes { get; set; }

    /// <summary>哈希类型（官方 <c>hash_type</c>）：当前取值 <see cref="Mud.Wechat.Pay.DataModels.Ecommerce.EcommerceBillHashTypes.Sha1"/>；可空 = 跳过完整性校验。</summary>
    public string? HashType { get; set; }

    /// <summary>哈希值（官方 <c>hash_value</c>）：对「解密 + 解压后」的原始账单分片的摘要；可空 = 跳过完整性校验。</summary>
    public string? HashValue { get; set; }
}

/// <summary>账单解密请求（一次调用覆盖一份账单的全部分片）。</summary>
public sealed class WechatPayBillDecryptRequest
{
    /// <summary>
    /// 账单文件加密算法（官方 <c>algorithm</c>）：取值见
    /// <see cref="Mud.Wechat.Pay.DataModels.Ecommerce.EcommerceBillAlgorithms"/>（<c>AEAD_AES_256_GCM</c> / <c>SM4_GCM</c>）。
    /// </summary>
    public string Algorithm { get; set; } = Mud.Wechat.Pay.DataModels.Ecommerce.EcommerceBillAlgorithms.AeadAes256Gcm;

    /// <summary>密文分片列表（至少一片；按 <see cref="WechatPayBillShard.Sequence"/> 升序合并）。</summary>
    public IReadOnlyList<WechatPayBillShard>? Shards { get; set; }

    /// <summary>
    /// 分片明文是否为 gzip 压缩（官方 <c>tar_type = GZIP</c> 时为 <see langword="true"/>）：
    /// <see langword="true"/> 时编排层在解密后<b>逐片解压</b>，再做哈希校验与合并。
    /// </summary>
    public bool IsGzipCompressed { get; set; }
}

/// <summary>账单解密结果。</summary>
public sealed class WechatPayBillDecryptResult
{
    /// <summary>初始化解密结果。</summary>
    /// <param name="shardPlaintexts">逐片明文（已按 <see cref="WechatPayBillDecryptRequest.IsGzipCompressed"/> 解压、已通过完整性校验，按分片顺序排列）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="shardPlaintexts"/> 为 <see langword="null"/>。</exception>
    public WechatPayBillDecryptResult(IReadOnlyList<byte[]> shardPlaintexts)
    {
        ShardPlaintexts = shardPlaintexts ?? throw new ArgumentNullException(nameof(shardPlaintexts));
        var merged = new byte[shardPlaintexts.Sum(static s => s.Length)];
        var offset = 0;
        foreach (var shard in shardPlaintexts)
        {
            Buffer.BlockCopy(shard, 0, merged, offset, shard.Length);
            offset += shard.Length;
        }

        MergedBill = merged;
    }

    /// <summary>逐片明文（顺序与请求分片的 <see cref="WechatPayBillShard.Sequence"/> 升序一致）。</summary>
    public IReadOnlyList<byte[]> ShardPlaintexts { get; }

    /// <summary>全量账单明文（逐片明文按序合并；可直接按 CSV 解析）。</summary>
    public byte[] MergedBill { get; }
}
