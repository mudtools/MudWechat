// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信应用上下文接口，提供微信应用相关的上下文信息和令牌路由。
/// </summary>
/// <remarks>
/// 原接口公共契约不变（<see cref="IMudAppContext"/> + <see cref="IDisposable"/>），
/// 属性按 <see cref="Configuration.WechatAppConfig"/> 字段形状补齐（原 Endpoint 更名 BaseUrl）。
/// </remarks>
public interface IWechatAppContext : IMudAppContext, IDisposable
{
    /// <summary>获取应用配置。</summary>
    WechatAppConfig Config { get; }

    /// <summary>获取应用类型（决定令牌链与必填项组合）。</summary>
    WechatAppType AppType { get; }

    /// <summary>获取企业 CorpId。</summary>
    string CorpId { get; }

    /// <summary>获取应用 AgentId（仅自建应用；业务可选）。</summary>
    string AgentId { get; }

    /// <summary>获取应用凭证密钥（自建应用为 AgentSecret；永不写入日志）。</summary>
    string CorpSecret { get; }

    /// <summary>获取企业微信 API 入口点（默认 https://qyapi.weixin.qq.com）。</summary>
    string BaseUrl { get; }

    /// <summary>
    /// 获取当前代操作的企业（企业级 access_token 的 scope）。仅第三方/服务商代开发场景使用。
    /// </summary>
    /// <remarks>经 <see cref="IWechatAppContextSwitcher.SetCorp"/> 切换（环境上下文，异步流隔离）。</remarks>
    string? AuthCorpId { get; }

    /// <summary>获取当前代操作企业的永久授权码（与 <see cref="AuthCorpId"/> 配套）。</summary>
    string? PermanentCode { get; }

    /// <summary>获取自建应用令牌管理器（仅自建应用非 null）。</summary>
    IWechatInternalAppTokenManager? InternalAppTokenManager { get; }

    /// <summary>获取授权企业令牌管理器（仅第三方/服务商应用非 null）。</summary>
    IWechatCorpTokenManager? CorpTokenManager { get; }

    /// <summary>获取服务商令牌管理器（仅第三方/服务商应用非 null）。</summary>
    IWechatProviderTokenManager? ProviderTokenManager { get; }

    /// <summary>获取套件令牌管理器（仅第三方/服务商应用非 null）。</summary>
    IWechatSuiteTokenManager? SuiteTokenManager { get; }
}
