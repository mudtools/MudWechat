// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「ID 迁移完成状态」接口族第三方应用 SDK（含 external_userid 迁移完成差异端点）。
/// <para>
/// 继承公共父接口 <see cref="IWechatWorkAccountIdMigrationService"/> 的「设置迁移完成」端点；
/// 另持官方仅向第三方应用开放的「设置迁移完成（external_userid）」差异端点（99375）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AccountId",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAccountIdMigrationService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyAccountIdMigrationService : IWechatWorkAccountIdMigrationService
{
    /// <summary>
    /// 设置迁移完成（external_userid）
    /// <para>服务商完成企业下所有第三方应用 external_userid 新旧 id 迁移后，主动设置为「迁移完成」；
    /// 此后接口与回调启用新 ID。当该企业同时是服务商并对自己授权的情况，无需调用本接口。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="FinishExternalUserIdMigrationRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para>与父接口「ID 迁移完成状态的设置」同属官方同一文档页（99375）：该页同时收录
    /// <c>finish_openid_migration</c>（userid/corpid）与本端点（external_userid），
    /// 并注明「userid 与 corpid 只能同时设置为迁移完成，external_userid 可以单独设置」。</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99375"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/externalcontact/finish_external_userid_migration")]
    Task<WechatWorkResponse> FinishExternalUserIdMigrationAsync(
        [Body] FinishExternalUserIdMigrationRequest request,
        CancellationToken cancellationToken = default);
}
