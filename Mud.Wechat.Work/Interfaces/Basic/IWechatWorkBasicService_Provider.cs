// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「基础接口」域服务商代开发应用 SDK。
/// <para>
/// 官方向服务商代开发开放本域全部 2 个端点（获取企业微信接口IP段 97073 + 获取企业微信回调IP段 98988），
/// 与自建应用的端点集（同路由同契约）完全重合，故全部继承自 <see cref="IWechatWorkBasicService"/>；
/// 因此本接口不新增端点，仅作为代开发应用的类型化契约入口存在（形态对齐
/// <see cref="IWechatWorkProviderJsSdkService"/> 空标记子接口）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalBasicService"/>；官方第三方应用开发文档树无「基础接口」分组，故无第三方子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId；
/// 代开发 K1 契约：<c>permanent_code</c> 语义是「应用 secret」，经 <c>gettoken</c> 换取），
/// 调用前须经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c>
/// 建立「应用 + 企业」作用域后再调用。
/// 官方权限口径：两个端点的权限说明均为「无限定」，不额外要求应用配置到任何白名单。
/// </para>
/// <para>
/// 官方业务约束：IP 段有变更可能，新旧 IP 段会同时保留一段时间，官方建议每天定时拉取、更新防火墙设置
/// （两端点均无独立的数值型频率限制，走官方全局访问频率限制）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Basic",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkBasicService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderBasicService : IWechatWorkBasicService
{
}
