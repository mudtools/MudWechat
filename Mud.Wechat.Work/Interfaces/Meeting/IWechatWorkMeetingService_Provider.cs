// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块预约会议基础管理域服务商代开发 SDK（零差异端点空标记）。
/// <para>
/// 官方代开发章节对会议开放的端点为「创建预约会议 / 修改预约会议 / 取消预约会议 / 获取成员会议 ID 列表」4 端点，
/// 全部收敛声明于父接口 <see cref="IWechatWorkMeetingService"/>；本接口不承载差异端点。
/// 官方代开发章节未开放「获取会议详情」（自建应用见 <see cref="IWechatWorkInternalMeetingService"/>、
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMeetingService"/> 的差异端点）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：
/// 发起人和企业内部参与人必须在应用可见范围内；第三方应用必须指定 cal_id（会议所属日历须为 access_token 对应应用创建的日历）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderMeetingService : IWechatWorkMeetingService
{
}
