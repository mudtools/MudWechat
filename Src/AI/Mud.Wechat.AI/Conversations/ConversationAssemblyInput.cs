// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.AI.Abstractions.Conversations;

/// <summary>
/// 事件装配输入（中性）：各线把「本线信封抽取出的事实 + 回复通道凭据 + 工具身份维度」打包成它，
/// 交给 <see cref="IContextAssembler"/> 装配成可执行的回合。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么凭据与身份也进输入</b>：回复通道的一次性凭据（企微 = <c>response_code</c>）与身份维度
/// （企微 = 应用 + 授权企业）都是「本次事件」的事实，不是全局配置；让它们随输入流动，
/// 装配器才能给出完整的回合上下文，而调用方（各线处理器）无需理解装配细节。
/// </para>
/// <para>
/// <b>回复通道凭据只透传不解释</b>：端口不知道也不关心它是什么形态（令牌 / URL / 会话 ID），
/// 「凭据 → 回复通道」的落地留在各线（企微侧见 <c>WechatAibotReplySink</c>）。
/// </para>
/// </remarks>
public sealed class ConversationAssemblyInput
{
    /// <summary>初始化装配输入。</summary>
    /// <param name="facts">会话事实（信封抽取的结果；不得为 <see langword="null"/>）。</param>
    /// <param name="replyChannelKey">回复通道一次性凭据（可空 = 该事件无法主动回复）。</param>
    /// <param name="principal">工具身份维度（可空 = 该线无身份维度）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="facts"/> 为 <see langword="null"/>。</exception>
    public ConversationAssemblyInput(
        WechatConversationFacts facts,
        string? replyChannelKey = null,
        WechatToolPrincipal? principal = null)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        ReplyChannelKey = replyChannelKey;
        Principal = principal;
    }

    /// <summary>会话事实（会话键维度 + 发送者 + 触发文本 + 引用 + 事件族键）。</summary>
    public WechatConversationFacts Facts { get; }

    /// <summary>回复通道一次性凭据（可空 = 无法主动回复，调用方应按「不进回合」处理）。</summary>
    public string? ReplyChannelKey { get; }

    /// <summary>工具身份维度（可空 = 该线无身份维度）。</summary>
    public WechatToolPrincipal? Principal { get; }
}
