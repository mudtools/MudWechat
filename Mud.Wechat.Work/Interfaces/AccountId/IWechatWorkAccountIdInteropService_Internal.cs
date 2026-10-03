// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「自建应用对接」接口族企业自建应用 SDK：
/// 将代开发应用/第三方应用获取的密文 ID 转换为企业主体对应 ID，以及将智能机器人获取的
/// 密文 open_userid 转换为明文 userid，全部声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 需要使用自建应用的 access_token；成员（或客户跟进人/客户群群主）需要同时在 access_token 和
/// source_agentid 所对应应用的可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AccountId",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAccountIdInteropService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAccountIdInteropService : IWechatWorkAccountIdInteropService
{
    /// <summary>
    /// userid 转换（自建应用与第三方/代开发应用的对接）
    /// <para>将代开发应用或第三方应用获取的密文 open_userid 转换为明文 userid。</para>
    /// <para>open_userid_list 最多不超过 1000 个，必须是 source_agentid 对应的应用所获取；
    /// 成员需要同时在 access_token 和 source_agentid 所对应应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="OpenUserIdToUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换成功的明文 userid 映射列表（userid_list）及不合法列表（invalid_open_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95884"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/openuserid_to_userid")]
    Task<OpenUserIdToUserIdResponse> OpenUserIdToUserIdAsync(
        [Body] OpenUserIdToUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// openuserid 转 userid（自建应用与智能机器人的对接）
    /// <para>将智能机器人获取的密文 open_userid 转换为明文 userid。
    /// 官方契约陷阱：与「自建应用与第三方/代开发应用的对接」场景共用路由
    /// <c>/cgi-bin/batch/openuserid_to_userid</c>，但本场景请求体无需 source_agentid。</para>
    /// <para>open_userid_list 最多不超过 1000 个；成员需要在 access_token 所对应应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="OpenUserIdToUserIdForBotRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换成功的明文 userid 映射列表（userid_list）及不合法列表（invalid_open_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101521"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/openuserid_to_userid")]
    Task<OpenUserIdToUserIdResponse> OpenUserIdToUserIdForBotAsync(
        [Body] OpenUserIdToUserIdForBotRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// external_userid 转换（自建应用与第三方/代开发应用的对接）
    /// <para>将代开发应用或第三方应用获取的 external_userid 转换成自建应用（企业主体）的 external_userid。</para>
    /// <para>客户的跟进人，或者用户所在客户群的群主，需要同时在 access_token 和
    /// source_agentid 所对应应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="FromServiceExternalUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业主体的 external_userid。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95884"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/from_service_external_userid")]
    Task<FromServiceExternalUserIdResponse> ConvertFromServiceExternalUserIdAsync(
        [Body] FromServiceExternalUserIdRequest request,
        CancellationToken cancellationToken = default);
}
