// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.OpenPlatform.Abstractions.Authentication;

/// <summary>
/// 微信开放平台（第三方平台）应用上下文。
/// </summary>
/// <remarks>
/// <para>
/// 本线是「一容器一平台 + 多授权方」形态：<see cref="AuthorizerAppId"/> 为 <c>null</c> 的实例是
/// <b>平台自身上下文</b>（即默认应用，消费 <see cref="OpenPlatformTokenTypes.ComponentAccessToken"/>）；
/// 非空的实例是<b>授权方作用域上下文</b>（消费 <see cref="OpenPlatformTokenTypes.AuthorizerAccessToken"/>，
/// 由上下文内绑定的管理器按 <see cref="AuthorizerAppId"/> 定位令牌）。
/// </para>
/// <para>
/// 授权方上下文同时可解析平台自身令牌（官方允许在授权方流程内携带 <c>component_access_token</c>），
/// 反之<b>不允许</b>：平台上下文解析授权方令牌即编程错误，运行期 fail-fast。
/// </para>
/// </remarks>
public interface IOpenPlatformAppContext : IMudAppContext
{
    /// <summary>
    /// 获取本上下文绑定的授权方应用 <c>appid</c>；平台自身上下文为 <c>null</c>。
    /// </summary>
    string? AuthorizerAppId { get; }
}
