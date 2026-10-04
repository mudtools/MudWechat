// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Customer;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块客户管理域第三方应用 SDK。
/// <para>
/// 除继承自 <see cref="IWechatWorkExternalContactCustomerService"/> 的公共端点外，
/// 本接口提供第三方应用独有的身份转换端点：unionid 与 external_userid 的关联
/// （unionid 转换为第三方 external_userid、external_userid 查询 pending_id）
/// 与代开发应用 external_userid 转换。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalExternalContactCustomerService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactCustomerService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactCustomerService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyExternalContactCustomerService : IWechatWorkExternalContactCustomerService
{
    /// <summary>
    /// unionid 转换为第三方 external_userid
    /// <para>将微信客户的 unionid 转为第三方主体的 external_userid；若该微信用户尚未成为企业的客户，
    /// 则返回 pending_id（90 天内有效，仅用于关联，不能当成 external_userid 调用接口）。</para>
    /// <para>unionid 与 openid 必须取自同一个小程序（或公众号），且账号主体名称需与当前授权企业主体一致
    /// （或与服务商主体一致）。subject_type = 0 按企业限频（10 万次/小时），subject_type = 1 按服务商限频。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ConvertUnionIdToExternalUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>外部联系人 ID（external_userid）或临时 ID（pending_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101134"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/unionid_to_external_userid")]
    Task<ConvertUnionIdToExternalUserIdResponse> ConvertUnionIdToExternalUserIdAsync(
        [Body] ConvertUnionIdToExternalUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// external_userid 查询 pending_id
    /// <para>将已关联过 unionid 的客户 external_userid 批量换取 pending_id
    /// （最多 100 个；pending_id 有效期 90 天）。传入 chat_id 时只检查群主可见性并忽略群外 ID。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="ExternalUserIdToPendingIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（result：external_userid 与 pending_id 映射）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101134"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/batch/external_userid_to_pending_id")]
    Task<ExternalUserIdToPendingIdResponse> GetPendingIdByExternalUserIdsAsync(
        [Body] ExternalUserIdToPendingIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代开发应用 external_userid 转换
    /// <para>企业同时授权了服务商的第三方应用与代开发应用时，将代开发应用获取到的 external_userid
    /// 转换为第三方应用的 external_userid（代开发自建应用已升级后无需调用）。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ConvertToServiceExternalUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>第三方应用主体的外部联系人 ID（external_userid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95195"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/to_service_external_userid")]
    Task<ConvertToServiceExternalUserIdResponse> ConvertToServiceExternalUserIdAsync(
        [Body] ConvertToServiceExternalUserIdRequest request,
        CancellationToken cancellationToken = default);
}
