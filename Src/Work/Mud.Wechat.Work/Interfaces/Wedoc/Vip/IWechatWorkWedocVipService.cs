// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块高级功能账号管理域公共 SDK（零端点父接口）。
/// <para>
/// 官方「高级功能账号管理」3 个端点（分配高级功能账号 / 取消高级功能账号 / 获取高级功能账号列表）
/// 官方文档面仅出现在企业自建应用开发树（第三方应用开发与服务商代开发树无对应页面），
/// 端点全部承载于唯一自建子接口 <see cref="IWechatWorkInternalWedocVipService"/>；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// </para>
/// <para>与微盘线同名族（<see cref="IWechatWorkWedriveVipService"/>）是两族独立端点：
/// 官方按产品线各持一套「高级功能账号管理」页面，路由段分别为 <c>wedoc/vip/</c> 与 <c>wedrive/vip/</c>，不得混用。</para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）；
/// 本域实际仅自建应用可调用，消费应用自身 access_token。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocVipService
{
}
