// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Callback;

/// <summary>
/// 微信小店回调事件拦截器（Before / After 观测点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>Before</b>：先于并发闸与软超时执行（异常传播 → 中间件 500）；返回 <c>false</c> 即中断分发，
/// 中间件按官方语义回明文 <c>success</c> + 200（<b>不触发重推</b>）。
/// <b>After</b>：观测点，异常记日志不打断应答。
/// </para>
/// <para>
/// 注册经 <c>AddWechatChannelsCallback().AddInterceptor&lt;T&gt;(appKey)</c>；匹配序与处理器一致
/// （appKey 专属先于全局通配）。
/// </para>
/// </remarks>
public interface IChannelsCallbackEventInterceptor
{
    /// <summary>事件处理前拦截（返回 <c>false</c> 中断分发）。</summary>
    /// <param name="eventType">事件类型键（= <see cref="WechatChannelsCallbackEvent.EventTypeKey"/>）。</param>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns><c>true</c> 继续分发；<c>false</c> 中断（回 success）。</returns>
    Task<bool> BeforeHandleAsync(string eventType, WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default);

    /// <summary>事件处理后观测（异常被隔离）。</summary>
    /// <param name="eventType">事件类型键。</param>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>任务。</returns>
    Task AfterHandleAsync(string eventType, WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default);
}
