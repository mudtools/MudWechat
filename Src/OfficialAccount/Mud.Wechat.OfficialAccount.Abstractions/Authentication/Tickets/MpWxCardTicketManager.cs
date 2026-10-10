// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 微信卡券票据管理器（<c>type=wx_card</c>）。
/// </summary>
/// <remarks>
/// 持久化键前缀 <c>Wechat.Mp.WxCardTicket</c>。
/// <b>官方数值锁定</b>：<c>type</c> 仅 <c>jsapi</c> 与 <c>wx_card</c> 两个合法取值
/// ⇒ 两类票据各由独立管理器持有，键空间互不覆盖（若共用一份缓存，卡券签名会拿到 JS-SDK 票据）。
/// </remarks>
internal sealed class MpWxCardTicketManager : MpTicketManagerBase, IMpWxCardTicketManager
{
    /// <summary>创建卡券票据管理器。</summary>
    /// <param name="ticketService">票据签发客户端（per-app）。</param>
    /// <param name="accessTokenManager">本应用的令牌管理器。</param>
    /// <param name="options">应用配置。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储。</param>
    public MpWxCardTicketManager(
        IMpTicketService ticketService,
        IMpAccessTokenManager accessTokenManager,
        IOptions<MpAppConfig> options,
        ILogger<MpWxCardTicketManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(ticketService, accessTokenManager, options, logger,
               MpTicketTypes.WxCard, MpTicketTypes.WxCardChannelKeyPrefix, tokenStore)
    {
    }
}
