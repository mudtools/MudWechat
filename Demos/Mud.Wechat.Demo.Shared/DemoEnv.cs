// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Demo.Shared;

/// <summary>
/// Demo 凭据与开关的统一读取入口：所有值取自 <c>MUDWECHAT_DEMO_</c> 前缀的环境变量，
/// 仓库内不保留任何真实密钥，占位符形态一律视为「未配置」。
/// </summary>
/// <remarks>
/// <para>
/// <b>启动期校验优先于首次调用</b>：<see cref="RequireAll"/> 一次性收集全部缺失项后再抛出，
/// 避免使用者「改一个、跑一次、再报下一个」；<see cref="Require"/> 用于单值场景。
/// </para>
/// <para>
/// <b>占位符识别</b>：值含 <c>&lt;</c> / <c>&gt;</c> 或含 <c>your-</c> 时视为未配置——
/// 这是既有 Demo 把 <c>&lt;企业 CorpId&gt;</c> 直接写进代码留下的误用面，此处从读取层堵死。
/// </para>
/// </remarks>
public static class DemoEnv
{
    /// <summary>环境变量统一前缀（与真实部署的环境变量隔离，避免误命中）。</summary>
    public const string Prefix = "MUDWECHAT_DEMO_";

    /// <summary>README 指引位置（缺失项错误消息里给出，使用者无需先读源码）。</summary>
    private const string ReadmeHint = "Demos/README.md 的凭据矩阵";

    /// <summary>读取必填项；缺失或仍是占位符即抛错。</summary>
    /// <param name="name">变量名（不含 <see cref="Prefix"/>），如 <c>WORK_CORP_ID</c>。</param>
    public static string Require(string name)
    {
        var value = Read(name);
        if (value == null)
        {
            throw Missing(new[] { name });
        }

        return value;
    }

    /// <summary>
    /// 一次读取多个必填项：先把全部缺失项收集完再抛出，返回「变量名 → 值」。
    /// </summary>
    /// <param name="names">变量名集合（不含 <see cref="Prefix"/>）。</param>
    public static IReadOnlyDictionary<string, string> RequireAll(params string[] names)
    {
        if (names == null || names.Length == 0)
        {
            throw new ArgumentException("至少需要一个必填变量名。", nameof(names));
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        List<string>? missing = null;
        foreach (var name in names)
        {
            var value = Read(name);
            if (value == null)
            {
                (missing ??= new List<string>()).Add(name);
                continue;
            }

            values[name] = value;
        }

        if (missing != null)
        {
            throw Missing(missing);
        }

        return values;
    }

    /// <summary>读取选填项：缺失或占位符形态返回 <see langword="null"/>。</summary>
    public static string? Optional(string name) => Read(name);

    /// <summary>读取开关：值恰为 <c>1</c> 时为 <see langword="true"/>（其它值一律视为关闭）。</summary>
    public static bool Flag(string name)
        => string.Equals(Environment.GetEnvironmentVariable(FullName(name)), "1", StringComparison.Ordinal);

    /// <summary>拼接完整环境变量名（用于错误消息，让使用者直接照着设置）。</summary>
    public static string FullName(string name) => Prefix + name;

    private static string? Read(string name)
    {
        var value = Environment.GetEnvironmentVariable(FullName(name));
        if (string.IsNullOrWhiteSpace(value) || IsPlaceholder(value))
        {
            return null;
        }

        return value;
    }

    private static bool IsPlaceholder(string value)
        => value.IndexOfAny(new[] { '<', '>' }) >= 0
           || value.Contains("your-", StringComparison.OrdinalIgnoreCase);

    private static InvalidOperationException Missing(IReadOnlyList<string> names)
    {
        var full = new string[names.Count];
        for (var i = 0; i < names.Count; i++)
        {
            full[i] = FullName(names[i]);
        }

        return new InvalidOperationException(
            $"缺少必需环境变量：{string.Join("、", full)}。" +
            $"请对照 {ReadmeHint} 配置后重试；密钥只走环境变量或本机用户机密，不得写进配置文件或提交仓库。");
    }
}
