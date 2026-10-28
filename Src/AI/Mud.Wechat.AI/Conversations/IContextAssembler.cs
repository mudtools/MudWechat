// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Conversations;

/// <summary>
/// 事件装配器端口（跨七线）：把「本线事件抽取出的会话事实」装配成一次可执行的回合上下文。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要这个端口</b>（方案 §四 S3「事件/上下文装配」）：回调事件按族增长（文本 / 图文混排 / 语音 /
/// 进入会话 / 模板卡片 / 反馈……），每族都要走同一套后续机器（身份写入 → 后台调度 → 中立请求 → 回复通道 →
/// 回合 → 异常收口）。把「事实 → 回合上下文」收成端口后：① 新增事件族只写一个薄处理器（信封 → 输入），
/// 后续机器零复制；② 各线可覆写装配策略（例如给某族加系统提示、改写会话键），无需改派发内核；
/// ③ 装配逻辑可脱离 LLM 单测（纯投影，无网络无模型）。
/// </para>
/// <para>
/// <b>与「信封 → 事实」抽取的分界</b>：抽取留在各线（信封形态千线千面，且含各线硬约束），
/// 装配是跨线共性（事实 → 中立请求只有一种正确投影）。二者在 <see cref="ConversationAssemblyInput"/> 上相接。
/// </para>
/// <para>
/// <b><see cref="SupportedEventKey"/> 语义</b>：填本线事件族常量做<b>精确路由</b>；空串 =
/// <b>兜底装配器</b>（未被任何精确装配器命中的事件族走它）。与回调处理器契约的
/// 「精确匹配 + 兜底」同构，便于宿主按同一心智模型注册。
/// </para>
/// <para>
/// <b>返回 <see langword="null"/> = 不进回合</b>：必填维度缺失（会话键塌陷风险）、该族无可用文本等
/// 情形下装配器应返回 <see langword="null"/> 而<b>不抛</b>——调用方（各线处理器）按「空包应答」处理，
/// 与「快速返回」的回调契约一致；抛异常会让分发内核把产品缺陷变成 5 秒超时。
/// </para>
/// </remarks>
public interface IContextAssembler
{
    /// <summary>负责的事件族键（本线常量；空串 = 兜底装配器）。</summary>
    string SupportedEventKey { get; }

    /// <summary>把装配输入投影为可执行的回合上下文。</summary>
    /// <param name="input">装配输入（事实 + 回复通道凭据 + 身份维度）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>回合上下文；<see langword="null"/> = 该事件不应进入回合（调用方按空包处理）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> 为 <see langword="null"/>。</exception>
    ValueTask<ConversationAssembly?> AssembleAsync(
        ConversationAssemblyInput input,
        CancellationToken cancellationToken = default);
}
