// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「邮件」模块高级功能账号族公共 SDK（零端点父接口；
/// 官方仅向企业自建应用开放分配/取消/获取列表 3 个端点）。
/// <para>
/// 全部端点承载于 <see cref="IWechatWorkInternalMailVipService"/>，本接口不声明任何端点
/// （形态对齐 <see cref="IWechatWorkSecurityVipService"/> 零端点父接口——安全管理域高级功能账号
/// 管理走 <c>/cgi-bin/security/vip/*</c> 异步任务形态，与邮件域 <c>/cgi-bin/exmail/vip/*</c> 同步批量形态互不相干）。
/// 官方未向第三方应用与服务商代开发应用开放本域，故本家族不声明对应应用类型子接口
/// （能力漂移守卫：继承链上恰好只有自建子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 自建应用消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 高级功能账号分配/撤销均面向应用可见范围内的企业成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMailVipService
{
}
