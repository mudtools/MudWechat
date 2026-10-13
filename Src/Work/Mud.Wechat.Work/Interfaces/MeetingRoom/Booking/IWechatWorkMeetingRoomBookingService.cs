// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「会议室预定管理」模块公共 SDK（查询会议室预定信息 / 预定会议室 / 取消预定 / 获取预定详情）。
/// <para>
/// 官方「会议室」文档树只在<b>企业自建应用开发</b>根下存在页面（93620 会议室预定管理，分类 93617），
/// 第三方应用开发与服务商代开发两棵树均无对应页面 ⇒ 本父接口没有公共端点，亦不设第三方 / 代开发子接口；
/// 全部 6 个端点声明于 <see cref="IWechatWorkInternalMeetingRoomBookingService"/>
/// （形态对齐 <see cref="IWechatWorkMeetingRoomService"/> 零端点父接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkDialService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由。
/// </para>
/// <para>
/// 与「日程」域（<see cref="IWechatWorkScheduleService"/>）的日程排期是两套 ID 体系：本域排期内的
/// <c>schedule_id</c> / <c>meeting_id</c> 由会议室预定接口返回并以字符串透传，不经日程接口换取。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingRoomBookingService
{
}
