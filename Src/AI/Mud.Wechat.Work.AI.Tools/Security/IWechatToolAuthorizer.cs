// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具授权器端口（宿主实现）：回答「本次调用是否放行」。
/// </summary>
/// <remarks>
/// <para>
/// <b>fail-closed</b>：当 <see cref="WechatToolPolicy.EnforceAuthorization"/> 为 <see langword="true"/>（默认）
/// 且容器里没有本端口的实现时，<see cref="WechatToolDispatcher"/> <b>直接拒绝</b>而非放行——
/// 「忘了注册授权器」与「所有工具都不许调」必须是同一个结果，否则配置失误会静默退化成全开。
/// </para>
/// <para>
/// <b>实现纪律</b>：判定必须是<b>纯查询</b>（读缓存/权限点映射），不得在此发起企微 API 调用——
/// 授权器处在每次工具调用的关键路径上，一次远程往返会把延迟翻倍并引入新的失败面。
/// 需要远程事实的场景应在回合开始前预取进作用域上下文。
/// </para>
/// </remarks>
public interface IWechatToolAuthorizer
{
    /// <summary>判定一次工具调用是否放行。</summary>
    /// <param name="context">判定输入（工具 / 应用 / 企业 / 风险 / 权限点 / 入参摘要）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>放行或拒绝。</returns>
    ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken);
}
