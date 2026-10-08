// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块班级收款域第三方应用 SDK。
/// <para>
/// 官方向企业自建应用与第三方应用开放完全一致的 2 个端点，全部收敛于
/// <see cref="IWechatWorkSchoolClassPayService"/>；本接口不新增端点，仅作为第三方应用的
/// 类型化契约入口存在（形态对齐 <see cref="IWechatWorkThirdPartySchoolSettingService"/> 空标记）。
/// 服务商代开发官方未开放服务端查询接口，本域不声明代开发子接口。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalSchoolClassPayService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（scope = authCorpId，路由键 <see cref="WechatTokenTypes.AccessToken"/>），
/// 调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。官方权限口径：
/// 只允许获取由应用本身创建的收款项目详情 / 订单详情。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "School",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSchoolClassPayService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartySchoolClassPayService : IWechatWorkSchoolClassPayService
{
}
