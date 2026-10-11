// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Extensions;

/// <summary>
/// 微信开放平台 API 模块枚举（对齐 <c>MpModule</c> / <c>PayModule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>线内枚举，不动企微线 <c>WechatModule</c></b>（那被企微专属域占用）：开放平台是独立产品线，
/// 模块枚举与注册器同文件同批维护 —— 新增枚举值必须同步在 <see cref="OpenPlatformServiceBuilder"/>
/// 的注册表登记对应 <c>Add{组名}WebApiHttpClient()</c>，缺项即该域客户端静默不注册。
/// </para>
/// </remarks>
public enum OpenPlatformModule
{
    /// <summary>第三方平台管理域（<c>/cgi-bin/component/*</c> 管理面 + 免令牌推票引导；注册组 <c>Component</c>）。</summary>
    Component,

    /// <summary>开放账号管理域（<c>/cgi-bin/open/*</c>；注册组 <c>OpenAccount</c>）。</summary>
    OpenAccount,

    /// <summary>授权账号管理域（<c>/cgi-bin/account/*</c>；注册组 <c>Account</c>）。</summary>
    Account,

    /// <summary>代公众号网页授权域（<c>/sns/oauth2/component/*</c>；注册组 <c>Sns</c>）。</summary>
    Sns,
}
