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
/// 本接口仅提供代开发应用 external_userid 转换这1 个第三方侧差异端点。
/// </para>
/// <para>
/// 架构决策（单一所有者）：unionid转 external_userid、external_userid 查询 pending_id
/// （<c>/cgi-bin/idconvert/unionid_to_external_userid</c>、<c>/cgi-bin/idconvert/batch/external_userid_to_pending_id</c>）
/// 曾在本接口重复声明，因与「账号ID」域 <see cref="IWechatWorkAccountIdService"/> 构成同路由重复声明、
/// 且平行 DTO 家族互为真子集，现已收敛为「账号ID」域单一所有者声明，第三方应用侧请改用
/// <see cref="IWechatWorkAccountIdService.ConvertUnionidToExternalUserIdAsync"/> 与
/// <see cref="IWechatWorkAccountIdService.ConvertExternalUserIdToPendingIdAsync"/>。
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
