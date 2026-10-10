// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游通讯录管理」域服务商代开发应用 SDK。
/// <para>
/// 官方对代开发应用开放了与自建应用一致的 4 个读取端点（获取上下游列表 / 通讯录分组 / 分组下企业列表 /
/// 企业信息），全部继承自 <see cref="IWechatWorkCorpGroupContactsService"/>；导入 / 移除等写入与任务端点
/// 官方代开发文档树未开放（见域设计文档口径差异记录），本接口不新增端点，仅作为代开发应用的类型化契约入口
/// （形态对齐飞书用户态空接口 <c>IFeishuUserV1LingoEntity</c>）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalCorpGroupContactsService"/>；第三方应用官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费代开发应用的企业级 access_token（<c>gettoken</c> 以 <c>corpsecret = permanent_code</c> 换取——
/// 代开发 K1 契约：permanent_code 语义是「应用 secret」，路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// scope = authCorpId）——调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CorpGroup",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCorpGroupContactsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderCorpGroupContactsService : IWechatWorkCorpGroupContactsService
{
}
