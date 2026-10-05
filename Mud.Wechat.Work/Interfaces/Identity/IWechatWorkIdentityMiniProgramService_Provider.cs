// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块小程序登录域服务商代开发 SDK。
/// <para>
/// 官方对服务商代开发开放与自建/代开发公共面一致的 1 个端点（继承自
/// <see cref="IWechatWorkIdentityMiniProgramService"/>）；本接口不新增端点，仅作为
/// 服务商代开发的类型化契约入口存在（形态对齐 <see cref="IWechatWorkProviderIdentityService"/> 空标记）。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalIdentityMiniProgramService"/>；
/// 第三方应用（独立路由与套件令牌）见 <see cref="IWechatWorkThirdPartyIdentityMiniProgramSuiteService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c>
/// 建立「应用 + 企业」作用域后再调用。官方权限说明：access_token 必须是由该小程序关联的
/// 企业微信应用 secret 所获得（代开发模板下的小程序为第三方小程序，此处返回加密的 userid）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Identity",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkIdentityMiniProgramService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderIdentityMiniProgramService : IWechatWorkIdentityMiniProgramService
{
}
