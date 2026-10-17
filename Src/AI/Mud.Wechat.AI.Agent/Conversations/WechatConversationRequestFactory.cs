// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;

namespace Mud.Wechat.AI.Agent.Conversations;

/// <summary>
/// 微信生态会话请求投影工厂（跨七线）：<see cref="WechatConversationFacts"/> → 中立
/// <see cref="ConversationRequest"/>（回合管线的统一输入）。
/// </summary>
/// <remarks>
/// <para>
/// <b>职责边界</b>：本工厂只做「事实 → 中立请求」的<b>纯投影</b>（空值归一 + 会话维度判定），
/// 不做信封解析、不做准入判定（准入属 <c>ConversationalTurnOptions.ValidateRequestPolicies</c>，是产品策略）。
/// 「信封 → 事实」的抽取留在各线（企微侧在 <c>Mud.Wechat.Work.AI</c>）。
/// </para>
/// <para>
/// <b>必填维度 fail-fast</b>：<see cref="ConversationRequest"/> 的会话键维度
/// （<c>AppKey</c> / <c>SubjectId</c> / <c>SenderId</c> / <c>MessageId</c>）为空时直接抛
/// <see cref="ArgumentException"/> —— 空白会话键会让多租户隔离塌陷为同一会话，必须在进管线前拦住。
/// </para>
/// <para>
/// <b>可空维度原样透传</b>：<c>MentionedText</c> / <c>ChatId</c> / <c>ParentId</c> / <c>ThreadId</c> /
/// <c>EventKey</c> 的中立语义本就是「可空 = 该维度未知/不适用」，故<b>不</b>给空串默认值（空串会被装配器当成
/// 「有值但为空」而产出噪声片段）。
/// </para>
/// </remarks>
public static class WechatConversationRequestFactory
{
    /// <summary>把微信生态会话事实投影为中立的会话请求。</summary>
    /// <param name="facts">会话事实（不得为 <see langword="null"/>）。</param>
    /// <returns>中立会话请求。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="facts"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">必填维度（AppKey / SubjectId / SenderId / MessageId）为空。</exception>
    public static ConversationRequest Create(WechatConversationFacts facts)
    {
        if (facts is null)
        {
            throw new ArgumentNullException(nameof(facts));
        }

        return new ConversationRequest(
            AppKey: Require(facts.AppKey, nameof(facts.AppKey)),
            Scope: facts.IsGroup ? ConversationScope.Group() : ConversationScope.P2P(),
            SubjectId: Require(facts.SubjectId, nameof(facts.SubjectId)),
            SenderId: Require(facts.SenderId, nameof(facts.SenderId)),
            MessageId: Require(facts.MessageId, nameof(facts.MessageId)),
            MentionedText: facts.MentionedText,
            ChatId: facts.ChatId,
            ParentId: facts.ParentId,
            ThreadId: facts.ThreadId,
            EventKey: facts.EventKey);
    }

    /// <summary>必填维度校验（空/空白即抛，附维度名便于定位信封抽取缺陷）。</summary>
    private static string Require(string? value, string dimension)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"会话请求的必填维度 '{dimension}' 为空——请检查信封抽取是否漏填。", dimension);
        }

        return value!;
    }
}
