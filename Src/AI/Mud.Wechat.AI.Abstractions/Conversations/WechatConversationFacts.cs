// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Conversations;

/// <summary>
/// 微信生态会话事实（中性词汇）：各线把本线回调信封抽取为本类型，再由共享工厂投影为中立的
/// <c>ConversationRequest</c>（见 <c>Mud.Wechat.AI.Agent</c> 的 <c>WechatConversationRequestFactory</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何先抽事实再投影</b>：七线信封形态各异（企微 Bot JSON / 公众号 XML / 小店 JSON），
/// 但「会话键维度 + 发送者 + 触发文本 + 引用」是同一组概念；把「信封 → 事实」留在各线，
/// 「事实 → 中立请求」收成共享工厂一处，可避免七份逐字相同的投影代码。
/// </para>
/// <para>
/// <b>词汇纪律</b>：本类型不出现任何生态专有名词（<c>AppKey</c> 沿用「租户/应用维度标识」的既有通用语义，
/// 与中立 <c>ConversationRequest.AppKey</c> 同义）。字段全部可空 —— 缺失由消费方按用例语义决定是否拒绝
/// （工厂对必填维度做 fail-fast，见其注释）。
/// </para>
/// </remarks>
public sealed class WechatConversationFacts
{
    /// <summary>创建会话事实。</summary>
    /// <param name="appKey">应用维度标识（多应用注册表键）。</param>
    /// <param name="isGroup">是否群聊（决定会话主体维度：群聊按 chat、单聊按 user）。</param>
    /// <param name="subjectId">会话主体 ID（群聊 = chat_id；单聊 = 发送者 user_id）。</param>
    /// <param name="senderId">发送者用户 ID。</param>
    /// <param name="messageId">触发消息 ID（回复定位用）。</param>
    /// <param name="mentionedText">触发文本（可空；媒体/事件类回调无文本）。</param>
    /// <param name="chatId">回复目标会话 ID（可空；群聊缺省回退会话主体）。</param>
    /// <param name="parentId">引用（父）消息 ID（可空）。</param>
    /// <param name="threadId">话题维度（可空）。</param>
    /// <param name="eventKey">事件族判别键（可空；取值单一源留各线常量）。</param>
    public WechatConversationFacts(
        string? appKey,
        bool isGroup,
        string? subjectId,
        string? senderId,
        string? messageId,
        string? mentionedText = null,
        string? chatId = null,
        string? parentId = null,
        string? threadId = null,
        string? eventKey = null)
    {
        AppKey = appKey;
        IsGroup = isGroup;
        SubjectId = subjectId;
        SenderId = senderId;
        MessageId = messageId;
        MentionedText = mentionedText;
        ChatId = chatId;
        ParentId = parentId;
        ThreadId = threadId;
        EventKey = eventKey;
    }

    /// <summary>应用维度标识（多应用注册表键）。</summary>
    public string? AppKey { get; }

    /// <summary>是否群聊。</summary>
    public bool IsGroup { get; }

    /// <summary>会话主体 ID（群聊 = chat_id；单聊 = 发送者 user_id）。</summary>
    public string? SubjectId { get; }

    /// <summary>发送者用户 ID。</summary>
    public string? SenderId { get; }

    /// <summary>触发消息 ID（回复定位用）。</summary>
    public string? MessageId { get; }

    /// <summary>触发文本（可空）。</summary>
    public string? MentionedText { get; }

    /// <summary>回复目标会话 ID（可空）。</summary>
    public string? ChatId { get; }

    /// <summary>引用（父）消息 ID（可空）。</summary>
    public string? ParentId { get; }

    /// <summary>话题维度（可空）。</summary>
    public string? ThreadId { get; }

    /// <summary>事件族判别键（可空）。</summary>
    public string? EventKey { get; }
}
