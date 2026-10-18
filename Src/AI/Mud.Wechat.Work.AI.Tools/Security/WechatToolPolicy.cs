// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.AgentTools;
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
/// <para>
/// <b>出站纪律也在本类型上</b>（<see cref="MaxResultLength"/> / <see cref="ContentSafetyMode"/>）：二者与白名单同属
/// 「工具面的装配期策略」，且必须在冻结前定稿——运行期改预算等于让同一批工具在不同轮次有不同结果形态，
/// 无法观测也无法回归。默认值取自中立层（<c>ToolResultPipeline</c>）的推荐区间。
/// </para>
/// </remarks>
public sealed class WechatToolPolicy
{
    private readonly HashSet<string> _enabled = new(WechatToolNames.ReadonlyAll, StringComparer.Ordinal);
    private bool _frozen;

    /// <summary>
    /// 单次工具结果的预算上限（字符数；默认 8192，<c>0</c> = 不限制）。
    /// </summary>
    /// <remarks>
    /// 由出站出口统一施加（JSON 感知截断 + <c>truncated</c>/<c>hint</c> 标记）。上限取「一次部门成员列表
    /// 在预算内仍可读」与「单次调用不吞掉整个上下文窗口」之间的实测折中：万人部门全量投影约 1~2 MB，
    /// 是典型的上下文爆点，必须在出口拦截而不是靠模型自觉缩小范围。
    /// </remarks>
    public int MaxResultLength { get; set; } = 8192;

    /// <summary>
    /// 工具结果的注入内容检测模式（闭集：<c>warn</c>（默认，命中即标注）/ <c>off</c>）。
    /// </summary>
    /// <remarks>
    /// 工具结果里的外部可控文本（客户昵称、群公告、朋友圈正文、汇报正文）是间接提示注入的主要载体。
    /// 默认 <c>warn</c> 而非阻断：合法文档里完全可能出现「忽略上一段」这类字面文本，阻断会误伤；
    /// 标注把「这段内容不可信」显式交给模型判断。
    /// </remarks>
    public string ContentSafetyMode { get; set; } = ContentSafetyWarn;

    /// <summary>默认的注入检测模式（命中即标注）。</summary>
    public const string ContentSafetyWarn = "warn";

    /// <summary>关闭注入检测的模式字面量。</summary>
    public const string ContentSafetyOff = "off";

    /// <summary>
    /// 单次工具调用的超时（默认 30 秒；<see cref="TimeSpan.Zero"/> = 不限制）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 工具会调用官方接口，而官方接口存在限流与慢查询（部门成员列表、审批详情）。没有超时，一次慢调用会把
    /// 整个回合挂住——模型侧看到的是「卡死」，而日志里只有一条未完成的请求。
    /// </para>
    /// <para>
    /// <b>只读面与写面共用同一上限</b>：写面更怕超时后仍被服务端执行（不确定态），所以真正的护栏是
    /// 「写工具永不自动重试」+ 超时后如实报错，而不是给写面更长的窗口。
    /// </para>
    /// </remarks>
    public TimeSpan CallTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 单个实例内并发执行的工具调用上限（默认 8；<c>0</c> = 不限制）。
    /// </summary>
    /// <remarks>
    /// 模型可以在一个回合里并行发起多个 tool_call（MAF 默认并行）。企微的多数接口有频率上限
    /// （如审批查询 600 次/分），不限并发等于把限流交给服务端拒绝——表现为一批工具同时返回 <c>45009</c>，
    /// 而模型会逐个重试，进一步放大。限并发让过载变成「排队」而不是「一起失败」。
    /// </remarks>
    public int MaxConcurrentCalls { get; set; } = 8;

    /// <summary>是否强制授权（默认 <see langword="true"/>；关闭即「白名单命中即可调」，仅限只读面）。</summary>
    public bool EnforceAuthorization { get; set; } = true;

    /// <summary>
    /// 写工具（<see cref="WechatToolRisk.Write"/>）是否要求 HITL 审批（默认 <see langword="true"/>）。
    /// </summary>
    /// <remarks>
    /// <see cref="WechatToolRisk.HighRiskWrite"/> <b>不受本开关影响</b>：高危写恒需审批
    /// （关闭即装配期失败），因为它是「不可逆 / 外部可见」动作的唯一护栏。
    /// </remarks>
    public bool RequireApprovalForWrites { get; set; } = true;

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

        var highRisk = writes
            .Where(static name => WechatToolContracts.ByToolName[name].Risk == WechatToolRisk.HighRiskWrite)
            .ToArray();
        if (highRisk.Length > 0 && !RequireApprovalForWrites)
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy 启用了高危写工具（{string.Join(", ", highRisk)}）却关闭了审批——"
                + "高危写恒需 HITL 确认门（不可逆动作不得只靠授权器放行）。");
        }

        if (MaxResultLength is not 0 and (< 512 or > 65536))
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy.MaxResultLength 为 0（不限制）或 512..65536，实际值: {MaxResultLength}"
                + "（过小会让列表工具永远拿不到完整答案，过大则失去拦截上下文爆点的意义）。");
        }

        if (!string.Equals(ContentSafetyMode, ContentSafetyWarn, StringComparison.Ordinal)
            && !string.Equals(ContentSafetyMode, ContentSafetyOff, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy.ContentSafetyMode 只能是 \"{ContentSafetyWarn}\" 或 \"{ContentSafetyOff}\"，实际值: {ContentSafetyMode}");
        }

        if (CallTimeout != TimeSpan.Zero && (CallTimeout < TimeSpan.FromSeconds(1) || CallTimeout > TimeSpan.FromSeconds(600)))
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy.CallTimeout 为 0（不限制）或 1..600 秒，实际值: {CallTimeout}"
                + "（过短会把正常慢接口判成超时，过长等于没有超时）。");
        }

        if (MaxConcurrentCalls is not 0 and (< 1 or > 64))
        {
            throw new InvalidOperationException(
                $"WechatToolPolicy.MaxConcurrentCalls 为 0（不限制）或 1..64，实际值: {MaxConcurrentCalls}"
                + "（并发上限的意义是「排队而不是一起触发限流」）。");
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
