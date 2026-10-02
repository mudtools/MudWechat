// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Cryptography;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调加解密实现（对齐微信消息加解密协议）。
/// </summary>
/// <remarks>
/// <para>
/// 原主包 <c>WechatWorkHttpClient.EncryptContent</c>（NotImplementedException）的加解密职责移入本类。
/// </para>
/// <para>
/// 解密协议（90968）：Base64 解码密文 → AES-256-CBC 解密（Key = Base64Decode(EncodingAESKey + "=")，
/// IV = Key 前 16 字节，<b>PKCS#7 填充至 32 字节的倍数</b>）→ 手工剥离填充 →
/// 明文 = 16 字节随机串 + 4 字节网络序消息长度 + 消息 + receiveid。
/// </para>
/// <para>
/// <b>P0-1</b>：官方填充块为 32 字节（pad ∈ [1..32]，已对齐时补满 32 字节），而非 .NET 内置
/// PKCS7 的 16 字节块校验（pad ∈ [17..32] 时解密必抛 <c>CryptographicException</c>）——
/// 本类统一使用 <see cref="PaddingMode.None"/> 并手工补位/剥离 32 块 PKCS7（决策 D1）。
/// </para>
/// <para>
/// <b>P2-4</b>：Aes 实例按 EncodingAESKey 复用（静态缓存）；实例仅在本类工厂内一次性配置
/// Mode/Key/IV/Padding，<b>运行期不得变更</b>（SymmetricAlgorithm 实例成员非线程安全）；
/// <c>ICryptoTransform</c> 每请求新建并经 <c>using</c> 释放，绝不跨线程复用。
/// </para>
/// </remarks>
public static class WechatCallbackCrypto
{
    /// <summary>官方填充块大小（字节）：90968 逐字「PKCS#7 填充至 32 字节的倍数」。</summary>
    private const int Pkcs7BlockSize = 32;

    /// <summary>AES 块大小（字节）：密文长度前置校验口径（官方密文恒为 32 倍数 ⊂ 16 倍数）。</summary>
    private const int AesBlockSize = 16;

    /// <summary>明文长度头之前的随机串与长度头合计（random(16B) + msg_len(4B)）。</summary>
    private const int PlainHeaderLength = 20;

    /// <summary>Aes 实例缓存（P2-4）：键 = 派生前 43 位 EncodingAESKey，实例数以回调配置数为上界。</summary>
    private static readonly ConcurrentDictionary<string, Aes> AesCache = new(StringComparer.Ordinal);

    /// <summary>
    /// 验证回调签名：<c>SHA1(Sort(token, timestamp, nonce, encrypt) 拼接)</c> 与 msg_signature 比对。
    /// </summary>
    /// <remarks>
    /// F13（D8 保持现状）：比对采用 <see cref="StringComparison.OrdinalIgnoreCase"/>，宽于官方的
    /// 小写精确比较——签名为十六进制值（大小写仅是表示形式，值唯一），不构成安全弱化（有意取舍）。
    /// </remarks>
    public static bool VerifySignature(string token, string timestamp, string nonce, string encrypt, string signature)
    {
        var expected = ComputeSignature(token, timestamp, nonce, encrypt);
        return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>计算回调签名。</summary>
    /// <remarks>
    /// F14（D8 保持现状）：null 过滤仅影响直接调用方（容忍空参）；SDK 管线内四参数恒非 null
    /// （接收器对缺失参数显式 <c>?? string.Empty</c>），保留过滤（移除无收益且可能破坏外部调用者）。
    /// </remarks>
    public static string ComputeSignature(string token, string timestamp, string nonce, string encrypt)
    {
        var items = new[] { token, timestamp, nonce, encrypt }
            .Where(s => s != null)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(items)));
        return ToHexLower(hash);
    }

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

    /// <summary>
    /// 解密回调密文（AES-256-CBC，官方 32 块 PKCS7），返回去掉随机前缀与长度头后的消息明文。
    /// </summary>
    /// <param name="encodingAESKey">43 位 EncodingAESKey。</param>
    /// <param name="encryptedBase64">Base64 编码的密文（XML 报文 Encrypt 节点内容）。</param>
    /// <exception cref="WechatCallbackException">密钥非法或密文解密失败时抛出（Kind = <see cref="WechatCallbackFailureKind.DecryptFailed"/>）。</exception>
    public static string Decrypt(string encodingAESKey, string encryptedBase64)
        => Decrypt(encodingAESKey, encryptedBase64, out _);

    /// <summary>
    /// 解密回调密文（AES-256-CBC，官方 32 块 PKCS7），并输出明文尾部的接收方 ID（<c>receiveid</c>）。
    /// </summary>
    /// <param name="encodingAESKey">43 位 EncodingAESKey。</param>
    /// <param name="encryptedBase64">Base64 编码的密文（XML 报文 Encrypt 节点内容）。</param>
    /// <param name="receiveId">
    /// 明文尾部的接收方 ID：企业自建应用回调为企业 <c>CorpId</c>，第三方/服务商套件回调为 <c>SuiteId</c>。
    /// 报文未附带时为 <see cref="string.Empty"/>（兼容官方「个人主体第三方为空串」形态）。
    /// </param>
    /// <returns>去掉随机前缀、长度头与接收方 ID 后的消息明文。</returns>
    /// <exception cref="WechatCallbackException">密钥非法或密文解密失败时抛出（Kind = <see cref="WechatCallbackFailureKind.DecryptFailed"/>）。</exception>
    public static string Decrypt(string encodingAESKey, string encryptedBase64, out string receiveId)
    {
        if (string.IsNullOrWhiteSpace(encodingAESKey) || encodingAESKey.Length != 43)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed, "EncodingAESKey 必须为 43 位字符。");
        }

        Aes aes;
        byte[] cipher;
        try
        {
            aes = GetOrCreateAes(encodingAESKey);
            cipher = Convert.FromBase64String(encryptedBase64 ?? string.Empty);
        }
        catch (FormatException ex)
        {
            // P0-1 ③：Base64 非法字符抛 FormatException，包裹为统一异常面（消息不含密钥/密文材料）。
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed,
                "回调密文解密失败：密文或 EncodingAESKey 不是合法的 Base64 字符串。",
                ex);
        }

        // P0-1 ①：密文前置校验（fail-closed）：空或非 16 倍数一律拒绝（官方密文恒为 32 倍数 ⊂ 16 倍数）。
        if (cipher.Length == 0 || cipher.Length % AesBlockSize != 0)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed,
                "回调密文解密失败：密文长度非法（须为 AES 块大小整数倍）。");
        }

        // P0-1 ②：PaddingMode.None 解密（不依赖运行时 16 块填充校验），随后手工剥离 32 块填充。
        byte[] plain;
        try
        {
            using var decryptor = aes.CreateDecryptor();
            plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        }
        catch (CryptographicException ex)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed,
                "回调密文解密失败：请检查 PushEncodingAESKey 是否与企业微信后台一致。",
                ex);
        }

        plain = StripPkcs7Padding(plain);

        if (plain.Length < PlainHeaderLength)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed, "回调密文解密失败：明文长度不足（缺少长度头）。");
        }

        // 明文结构：random(16B) + msg_len(4B, 网络字节序) + msg + receiveid
        var msgLen = (plain[16] << 24) | (plain[17] << 16) | (plain[18] << 8) | plain[19];
        if (msgLen < 0 || (long)PlainHeaderLength + msgLen > plain.Length)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed, "回调密文解密失败：消息长度头非法。");
        }

        // P2-5：明文尾部为接收方 ID（receiveid），供接收器校验（原实现直接丢弃）。
        var receiveIdLength = plain.Length - PlainHeaderLength - msgLen;
        receiveId = receiveIdLength > 0
            ? Encoding.UTF8.GetString(plain, PlainHeaderLength + msgLen, receiveIdLength)
            : string.Empty;

        return Encoding.UTF8.GetString(plain, PlainHeaderLength, msgLen);
    }

    /// <summary>
    /// 手工剥离 32 块 PKCS7 填充（P0-1，官方协议）：padLen 取明文最后一字节，
    /// 校验 padLen ∈ [1..32]、padLen ≤ 明文长度、且末尾 padLen 字节全部等于 padLen；失败即 fail-closed。
    /// </summary>
    /// <remarks>
    /// 兼容性：16 块 PKCS7 密文（padLen ≤ 16）同样满足该校验（校验界为 [1..32] 而非 [17..32]），可正常剥离。
    /// </remarks>
    private static byte[] StripPkcs7Padding(byte[] plain)
    {
        var padLen = plain[plain.Length - 1];
        if (padLen < 1 || padLen > Pkcs7BlockSize || padLen > plain.Length)
        {
            throw PaddingInvalid();
        }

        for (var i = plain.Length - padLen; i < plain.Length; i++)
        {
            if (plain[i] != padLen)
            {
                throw PaddingInvalid();
            }
        }

        var stripped = new byte[plain.Length - padLen];
        Buffer.BlockCopy(plain, 0, stripped, 0, stripped.Length);
        return stripped;
    }

    private static WechatCallbackException PaddingInvalid()
        => new(
            WechatCallbackFailureKind.DecryptFailed,
            "回调密文解密失败：填充校验不通过（密文被篡改或 EncodingAESKey 不一致）。");

    /// <summary>
    /// 加密回调应答明文（AES-256-CBC，官方 32 块 PKCS7 填充），供被动应答（加密模式 echo）场景使用。
    /// </summary>
    /// <remarks>
    /// P0-1 ②：严格对齐官方「PKCS#7 填充至 32 字节的倍数」——padLen = 32 - 长度 % 32（恒 ≥ 1，
    /// 已对齐时补满 32 字节），填充字节值 = padLen；官方消费方按 <c>msg_len</c> 切片或按 32 块剥填充均可正确处理。
    /// </remarks>
    /// <param name="encodingAESKey">43 位 EncodingAESKey。</param>
    /// <param name="plainText">待加密的消息明文。</param>
    /// <param name="receiveId">接收方 ID（拼在明文尾部；企业自建=CorpId / 套件=SuiteId）。</param>
    /// <returns>Base64 编码的密文。</returns>
    /// <exception cref="InvalidOperationException">密钥缺失、非 43 位或非合法 Base64 字符串时抛出。</exception>
    public static string Encrypt(string encodingAESKey, string plainText, string receiveId)
    {
        if (string.IsNullOrWhiteSpace(encodingAESKey) || encodingAESKey.Length != 43)
        {
            throw new InvalidOperationException("EncodingAESKey 必须为 43 位字符。");
        }

        Aes aes;
        try
        {
            aes = GetOrCreateAes(encodingAESKey);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("EncodingAESKey 不是合法的 Base64 字符串。", ex);
        }

        var msgBytes = Encoding.UTF8.GetBytes(plainText ?? string.Empty);
        var receiveBytes = Encoding.UTF8.GetBytes(receiveId ?? string.Empty);
        var random = new byte[16];
#if NET6_0_OR_GREATER
        RandomNumberGenerator.Fill(random);
#else
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(random);
        }
#endif

        var lenBytes = BitConverter.GetBytes(msgBytes.Length);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lenBytes);
        }

        var buffer = new byte[PlainHeaderLength + msgBytes.Length + receiveBytes.Length];
        random.CopyTo(buffer, 0);
        lenBytes.CopyTo(buffer, 16);
        msgBytes.CopyTo(buffer, PlainHeaderLength);
        receiveBytes.CopyTo(buffer, PlainHeaderLength + msgBytes.Length);

        // P0-1 ②：手工 32 块 PKCS7 补位（padLen 恒 ≥ 1，已对齐时补满 32 字节）。
        var padLen = Pkcs7BlockSize - (buffer.Length % Pkcs7BlockSize);
        var padded = new byte[buffer.Length + padLen];
        Buffer.BlockCopy(buffer, 0, padded, 0, buffer.Length);
        for (var i = buffer.Length; i < padded.Length; i++)
        {
            padded[i] = (byte)padLen;
        }

        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(padded, 0, padded.Length);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// 获取共享 Aes 实例（P2-4）：按派生前 43 位 EncodingAESKey 缓存；实例仅在创建时一次性配置
    /// Mode/Key/IV/Padding，<b>运行期不得变更 Key/IV</b>。
    /// </summary>
    /// <remarks>
    /// 缓存实例数以回调配置数为上界（多套件场景天然有限）；<c>ICryptoTransform</c> 每请求新建。
    /// 全程无反射（AOT/裁剪安全）。
    /// </remarks>
    private static Aes GetOrCreateAes(string encodingAESKey)
    {
        if (AesCache.TryGetValue(encodingAESKey, out var cached))
        {
            return cached;
        }

        lock (AesCache)
        {
            if (!AesCache.TryGetValue(encodingAESKey, out var aes))
            {
                aes = CreateAes(encodingAESKey);
                AesCache[encodingAESKey] = aes;
            }

            return aes;
        }
    }

    private static Aes CreateAes(string encodingAESKey)
    {
        var keyBytes = Convert.FromBase64String(encodingAESKey + "=");
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        // P0-1：官方 32 块 PKCS7 填充由本类手工补位/剥离，不使用 .NET 内置 16 块 PKCS7。
        aes.Padding = PaddingMode.None;
        aes.Key = keyBytes;
        aes.IV = keyBytes.AsSpan(0, 16).ToArray();
        return aes;
    }

    /// <summary>
    /// 解析 URL 查询串中的回调验签参数（msg_signature / timestamp / nonce）。
    /// </summary>
    /// <remarks>容忍前导 <c>?</c>（如 <c>HttpRequest.QueryString.Value</c> 的原始形态）。</remarks>
    public static (string? Signature, string? Timestamp, string? Nonce) ParseSignatureQuery(string urlQuery)
    {
        string? signature = null, timestamp = null, nonce = null;
        foreach (var pair in (urlQuery ?? string.Empty).TrimStart('?')
                     .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split(new[] { '=' }, 2);
            if (kv.Length != 2)
            {
                continue;
            }

            var value = Uri.UnescapeDataString(kv[1]);
            switch (kv[0])
            {
                case "msg_signature": signature = value; break;
                case "timestamp": timestamp = value; break;
                case "nonce": nonce = value; break;
            }
        }

        return (signature, timestamp, nonce);
    }
}
