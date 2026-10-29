// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Mud.Wechat.Pay.Internal;

/// <summary>
/// 密码学「低 TFM 等价物」集中地：把 net5.0+/net6.0+ 才有的便利静态方法收敛成四档同一入口，
/// 避免 40+ 调用点各自 <c>#if</c>。<b>语义与 BCL 完全一致</b>，不做任何算法替换。
/// </summary>
internal static class PayCryptoCompat
{
    /// <summary>填充密码学随机字节（对应 <c>RandomNumberGenerator.Fill</c>，net5.0+ 才提供）。</summary>
    /// <param name="buffer">目标缓冲。</param>
    internal static void FillRandom(Span<byte> buffer)
    {
#if NETSTANDARD2_0
        // ns2.0 无 GetBytes(Span<byte>)：先落到数组再回填，取完立即清零临时缓冲。
        var temp = new byte[buffer.Length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(temp);
        }

        temp.AsSpan().CopyTo(buffer);
        Array.Clear(temp, 0, temp.Length);
#else
        RandomNumberGenerator.Fill(buffer);
#endif
    }

    /// <summary>MD5 摘要（对应 <c>MD5.HashData</c>，net5.0+ 才提供）。</summary>
    /// <param name="data">待摘要数据。</param>
    /// <returns>16 字节摘要。</returns>
    internal static byte[] Md5(byte[] data)
    {
#if NETSTANDARD2_0
        using (var md5 = MD5.Create())
        {
            return md5.ComputeHash(data);
        }
#else
        return MD5.HashData(data);
#endif
    }

    /// <summary>HMAC-SHA256（对应 <c>HMACSHA256.HashData</c>，net5.0+ 才提供）。</summary>
    /// <param name="key">密钥。</param>
    /// <param name="data">待摘要数据。</param>
    /// <returns>32 字节摘要。</returns>
    internal static byte[] HmacSha256(byte[] key, byte[] data)
    {
#if NETSTANDARD2_0
        using (var hmac = new HMACSHA256(key))
        {
            return hmac.ComputeHash(data);
        }
#else
        return HMACSHA256.HashData(key, data);
#endif
    }

    /// <summary>SHA-256 摘要（对应 <c>SHA256.HashData</c>）。</summary>
    /// <param name="data">待摘要数据。</param>
    /// <returns>32 字节摘要。</returns>
    internal static byte[] Sha256(byte[] data)
    {
#if NETSTANDARD2_0
        using (var sha = SHA256.Create())
        {
            return sha.ComputeHash(data);
        }
#else
        return SHA256.HashData(data);
#endif
    }

    /// <summary>从 PEM 载入 X509 证书（对应 <c>X509Certificate2.CreateFromPem</c>，net5.0+ 才提供）。</summary>
    /// <param name="pem">单段 CERTIFICATE PEM。</param>
    /// <returns>证书实例（调用方负责释放）。</returns>
    /// <exception cref="CryptographicException">内容不是合法 DER 证书。</exception>
    internal static X509Certificate2 LoadCertificateFromPem(string pem)
    {
#if NETSTANDARD2_0
        // ns2.0 无 PEM 解析器：手工剥掉头尾与空白后 Base64 解码，再由 X509Certificate2(byte[]) 解析 DER。
        // 只做容器解码，不做任何密码学运算。
        var base64 = new StringBuilder(pem.Length);
        foreach (var rawLine in pem.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("-----", StringComparison.Ordinal))
            {
                continue;
            }

            base64.Append(line);
        }

        var der = Convert.FromBase64String(base64.ToString());
        try
        {
            return new X509Certificate2(der);
        }
        finally
        {
            Array.Clear(der, 0, der.Length);
        }
#else
        return X509Certificate2.CreateFromPem(pem);
#endif
    }

    /// <summary>把 PEM 私钥导入 RSA 实例（对应 <c>RSA.ImportFromPem</c>，net5.0+ 才提供）。</summary>
    /// <param name="rsa">目标 RSA 实例。</param>
    /// <param name="pem">PKCS#8 或 PKCS#1 单段 PEM。</param>
    /// <exception cref="CryptographicException">PEM 结构不受支持。</exception>
    internal static void ImportRsaPrivateKey(RSA rsa, string pem)
    {
#if NETSTANDARD2_0
        var pkcs8 = pem.Contains("BEGIN PRIVATE KEY");
        var der = PemToDer(pem);
        try
        {
            rsa.ImportParameters(ParseRsaPrivateKey(der, pkcs8));
        }
        finally
        {
            Array.Clear(der, 0, der.Length);
        }
#else
        rsa.ImportFromPem(pem);
#endif
    }

#if NETSTANDARD2_0
    private static byte[] PemToDer(string pem)
    {
        var base64 = new StringBuilder(pem.Length);
        foreach (var rawLine in pem.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("-----", StringComparison.Ordinal))
            {
                continue;
            }

            base64.Append(line);
        }

        return Convert.FromBase64String(base64.ToString());
    }

    private static RSAParameters ParseRsaPrivateKey(byte[] der, bool pkcs8)
    {
        var reader = new System.Formats.Asn1.AsnReader(der, System.Formats.Asn1.AsnEncodingRules.DER);
        var outer = reader.ReadSequence();
        outer.ReadInteger(); // version

        System.Formats.Asn1.AsnReader keySequence;
        if (pkcs8)
        {
            var algorithm = outer.ReadSequence();
            var oid = algorithm.ReadObjectIdentifier();
            if (!string.Equals(oid, "1.2.840.113549.1.1.1", StringComparison.Ordinal))
            {
                throw new CryptographicException("私钥 PEM 不是 RSA（仅支持 rsaEncryption）。");
            }

            if (algorithm.HasData)
            {
                algorithm.ReadNull();
            }

            var inner = outer.ReadOctetString();
            var innerReader = new System.Formats.Asn1.AsnReader(inner, System.Formats.Asn1.AsnEncodingRules.DER);
            keySequence = innerReader.ReadSequence();
            keySequence.ReadInteger(); // version
        }
        else
        {
            keySequence = outer;
        }

        // RSAPrivateKey ::= SEQUENCE { version, n, e, d, p, q, dp, dq, qInv }
        return new RSAParameters
        {
            Modulus = keySequence.ReadIntegerBytes().ToArray(),
            Exponent = keySequence.ReadIntegerBytes().ToArray(),
            D = keySequence.ReadIntegerBytes().ToArray(),
            P = keySequence.ReadIntegerBytes().ToArray(),
            Q = keySequence.ReadIntegerBytes().ToArray(),
            DP = keySequence.ReadIntegerBytes().ToArray(),
            DQ = keySequence.ReadIntegerBytes().ToArray(),
            InverseQ = keySequence.ReadIntegerBytes().ToArray(),
        };
    }
#endif
}
