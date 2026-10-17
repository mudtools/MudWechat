// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Channels;

/// <summary>
/// 从智能机器人回调的 <c>response_url</c> 提取一次性应答凭据 <c>response_code</c>
/// （官方 101138：每个 url 仅可调用一次、有效期 1 小时）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要提取</b>：SDK 的主动回复端点以 <c>response_code</c> 为查询参数（<c>POST /cgi-bin/aibot/response</c>），
/// 而回调只给出完整 <c>response_url</c>；提取是本层（渠道适配）的职责，不得散落到宿主。
/// </para>
/// <para>
/// <b>不解析完整 URL 之外的语义</b>：只取查询串中的 <c>response_code</c>，不做主机校验（SSRF 白名单在传输层）。
/// 提取失败返回 <see langword="null"/> —— 由调用方 fail-closed（无凭据即无法主动回复，只能记日志）。
/// </para>
/// </remarks>
internal static class WechatAibotResponseCode
{
    /// <summary>查询参数名（官方固定为 <c>response_code</c>）。</summary>
    internal const string QueryName = "response_code";

    /// <summary>从 <c>response_url</c> 提取 <c>response_code</c>。</summary>
    /// <param name="responseUrl">回调报文中的 <c>response_url</c>（可空）。</param>
    /// <returns>提取到的凭据；缺失或 URL 非法时返回 <see langword="null"/>。</returns>
    public static string? Extract(string? responseUrl)
    {
        if (string.IsNullOrWhiteSpace(responseUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(responseUrl, UriKind.Absolute, out var uri))
        {
            return null;
        }

        var query = uri.Query;
        if (query.Length <= 1)
        {
            return null;
        }

        // 查询串形如 "?a=1&response_code=xxx"；逐段比较键名，值做一次反转义（凭据可能被 UrlEncode）。
        var pairs = query.Substring(1).Split('&');
        for (var i = 0; i < pairs.Length; i++)
        {
            var pair = pairs[i];
            if (pair.Length == 0)
            {
                continue;
            }

            var separator = pair.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = pair.Substring(0, separator);
            if (!string.Equals(key, QueryName, StringComparison.Ordinal))
            {
                continue;
            }

            var value = pair.Substring(separator + 1);
            if (value.Length == 0)
            {
                return null;
            }

            return Uri.UnescapeDataString(value);
        }

        return null;
    }
}
