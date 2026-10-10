// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「素材管理」域服务商通道「上传临时素材」域公共 SDK（<c>/cgi-bin/service/media/</c> 路由族）。
/// <para>
/// 官方仅向<b>第三方应用</b>开放本族端点（走服务商凭证 provider_access_token，
/// 与三类应用公共面 <see cref="IWechatWorkMediaService"/> 的 access_token 路由键不同，故独立成族），
/// 因此本父接口没有公共端点；全部 1 个端点声明于
/// <see cref="IWechatWorkThirdPartyServiceMediaService"/>（唯一的应用类型子接口，落位形态对齐
/// <see cref="IWechatWorkIdentitySuiteService"/> 零端点父接口 + 唯一子接口承载）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 与三类应用公共面（<see cref="IWechatWorkMediaService"/>）路由、令牌均不同：
/// 令牌路由键为 <see cref="WechatTokenTypes.ProviderAccessToken"/>（Query 注入 <c>provider_access_token</c>，
/// 服务商级凭证）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkServiceMediaService
{
}
