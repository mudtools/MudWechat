// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.Tools.Generated;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具白名单策略（宿主可见的运行期旋钮）：<b>注册进表 ≠ 允许调用</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>默认封闭</b>：只读工具默认全部启用（L1，见方案 §3.7），写工具（L2/L3）默认一个都不启用——
/// 生成物已把只读/写分成 <c>WechatToolNames.ReadonlyAll</c> / <c>WriteAll</c> 两份清单，本类型直接消费，
/// 不另维护平行名单（手写名单一定会与生成物漂移）。
/// </para>
/// <para>
/// <b>冻结语义</b>：<see cref="Validate"/> 之后本实例只读；DI 装配入口在注册前完成校验，
/// 运行期改动会抛 <see cref="InvalidOperationException"/> —— 白名单是装配期事实，不是运行期开关。
/// </para>
/// <para>
/// <b>写工具不得绕过授权</b>：启用任何写工具却把 <see cref="EnforceAuthorization"/> 关掉属于配置错误（fail-fast），
/// 因为写工具的唯一护栏就是授权器 + 白名单两段。
/// </para>
/// </remarks>
public sealed class WechatToolPolicy
{
    private readonly HashSet<string> _enabled = new(WechatToolNames.ReadonlyAll, StringComparer.Ordinal);
    private bool _frozen;

    /// <summary>是否强制授权（默认 <see langword="true"/>；关闭即「白名单命中即可调」，仅限只读面）。</summary>
    public bool EnforceAuthorization { get; set; } = true;

    /// <summary>已启用的工具名（按名字排序）。</summary>
    public IReadOnlyList<string> EnabledTools
    {
        get
        {
            var names = new List<string>(_enabled);
            names.Sort(StringComparer.Ordinal);
            return names;
        }
    }

    /// <summary>启用工具（写工具需宿主显式加入）。</summary>
    /// <param name="toolNames">工具名（契约名）。</param>
    /// <returns>本实例（可串联）。</returns>
    /// <exception cref="InvalidOperationException">策略已冻结。</exception>
    public WechatToolPolicy Enable(params string[] toolNames)
    {
        EnsureMutable();
        foreach (var name in toolNames)
        {
            _enabled.Add(name);
        }

        return this;
    }

    /// <summary>禁用工具（从白名单移除）。</summary>
    /// <param name="toolNames">工具名（契约名）。</param>
    /// <returns>本实例（可串联）。</returns>
    /// <exception cref="InvalidOperationException">策略已冻结。</exception>
    public WechatToolPolicy Disable(params string[] toolNames)
    {
        EnsureMutable();
        foreach (var name in toolNames)
        {
            _enabled.Remove(name);
        }

        return this;
    }

    /// <summary>判断工具是否在白名单内。</summary>
    /// <param name="toolName">工具名。</param>
    /// <returns>在白名单内返回 <see langword="true"/>。</returns>
    public bool IsEnabled(string toolName) => _enabled.Contains(toolName);

    /// <summary>校验并冻结策略（装配期调用一次）。</summary>
    /// <exception cref="InvalidOperationException">名单含未知工具名，或启用写工具却关闭了授权。</exception>
    public void Validate()
    {
        var unknown = _enabled.Where(static name => !WechatToolContracts.ByToolName.ContainsKey(name)).ToArray();
        if (unknown.Length > 0)
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy 启用了不存在的工具：{string.Join(", ", unknown)}"
                + "（白名单只能引用生成物里的契约名，拼错的名单等于静默禁用）。");
        }

        var writes = _enabled.Where(static name => WechatToolNames.IsWriteTool(name)).ToArray();
        if (writes.Length > 0 && !EnforceAuthorization)
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy 启用了写工具（{string.Join(", ", writes)}）却关闭了授权——"
                + "写面必须同时受白名单与授权器两段管控。");
        }

        _frozen = true;
    }

    private void EnsureMutable()
    {
        if (_frozen)
        {
            throw new InvalidOperationException("WechatToolPolicy 已冻结：白名单是装配期事实，不得运行期改动。");
        }
    }
}
