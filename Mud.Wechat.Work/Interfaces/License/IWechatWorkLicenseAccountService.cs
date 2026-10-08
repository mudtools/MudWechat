// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「接口调用许可」模块账号管理域公共 SDK
/// （激活账号 / 获取激活码详情 / 获取企业的账号列表 / 获取成员的激活详情 / 账号继承 /
/// 分配激活码给下游或下级企业，共 9 个端点）。
/// </summary>
/// <remarks>
/// <para>
/// 官方在第三方应用开发与服务商代开发两棵文档树的「接口调用许可」分组下提供本域端点
/// （97188/97189/97190/97191/97192/97193，两棵文档树共享同一端点页），
/// 企业自建应用开发文档树无对应 API；且全部端点走服务商凭证 <c>provider_access_token</c>，
/// 与自建 / 授权企业的 <c>access_token</c> 路由键不同，
/// 故按「独立 provider 令牌 ⇒ 独立成族」落位：父接口零端点（<see cref="IWechatWorkLicenseAccountService"/>
/// 为 <c>IsAbstract</c>），全部 9 个端点声明于唯一子接口
/// <see cref="IWechatWorkThirdPartyLicenseAccountService"/>
/// （落位形态对齐 <see cref="IWechatWorkThirdPartyPayToolOrderService"/> 零端点父接口 + 唯一第三方子接口承载）。
/// </para>
/// <para>
/// 同域另三族：订单管理族见 <see cref="IWechatWorkLicenseOrderService"/>；
/// 应用管理族见 <see cref="IWechatWorkLicenseAppService"/>；
/// 自动激活设置族见 <see cref="IWechatWorkLicenseAutoActiveService"/>（均走 provider_access_token）。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkLicenseAccountService
{
}
