// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Payloads;

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

/// <summary>
/// 载荷感知处理器的<b>非泛型桥</b>：供分发器在「只拿到处理器实例」时按声明的载荷类型读取载荷。
/// </summary>
/// <remarks>
/// <b>为何需要桥</b>：分发器在请求 scope 内解析出的是 <see cref="IWechatCallbackEventHandler"/> 实例集合，
/// 无法静态知晓每个处理器期望的 <c>TPayload</c>。本接口把该信息以 <see cref="PayloadType"/> 暴露，
/// 由分发器经 <c>IWechatPayloadReader</c> 读取后再回调 <see cref="HandlePayloadAsync"/> ——
/// <b>全程无反射、无 <c>dynamic</c></b>（AOT 安全）。
/// </remarks>
public interface IWechatCallbackPayloadHandler : IWechatCallbackEventHandler
{
    /// <summary>期望的载荷类型（供读取器分派与诊断）。</summary>
    Type PayloadType { get; }

    /// <summary>
    /// 非泛型回调入口（由 <see cref="IWechatCallbackEventHandler{TPayload}"/> 显式实现；
    /// 分发器为唯一调用点）。
    /// </summary>
    /// <param name="eventData">回调事件信封。</param>
    /// <param name="payload">已绑定的载荷实例（类型与 <see cref="PayloadType"/> 一致）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandlePayloadAsync(WechatCallbackEvent eventData, object payload, CancellationToken cancellationToken);
}

/// <summary>
/// 类型化载荷处理器（v2 方案 ADR-10）：处理器侧<b>编译期</b>拿到强类型载荷，无需自行解析与判空。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须在事件键契约表中登记）。</typeparam>
/// <remarks>
/// <para>
/// 实现方只需实现 <see cref="HandleAsync(WechatCallbackEvent, TPayload, CancellationToken)"/>；
/// 两个桥接成员已在本接口内<b>显式实现</b>（<c>default</c> 不适用 —— 需引用 <c>TPayload</c> 做转型）。
/// </para>
/// <para>
/// 未知事件的接法：实现
/// <see cref="IWechatCallbackEventHandler{TPayload}"/> 并把 <c>TPayload</c> 取为
/// <c>GenericCallbackPayload</c>，即可接收<b>全部</b>事件的 <c>Values</c> 视图
/// （读取状态为 <c>GenericFallback</c>，属正常降级而非失败）。
/// </para>
/// </remarks>
public interface IWechatCallbackEventHandler<TPayload> : IWechatCallbackPayloadHandler
    where TPayload : WechatCallbackPayload
{
    /// <summary>处理回调事件（已绑定强类型载荷）。</summary>
    /// <param name="eventData">回调事件信封。</param>
    /// <param name="payload">强类型载荷（字段缺失即 <c>null</c>，处理器不得假设必有值）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(WechatCallbackEvent eventData, TPayload payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// 类型化载荷处理器的<b>抽象基类</b>：提供非泛型桥接实现，实现方只需覆写
/// <see cref="HandleAsync(WechatCallbackEvent, TPayload, CancellationToken)"/> 与
/// <see cref="IWechatCallbackEventHandler.SupportedEventType"/>。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须在事件键契约表中登记）。</typeparam>
/// <remarks>
/// <para>
/// <b>为何用抽象基类而非「接口默认实现」</b>：桥接成员（<c>PayloadType</c> / <c>HandlePayloadAsync</c>）
/// 若以<b>默认接口实现</b>写在 <see cref="IWechatCallbackEventHandler{TPayload}"/> 内，
/// 属 C# 8 特性且需要<b>运行时支持</b> ⇒ <c>netstandard2.0</c> 目标报 <c>CS8701</c>
/// （本仓库含 4 个 TFM，该写法不可用）。抽象基类在所有 TFM 上均可用，且把桥接样板收口到一处。
/// </para>
/// <para>
/// <b>未知事件的接法</b>：把 <typeparamref name="TPayload"/> 取为 <c>GenericCallbackPayload</c>，
/// 即可接收<b>全部</b>事件的 <c>Values</c> 视图（读取状态为 <c>GenericFallback</c>，属正常降级）。
/// </para>
/// </remarks>
public abstract class WechatCallbackPayloadHandler<TPayload> : IWechatCallbackEventHandler<TPayload>
    where TPayload : WechatCallbackPayload
{
    /// <inheritdoc />
    public abstract string SupportedEventType { get; }

    /// <inheritdoc />
    public abstract Task HandleAsync(
        WechatCallbackEvent eventData, TPayload payload, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    Type IWechatCallbackPayloadHandler.PayloadType => typeof(TPayload);

    /// <inheritdoc />
    Task IWechatCallbackPayloadHandler.HandlePayloadAsync(
        WechatCallbackEvent eventData, object payload, CancellationToken cancellationToken)
    {
        if (payload is not TPayload typed)
        {
            throw new ArgumentException(
                "载荷实例类型与处理器声明的载荷类型不一致：" + typeof(TPayload).FullName + "。", nameof(payload));
        }

        return HandleAsync(eventData, typed, cancellationToken);
    }

    /// <summary>
    /// 信封-only 调用入口对本类型<b>不适用</b>：载荷处理器必须先读取载荷才能执行。
    /// </summary>
    /// <remarks>
    /// 分发器在调用前先判定 <see cref="IWechatCallbackPayloadHandler"/> 并走
    /// <see cref="IWechatCallbackPayloadHandler.HandlePayloadAsync"/>，故本路径<b>在分发管线中不可达</b>。
    /// 若直接以 <see cref="IWechatCallbackEventHandler"/> 使用本类型并调用本方法，将得到本异常 ——
    /// 措辞指向正确用法（改为经分发器，或以 <c>IWechatCallbackPayloadHandler</c> 视角调用）。
    /// </remarks>
    /// <exception cref="NotSupportedException">恒抛：请以载荷处理器路径调用。</exception>
    Task IWechatCallbackEventHandler.HandleAsync(
        WechatCallbackEvent eventData, CancellationToken cancellationToken)
    {
        throw new NotSupportedException(
            GetType().FullName + " 是载荷处理器，必须先读取载荷；请经分发器调用" +
            "（或直接以 IWechatCallbackPayloadHandler 视角调用 HandlePayloadAsync）。");
    }
}
