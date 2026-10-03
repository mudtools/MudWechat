// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「自建应用对接」接口族公共 SDK。
/// <para>
/// 企业微信不允许第三方应用获取企业的明文 userid 与 external_userid；本族供<b>企业自建应用</b>将
/// 服务商主体密文 ID 转换为企业主体对应 ID（企业与服务商应用对接场景，95884），以及将智能机器人获取的
/// 密文 open_userid 转换为明文 userid（自建应用与智能机器人对接场景，101521）。
/// 官方仅向自建应用开放本族端点（第三方/代开发无对应文档），本父接口没有公共端点，
/// 全部 3 个端点声明于 <see cref="IWechatWorkInternalAccountIdInteropService"/>（唯一的应用类型子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，
/// 即企业自建应用的调用凭证）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAccountIdInteropService
{
}
