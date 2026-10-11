// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 会话存档的随机密钥解密：<c>encrypt_random_key</c>（base64 of RSA-PKCS#1 密文）→ AES 密钥明文。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方时序的第一步，且不可跳过版本匹配</b>：官方支持公钥/私钥多版本并存（轮换），每条记录带
/// <c>publickey_ver</c> ⇒ 必须按版本号取到<b>那一把</b>私钥。拿错版本时 RSA 解出来的不是合法密钥，
/// 失败点落在后面的 <c>DecryptData</c>，症状与「密文损坏」无法区分。
/// </para>
/// <para>
/// <b>私钥不进配置 DTO、不进日志</b>：私钥只以「密钥名」出现在配置里，运行期经组件
/// <c>ISecretProvider</c> 取用（与微信支付侧同一治理，守卫 FIN-B2）。本类的入参已是私钥文本，
/// 但异常消息<b>只带机器人与版本号</b>，绝不回显 PEM 或密文（守卫 FIN-B5）。
/// </para>
/// <para>
/// <b>TFM 收窄（不静默降级）</b>：<c>RSA.ImportFromPem</c> 自 <c>net5.0</c> 才有。<c>netstandard2.0</c>
/// 档上本方法<b>点名抛错</b>，而不是退化成「支持 PKCS#8 但不支持 PKCS#1」之类的半可用状态，
/// 也不去做手写 DER 解析（未核验的 ASN.1 分支比明确不支持更危险）。需要该档的宿主请改用 net6.0+ 运行时。
/// </para>
/// </remarks>
internal static class FinanceCipher
{
    /// <summary>当前目标框架是否具备 RSA 私钥 PEM 导入能力。</summary>
    internal static bool SupportsPemImport
#if NET6_0_OR_GREATER
        => true;
#else
        => false;
#endif

    /// <summary>
    /// 用 RSA 私钥解出 AES 会话密钥。
    /// </summary>
    /// <param name="privateKeyPem">PKCS#1（<c>-----BEGIN RSA PRIVATE KEY-----</c>）或
    /// PKCS#8（<c>-----BEGIN PRIVATE KEY-----</c>）单段 PEM。</param>
    /// <param name="encryptRandomKey"><c>encrypt_random_key</c> 原文（base64）。</param>
    /// <param name="robotKey">机器人键（仅诊断文本使用）。</param>
    /// <param name="publicKeyVersion">公钥版本号（仅诊断文本使用）。</param>
    /// <returns>AES 密钥明文（UTF-8）。</returns>
    /// <exception cref="InvalidOperationException">任一环节失败，或当前目标框架不支持 PEM 导入。</exception>
    internal static string DecryptRandomKey(
        string privateKeyPem, string encryptRandomKey, string robotKey, int publicKeyVersion)
    {
#if NET6_0_OR_GREATER
        if (string.IsNullOrEmpty(privateKeyPem))
        {
            throw new InvalidOperationException(
                $"机器人「{robotKey}」的 publickey_ver={publicKeyVersion} 私钥为空。");
        }

        if (string.IsNullOrWhiteSpace(encryptRandomKey))
        {
            throw new InvalidOperationException(
                $"机器人「{robotKey}」的记录缺少 encrypt_random_key，无法解出会话密钥。");
        }

        byte[] cipher;
        try
        {
            cipher = Convert.FromBase64String(encryptRandomKey);
        }
        catch (FormatException ex)
        {
            // 密文本身不写进消息（不是密钥、但属报文内容，一律不入诊断文本）。
            throw new InvalidOperationException(
                $"机器人「{robotKey}」的 encrypt_random_key 不是合法 base64（{ex.GetType().Name}）。", ex);
        }

        using var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(privateKeyPem);
        }
        catch (Exception ex)
        {
            // 只带异常类型名：**绝不回显 PEM**（FIN-B5）。
            throw new InvalidOperationException(
                $"机器人「{robotKey}」的 publickey_ver={publicKeyVersion} 私钥 PEM 解析失败" +
                $"（{ex.GetType().Name}）。请确认为 PKCS#1 或 PKCS#8 单段 RSA-2048 私钥。", ex);
        }

        byte[] plainKey;
        try
        {
            plainKey = rsa.Decrypt(cipher, RSAEncryptionPadding.Pkcs1);
        }
        catch (CryptographicException ex)
        {
            // 最常见成因是「版本号取到另一把私钥」，故消息点名版本而不提密文。
            throw new InvalidOperationException(
                $"机器人「{robotKey}」的 publickey_ver={publicKeyVersion} 无法解开 encrypt_random_key" +
                "（RSA 解密失败）。请确认该版本号的私钥与上传给官方的公钥配对。", ex);
        }

        // 官方以 UTF-8 文本承载该密钥 ⇒ 不解码就直接送 DecryptData 会失败。
        return Encoding.UTF8.GetString(plainKey, 0, plainKey.Length);
#else
        throw new InvalidOperationException(
            $"当前运行时（netstandard2.0，如 .NET Framework 宿主）不具备 RSA 私钥 PEM 导入能力，" +
            $"无法解密机器人「{robotKey}」的会话记录。请改用 net6.0 及以上目标框架运行会话内容存档拉取。");
#endif
    }
}
