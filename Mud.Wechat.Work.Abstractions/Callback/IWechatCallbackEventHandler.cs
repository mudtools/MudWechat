// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback;

/// <summary>
/// 企业微信回调事件处理器（对齐 <c>IFeishuEventHandler</c> 的类型化处理模型；v1 方案 §5.6）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SupportedEventType"/> 语义 = <see cref="WechatCallbackEvent.EventTypeKey"/>（v1 方案 D4）：
/// 填 <see cref="WechatCallbackEventTypes"/> 常量之一做<b>精确匹配</b>（如 <c>create_user</c> /
/// <c>batch_job_result</c> / <c>suite_ticket</c>）；返回<b>空串/null 表示兜底处理器</b>
/// （仅当事件未被任何精确处理器命中时调用；内置授权族处理器 <c>WechatCallbackHandler</c> 即此形态）。
/// </para>
/// <para>
/// 处理器实例由分发器在请求 scope 内解析（Transient），经 <c>WechatCallbackServiceBuilder.AddHandler&lt;T&gt;</c>
/// 注册（可按 AppKey 限定或全局注册到通配键）。实现方须保证：快速返回（软超时默认 4500ms，超时触发 503 重推）、
/// 尊重取消令牌、对重投递幂等（抗重放指纹在分发前已消费，见 v1 方案 §5.4.3 注）。
/// </para>
/// </remarks>
public interface IWechatCallbackEventHandler
{
    /// <summary>支持的事件类型键（= <see cref="WechatCallbackEvent.EventTypeKey"/>）；空串/null = 兜底处理器。</summary>
    string SupportedEventType { get; }

    /// <summary>处理回调事件。</summary>
    /// <param name="eventData">回调事件信封（解密后已解析通用与类别字段）。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止与分发软超时）。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default);
}
