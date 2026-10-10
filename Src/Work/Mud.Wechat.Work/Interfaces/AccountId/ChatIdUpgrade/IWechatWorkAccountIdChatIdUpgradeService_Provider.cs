// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「群 ID 升级（对所有新授权企业）」接口族服务商代开发 SDK：
/// 官方仅向代开发模板开放，全部端点声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AccountId",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAccountIdChatIdUpgradeService))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkProviderAccountIdChatIdUpgradeService : IWechatWorkAccountIdChatIdUpgradeService
{
    /// <summary>
    /// 对所有新授权企业升级群 ID
    /// <para>对代开发应用模板进行群 ID 升级；调用后该模板的所有新增授权企业都会升级为服务商主体的群 ID，
    /// 只对调用该接口后授权的企业生效，不影响已授权企业。</para>
    /// <para>官方契约陷阱：区别于「申请群ID的升级」，本端点使用代开发模板的接口调用凭证
    /// （suite_access_token），且为<b>无请求体的 GET</b>。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>升级结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99601"/></para>
    /// </remarks>
    [Get("/cgi-bin/idconvert/upgrade_chatid_for_new_corp")]
    Task<WechatWorkResponse> UpgradeChatIdForNewCorpAsync(
        CancellationToken cancellationToken = default);
}
