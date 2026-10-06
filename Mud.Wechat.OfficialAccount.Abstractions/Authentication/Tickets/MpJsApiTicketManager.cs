// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// JS-SDK 票据管理器（<c>type=jsapi</c>）。
/// </summary>
/// <remarks>
/// 持久化键前缀 <c>Wechat.Mp.JsApiTicket</c>，与卡券票据（<c>Wechat.Mp.WxCardTicket</c>）互不覆盖。
/// 票据有效期官方为 7200 秒，由基座按「提前 <c>TokenRefreshThreshold</c> 秒」刷新。
/// </remarks>
internal sealed class MpJsApiTicketManager : MpTicketManagerBase, IMpJsApiTicketManager
{
    /// <summary>创建 JS-SDK 票据管理器。</summary>
    /// <param name="ticketService">票据签发客户端（per-app）。</param>
    /// <param name="accessTokenManager">本应用的令牌管理器。</param>
    /// <param name="options">应用配置。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储。</param>
    public MpJsApiTicketManager(
        IMpTicketService ticketService,
        IMpAccessTokenManager accessTokenManager,
        IOptions<MpAppConfig> options,
        ILogger<MpJsApiTicketManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(ticketService, accessTokenManager, options, logger,
               MpTicketTypes.JsApi, MpTicketTypes.JsApiChannelKeyPrefix, tokenStore)
    {
    }
}
