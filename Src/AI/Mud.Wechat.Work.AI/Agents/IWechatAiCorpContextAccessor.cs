// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Context;
using Mud.AI.Agent.Events;
using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 当前异步流的工具身份上下文（<b>唯一建立点在事件层</b>）：回调处理器在启动回合前进入作用域，
/// 工具作用域工厂（<see cref="ITurnScopeFactory"/> 实现）在工具执行期读取，回合结束自动还原。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要它</b>：中立回合管线只透传 <c>TurnScopeContext</c>（<c>TenantKey</c>/<c>ConversationKey</c>/
/// <c>ChatId</c>/<c>UserId</c>/<c>ThreadId</c>/<c>Facts</c>），其中「授权方企业 CorpId」属产品事实；
/// 第三方/代开发工具执行必须切到「应用 + 企业」复合作用域。故企业维度经本端口由事件层注入，
/// 与上游「产品实现持有环境量、中立侧只知进出」的定式一致（不新建第二个上下文槽位）。
/// </para>
/// <para>
/// <b>实现是中立 <see cref="AmbientScope{T}"/> 的薄壳</b>：栈式恢复 + 所有权校验由中立机制承担，
/// 本端口只声明「T = <see cref="WechatToolPrincipal"/>」与生命周期——产品侧不再自写
/// <c>AsyncLocal</c> + 手配对复位（乱序释放覆盖内层 = 多租户越权读取）。
/// </para>
/// </remarks>
public interface IWechatAiCorpContextAccessor
{
    /// <summary>当前异步流的工具身份（<see langword="null"/> = 未声明，按自建应用缺省语义处理）。</summary>
    WechatToolPrincipal? Current { get; set; }

    /// <summary>进入工具身份作用域（<b>推荐用法</b>：<c>using var _ = accessor.Begin(principal);</c>）。</summary>
    /// <param name="principal">工具身份（<see langword="null"/> = 显式「无身份」作用域）。</param>
    /// <returns>作用域（释放时还原进入前的值；重复释放无副作用）。</returns>
    IDisposable Begin(WechatToolPrincipal? principal);
}

/// <summary>
/// <see cref="IWechatAiCorpContextAccessor"/> 的中立环境量实现（DI 单例，按异步流隔离）。
/// </summary>
/// <remarks>
/// <b><see cref="Current"/> 的写入口是兼容形态</b>（历史调用点用「赋值 → finally 复位」）：赋值等价于
/// 「就地建立一层作用域」，读取语义与赋值逐字节一致；生产路径一律改用 <see cref="Begin"/>，
/// 以配对释放而不是靠手工复位（手工复位会覆盖更外层的作用域）。
/// </remarks>
internal sealed class WechatAiCorpContextAccessor : IWechatAiCorpContextAccessor
{
    /// <inheritdoc />
    public WechatToolPrincipal? Current
    {
        get => AmbientScope<WechatToolPrincipal>.Current;
        set => AmbientScope<WechatToolPrincipal>.Begin(value!);
    }

    /// <inheritdoc />
    public IDisposable Begin(WechatToolPrincipal? principal)
        => AmbientScope<WechatToolPrincipal>.Begin(principal!);
}
