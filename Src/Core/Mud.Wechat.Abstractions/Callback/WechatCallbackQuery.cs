// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 回调 URL 查询串解析（企业微信 / 公众号共用；零反射、AOT 安全）。
/// </summary>
/// <remarks>
/// <para>
/// 两条产品线的验签参数<b>不完全相同</b>：企微用 <c>msg_signature</c>；公众号 GET 用 <c>signature</c>、
/// POST 用 <c>msg_signature</c>，并额外携带 <c>echostr</c> / <c>encrypt_type</c>。
/// 故叶层只提供「查询串 → 键值字典」的中立解析，**不**内置任何产品线的参数名语义。
/// </para>
/// <para>
/// 容忍前导 <c>?</c>（如 <c>HttpRequest.QueryString.Value</c> 的原始形态）；
/// 键按 <see cref="StringComparer.OrdinalIgnoreCase"/> 比较（平台参数大小写在不同网关下可能不一致）；
/// 值做一次 <c>Uri.UnescapeDataString</c> 反转义（平台会对 echostr 等做 URL 编码）。
/// </para>
/// </remarks>
public static class WechatCallbackQuery
{
    /// <summary>解析查询串为大小写不敏感的键值字典（同名键后者覆盖前者）。</summary>
    /// <param name="urlQuery">查询串（可含前导 <c>?</c>）。</param>
    /// <returns>键值字典；入参为空时返回空字典（不抛）。</returns>
    public static Dictionary<string, string> Parse(string? urlQuery)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(urlQuery))
        {
            return result;
        }

        foreach (var pair in urlQuery!.TrimStart('?')
                     .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split(new[] { '=' }, 2);
            if (kv.Length != 2)
            {
                continue;
            }

            result[kv[0]] = Uri.UnescapeDataString(kv[1]);
        }

        return result;
    }

    /// <summary>解析并取指定键的值（缺失或空串时返回 <c>null</c>）。</summary>
    /// <param name="urlQuery">查询串。</param>
    /// <param name="name">参数名（大小写不敏感）。</param>
    /// <returns>参数值；缺失或为空时 <c>null</c>。</returns>
    public static string? Get(string? urlQuery, string name)
    {
        if (string.IsNullOrEmpty(urlQuery))
        {
            return null;
        }

        foreach (var pair in urlQuery!.TrimStart('?')
                     .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split(new[] { '=' }, 2);
            if (kv.Length == 2 && string.Equals(kv[0], name, StringComparison.OrdinalIgnoreCase))
            {
                var value = Uri.UnescapeDataString(kv[1]);
                return value.Length == 0 ? null : value;
            }
        }

        return null;
    }
}
