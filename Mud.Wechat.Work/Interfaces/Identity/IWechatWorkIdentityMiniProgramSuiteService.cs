// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「身份验证」模块小程序登录域第三方套件级 SDK 公共父接口（零端点）。
/// <para>
/// 官方对第三方应用开放的小程序登录校验是<b>独立路由</b>端点
///（<c>/cgi-bin/service/miniprogram/jscode2session</c>），且以 <c>suite_access_token</c>（套件级凭证）鉴权——
/// 与企业自建/代开发的 <c>access_token</c> 端点（见 <see cref="IWechatWorkIdentityMiniProgramService"/>）
/// 分属不同令牌路由键，故独立成族（形态对齐 <see cref="IWechatWorkIdentitySuiteService"/>），
/// 端点由唯一第三方子接口 <see cref="IWechatWorkThirdPartyIdentityMiniProgramSuiteService"/> 承载。
/// </para>
/// </summary>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkIdentityMiniProgramSuiteService
{
}
