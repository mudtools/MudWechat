// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「政民沟通」模块居民上报族公共 SDK
/// （获取配置的网格及网格负责人 + 获取单位/个人居民上报数据统计 + 获取上报事件分类统计 +
/// 获取居民上报事件列表 + 获取居民上报事件详情）。
/// <para>
/// 官方仅向企业自建应用开放本族全部 6 个端点：自建应用须配置到「居民上报 - 可调用接口的应用」中；
/// 官方权限表对服务商代开发与第三方应用均标注「暂不支持」
/// （代开发文档页与自建页内容一致、权限表同为「暂不支持」）。
/// 按官方开放面收敛为「零端点父接口 + 唯一自建子接口承载端点」形态
/// （对齐身份验证二次验证族 / 企业支付域 / 会话内容存档域）：
/// 端点全部声明于 <see cref="IWechatWorkInternalGovResidentService"/>，
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// </para>
/// <para>巡查上报族（patrol 段路由）见 <see cref="IWechatWorkGovPatrolService"/>；
/// 配置网格结构域 / 配置事件类别域见 <see cref="IWechatWorkGovGridService"/>、
/// <see cref="IWechatWorkGovEventCategoryService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>）。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkGovResidentService
{
}
