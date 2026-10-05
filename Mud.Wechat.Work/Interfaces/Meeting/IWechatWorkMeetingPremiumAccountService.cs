// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块高级功能账号管理域公共 SDK
/// （分配高级功能账号 + 查询分配结果 + 取消高级功能账号 + 查询取消结果 + 获取高级功能账号列表，共 5 端点；
/// 「分配高级功能账号」与「取消高级功能账号」两个官方文档页各承载提交任务与查询结果两个端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），端点全部声明于本接口的
/// 企业自建应用子接口 <see cref="IWechatWorkInternalMeetingPremiumAccountService"/>；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// 全部路由挂 <c>/cgi-bin/meeting/vip/</c> 段。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：企业自建应用消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文（AppKey）路由。
/// </para>
/// <para>
/// 官方权限口径：自建应用需配置到「协作 - 会议 - 可调用接口的应用」中（代开发应用、第三方应用暂不支持）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingPremiumAccountService
{
}
