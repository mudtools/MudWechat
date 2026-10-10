// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.OfficialAccount.Web;

/// <summary>
/// JS-SDK 使用权限签名算法（官方「JS-SDK」页附录1；V7 已按页面原文核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方原文（逐字要点）</b>：参与签名的字段为 <c>noncestr</c>（随机字符串）、有效的 <c>jsapi_ticket</c>、
/// <c>timestamp</c>（时间戳）、<c>url</c>（当前网页 URL，**不包含 <c>#</c> 及其后面部分**）；
/// 对所有待签名参数按**字段名的 ASCII 码从小到大排序（字典序）**后，以 URL 键值对格式
/// （<c>key1=value1&amp;key2=value2…</c>）拼接成 <c>string1</c>；
/// 「**所有参数名均为小写字符**」「字段名和字段值都采用**原始值，不进行 URL 转义**」；
/// <c>signature = sha1(string1)</c>。
/// </para>
/// <para>
/// <b>排序结果固定为</b>：<c>jsapi_ticket</c> → <c>noncestr</c> → <c>timestamp</c> → <c>url</c>
/// （小写 ASCII 序；故 <see cref="BuildStringToSign"/> 直接按该序拼接，不做运行期排序 —— 顺序即契约，
/// 由用例 <c>OfficialSample_ShouldMatchOfficialSignature</c> 的官方向量锁定）。
/// </para>
/// <para>
/// <b>官方样例向量（页面原文）</b>：<c>noncestr=Wm3WZYTPz0wzccnW</c>、
/// <c>jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg</c>、
/// <c>timestamp=1414587457</c>、<c>url=http://mp.weixin.qq.com?params=value</c>
/// ⇒ <c>signature=0f9de62fce790f9a083d5c99e95740ceb90c27ed</c>。
/// </para>
/// <para>
/// <b>易错点（官方原文亦强调）</b>：前端 <c>wx.config</c> 中 <c>nonceStr</c>（驼峰）必须与签名用的
/// <c>noncestr</c> 完全一致；<c>timestamp</c> 亦须一致；<c>url</c> 必须与页面实际地址一致（含 query）。
/// </para>
/// </remarks>
public static class MpJsApiSignature
{
    /// <summary>签名参与项（小写字段名，顺序即字典序）。</summary>
    internal static readonly string[] FieldNames = { "jsapi_ticket", "noncestr", "timestamp", "url" };

    /// <summary>
    /// 规范化待签名 URL：去除 <c>#</c> 及其后的片段（官方明确要求），其余原样保留（含 <c>?</c> 查询串）。
    /// </summary>
    /// <param name="url">页面 URL（通常取自 <c>location.href</c>）。</param>
    /// <returns>不含 fragment 的 URL。</returns>
    /// <exception cref="ArgumentException"><paramref name="url"/> 为空白。</exception>
    public static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("JS-SDK 签名的 url 不能为空。", nameof(url));
        }

        var hashIndex = url!.IndexOf('#');
        return hashIndex < 0 ? url : url.Substring(0, hashIndex);
    }

    /// <summary>构造待签名串 <c>string1</c>（**不做 URL 转义**，字段名为小写并按字典序排列）。</summary>
    /// <param name="jsapiTicket">JS-SDK 临时票据（<c>type=jsapi</c>）。</param>
    /// <param name="nonceStr">随机字符串。</param>
    /// <param name="timestamp">时间戳（Unix 秒）。</param>
    /// <param name="url">已规范化的页面 URL（不含 <c>#</c> 片段）。</param>
    /// <returns>待签名串。</returns>
    public static string BuildStringToSign(string jsapiTicket, string nonceStr, long timestamp, string url)
    {
        // 顺序即小写 ASCII 字典序：jsapi_ticket < noncestr < timestamp < url。
        var builder = new StringBuilder(160);
        builder.Append("jsapi_ticket=").Append(jsapiTicket ?? string.Empty);
        builder.Append("&noncestr=").Append(nonceStr ?? string.Empty);
        builder.Append("&timestamp=").Append(timestamp.ToString(CultureInfo.InvariantCulture));
        builder.Append("&url=").Append(url ?? string.Empty);
        return builder.ToString();
    }

    /// <summary>计算签名（<c>signature = sha1(string1)</c>，40 位小写十六进制）。</summary>
    /// <param name="jsapiTicket">JS-SDK 临时票据。</param>
    /// <param name="nonceStr">随机字符串。</param>
    /// <param name="timestamp">时间戳（Unix 秒）。</param>
    /// <param name="url">页面 URL（内部先经 <see cref="NormalizeUrl"/> 去除 fragment）。</param>
    /// <returns>签名值。</returns>
    public static string Compute(string jsapiTicket, string nonceStr, long timestamp, string url)
        => Sha1Hex(BuildStringToSign(jsapiTicket, nonceStr, timestamp, NormalizeUrl(url)));

    /// <summary>生成随机字符串（官方样例为 16 位字母数字；本实现取 16 位 Base62，密码学随机）。</summary>
    /// <returns>随机字符串。</returns>
    public static string CreateNonceStr()
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[16];

        // netstandard2.0 无 RandomNumberGenerator.Fill（.NET Core 3.0+）⇒ 用 Create() + GetBytes 的可移植形态。
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        var builder = new StringBuilder(16);
        for (var i = 0; i < bytes.Length; i++)
        {
            builder.Append(alphabet[bytes[i] % alphabet.Length]);
        }

        return builder.ToString();
    }

    /// <summary>SHA1 十六进制小写（与微信各签名口径一致）。</summary>
    private static string Sha1Hex(string text)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
        var builder = new StringBuilder(hash.Length * 2);
        for (var i = 0; i < hash.Length; i++)
        {
            builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }
}
