// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.AI.Abstractions.Conversations;

/// <summary>
/// 装配结果（中性）：一次可执行的回合上下文 = 中立请求 + 回复通道凭据 + 工具身份维度。
/// </summary>
/// <remarks>
/// <para>
/// <b>凭据与身份原样带回</b>：装配器不消费它们，只负责「不丢」——回复通道由各线的通道实现落地，
/// 身份维度由各线在执行前写入作用域。三者分离使「装配」可被单独测试（见
/// <c>Mud.Wechat.AI.Agent</c> 的默认装配器用例）。
/// </para>
/// </remarks>
public sealed class ConversationAssembly
{
    /// <summary>初始化装配结果。</summary>
    /// <param name="request">中立会话请求（回合管线的统一输入）。</param>
    /// <param name="replyChannelKey">回复通道一次性凭据（可空）。</param>
    /// <param name="principal">工具身份维度（可空）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> 为 <see langword="null"/>。</exception>
    public ConversationAssembly(
        ConversationRequest request,
        string? replyChannelKey = null,
        WechatToolPrincipal? principal = null)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        ReplyChannelKey = replyChannelKey;
        Principal = principal;
    }

    /// <summary>中立会话请求。</summary>
    public ConversationRequest Request { get; }

    /// <summary>回复通道一次性凭据（可空 = 无法主动回复）。</summary>
    public string? ReplyChannelKey { get; }

    /// <summary>工具身份维度（可空）。</summary>
    public WechatToolPrincipal? Principal { get; }
}
