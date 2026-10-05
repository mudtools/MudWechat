// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「日程」模块管理日历域企业自建应用 SDK（零差异端点空标记）。
/// <para>
/// 官方对三类应用开放一致的 4 个日历端点全部收敛声明于父接口 <see cref="IWechatWorkScheduleCalendarService"/>，
/// 本接口不承载差异端点。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderScheduleCalendarService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyScheduleCalendarService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Schedule",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkScheduleCalendarService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalScheduleCalendarService : IWechatWorkScheduleCalendarService
{
}
