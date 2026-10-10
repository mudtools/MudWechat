// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「文档」模块设置文档权限域企业自建应用 SDK。
/// <para>
/// 官方对企业自建应用开放与三类应用公共面一致的 9 个端点（继承自 <see cref="IWechatWorkWedocDocPermissionService"/>）；
/// 本接口不新增端点，仅作为企业自建应用的类型化契约入口存在。
/// </para>
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartyWedocDocPermissionService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocDocPermissionService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；仅可操作该应用自己创建的文档。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Wedoc",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkWedocDocPermissionService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalWedocDocPermissionService : IWechatWorkWedocDocPermissionService
{
}
