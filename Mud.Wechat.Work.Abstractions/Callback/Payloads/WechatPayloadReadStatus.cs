// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 载荷读取状态（v2 方案 ADR-7：以状态化结果<b>取代</b>旧解析器的「静默 <c>null</c>」）。
/// </summary>
/// <remarks>
/// 旧 <c>WechatCallbackEventParser</c> 把「事件键不匹配 / 明文缺失 / 明文非 XML / 事件为 null / 契约错配」
/// 五种完全不同的失败一律压成一个 <c>null</c> 返回值，宿主无法区分。本枚举把这五种 + 一种降级路径显式化，
/// 使宿主可按状态分支，也让兜底处理器能对全部事件做统一审计。
/// </remarks>
public enum WechatPayloadReadStatus
{
    /// <summary>契约命中并绑定成功。</summary>
    Matched,

    /// <summary>
    /// 事件键未登记契约 ⇒ 降级为 <see cref="GenericCallbackPayload"/>（ADR-4）。
    /// 属<b>可观测的降级</b>而非错误：官方新增事件当天即可被结构化读取，无需 SDK 发版。
    /// </summary>
    GenericFallback,

    /// <summary>事件键为空 / 不可判别（协议外报文，<c>InfoType</c>、<c>ChangeType</c>、<c>Event</c> 三者皆空）。</summary>
    KeyMismatch,

    /// <summary>事件键已登记，但登记的契约载荷类型与请求的载荷类型不一致（宿主接线错误）。</summary>
    ContractMismatch,

    /// <summary>解密明文缺失或不是合法 XML（等价于旧的「明文非 XML 返回 null」）。</summary>
    MalformedPayload,

    /// <summary>事件为 <c>null</c>，或信封未携带解密明文。</summary>
    EnvelopeMissing,
}
