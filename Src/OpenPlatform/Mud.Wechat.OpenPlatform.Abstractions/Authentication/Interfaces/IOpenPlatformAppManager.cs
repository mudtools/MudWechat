// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.OpenPlatform.Abstractions.Authentication;

/// <summary>
/// 微信开放平台（第三方平台）应用管理器。
/// </summary>
/// <remarks>
/// <para>
/// 实现形态：Singleton + <b>单平台应用</b>（配置唯一来源 <see cref="Configuration.OpenPlatformAppConfig"/>）
/// + 按授权方 <c>appid</c> 惰性物化的授权方子上下文（<see cref="IAppManager{t}.GetApp"/> 以 <c>appid</c> 为键，
/// 缓存后复用——授权方上下文内绑定该 <c>appid</c> 的令牌管理器）。
/// </para>
/// <para>
/// <b>写入口不支持</b>（与企微 <c>WechatAppManager</c> 同款 fail-fast）：
/// 平台配置来自注册期 <see cref="OpenPlatformAppConfig"/>，授权方注册属授权编排职责
/// （<c>ComponentAuthorizationService</c>，主包），不存在「运行时注册应用」语义 ——
/// <c>RegisterApp</c> / <c>UpdateApp</c> / <c>SetDefaultApp</c> 等一律抛
/// <see cref="NotSupportedException"/>，避免静默写进读不到的表。
/// </para>
/// </remarks>
public interface IOpenPlatformAppManager : IAppManager<IOpenPlatformAppContext>
{
    /// <summary>
    /// 获取平台自身（<c>component</c>）上下文；等价于 <see cref="IAppManager{TAppContext}.GetDefaultApp"/>。
    /// </summary>
    IOpenPlatformAppContext PlatformContext { get; }
}
