// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 公众号回调事件拦截器：处理器执行<b>前</b>的准入/审计与执行<b>后</b>的观测。
/// </summary>
/// <remarks>
/// <para>
/// 注册经 <c>MpCallbackServiceBuilder.AddInterceptor&lt;T&gt;</c>（可按 AppKey 限定或注册到通配键）。
/// </para>
/// <para>
/// <see cref="BeforeHandleAsync"/> 返回 <c>false</c> 即中断分发（分发结果 <c>Interrupted</c>）——
/// 公众号侧的中断出口为明文 <c>success</c> + 200（<b>不</b>触发重推，见方案 §5.3）。
/// </para>
/// <para>
/// 拦截器实例在请求 scope 内解析；<see cref="AfterHandleAsync"/> 抛出的非取消异常记日志后忽略。
/// </para>
/// </remarks>
public interface IMpCallbackEventInterceptor
{
    /// <summary>处理器执行前的准入判定。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns><c>true</c> = 继续分发；<c>false</c> = 中断。</returns>
    Task<bool> BeforeHandleAsync(
        string eventTypeKey, MpCallbackEnvelope envelope, CancellationToken cancellationToken = default);

    /// <summary>处理器执行后的观测（审计/埋点）。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完成任务。</returns>
    Task AfterHandleAsync(
        string eventTypeKey, MpCallbackEnvelope envelope, CancellationToken cancellationToken = default);
}
