// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 未知事件降级载荷（ADR-4）：零字段绑定，仅承载 <see cref="WechatCallbackPayload.Values"/> 全量读面。
/// </summary>
/// <remarks>
/// <para>
/// 事件键未登记契约时，读取器返回本类型（<see cref="WechatPayloadReadStatus.GenericFallback"/>）。
/// 这让「官方新增事件」或「宿主私有事件」在<b>零 SDK 改动</b>的前提下仍以结构化形态到达宿主。
/// </para>
/// <para>
/// 映射表为<b>零绑定</b>：<c>Elements</c> 为空 ⇒ 上游 <c>ResolveScope</c> 恒返回根节点，
/// <c>Bind</c> 不产生任何赋值，全部信息经 <c>Values</c> 呈现。
/// </para>
/// </remarks>
public sealed class GenericCallbackPayload : WechatCallbackPayload
{
    /// <summary>零绑定映射契约（上方游 <c>PayloadFieldMap&lt;T&gt;.Create</c>，无 <c>Map</c> 调用）。</summary>
    public static IPayloadFieldMap<GenericCallbackPayload> PayloadFieldMap { get; } =
        PayloadFieldMap<GenericCallbackPayload>.Create(nameof(GenericCallbackPayload));
}
