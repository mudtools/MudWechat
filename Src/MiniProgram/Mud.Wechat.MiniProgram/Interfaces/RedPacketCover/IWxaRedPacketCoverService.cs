// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「微信红包封面」域 SDK（1 端点：获取微信红包封面）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 微信红包封面，2026-10-10 依据官方清单核验）：
/// <c>red-packet-cover/api_getredpacketcoverurl.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：本接口用于获得<b>指定用户</b>可以领取的红包封面链接；
/// <c>ctoken</c> 在微信红包封面开放平台获取（发放凭据）。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：走应用级 <c>access_token</c>（Query）。官方为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "RedPacketCover", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaRedPacketCoverService
{
    /// <summary>
    /// 获取微信红包封面（指定用户可领取的链接）。官方文档：<c>red-packet-cover/api_getredpacketcoverurl.html</c>。
    /// </summary>
    /// <param name="request">获取请求（<c>openid</c> / <c>ctoken</c> 必填），见 <see cref="DataModels.RedPacketCover.WxaRedPacketCoverRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>领取链接（<c>data.url</c>），见 <see cref="DataModels.RedPacketCover.WxaRedPacketCoverResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/redpacketcover/wxapp/cover_url/get_by_token</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>凭据警示（MP-X7）</b>：<c>ctoken</c> 为红包封面开放平台发放的<b>发放凭据</b>，
    /// 禁止写入日志、遥测或异常消息。
    /// </para>
    /// </remarks>
    [Post("/redpacketcover/wxapp/cover_url/get_by_token")]
    Task<DataModels.RedPacketCover.WxaRedPacketCoverResponse> GetCoverUrlAsync(
        [Body] DataModels.RedPacketCover.WxaRedPacketCoverRequest request,
        CancellationToken cancellationToken = default);
}