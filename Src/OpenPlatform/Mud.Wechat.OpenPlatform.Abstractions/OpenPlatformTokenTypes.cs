// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 微信开放平台（第三方平台）令牌类型常量。
/// </summary>
/// <remarks>
/// <para>
/// <b>独立令牌域（避免串号）</b>：两个常量均带 <c>Wechat.OpenPlatform.</c> 前缀，
/// 与公众号（<c>Wechat.Mp.AccessToken</c>）、小店（<c>Wechat.Channels.AccessToken</c>）
/// 在共享令牌路由表中天然隔离——同一宿主并存多产品线时互不可见。
/// </para>
/// <para>
/// <b>两个作用域不可混用</b>：<see cref="ComponentAccessToken"/> 属「平台自身」作用域
/// （全容器一份，由 <see cref="IComponentTokenProvider"/> 承载）；
/// <see cref="AuthorizerAccessToken"/> 属「授权方」作用域（一授权方 <c>appid</c> 一份，
/// 由 <see cref="IAuthorizerTokenProvider"/> 承载）——后者必须经
/// <see cref="Authentication.IComponentAppContextSwitcher.UseAuthorizerScope"/> 显式声明作用域，
/// 否则运行期 fail-fast。
/// </para>
/// </remarks>
public static class OpenPlatformTokenTypes
{
    /// <summary>
    /// 平台自身令牌（<c>component_access_token</c>）的路由键。
    /// </summary>
    public const string ComponentAccessToken = "Wechat.OpenPlatform.ComponentAccessToken";

    /// <summary>
    /// 授权方令牌（<c>authorizer_access_token</c>）的路由键（按授权方 <c>appid</c> 分槽）。
    /// </summary>
    public const string AuthorizerAccessToken = "Wechat.OpenPlatform.AuthorizerAccessToken";
}
