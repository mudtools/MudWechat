// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 回调事件载荷读取端口（v2 方案 ADR-7）：把事件信封解析为<b>状态化</b>的强类型载荷。
/// </summary>
/// <remarks>
/// <para>
/// <b>单次解析</b>（ADR-6）：读取器与接收器共享同一份 XML 源缓存，
/// 每个事件的 <c>XDocument.Parse</c> 与节点投影各发生<b>恰好一次</b>。
/// </para>
/// <para>
/// <b>不抛</b>：沿用旧解析器「键不匹配 / 明文非 XML 均不抛」的语义（行为保持矩阵 B1/B10），
/// 失败以 <see cref="WechatPayloadReadStatus"/> 表达。
/// </para>
/// </remarks>
public interface IWechatPayloadReader
{
    /// <summary>
    /// 按事件键解析指定类型的载荷。
    /// </summary>
    /// <typeparam name="TPayload">目标载荷类型（须已按事件键登记契约；<see cref="GenericCallbackPayload"/> 免登记）。</typeparam>
    /// <param name="evt">回调事件信封（含解密明文）。</param>
    /// <returns>状态化读取结果。</returns>
    WechatPayloadReadResult<TPayload> Read<TPayload>(WechatCallbackEvent evt)
        where TPayload : WechatCallbackPayload;

    /// <summary>
    /// 按事件键解析载荷（动态路径：由分发器按处理器声明的载荷类型调用）。
    /// </summary>
    /// <param name="evt">回调事件信封。</param>
    /// <param name="payloadType">目标载荷类型。</param>
    /// <returns>状态化读取结果；未登记契约时降级为 <see cref="GenericCallbackPayload"/>。</returns>
    WechatPayloadReadResult<WechatCallbackPayload> Read(WechatCallbackEvent evt, Type payloadType);
}
