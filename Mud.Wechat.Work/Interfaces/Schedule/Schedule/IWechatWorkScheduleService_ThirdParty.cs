// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「日程」模块管理日程域第三方应用 SDK（零差异端点空标记）。
/// <para>
/// 官方对三类应用开放一致的 7 个日程端点全部收敛声明于父接口 <see cref="IWechatWorkScheduleService"/>，
/// 本接口不承载差异端点；官方要求第三方应用创建日程必须指定 <c>cal_id</c>（请求 DTO 层面承载）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalScheduleService"/>；服务商代开发见 <see cref="IWechatWorkProviderScheduleService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（scope = authCorpId，路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用；创建日程的 <c>agentid</c> 参数仅旧的第三方多应用套件需填。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Schedule",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkScheduleService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyScheduleService : IWechatWorkScheduleService
{
}
