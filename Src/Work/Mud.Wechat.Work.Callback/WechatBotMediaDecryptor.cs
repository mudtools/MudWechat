// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 智能机器人媒体解密器（官方 100719 / 101463）：AES-256-CBC + PKCS#7 填充至 32 字节的倍数，
/// IV 取 key 前 16 字节。
/// </summary>
/// <remarks>
/// <para>
/// <b>SDK 只提供解密器，不代下载</b>：官方媒体下载 url 仅 5 分钟有效且下载内容已加密；
/// 由宿主即时下载后调用本类解密。SDK 不发起下载可避免把「任意 URL 抓取」引入库内（收紧 SSRF 面）。
/// </para>
/// <para>
/// <b>密钥来源按模式区分</b>：
/// 回调地址模式（100719）<c>image</c>/<c>file</c>/<c>video</c> 结构体<b>不返回独立 aeskey</b>，
/// 密钥即回调配置的 <c>EncodingAESKey</c>（43 位，与回调报文同密钥）；
/// 长连接模式（101463）结构体额外返回 <c>aeskey</c>，且<b>每个下载链接唯一</b>。
/// </para>
/// <para>
/// <b>实现复用</b>：Aes 构造与 32 块 PKCS7 剥离直接复用 <see cref="WechatCallbackCrypto"/> 的内部基座
/// （同一份协议实现，避免第二份填充校验漂移）；因长连接 <c>aeskey</c> 每链接唯一，
/// 此处使用<b>无缓存</b>工厂，防止按 key 缓存无界增长。
/// </para>
/// </remarks>
public static class WechatBotMediaDecryptor
{
    /// <summary>
    /// 解密智能机器人媒体数据。
    /// </summary>
    /// <param name="aesKey">
    /// 43 位密钥：回调地址模式传回调 <c>EncodingAESKey</c>；长连接模式传结构体返回的 <c>aeskey</c>。
    /// </param>
    /// <param name="cipherData">下载得到的<b>密文</b>原始字节（官方：下载内容已加密，不能直接打开）。</param>
    /// <returns>解密后的媒体明文字节。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cipherData"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">密钥非法或密文解密/填充校验失败。</exception>
    /// <remarks>
    /// 官方媒体密文与回调密文<b>结构不同</b>：媒体密文无 <c>random(16) + msg_len(4) + receiveid</c> 头，
    /// 直接是「明文 + PKCS#7 填充（32 字节倍数）」，故此处只做 AES 解密 + 剥离填充。
    /// </remarks>
    public static byte[] DecryptMediaData(string aesKey, byte[] cipherData)
    {
        if (cipherData == null)
        {
            throw new ArgumentNullException(nameof(cipherData));
        }

        if (string.IsNullOrWhiteSpace(aesKey) || aesKey.Length != 43)
        {
            throw new InvalidOperationException(
                "智能机器人媒体解密失败：密钥必须为 43 位字符（回调地址模式为 EncodingAESKey；" +
                "长连接模式为结构体返回的 aeskey）。");
        }

        if (cipherData.Length == 0 || cipherData.Length % 16 != 0)
        {
            throw new InvalidOperationException(
                "智能机器人媒体解密失败：密文长度非法（须为 AES 块大小整数倍）。");
        }

        Aes aes;
        try
        {
            aes = WechatCallbackCrypto.CreateAes(aesKey);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("智能机器人媒体解密失败：密钥不是合法的 Base64 字符串。", ex);
        }

        byte[] plain;
        using (aes)
        {
            try
            {
                using var decryptor = aes.CreateDecryptor();
                plain = decryptor.TransformFinalBlock(cipherData, 0, cipherData.Length);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException(
                    "智能机器人媒体解密失败：请检查密钥是否与下载链接匹配。", ex);
            }
        }

        try
        {
            return WechatCallbackCrypto.StripPkcs7Padding(plain);
        }
        catch (WechatCallbackException ex)
        {
            // 复用回调基座的填充校验（fail-closed），并把异常面统一为本类的 InvalidOperationException。
            throw new InvalidOperationException("智能机器人媒体解密失败：填充校验不通过（密文被篡改或密钥不一致）。", ex);
        }
    }
}
