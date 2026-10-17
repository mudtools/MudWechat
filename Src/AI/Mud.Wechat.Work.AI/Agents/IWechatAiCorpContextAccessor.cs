// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Events;
using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 当前异步流的工具身份上下文（<b>唯一建立点在事件层</b>）：回调处理器在启动回合前写入，
/// 工具作用域工厂（<see cref="ITurnScopeFactory"/> 实现）在工具执行期读取，回合结束清空。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要它</b>：中立回合管线只透传 <c>TurnScopeContext</c>（<c>TenantKey</c>/<c>ConversationKey</c>/
/// <c>ChatId</c>/<c>UserId</c>/<c>ThreadId</c>），<b>不含</b>「授权方企业 CorpId」这一企微专属维度；
/// 而第三方/代开发工具执行必须切到「应用 + 企业」复合作用域。故企业维度经本端口由事件层注入，
/// 与上游「产品实现持有 AsyncLocal、中立侧只知进出」的定式一致（不新建第二个上下文槽位）。
/// </para>
/// <para>
/// <b>并发语义</b>：实现基于 <see cref="AsyncLocal{T}"/>，按异步流隔离——同进程并发多回合互不污染；
/// 回合结束必须清空（处理器在 <c>finally</c> 中复位），否则同一请求内的后续处理器会读到残留身份。
/// </para>
/// </remarks>
public interface IWechatAiCorpContextAccessor
{
    /// <summary>当前异步流的工具身份（<see langword="null"/> = 未声明，按自建应用缺省语义处理）。</summary>
    WechatToolPrincipal? Current { get; set; }
}

/// <summary>
/// <see cref="IWechatAiCorpContextAccessor"/> 的 <see cref="AsyncLocal{T}"/> 实现（DI 单例，按异步流隔离）。
/// </summary>
internal sealed class WechatAiCorpContextAccessor : IWechatAiCorpContextAccessor
{
    private readonly AsyncLocal<WechatToolPrincipal?> _current = new();

    /// <inheritdoc />
    public WechatToolPrincipal? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}
