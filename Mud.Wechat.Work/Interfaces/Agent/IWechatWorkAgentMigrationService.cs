// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块「自建应用迁移成代开发应用」域公共 SDK。
/// <para>
/// 官方仅在企业微信「服务商代开发」文档树的应用管理章节提供该端点
/// （自建应用与第三方应用文档树均无对应页面），但端点消费的是
/// <b>待迁移或尚未验证归属的自建应用</b>自身的 access_token（URL 参数），
/// 调用时应用上下文须为该自建应用（AppType = Internal），
/// 故本父接口零端点，唯一 1 个端点由 <see cref="IWechatWorkInternalAgentMigrationService"/> 承载。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：消费待迁移自建应用自身的 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAgentMigrationService
{
}
