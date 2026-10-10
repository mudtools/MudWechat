// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 微信公众号应用上下文（多公众号隔离单元）。
/// </summary>
/// <remarks>
/// <para>
/// 与企微 <c>IWechatAppContext</c> 的关键差异：公众号无套件 / 代开发 / 企业级 scope，
/// 故上下文只有「单一 access_token 管理器」而无企业级作用域语义。
/// </para>
/// <para>
/// 声明 <see cref="IDisposable"/>：上下文持有 DI scope 与令牌管理器（含后台刷新 Timer），
/// 必须可确定性释放（由 <c>IMpAppManager</c> 在移除应用 / 释放管理器时调用）。
/// </para>
/// </remarks>
public interface IMpAppContext : IMudAppContext, IDisposable
{
    /// <summary>应用配置。</summary>
    Configuration.MpAppConfig Config { get; }

    /// <summary>公众号唯一凭证（AppID）。</summary>
    string AppId { get; }

    /// <summary>本应用的 API 入口点。</summary>
    string BaseUrl { get; }

    /// <summary>本应用的 <c>access_token</c> 令牌管理器（普通 / 稳定版通道按配置装配）。</summary>
    IMpAccessTokenManager AccessTokenManager { get; }
}
