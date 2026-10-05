// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块上课直播域企业自建应用 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 7 个端点，全部收敛于 <see cref="IWechatWorkSchoolLivingService"/>；
/// 本接口不新增端点，仅作为自建应用的类型化契约入口存在
/// （形态对齐 <see cref="IWechatWorkInternalSchoolService"/> 空标记）。
/// </para>
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartySchoolLivingService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolLivingService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 获取老师直播 ID 列表与删除直播回放须配置到「上课直播/直播 - 可调用接口的应用」中；
/// 其余端点须同时配置到「上课直播 - 可调用接口的应用」和「家长可以使用的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "School",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSchoolLivingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSchoolLivingService : IWechatWorkSchoolLivingService
{
}
