// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 回调事件载荷基类（v2 方案 ADR-1/ADR-5/ADR-9）。
/// </summary>
/// <remarks>
/// <para>
/// <b>不含任何映射表</b>：字段映射契约由上游 <c>Mud.HttpUtils.Payloads.IPayloadFieldMap&lt;T&gt;</c>
/// 承载并经 <see cref="Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatPayloadContract"/> 在组合根期登记，
/// 载荷类型只保留「数据 + 读面」⇒ 手写链与生成链可<b>逐类</b>切换而不触碰类型定义。
/// </para>
/// <para>
/// <b>ADR-9 敏感面收敛</b>：载荷<b>不持信封引用</b>，且 <see cref="Values"/> 排除凭据节点，
/// 故载荷对象本身不可能携带 <c>SuiteTicket</c>/<c>AuthCode</c>，可安全地日志与序列化。
/// </para>
/// <para>
/// <b>跨程序集不变式</b>：三个属性为 <c>internal set</c>，由 Abstractions 内的物化器填写；
/// 调用方（<c>Mud.Wechat.Work.Callback</c>）凭本程序集既有的
/// <c>&lt;InternalsVisibleTo Include="Mud.Wechat.Work.Callback" /&gt;</c> 获得写权限。
/// 删除该 <c>InternalsVisibleTo</c> 会使读取器<b>编译失败</b>（编译器兜底，不会静默）。
/// </para>
/// </remarks>
public abstract class WechatCallbackPayload
{
    /// <summary>事件类型键（= <c>WechatCallbackEvent.EventTypeKey</c>：<c>InfoType</c> → <c>ChangeType</c> → <c>Event</c>）。</summary>
    public string EventTypeKey { get; internal set; } = string.Empty;

    /// <summary>事件族（判别与信封一致，驱动开放面闸语义）。</summary>
    public WechatCallbackEventFamily Family { get; internal set; }

    /// <summary>
    /// 生效作用域下直系子节点的「元素名 → 全后代文本」全量只读视图（ADR-5）。
    /// 官方新增字段无需 SDK 发版即可读取；敏感凭据节点已排除（ADR-9）。
    /// </summary>
    public IReadOnlyDictionary<string, string> Values { get; internal set; } =
        new Dictionary<string, string>();
}
