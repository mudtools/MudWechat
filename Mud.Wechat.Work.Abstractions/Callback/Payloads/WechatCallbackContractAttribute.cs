// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 声明载荷类型承载的官方事件键、族前置条件与事件键级开放面 —— 契约登记的<b>单一来源</b>。
/// </summary>
/// <remarks>
/// <para>
/// 事件键登记（<c>OfficialPayloadContracts.RegisterAll</c> 方法体）由本仓库
/// <c>WechatPayloadContractRegistrationGenerator</c> 依据本特性在编译期发射：
/// 载荷类同时承载字段映射（<c>[PayloadContract]</c>）与事件键/开放面（本特性），不再手工维护契约表。
/// </para>
/// <para>
/// <b>开放面校验仍在运行期</b>：生成物调用 <c>WechatPayloadContract.CreateWithOpenSurface</c>，
/// 其内部「不宽于官方族默认」校验（ADR-15）在组合根期 fail-fast —— 生成器不做该编译期校验
/// （官方基线不搬进生成器）。
/// </para>
/// <para>
/// <b>允许叠加</b>（<c>AllowMultiple = true</c>）：同一载荷结构族的不同事件键子集可能开放面不同
/// （如 <c>PlainEventPayload</c> 的平铺键为三类应用、<c>share_agent_change</c>/<c>share_chain_change</c>
/// 仅自建）—— 每个开放面组各标一条本特性。
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class WechatCallbackContractAttribute : Attribute
{
    /// <summary>
    /// 族前置条件：要求的信封 <c>Event</c> 值（如 <c>change_contact</c>）；<c>null</c> = 逐键自指
    /// （90240 形态：<c>Event</c> 节点本身即事件键，生成器为每个键发射 <c>requiredEvent: 键值</c>，
    /// 即原 <c>RegisterKeys</c> 语义 —— 与 <c>CreateWithOpenSurface(requiredEvent: null)</c> 的
    /// 「不校验」语义不同，本特性面不支持「不校验」声明）。
    /// </summary>
    public string? RequiredEvent { get; set; }

    /// <summary>
    /// 族前置条件：要求的事件族（默认 <see cref="WechatCallbackEventFamily.Unknown"/> = 不按族校验，
    /// 与原 <c>RegisterKeys</c> 显式传 <c>Unknown</c> 的语义一致）。
    /// <para>
    /// <b>特性命名参数不接受可空枚举</b>（CS0655），故不提供 <c>WechatCallbackEventFamily?</c> 形态；
    /// 运行期 <c>CreateWithOpenSurface(requiredFamily: null)</c> 的「不校验」形态在特性面不存在 ——
    /// 当前 41 键全部声明具体族（ContactChange/BatchJob/ChainChange/Unknown）。
    /// </para>
    /// </summary>
    public WechatCallbackEventFamily RequiredFamily { get; set; }

    /// <summary>
    /// 事件键级开放面：允许的应用模式集合（官方事件键必须显式声明，不得隐式继承族默认 —— ADR-15/CB4c）。
    /// </summary>
    public WechatAppTypeSet SupportedAppTypes { get; set; }

    /// <summary>事件键级开放面：要求的回调通道（<see cref="WechatCallbackChannel"/>，必须显式声明）。</summary>
    public WechatCallbackChannel RequiredChannel { get; set; }

    /// <summary>事件类型键（<c>EventTypeKey</c>，≥1 个；引用 <see cref="WechatCallbackEventTypes"/> 常量书写）。</summary>
    public string[] EventTypes { get; set; } = Array.Empty<string>();
}
