// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization.Models;

/// <summary>第三方应用授权链接生成请求（可选取授权配置下发）。</summary>
/// <remarks>纯进程内流转模型，不进 DataModels 的域 JsonContext（Generated/ 只覆盖官方传输 DTO）。</remarks>
public sealed class WechatAuthorizationUrlRequest
{
    /// <summary>授权回跳地址（redirect_uri）；为空时回退
    /// <see cref="WechatAuthorizationOptions.AuthorizationRedirectUri"/>，仍为空则抛。</summary>
    public string? RedirectUri { get; set; }

    /// <summary>授权方标识（state，≤128 字节；原样回传，用于关联会话）。</summary>
    public string? State { get; set; }

    /// <summary>可选的授权配置下发（对应 <c>set_session_info</c>）；为 null 则跳过该步。
    /// 其 <c>PreAuthCode</c> 由编排服务用本次取得的预授权码覆盖。</summary>
    public DataModels.ProviderAuthentication.SetSessionInfoRequest? SessionInfo { get; set; }
}