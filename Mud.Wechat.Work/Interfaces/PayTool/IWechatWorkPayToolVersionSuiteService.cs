// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「收银台」模块应用版本付费域套件令牌族公共 SDK
/// （<c>/cgi-bin/service/</c> 下的订单路由族：获取订单列表 / 获取订单详情 / 延长试用期）。
/// </summary>
/// <remarks>
/// <para>
/// 官方在<b>第三方应用开发</b>文档树的「收银台 → 应用版本付费」分组下提供本族端点
/// （91910 / 91909 / 91913），企业自建应用开发与服务商代开发文档树<b>均无对应 API</b>；
/// 且三个端点均消费 <b>suite_access_token</b>（套件级令牌），与同域另两族的
/// <c>provider_access_token</c> 路由键不同，故按「独立套件令牌 ⇒ 独立成族」落位：
/// 父接口零端点（<see cref="IWechatWorkPayToolVersionSuiteService"/> 为 <c>IsAbstract</c>），
/// 全部 3 个端点声明于唯一子接口 <see cref="IWechatWorkThirdPartyPayToolVersionSuiteService"/>
/// （落位形态对齐 <see cref="IWechatWorkIdentitySuiteService"/> 零端点父接口 + 唯一第三方子接口承载）。
/// </para>
/// <para>
/// <b>与既有授权流族的边界</b>：同族官方文档另列「获取企业永久授权码（91911）/ 获取企业授权信息（91912）」，
/// 二者与授权流接口族（<see cref="IWechatWorkProviderAuthenticationService"/>）为<b>同一端点</b>，
/// 已在该族承载，故本族<b>不重复声明</b>，避免同一路由出现两处映射。</para>
/// <para>
/// <b>签名</b>：本族端点走 <c>suite_access_token</c> 鉴权，官方参数表<b>不含</b>
/// <c>nonce_str</c> / <c>ts</c> / <c>sig</c>（签名仅「收款工具」族需要）。</para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkPayToolVersionSuiteService
{
}