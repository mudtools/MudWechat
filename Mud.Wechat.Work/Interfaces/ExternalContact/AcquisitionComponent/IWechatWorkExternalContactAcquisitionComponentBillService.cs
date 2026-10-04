// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块「获客助手组件」域「代支付流水」接口族公共 SDK。
/// <para>
/// 获取代支付流水（<c>/cgi-bin/service/customer_acquisition/get_bill_list</c>）官方契约以
/// <c>suite_access_token</c>（<b>获客助手组件的应用凭证</b>）鉴权，路由位于 <c>/cgi-bin/service/</c> 下，
/// 与本域其余端点的企业级 <c>access_token</c>（<see cref="IWechatWorkExternalContactAcquisitionComponentService"/>）
/// 分属不同令牌路由键 —— 一接口族一令牌路由键，故独立成族；官方仅向第三方应用开放
/// （企业自建应用与服务商代开发均无对应功能），因此本父接口没有公共端点，亦不设自建 / 代开发子接口，
/// 唯一的 1 个端点声明于 <see cref="IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService"/>。
/// </para>
/// <para>
/// 注意：<c>suite_access_token</c> 为套件级凭证、无企业 scope，授权企业以请求体 <c>auth_corpid</c>
/// 参数显式指定（官方契约），不经由令牌作用域机制表达。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键对齐 Authentication 域第三方授权端点的
/// <see cref="WechatTokenTypes.SuiteAccessToken"/> 形态（Query 注入 <c>suite_access_token</c>，
/// 参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，无 G7 豁免负担）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkExternalContactAcquisitionComponentBillService
{
}
