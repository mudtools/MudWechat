// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.Work.WebDemo;

/// <summary>
/// 企业微信 JS-SDK 签名（<b>宿主实现</b>，SDK 只提供 ticket 不提供签名）。
/// </summary>
/// <remarks>
/// <para>
/// 官方把签名算法写死成「四参数按固定顺序拼接后取 SHA-1 小写十六进制」：
/// <c>jsapi_ticket=TICKET&amp;noncestr=NONCESTR&amp;timestamp=TIMESTAMP&amp;url=URL</c>
/// （见官方文档 90506）。<b>顺序不可调换、等号与 &amp; 不可变形、整体不做 URL encode</b>，
/// 拼错任何一处官方端都会报 <c>invalid signature</c>。
/// </para>
/// <para>
/// <c>url</c> 必须是<b>当前页面完整 URL 去掉 <c>#</c> 及其后部分</c>，且与浏览器实际地址逐字符一致
///（含查询串、含末尾斜杠）—— 这是最常见的失败原因，故本方法原样使用调用方传入的 URL，不做任何规整。
/// </para>
/// <para>
/// 官方要求签名逻辑<b>必须在服务器端实现</b>：ticket 一旦下发到前端即可被任意人换取，
/// 因此本 Demo 只返回签名四元组，绝不把 ticket 放进响应。
/// </para>
/// </remarks>
internal static class JsSdkSignature
{
    /// <summary>计算 <c>wx.config</c> / <c>wx.agentConfig</c> 所需签名。</summary>
    /// <param name="jsapiTicket">企业或应用的 jsapi_ticket（来自 SDK，勿下发前端）。</param>
    /// <param name="nonceStr">随机字符串，须与返回给前端的 <c>nonceStr</c> 同值。</param>
    /// <param name="timestamp">秒级时间戳，须与返回给前端的 <c>timestamp</c> 同值。</param>
    /// <param name="url">当前页面 URL（不含 <c>#</c> 及后面部分），逐字符参与拼接。</param>
    public static string Compute(string jsapiTicket, string nonceStr, long timestamp, string url)
    {
        var plain = $"jsapi_ticket={jsapiTicket}&noncestr={nonceStr}&timestamp={timestamp}&url={url}";
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(plain));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>生成随机 <c>noncestr</c>（官方未限定长度与字符集，此处取 16 位十六进制）。</summary>
    public static string NewNonce()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>当前秒级时间戳（与签名、下发前端三处必须同值）。</summary>
    public static long CurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
