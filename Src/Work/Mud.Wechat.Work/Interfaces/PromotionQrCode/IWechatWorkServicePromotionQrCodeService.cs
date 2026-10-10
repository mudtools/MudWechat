// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「推广二维码」域服务商通道「企业注册」域公共 SDK
/// （<c>/cgi-bin/service/</c> 下的注册码路由族）。
/// <para>
/// 官方仅在<b>第三方应用开发</b>文档树下提供本族端点（服务商以推广包引导企业注册），
/// 且走服务商凭证 <c>provider_access_token</c>，与自建 / 代开发应用的 <c>access_token</c> 路由键不同，
/// 故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（<see cref="IWechatWorkServicePromotionQrCodeService"/>
/// 为 <c>IsAbstract</c>），全部 2 个端点声明于唯一子接口
/// <see cref="IWechatWorkThirdPartyServicePromotionQrCodeService"/>
/// （落位形态对齐 <see cref="IWechatWorkServiceMediaService"/> 零端点父接口 + 唯一第三方子接口承载）。
/// </para>
/// <para>
/// 同域的「通讯录迁移收尾」2 个端点（设置授权应用可见范围 / 设置通讯录同步完成）消费的是
/// 「查询注册状态」返回的<b>通讯录迁移 access_token</b>（SDK 令牌基座不管理该凭证），
/// 令牌路由键与本族不同，已独立成 <see cref="IWechatWorkPromotionQrCodeContactSyncService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.ProviderAccessToken"/>（Query 注入 <c>provider_access_token</c>，
/// 服务商级凭证），<b>不声明凭据归属域键</b>——该令牌本身已无歧义（AGENTS.md §5.1 / 归属域守卫 TO1）。
/// </para>
/// <para>
/// 官方权限口径：本族端点页面未列独立权限范围，仅要求使用服务商的 <c>provider_access_token</c>
/// （获取方法参见服务商的凭证）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkServicePromotionQrCodeService
{
}
