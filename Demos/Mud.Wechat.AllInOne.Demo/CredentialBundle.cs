// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;

namespace Mud.Wechat.AllInOne.Demo;

/// <summary>
/// 「一线凭据束」的读取结果：要么整束齐备，要么整束缺席并列出缺哪些<b>变量名</b>。
/// </summary>
/// <param name="Name">束名（用于诊断文案，如「公众号回调」）。</param>
/// <param name="Missing">缺失的变量名（含 <c>MUDWECHAT_DEMO_</c> 前缀的完整形态，绝不含值）。</param>
/// <param name="Values">变量名 → 值（仅当 <see cref="Present"/> 时非空）。</param>
internal sealed record CredentialBundle(
    string Name,
    IReadOnlyList<string> Missing,
    IReadOnlyDictionary<string, string> Values)
{
    /// <summary>整束凭据是否齐备 —— 本 Demo 的「整线激活」判据。</summary>
    public bool Present => Missing.Count == 0;

    /// <summary>取值（束不齐备时调用即抛，避免用空串把配置错误拖到请求期）。</summary>
    public string this[string name] => Values.TryGetValue(name, out var value)
        ? value
        : throw new InvalidOperationException($"凭据束「{Name}」不含变量 {name}（装配代码写错了变量名）。");

    /// <summary>缺失项的整句诊断（只含变量名）。</summary>
    public string MissingText() => string.Join("、", Missing);
}

/// <summary>
/// 凭据束读取器：把「按线聚合必填环境变量」收敛成一个判据。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不用 <see cref="DemoEnv.RequireAll"/> 的形态</b>：那个入口的语义是「缺任一即启动失败」，
/// 而本 Demo 的形态是「<b>凭据不齐的线整线不装</b>」—— 一个同时引用六线 SDK 的宿主，
/// 只有一线凭据时也该起得来，否则「共存」这件事根本没法在开发机上验证。
/// </para>
/// <para>
/// <b>为什么整束、不做逐变量降级</b>：一条线的回调链要「应用凭据 + 推送凭据」两类齐备才有意义，
/// 半装状态（装了应用底座却没有推送凭据）在路由表上表现为「有路由但恒 403」，
/// 比整线缺席更难排查。故每条线拆成若干束，束内全有才装，缺束只在路由表留一行原因。
/// </para>
/// </remarks>
internal static class CredentialSet
{
    /// <summary>读取一束凭据：全部齐备返回可用值，任一缺失返回缺失变量名清单（不抛）。</summary>
    /// <param name="name">束名。</param>
    /// <param name="required">必填变量名（不含 <see cref="DemoEnv.Prefix"/>）。</param>
    public static CredentialBundle Read(string name, params string[] required)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var missing = new List<string>();

        foreach (var key in required)
        {
            var value = DemoEnv.Optional(key);
            if (value == null)
            {
                missing.Add(DemoEnv.FullName(key));
                continue;
            }

            values[key] = value;
        }

        return new CredentialBundle(name, missing, values);
    }
}
