// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「成员管理」域服务商代开发应用 SDK。
/// <para>
/// 代开发应用的成员管理官方端点集（读取成员、获取部门成员、获取部门成员详情、获取成员 ID 列表、
/// 手机号/邮箱获取 userid、userid 与 openid 互换）与三类应用的公共读取面完全重合，
/// 全部继承自 <see cref="IWechatWorkUsersService"/>；官方未向代开发应用开放通讯录写入与邀请端点，
/// 因此本接口不新增端点，仅作为代开发应用的类型化契约入口存在（形态对齐飞书用户态空接口
/// <c>IFeishuUserV1LingoEntity</c>）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalUsersService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyUsersService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费代开发应用的企业级 access_token（<c>gettoken</c> 以 <c>corpsecret = permanent_code</c> 换取——
/// 代开发 K1 契约：permanent_code 语义是「应用 secret」，路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// scope = authCorpId）——调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkUsersService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderUsersService : IWechatWorkUsersService
{
}
