// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Configuration;

/// <summary>
/// 企微 AI Agent 的宿主可见配置面（配置节 <c>WechatAi</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影而非继承</b>：本类型是<b>宿主可见面</b>（节名/属性名/异常文案一个字节都不能变），
/// 中立运行期旋钮由 <c>WechatAgent</c> <b>投影</b>构造 <c>AgentRuntimeOptions</c>（中立类型不直配）。
/// 本类型<b>不</b>派生 <c>WechatAppConfigBase</c>：Agent 是进程内运行期能力（单实例），
/// 而应用凭据经 <c>appKey</c> 从多应用注册表解析（一个 Agent 服务多个应用）。
/// </para>
/// <para>
/// <b>源生成绑定纪律</b>：属性<b>禁</b> <c>required</c>（生成器以 <c>new T()</c> 构造），校验写进
/// <see cref="Validate"/>，由 <c>WechatAgentOptionsValidator</c> 在解析时触发。
/// </para>
/// <para>
/// <b>状态键是产品事实</b>：<see cref="HistoryStateKey"/> / <see cref="PendingReplyStateKey"/> 的字面量归属产品侧
/// （中立包不得硬编码任何产品前缀）；后者留空 = <b>关闭 outbox 补发</b>（管线按「未配置状态键即禁用」处理）。
/// </para>
/// </remarks>
public sealed class WechatAgentOptions
{
    /// <summary>默认配置节名。</summary>
    public const string DefaultSectionName = "WechatAi";

    /// <summary>默认的会话历史状态键（产品事实字面量，单一来源）。</summary>
    public const string DefaultHistoryStateKey = "wechat.agent.history";

    /// <summary>默认的待确认回复状态键（outbox 补发键；留空即禁用补发）。</summary>
    public const string DefaultPendingReplyStateKey = "wechat.agent.pending-reply";

    /// <summary>默认的待确认回复「轮次标识」状态键（outbox 补发键，与 <see cref="DefaultPendingReplyStateKey"/> 成对）。</summary>
    public const string DefaultPendingReplyTurnStateKey = "wechat.agent.pending-reply-turn";

    /// <summary>默认系统指令：企微智能机器人助手（简洁、可执行、不臆造数据）。</summary>
    public const string DefaultInstructions =
        "你是企业微信中的智能助手。回答要简洁、准确、可执行；不确定的事实要明确说明不确定，不要臆造组织架构、人员或业务数据。"
        + "涉及多条信息时优先用短列表；涉及操作时先说明将要执行的动作。";

    /// <summary>系统指令（Agent 的人格与边界；空白非法）。</summary>
    public string Instructions { get; set; } = DefaultInstructions;

    /// <summary>会话存储 TTL（秒；进程内默认存储与后续分布式存储共用同一阈值源）。</summary>
    public int SessionTtlSeconds { get; set; } = 1800;

    /// <summary>会话历史裁剪窗（非 system 消息条数上限）。</summary>
    public int MaxHistoryMessages { get; set; } = 50;

    /// <summary>触发渐进式摘要的消息数阈值（0 = 禁用摘要）。</summary>
    public int SummaryThreshold { get; set; } = 30;

    /// <summary>触发摘要的历史 token 上限（0 = 仅按条数阈值判定）。</summary>
    public int MaxHistoryTokens { get; set; } = 8000;

    /// <summary>单轮工具调用预算上限（0 = 不限制）。</summary>
    public int MaxToolCallsPerTurn { get; set; } = 10;

    /// <summary>
    /// 回合上下文注入的时区（IANA 或 Windows 时区标识；默认 <see cref="DefaultTimeZone"/>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 模型没有时钟，也不能从历史消息推断「今天」。若不注入，任何「本周的日程 / 昨天的日报 / 下周一开会」
    /// 都会被解析到错误的日期，而<b>错误日期不报错</b>（工具调用成功、结果看起来正常）。这是唯一必须
    /// 每回合注入的动态事实，故做成配置而不是让模型调工具（多一次工具调用只为拿时间，纯浪费预算）。
    /// </para>
    /// <para>
    /// 同时注入 UTC 与本地时间是刻意的：企微接口的时间字段多为 Unix 秒（UTC 语义），
    /// 而人类口语里的「今天」是本地语义。只给一种，模型就必然在其中一侧算错边界。
    /// </para>
    /// </remarks>
    public string TimeZone { get; set; } = DefaultTimeZone;

    /// <summary>默认时区（中国标准时间）。</summary>
    public const string DefaultTimeZone = "Asia/Shanghai";

    /// <summary>会话历史在状态袋内的键（产品事实；不得为空）。</summary>
    public string HistoryStateKey { get; set; } = DefaultHistoryStateKey;

    /// <summary>待确认回复（outbox）状态键（产品事实；留空 = 禁用补发）。</summary>
    public string PendingReplyStateKey { get; set; } = DefaultPendingReplyStateKey;

    /// <summary>待确认回复「轮次标识」状态键（outbox 补发键，<b>与 <see cref="PendingReplyStateKey"/> 成对</b>；留空 = 禁用补发）。</summary>
    public string PendingReplyTurnStateKey { get; set; } = DefaultPendingReplyTurnStateKey;

    /// <summary>
    /// 校验配置（宿主可见文案；非法取值 fail-fast）。
    /// </summary>
    /// <remarks>
    /// 取值范围与中立 <c>AgentRuntimeOptions.Validate</c> 的默认语义一致，但文案归属产品侧
    /// （宿主看到的是 <c>WechatAi:</c> 前缀，便于在配置文件中定位）。
    /// </remarks>
    /// <exception cref="InvalidOperationException">存在非法取值。</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Instructions))
        {
            throw new InvalidOperationException($"WechatAi:{nameof(Instructions)} 不能为空。");
        }

        if (SessionTtlSeconds is < 1 or > 604_800)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(SessionTtlSeconds)} 须在 1..604800 之间，实际值: {SessionTtlSeconds}");
        }

        if (MaxHistoryMessages is < 1 or > 10_000)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(MaxHistoryMessages)} 须在 1..10000 之间，实际值: {MaxHistoryMessages}");
        }

        if (SummaryThreshold is < 0 or (> 0 and < 4) or > 10_000)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(SummaryThreshold)} 为 0（禁用）或 4..10000，实际值: {SummaryThreshold}");
        }

        if (MaxHistoryTokens is < 0 or > 1_000_000)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(MaxHistoryTokens)} 为 0（仅按条数）或 1..1000000，实际值: {MaxHistoryTokens}");
        }

        if (MaxToolCallsPerTurn is < 0 or > 100)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(MaxToolCallsPerTurn)} 为 0（不限制）或 1..100，实际值: {MaxToolCallsPerTurn}");
        }

        if (string.IsNullOrWhiteSpace(TimeZone))
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(TimeZone)} 不能为空（模型没有时钟，「今天」只能由此锚定）。");
        }

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(TimeZone)} 不是有效的时区标识（应为 IANA 形式，如 \"{DefaultTimeZone}\"），实际值: {TimeZone}",
                ex);
        }

        if (string.IsNullOrWhiteSpace(HistoryStateKey))
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(HistoryStateKey)} 不能为空（它是会话历史在状态袋内的同源锚点）。");
        }

        // 两个 outbox 状态键成对使用（中立管线在 SetPendingReply 同时写两份）：
        // 只配一半会在首轮生成后抛 ArgumentNullException（回合失败且不落库——用户收不到回复）。故要么都给、要么都空。
        var hasPendingKey = !string.IsNullOrWhiteSpace(PendingReplyStateKey);
        var hasPendingTurnKey = !string.IsNullOrWhiteSpace(PendingReplyTurnStateKey);
        if (hasPendingKey != hasPendingTurnKey)
        {
            throw new InvalidOperationException(
                $"WechatAi:{nameof(PendingReplyStateKey)} 与 {nameof(PendingReplyTurnStateKey)} 必须成对配置"
                + "（两者皆空 = 关闭 outbox 补发；只配其一会让回合在生成后抛异常且不落库）。");
        }
    }
}
