// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「素材管理」域第三方应用（Suite）SDK。
/// <para>
/// 官方向第三方应用开放的素材管理端点集（上传临时素材、获取临时素材、上传图片、获取高清语音素材、
/// 异步上传临时素材两个端点）与三类应用的公共面完全重合，全部继承自 <see cref="IWechatWorkMediaService"/>；
/// 因此本接口不新增端点，仅作为第三方应用的类型化契约入口存在。
/// </para>
/// <para>服务商通道「上传临时素材」（<c>/cgi-bin/service/media/upload</c>）仅第三方应用开放，
/// 走 provider_access_token、令牌不同，独立成族声明于
/// <see cref="IWechatWorkThirdPartyServiceMediaService"/>（零端点父接口 <see cref="IWechatWorkServiceMediaService"/>）。</para>
/// <para>自建应用见 <see cref="IWechatWorkInternalMediaService"/>；服务商代开发见 <see cref="IWechatWorkProviderMediaService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（经 <c>get_corp_token</c> 以 <c>permanent_code</c> 换取，路由键
/// <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId）——调用前须经
/// <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// 官方约束：异步上传临时素材须具有「客户联系」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Media",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMediaService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyMediaService : IWechatWorkMediaService
{
}
