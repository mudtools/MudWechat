// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Authentication;

/// <summary>
/// 微信小店 / 视频号应用上下文切换器（多应用场景下的推荐入口）。
/// </summary>
/// <remarks>
/// <para>
/// 同时满足组件两条契约：<see cref="IAppContextHolder"/>（经 <see cref="IAppScopeSwitcher"/> 与
/// <see cref="IAppContextSwitcher"/> 继承）与作用域式入口 <see cref="IAppScopeSwitcher"/>。
/// </para>
/// <para>
/// <b>DI 桥接不变量（照抄公众号 P0-1 与企微 §5.3）</b>：<c>IChannelsAppContextSwitcher</c> /
/// 组件 <c>IAppContextHolder</c> / 组件 <c>IAppContextSwitcher</c> 必须是<b>同一实例</b>，
/// 且注册必须先于 <c>AddMudHttpClient</c>（组件内部会 TryAdd 持有器）。破坏该不变量 ⇒
/// 声明式 <c>[Token]</c> 客户端读到的上下文恒为 <c>null</c>，多应用下静默回退默认应用令牌。
/// </para>
/// <para>小店无企业级 scope ⇒ 不提供 <c>UseCorpScope</c> / <c>SetCorp</c> / <c>ClearCorp</c>。</para>
/// </remarks>
public interface IChannelsAppContextSwitcher : IAppContextSwitcher, IAppScopeSwitcher
{
}