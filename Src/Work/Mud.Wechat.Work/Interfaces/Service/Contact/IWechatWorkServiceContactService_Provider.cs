// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「服务商服务端」域「通讯录搜索 / 排序 / id 转译」接口族<b>服务商代开发</b> SDK（类型化契约入口）。
/// <para>官方四支端点全部以 <c>provider_access_token</c> 鉴权、以请求体 <c>auth_corpid</c> 指定企业，
/// 声明于公共父接口 <see cref="IWechatWorkServiceContactService"/>，本接口为其服务商代开发形态的契约入口。</para>
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// <para>
/// 本接口为「服务商凭证面」，令牌类型为 <c>WechatTokenTypes.ProviderAccessToken</c>（服务商级、一服务商一份），
/// 故<b>不声明</b> <c>TokenManagerKey</c> 归属域键 —— 归属域键仅适用于 <c>Wechat.AccessToken</c>
/// 的自建 / 企业两级键族（由令牌归属域守卫 TO 系列锁定）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Service",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkServiceContactService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkProviderServiceContactService : IWechatWorkServiceContactService
{
}
