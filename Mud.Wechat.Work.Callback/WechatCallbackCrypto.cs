// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
/// 解密协议：Base64 解码密文 → AES-256-CBC 解密（Key = Base64Decode(EncodingAESKey + "=")，
/// IV = Key 前 16 字节，PKCS7 填充）→ 明文 = 16 字节随机串 + 4 字节网络序消息长度 + 消息 + receiveid。
/// </para>
/// </remarks>
public static class WechatCallbackCrypto
{
    /// <summary>
    /// 验证回调签名：<c>SHA1(Sort(token, timestamp, nonce, encrypt) 拼接)</c> 与 msg_signature 比对。
    /// </summary>
    public static bool VerifySignature(string token, string timestamp, string nonce, string encrypt, string signature)
    {
        var expected = ComputeSignature(token, timestamp, nonce, encrypt);
        return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>计算回调签名。</summary>
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
    /// 解密回调密文（AES-256-CBC，PKCS7），返回去掉随机前缀与长度头后的消息明文。
    /// </summary>
    /// <param name="encodingAESKey">43 位 EncodingAESKey。</param>
    /// <param name="encryptedBase64">Base64 编码的密文（XML 报文 Encrypt 节点内容）。</param>
    /// <exception cref="InvalidOperationException">密钥非法或密文解密失败时抛出。</exception>
    public static string Decrypt(string encodingAESKey, string encryptedBase64)
    {
        if (string.IsNullOrWhiteSpace(encodingAESKey) || encodingAESKey.Length != 43)
        {
            throw new InvalidOperationException("EncodingAESKey 必须为 43 位字符。");
        }

        var key = Convert.FromBase64String(encodingAESKey + "=");
        var cipher = Convert.FromBase64String(encryptedBase64);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = key.AsSpan(0, 16).ToArray();

        using var decryptor = aes.CreateDecryptor();
        byte[] plain;
        try
        {
            plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("回调密文解密失败：请检查 PushEncodingAESKey 是否与微信后台一致。", ex);
        }

        if (plain.Length < 20)
        {
            throw new InvalidOperationException("回调密文解密失败：明文长度不足（缺少长度头）。");
        }

        // 明文结构：random(16B) + msg_len(4B, 网络字节序) + msg + receiveid
        var msgLen = (plain[16] << 24) | (plain[17] << 16) | (plain[18] << 8) | plain[19];
        if (msgLen < 0 || 20L + msgLen > plain.Length)
        {
            throw new InvalidOperationException("回调密文解密失败：消息长度头非法。");
        }

        return Encoding.UTF8.GetString(plain, 20, msgLen);
    }

    /// <summary>
    /// 加密回调应答明文（AES-256-CBC），供被动应答（加密模式 echo）场景使用。
    /// </summary>
    public static string Encrypt(string encodingAESKey, string plainText, string receiveId)
    {
        if (string.IsNullOrWhiteSpace(encodingAESKey) || encodingAESKey.Length != 43)
        {
            throw new InvalidOperationException("EncodingAESKey 必须为 43 位字符。");
        }

        var key = Convert.FromBase64String(encodingAESKey + "=");
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

        var buffer = new byte[16 + 4 + msgBytes.Length + receiveBytes.Length];
        random.CopyTo(buffer, 0);
        lenBytes.CopyTo(buffer, 16);
        msgBytes.CopyTo(buffer, 20);
        receiveBytes.CopyTo(buffer, 20 + msgBytes.Length);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = key.AsSpan(0, 16).ToArray();

        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// 解析 URL 查询串中的回调验签参数（msg_signature / timestamp / nonce）。
    /// </summary>
    public static (string? Signature, string? Timestamp, string? Nonce) ParseSignatureQuery(string urlQuery)
    {
        string? signature = null, timestamp = null, nonce = null;
        foreach (var pair in (urlQuery ?? string.Empty)
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
