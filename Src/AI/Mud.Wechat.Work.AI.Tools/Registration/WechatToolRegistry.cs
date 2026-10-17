// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Registration;

/// <summary>
/// 工具注册表：装配期把「已策展工具的契约名 → 执行器 + 绑定」装进来，运行期由执行链按名字取用。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么注册表不是白名单</b>：注册表回答「引擎里有没有这个工具」，白名单回答「本次回合允不允许调用」——
/// 前者是编译期事实（<c>[AgentTool]</c> 声明 + 生成器），后者是宿主策略（默认封闭、按需开启）。
/// 把两者合成一个结构，会让「工具已实现但未启用」变成不可表达，进而诱发「默认全开」的实现。
/// </para>
/// <para>
/// <b>重名即失败</b>：同名注册说明两份策展声明撞名（生成器本应在编译期拦下），运行期不再静默覆盖 ——
/// 静默覆盖会让模型调到的实现与被策展的那个不是同一个。
/// </para>
/// </remarks>
public sealed class WechatToolRegistry
{
    private readonly Dictionary<string, WechatToolExecution> _executions = new(StringComparer.Ordinal);

    /// <summary>已注册工具数。</summary>
    public int Count => _executions.Count;

    /// <summary>已注册工具名（按名字排序返回，便于诊断与断言）。</summary>
    public IReadOnlyList<string> ToolNames
    {
        get
        {
            var names = new List<string>(_executions.Keys);
            names.Sort(StringComparer.Ordinal);
            return names;
        }
    }

    /// <summary>注册一个工具执行入口。</summary>
    /// <param name="execution">执行入口（不得为 <see langword="null"/>）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="execution"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="InvalidOperationException">同名工具已注册。</exception>
    public void Register(WechatToolExecution execution)
    {
        if (execution is null)
        {
            throw new ArgumentNullException(nameof(execution));
        }

        if (!_executions.TryAdd(execution.ToolName, execution))
        {
            throw new InvalidOperationException(
                $"工具 '{execution.ToolName}' 重复注册：工具名是模型可见契约，重名会让调用落到不可预期的实现。");
        }
    }

    /// <summary>按工具名查找执行入口。</summary>
    /// <param name="toolName">工具名。</param>
    /// <param name="execution">命中的执行入口。</param>
    /// <returns>命中返回 <see langword="true"/>。</returns>
    public bool TryGet(string toolName, out WechatToolExecution execution)
        => _executions.TryGetValue(toolName, out execution!);
}

/// <summary>
/// 单个工具的执行入口（注册表条目）：契约名 + 执行绑定 + 执行委托。
/// </summary>
/// <remarks>
/// 执行委托的实参形态由生成器的 Args 解包器决定（<c>IReadOnlyDictionary&lt;string, object?&gt;</c>），
/// 故注册表与具体工具的参数类型解耦：新增工具不改变注册表。
/// </remarks>
public sealed class WechatToolExecution
{
    /// <summary>初始化执行入口。</summary>
    /// <param name="toolName">工具名（模型可见契约名）。</param>
    /// <param name="binding">执行绑定（应用/企业归属）。</param>
    /// <param name="executeAsync">执行委托。</param>
    /// <exception cref="ArgumentException"><paramref name="toolName"/> 为空白。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="binding"/> 或 <paramref name="executeAsync"/> 为 <see langword="null"/>。</exception>
    public WechatToolExecution(
        string toolName,
        WechatToolBinding binding,
        Func<IReadOnlyDictionary<string, object?>, CancellationToken, Task<ToolResult>> executeAsync)
    {
        if (string.IsNullOrWhiteSpace(toolName))
        {
            throw new ArgumentException("工具名不能为空（它是模型可见契约的锚点）。", nameof(toolName));
        }

        ToolName = toolName;
        Binding = binding ?? throw new ArgumentNullException(nameof(binding));
        ExecuteAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
    }

    /// <summary>工具名。</summary>
    public string ToolName { get; }

    /// <summary>执行绑定。</summary>
    public WechatToolBinding Binding { get; }

    /// <summary>执行委托（入参表 + 取消令牌 → 工具结果）。</summary>
    public Func<IReadOnlyDictionary<string, object?>, CancellationToken, Task<ToolResult>> ExecuteAsync { get; }
}
