// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Callback;

/// <summary>
/// 微信小店回调事件处理器（信封级）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SupportedEventType"/> 语义 = <see cref="WechatChannelsCallbackEvent.EventTypeKey"/>（明文顶层
/// <c>Event</c> 字段）：填官方事件键常量做<b>精确匹配</b>；返回<b>空串/null 表示兜底处理器</b>
/// （仅当事件未被任何精确处理器命中时调用）。
/// </para>
/// <para>
/// 处理器经 <c>AddWechatChannelsCallback().AddHandler&lt;T&gt;(appKey)</c> 注册；实现方须保证：
/// <b>快速返回</b>（软超时默认 4000ms；官方 5 秒收不到响应即判推送失败，按
/// 0s/0s/10s/1min/5min/30min/60min… 重试至多 10 次）、尊重取消令牌、对重投递<b>幂等</b>
/// （抗重放指纹在分发前已消费，同指纹重推会被 403）。
/// </para>
/// </remarks>
public interface IChannelsCallbackEventHandler
{
    /// <summary>支持的事件类型键（= <see cref="WechatChannelsCallbackEvent.EventTypeKey"/>）；空串/null = 兜底处理器。</summary>
    string SupportedEventType { get; }

    /// <summary>处理回调事件。</summary>
    /// <param name="envelope">回调事件信封（解密后的 JSON 明文经接收器校验）。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止与分发软超时）。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default);
}

/// <summary>
/// 载荷感知处理器的<b>非泛型桥</b>：供分发器在「只拿到处理器实例」时按声明的事件键反序列化强类型载荷。
/// </summary>
/// <remarks>
/// <para>
/// 分发器解析出的是 <see cref="IChannelsCallbackEventHandler"/> 集合，无法静态知晓每个处理器期望的
/// <c>TPayload</c>；本接口把「事件键 → 载荷类型」信息经 <see cref="PayloadType"/> 暴露，
/// 分发器按事件键从契约注册表反查并用源生成 JSON 上下文反序列化 —— <b>全程无反射、无 <c>dynamic</c></b>（AOT 安全）。
/// </para>
/// <para>
/// 强类型载荷通道依赖源生成 JSON 上下文，仅 <c>net8.0+</c> 可用（与企微智能机器人 JSON 通道同款门控）；
/// <see cref="PayloadType"/> 在低目标框架下由分发器忽略（信封级处理器不受影响）。
/// </para>
/// </remarks>
public interface IChannelsCallbackPayloadHandler : IChannelsCallbackEventHandler
{
    /// <summary>期望的载荷类型（供分发器反序列化与诊断）。</summary>
    Type PayloadType { get; }

    /// <summary>非泛型回调入口（由泛型接口显式实现；分发器为唯一调用点）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="payload">已绑定的载荷实例（类型与 <see cref="PayloadType"/> 一致）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandlePayloadAsync(WechatChannelsCallbackEvent envelope, object payload, CancellationToken cancellationToken);
}

/// <summary>
/// 类型化载荷处理器（编译期强类型载荷）。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须以 <c>[ChannelsCallbackContract]</c> 登记事件键契约）。</typeparam>
public interface IChannelsCallbackEventHandler<TPayload> : IChannelsCallbackPayloadHandler
    where TPayload : class
{
    /// <summary>处理回调事件（已绑定强类型载荷）。</summary>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="payload">强类型载荷（字段缺失即 <c>null</c>/默认值，处理器不得假设必有值）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(WechatChannelsCallbackEvent envelope, TPayload payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// 类型化载荷处理器的<b>抽象基类</b>：提供非泛型桥接实现，实现方只需覆写
/// <see cref="HandleAsync(WechatChannelsCallbackEvent, TPayload, CancellationToken)"/> 与
/// <see cref="IChannelsCallbackEventHandler.SupportedEventType"/>。
/// </summary>
/// <typeparam name="TPayload">载荷类型（须以 <c>[ChannelsCallbackContract]</c> 登记事件键契约）。</typeparam>
/// <remarks>
/// 用抽象基类而非「接口默认实现」：后者属 C# 8 特性且需运行时支持，
/// <c>netstandard2.0</c> 目标会报 <c>CS8701</c>（本仓库含 4 个 TFM，对齐企微 / 公众号同款裁决）。
/// </remarks>
public abstract class WechatChannelsCallbackPayloadHandler<TPayload> : IChannelsCallbackEventHandler<TPayload>
    where TPayload : class
{
    /// <inheritdoc />
    public abstract string SupportedEventType { get; }

    /// <inheritdoc />
    public abstract Task HandleAsync(
        WechatChannelsCallbackEvent envelope, TPayload payload, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    Type IChannelsCallbackPayloadHandler.PayloadType => typeof(TPayload);

    /// <inheritdoc />
    Task IChannelsCallbackPayloadHandler.HandlePayloadAsync(
        WechatChannelsCallbackEvent envelope, object payload, CancellationToken cancellationToken)
    {
        if (payload is not TPayload typed)
        {
            throw new ArgumentException(
                "载荷实例类型与处理器声明的载荷类型不一致：" + typeof(TPayload).FullName + "。", nameof(payload));
        }

        return HandleAsync(envelope, typed, cancellationToken);
    }

    /// <summary>信封-only 入口对本类型<b>不适用</b>（请经分发器调用，或以 <see cref="IChannelsCallbackPayloadHandler"/> 视角调用）。</summary>
    /// <exception cref="NotSupportedException">恒抛：载荷处理器必须先反序列化载荷才能执行。</exception>
    Task IChannelsCallbackEventHandler.HandleAsync(WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken)
        => throw new NotSupportedException(
            GetType().FullName + " 是载荷处理器，必须先反序列化载荷；请经分发器调用" +
            "（或直接以 IChannelsCallbackPayloadHandler 视角调用 HandlePayloadAsync）。");
}
