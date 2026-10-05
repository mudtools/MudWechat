// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块会中控制管理域公共 SDK
/// （管理会中设置 + 管理联席主持人 + 静音成员 + 关闭或开启成员视频 + 关闭成员屏幕共享 +
/// 修改成员在会中显示的昵称 + 管理等候室成员 + 移出成员 + 结束会议 + 会议投票管理 8 端点，共 17 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），端点全部声明于本接口的
/// 企业自建应用子接口 <see cref="IWechatWorkInternalMeetingControlService"/>；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// 会控 9 端点路由挂 <c>/cgi-bin/meeting/realcontrol/</c> 段、会议投票 8 端点路由挂 <c>/cgi-bin/meeting/poll/</c> 段。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：企业自建应用消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文（AppKey）路由。
/// </para>
/// <para>
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许操作该应用创建的会议；
/// 会议投票端点还要求仅进行中的会议可以调用、操作者是主持人或者会议管理员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingControlService
{
}
