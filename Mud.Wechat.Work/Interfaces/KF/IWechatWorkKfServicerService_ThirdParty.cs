// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块接待人员管理域第三方应用 SDK。
/// <para>
/// 官方对第三方应用仅开放与三类应用公共面一致的 3 个端点（继承自
/// <see cref="IWechatWorkKfServicerService"/>）；本接口不新增端点，仅作为
/// 第三方应用的类型化契约入口存在（形态对齐 <see cref="IWechatWorkThirdPartyExternalContactCustomerAcquisitionService"/> 空标记）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalKfServicerService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderKfServicerService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：
/// 须具有「微信客服-&gt;管理账号、分配会话和收发消息」权限（添加 / 删除端点）
/// 或「微信客服-&gt;获取基础信息」权限（列表端点），且须有该客服账号的管理权限；
/// userid 填密文 userid（即 open_userid）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkKfServicerService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyKfServicerService : IWechatWorkKfServicerService
{
}
