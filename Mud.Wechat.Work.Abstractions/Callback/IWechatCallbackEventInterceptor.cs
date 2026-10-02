// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback;

/// <summary>
/// 企业微信回调事件拦截器（对齐 <c>IFeishuEventInterceptor</c>；v1 方案 §5.6）。
/// </summary>
/// <remarks>
/// <para>
/// <b>BeforeHandleAsync 返回 false 的后果</b>：中断分发流程，接收端以 <b>503</b> 应答
/// （触发企业微信按「5 秒超时重试 3 次」策略重推；语义对齐飞书 Webhook 通道的「拦截即请求重发」）。
/// 需要吞掉事件时应返回 true 且不做任何事——返回 false 即表达「未就绪，请重推」。
/// </para>
/// <para>
/// 实例由分发器在请求 scope 内解析（Transient），经 <c>WechatCallbackServiceBuilder.AddInterceptor&lt;T&gt;</c>
/// 注册（可按 AppKey 限定或全局注册到通配键）；执行顺序为 appKey 专属先于全局，同组按注册序。
/// </para>
/// </remarks>
public interface IWechatCallbackEventInterceptor
{
    /// <summary>事件处理前拦截。</summary>
    /// <param name="eventType">事件类型键（<see cref="WechatCallbackEvent.EventTypeKey"/>）。</param>
    /// <param name="eventData">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>true = 放行；false = 中断分发并以 503 触发企业微信重推。</returns>
    Task<bool> BeforeHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default);

    /// <summary>事件处理后拦截（处理器集合执行完毕后调用；处理器异常不影响本调用）。</summary>
    /// <param name="eventType">事件类型键（<see cref="WechatCallbackEvent.EventTypeKey"/>）。</param>
    /// <param name="eventData">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完成任务。</returns>
    Task AfterHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default);
}
