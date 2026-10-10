// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「直播」模块直播管理域企业自建应用 SDK（零差异端点空标记）。
/// <para>
/// 官方对自建应用开放与三类应用公共面一致的 9 个端点（创建预约直播 / 修改预约直播 / 取消预约直播 /
/// 删除直播回放 / 获取微信观看直播凭证 / 获取成员直播 ID 列表 / 获取直播详情 / 获取直播观看明细 /
/// 获取跳转小程序商城的直播观众信息），全部收敛声明于父接口 <see cref="IWechatWorkLivingService"/>；
/// 本接口不承载差异端点。第三方应用见 <see cref="IWechatWorkThirdPartyLivingService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderLivingService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用须配置到「上课直播/直播 - 可调用接口的应用」中；
/// 全域仅能获取 / 操作本应用创建的直播。自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口
/// （存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Living",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkLivingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalLivingService : IWechatWorkLivingService
{
}
