// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「会议」模块预约会议高级管理域公共 SDK
/// （获取/更新会议受邀成员列表 + 创建/获取用户专属参会链接 + 获取实时会中成员列表 + 获取已参会成员列表 +
/// 获取实时等候室成员列表 + 获取等候室成员记录 + 获取成员设备是否入会 + 获取/更新会议嘉宾列表 +
/// 获取会议健康度 + 修改/获取会议报名配置 + 获取会议成员报名 ID + 获取/审批/导入/删除会议报名信息，共 19 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），端点全部声明于本接口的
/// 企业自建应用子接口 <see cref="IWechatWorkInternalMeetingAdvancedService"/>；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// 创建/修改/取消预约会议、获取会议详情、获取成员会议 ID 列表 5 个与预约会议基础管理族同路由的端点
/// 不在本族重复声明（高级管理文档页为其参数超集，由 <see cref="IWechatWorkMeetingService"/> 接口族承载）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：企业自建应用消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文（AppKey）路由。
/// </para>
/// <para>
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许获取/修改该应用创建的会议的数据；
/// 成员相关查询仅返回在应用可见范围内的成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingAdvancedService
{
}
