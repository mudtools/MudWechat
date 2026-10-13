// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「会议室管理」模块公共 SDK（添加 / 编辑 / 删除 / 查询会议室）。
/// <para>
/// 官方「会议室」文档树只在<b>企业自建应用开发</b>根下存在页面（93619 会议室管理、
/// 93620 会议室预定管理，分类 93617），第三方应用开发与服务商代开发两棵树均无对应页面
/// ⇒ 本父接口没有公共端点，亦不设第三方 / 代开发子接口；全部 4 个端点声明于
/// <see cref="IWechatWorkInternalMeetingRoomService"/>
/// （形态对齐 <see cref="IWechatWorkDialService"/> 与 <see cref="IWechatWorkGovPatrolService"/> 零端点父接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkDialService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由。
/// </para>
/// <para>
/// 与「视频会议」域（<see cref="IWechatWorkMeetingService"/> 及其 MeetingRooms 会议室子接口，路由
/// <c>/cgi-bin/meeting/rooms/*</c>）分属官方两棵章节树：本域是 <b>OA 会议室</b>（实体会议室资源与预定），
/// 后者是视频会议硬件会议室，路由 <c>/cgi-bin/oa/meetingroom/</c> 与 <c>/cgi-bin/meeting/</c> 互不承载。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingRoomService
{
}
