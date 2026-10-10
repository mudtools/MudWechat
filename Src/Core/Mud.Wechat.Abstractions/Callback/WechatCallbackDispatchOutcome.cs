// -----------------------------------------------------------------------
//  作者：Mud Studio 版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 回调事件分发结果（由各产品线中间件映射为平台应答）。
/// </summary>
/// <remarks>
/// <para>
/// 四态语义产品线中立，<b>到 HTTP 的映射由各产品线决定</b>：
/// 企微把 <see cref="Interrupted"/> 映射为 <c>503</c>（利用平台重推）；
/// 公众号把 <see cref="Interrupted"/> 映射为明文 <c>success</c> + <c>200</c>
/// （公众号重试为「整封消息重发」，放大重复处理风险，故不触发重推）。
/// </para>
/// </remarks>
public enum WechatCallbackDispatchOutcome
{
    /// <summary>处理器已执行（含单处理器异常被隔离的情形）→ 200。</summary>
    Handled,

    /// <summary>无匹配处理器（unhandled，已告警）→ 200（平台契约：事件已接收）。</summary>
    Unhandled,

    /// <summary>拦截器中断 / 软超时中断（BeforeHandleAsync 返回 false）→ 产品线决定出口（企微 503 触发重推）。</summary>
    Interrupted,

    /// <summary>事件不适用于当前回调条目（合法性闸）→ 200（事件已接收、不重推）。</summary>
    Rejected,
}
