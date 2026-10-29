// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具调用审计端口（合规落库点）：每次工具调用<b>恰好一条</b>记录，成功路径也留痕。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么现在必须有</b>：企业客户落地 AI 助理的第一道合规问询是「谁在什么时候让 AI 做了什么」。
/// 这类事实<b>不能事后补</b>——当日志里没有成功路径，就永远没有。
/// </para>
/// <para>
/// <b>为什么是「尽力而为」而不是事务</b>：审计写入失败<b>不得</b>阻断工具执行（否则审计存储抖动会让业务停摆），
/// 但执行失败<b>必须</b>留痕。两个方向都只有一种正确处置，故由执行链统一 try/catch 并记 Warning，
/// 实现方不需要（也不应该）自己吞异常。
/// </para>
/// <para>
/// <b>默认实现是空实现</b>：未注册 sink 时零成本、零副作用；宿主注册即生效（<c>TryAdd</c> 语义，
/// 先注册者胜出）。
/// </para>
/// </remarks>
public interface IWechatToolAuditSink
{
    /// <summary>写入一条审计记录。</summary>
    /// <param name="record">审计记录（只含摘要与事实）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>写入任务。</returns>
    ValueTask WriteAsync(WechatToolAuditRecord record, CancellationToken cancellationToken = default);
}

/// <summary>审计端口的空实现（未注册 sink 时的默认值：零成本、零副作用）。</summary>
public sealed class NullWechatToolAuditSink : IWechatToolAuditSink
{
    /// <summary>空实现的单例（无状态，可安全共享）。</summary>
    public static readonly NullWechatToolAuditSink Instance = new();

    /// <inheritdoc />
    public ValueTask WriteAsync(WechatToolAuditRecord record, CancellationToken cancellationToken = default)
        => default; // ValueTask.CompletedTask 自 net5.0 起才提供，四档统一用 default（语义等价）
}
