// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「ID 迁移完成状态」接口族公共 SDK。
/// <para>
/// 服务商完成企业下应用的新旧 id（userid/corpid/external_userid）迁移后，主动将企业设置为
/// 「迁移完成」，此后应用获取到的将是升级后的 id。本族端点以 <c>provider_access_token</c>
/// （服务商凭证）鉴权 —— 与企业级 <c>access_token</c> 端点分属不同令牌路由键，故独立成族。
/// 第三方与代开发共用的「设置迁移完成」1 个端点声明于本公共父接口（99375/99378）；
/// 第三方子接口另持「设置迁移完成（external_userid）」差异端点（96516），
/// 代开发子接口为空标记；企业自建应用官方无本族端点，不设自建子接口。
/// </para>
/// <para>
/// 官方注意：userid 与 corpid 只能同时设置为迁移完成，external_userid 可以单独设置；
/// 设置迁移完成后接口不再返回该企业相关的 unionid；迁移完成不可回退。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.ProviderAccessToken"/>（Query 注入 <c>provider_access_token</c>，
/// 应用服务商的接口调用凭证）；该参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，无 G7 豁免负担。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkAccountIdMigrationService
{
    /// <summary>
    /// ID 迁移完成状态的设置
    /// <para>将企业设置为「迁移完成」：第三方应用场景（99375）传 corpid + openid_type；
    /// 代开发场景（99378）另传 agentid，且仅传入正确 corpid、未传入 agentid 时，
    /// 视为更新该企业下同服务商所有第三方应用的对应升级状态。</para>
    /// <para>当该企业同时是服务商并对自己授权时，无需调用本接口。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="FinishOpenIdMigrationRequest"/>；openid_type：1-userid 与 corpid、3-external_userid 及 external_tagid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99375"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99378"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/finish_openid_migration")]
    Task<WechatWorkResponse> FinishOpenIdMigrationAsync(
        [Body] FinishOpenIdMigrationRequest request,
        CancellationToken cancellationToken = default);
}
