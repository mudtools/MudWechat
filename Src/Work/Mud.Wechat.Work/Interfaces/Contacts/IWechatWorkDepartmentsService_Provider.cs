// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「部门管理」域服务商代开发应用 SDK。
/// <para>
/// 代开发应用的部门管理官方端点集（获取部门列表、获取子部门 ID 列表、获取单个部门详情）与三类应用的公共读取面
/// 完全重合，全部继承自 <see cref="IWechatWorkDepartmentsService"/>；官方未向代开发应用开放部门写入端点，
/// 因此本接口不新增端点，仅作为代开发应用的类型化契约入口存在（形态对齐飞书用户态空接口
/// <c>IFeishuUserV1LingoEntity</c>）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalDepartmentsService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyDepartmentsService"/>。</para>
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
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDepartmentsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderDepartmentsService : IWechatWorkDepartmentsService
{
}
