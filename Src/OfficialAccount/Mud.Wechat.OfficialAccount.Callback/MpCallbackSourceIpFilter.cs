// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 回调来源 IP 准入判定（静态白名单 ∪ 动态推送 IP；IPv4 精确匹配与 CIDR 网段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>语义</b>：静态白名单与动态列表为空 ⇒ <b>不限来源</b>（与既有行为一致）；
/// 任一非空 ⇒ 命中其中之一即放行。
/// </para>
/// <para>
/// <b>动态列表未就绪时 fail-open（有意为之）</b>：<c>getcallbackip</c> 刷新失败/首次刷新尚未完成时，
/// 一律放行并记录一次性告警 —— 白名单是**加固**而非鉴权（真正的准入闸是验签 + 解密后 appid 校验），
/// 若在启动窗口 fail-closed 会直接造成回调全量拒收（对业务是比加固更严重的故障）。
/// </para>
/// </remarks>
internal static class MpCallbackSourceIpFilter
{
    /// <summary>判定来源 IP 是否被允许。</summary>
    /// <param name="remoteIp">请求来源 IP（解析失败/未知时为 <c>null</c>）。</param>
    /// <param name="staticEntries">静态白名单条目（可为空 = 不限）。</param>
    /// <param name="dynamicEntries">动态推送 IP 列表（可为空）。</param>
    /// <param name="dynamicWhitelistActive">是否已启用动态列表（启用且为空 ⇒ 由调用方记一次告警）。</param>
    /// <returns><c>true</c> = 放行。</returns>
    internal static bool IsAllowed(
        IPAddress? remoteIp,
        IReadOnlyList<string>? staticEntries,
        IReadOnlyList<string>? dynamicEntries,
        out bool dynamicWhitelistActive)
    {
        var hasStatic = staticEntries is { Count: > 0 };
        var hasDynamic = dynamicEntries is { Count: > 0 };
        dynamicWhitelistActive = hasDynamic;

        // 两条来源皆未配置（含「动态已启用但尚未刷新出结果」）⇒ 不限来源（fail-open，见类型备注）。
        if (!hasStatic && !hasDynamic)
        {
            return true;
        }

        if (remoteIp == null)
        {
            return false; // 已配置白名单却拿不到来源 IP ⇒ fail-closed。
        }

        if (hasStatic && Matches(remoteIp, staticEntries!))
        {
            return true;
        }

        return hasDynamic && Matches(remoteIp, dynamicEntries!);
    }

    /// <summary>逐条比对（精确 IP 或 CIDR 网段）。</summary>
    internal static bool Matches(IPAddress address, IReadOnlyList<string> entries)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (string.IsNullOrWhiteSpace(entry))
            {
                continue;
            }

            var candidate = entry.Trim();
            var slash = candidate.IndexOf('/');
            if (slash < 0)
            {
                if (string.Equals(candidate, address.ToString(), StringComparison.Ordinal))
                {
                    return true;
                }

                continue;
            }

            if (IsInCidr(address, candidate.Substring(0, slash), candidate.Substring(slash + 1)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>IPv4 CIDR 归属判定（非 IPv4 或格式非法 ⇒ <c>false</c>）。</summary>
    internal static bool IsInCidr(IPAddress address, string network, string prefixText)
    {
        if (address.AddressFamily != AddressFamily.InterNetwork
            || !IPAddress.TryParse(network.Trim(), out var networkAddress)
            || networkAddress.AddressFamily != AddressFamily.InterNetwork
            || !int.TryParse(prefixText.Trim(), out var prefix)
            || prefix < 0
            || prefix > 32)
        {
            return false;
        }

        var addressBytes = address.GetAddressBytes();
        var networkBytes = networkAddress.GetAddressBytes();
        var fullBytes = prefix / 8;
        var remainingBits = prefix % 8;

        for (var i = 0; i < fullBytes; i++)
        {
            if (addressBytes[i] != networkBytes[i])
            {
                return false;
            }
        }

        if (remainingBits == 0)
        {
            return true;
        }

        var mask = (byte)(0xFF << (8 - remainingBits));
        return (addressBytes[fullBytes] & mask) == (networkBytes[fullBytes] & mask);
    }
}
