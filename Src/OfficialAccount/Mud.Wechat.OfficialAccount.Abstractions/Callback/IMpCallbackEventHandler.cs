// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 公众号回调事件处理器（类型化处理模型）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SupportedEventType"/> 语义 = <see cref="MpCallbackEnvelope.EventTypeKey"/>：
/// 填 <see cref="MpCallbackEventTypes"/> / <see cref="MpCallbackMessageTypes"/> 常量之一做<b>精确匹配</b>；
/// 返回<b>空串/null 表示兜底处理器</b>（仅当事件未被任何精确处理器命中时调用）。
/// </para>
/// <para>
/// 处理器经 <c>MpCallbackServiceBuilder.AddHandler&lt;T&gt;</c> 注册（可按 AppKey 限定或注册到通配键）。
/// 实现方须保证：<b>快速返回</b>（软超时默认 4000ms，超时平台按「不回复」语义断连并重试，共 3 次）、
/// 尊重取消令牌、对重投递<b>幂等</b>（抗重放指纹在分发前已消费）。
/// </para>
/// </remarks>
public interface IMpCallbackEventHandler
{
    /// <summary>支持的事件类型键（= <see cref="MpCallbackEnvelope.EventTypeKey"/>）；空串/null = 兜底处理器。</summary>
    string SupportedEventType { get; }

    /// <summary>处理回调事件。</summary>
    /// <param name="envelope">回调事件信封（解密后已解析公共与专有字段）。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止与分发软超时）。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default);
}

/// <summary>
/// 载荷感知处理器的<b>非泛型桥</b>：供分发器在「只拿到处理器实例」时按声明的载荷类型读取载荷。
/// </summary>
/// <remarks>
/// 分发器解析出的是 <see cref="IMpCallbackEventHandler"/> 集合，无法静态知晓每个处理器期望的
/// <c>TPayload</c>；本接口把该信息以 <see cref="PayloadType"/> 暴露，由分发器经读取器读取后再回调
/// —— <b>全程无反射、无 <c>dynamic</c></b>（AOT 安全）。
/// </remarks>
public interface IMpCallbackPayloadHandler : IMpCallbackEventHandler
{
    /// <summary>期望的载荷类型（供读取器分派与诊断）。</summary>
    Type PayloadType { get; }

    /// <summary>非泛型回调入口（由泛型接口显式实现；分发器为唯一调用点）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="payload">已绑定的载荷实例（类型与 <see cref="PayloadType"/> 一致）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandlePayloadAsync(MpCallbackEnvelope envelope, object payload, CancellationToken cancellationToken);
}

/// <summary>
/// 类型化载荷处理器：处理器侧<b>编译期</b>拿到强类型载荷，无需自行解析与判空。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须在事件键契约表中登记）。</typeparam>
/// <remarks>
/// 未知事件的接法：<c>TPayload</c> 取 <see cref="GenericCallbackPayload"/> 即可接收<b>全部</b>事件的
/// <c>Values</c> 视图（读取状态为 <c>GenericFallback</c>，属正常降级而非失败）。
/// </remarks>
public interface IMpCallbackEventHandler<TPayload> : IMpCallbackPayloadHandler
    where TPayload : MpCallbackPayload
{
    /// <summary>处理回调事件（已绑定强类型载荷）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="payload">强类型载荷（字段缺失即 <c>null</c>，处理器不得假设必有值）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(MpCallbackEnvelope envelope, TPayload payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// 类型化载荷处理器的<b>抽象基类</b>：提供非泛型桥接实现，实现方只需覆写
/// <see cref="HandleAsync(MpCallbackEnvelope, TPayload, CancellationToken)"/> 与
/// <see cref="IMpCallbackEventHandler.SupportedEventType"/>。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须在事件键契约表中登记）。</typeparam>
/// <remarks>
/// 用抽象基类而非「接口默认实现」：后者属 C# 8 特性且需运行时支持，
/// <c>netstandard2.0</c> 目标会报 <c>CS8701</c>（本仓库含 4 个 TFM）。
/// </remarks>
public abstract class MpCallbackPayloadHandler<TPayload> : IMpCallbackEventHandler<TPayload>
    where TPayload : MpCallbackPayload
{
    /// <inheritdoc />
    public abstract string SupportedEventType { get; }

    /// <inheritdoc />
    public abstract Task HandleAsync(
        MpCallbackEnvelope envelope, TPayload payload, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    Type IMpCallbackPayloadHandler.PayloadType => typeof(TPayload);

    /// <inheritdoc />
    Task IMpCallbackPayloadHandler.HandlePayloadAsync(
        MpCallbackEnvelope envelope, object payload, CancellationToken cancellationToken)
    {
        if (payload is not TPayload typed)
        {
            throw new ArgumentException(
                "载荷实例类型与处理器声明的载荷类型不一致：" + typeof(TPayload).FullName + "。", nameof(payload));
        }

        return HandleAsync(envelope, typed, cancellationToken);
    }

    /// <summary>信封-only 入口对本类型<b>不适用</b>（请经分发器调用，或以 <see cref="IMpCallbackPayloadHandler"/> 视角调用）。</summary>
    /// <exception cref="NotSupportedException">恒抛：载荷处理器必须先读取载荷才能执行。</exception>
    Task IMpCallbackEventHandler.HandleAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken)
        => throw new NotSupportedException(
            GetType().FullName + " 是载荷处理器，必须先读取载荷；请经分发器调用" +
            "（或直接以 IMpCallbackPayloadHandler 视角调用 HandlePayloadAsync）。");
}
