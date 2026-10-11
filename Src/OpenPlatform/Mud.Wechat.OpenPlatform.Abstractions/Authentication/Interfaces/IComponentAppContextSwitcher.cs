// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.OpenPlatform.Abstractions.Authentication;

/// <summary>
/// 微信开放平台（第三方平台）应用上下文切换器（授权方作用域的推荐入口）。
/// </summary>
/// <remarks>
/// <para>
/// 同时满足组件两条契约：<see cref="IAppContextHolder"/>（经 <see cref="IAppContextSwitcher"/> 继承）
/// 与作用域式入口 <see cref="IAppScopeSwitcher"/>。
/// </para>
/// <para>
/// <b>DI 桥接不变量（照抄企微 P0-1）</b>：<see cref="IComponentAppContextSwitcher"/> /
/// 组件 <see cref="IAppContextHolder"/> / 组件 <see cref="IAppContextSwitcher"/> 必须是<b>同一实例</b>，
/// 且注册必须先于 <c>AddMudHttpClient</c>（组件内部会 TryAdd 持有器）。破坏该不变量 ⇒
/// 声明式 <c>[Token]</c> 客户端读到的上下文恒为 <c>null</c>，授权方作用域失效、静默回退平台自身令牌。
/// </para>
/// <para>
/// <b>作用域语义</b>：<see cref="UseAuthorizerScope"/> 进入「平台 + 授权方」两级作用域
/// （一次性 <c>using</c>，释放时还原进入前上下文）；平台自身作用域用继承的
/// <see cref="IAppScopeSwitcher.UseDefaultAppScope"/>。
/// </para>
/// </remarks>
public interface IComponentAppContextSwitcher : IAppContextSwitcher, IAppScopeSwitcher
{
    /// <summary>
    /// 进入指定授权方作用域（<b>平台 + 授权方</b>两级，一次性 <c>using</c>）。
    /// </summary>
    /// <param name="authorizerAppId">授权方应用 <c>appid</c>（即授权方上下文的应用键）。</param>
    /// <returns>释放时恢复进入前上下文的作用域对象。建议配合 <c>using</c> 使用。</returns>
    /// <exception cref="ArgumentException"><paramref name="authorizerAppId"/> 为空白或格式非法。</exception>
    /// <remarks>
    /// 语义对齐企微 <c>UseCorpScope</c>：进入时解析授权方上下文并建作用域（组件
    /// <see cref="IAppContextHolder.BeginScope"/> 自行快照并在释放时还原）；授权方参数校验失败即抛，
    /// 不产生半进入状态。
    /// </remarks>
    IDisposable UseAuthorizerScope(string authorizerAppId);
}
