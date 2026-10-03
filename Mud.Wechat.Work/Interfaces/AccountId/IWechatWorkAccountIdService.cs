// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「ID 转换」接口族公共 SDK
/// （userid / external_userid / unionid / 客户标签 / 微信客服 ID 转换 + 迁移状态查询 + 大批量调用凭据）。
/// <para>
/// 企业微信「账号ID安全性升级」后，第三方与代开发应用获取的 corpid / userid / external_userid / 群 ID
/// 均为服务商主体的密文 ID（自建应用保持明文 ID）。本族收敛官方对<b>第三方应用与服务商代开发开放完全一致</b>
/// 的 9 个端点（userid 转换 96516/97106、external_userid 转换与群成员转换 97063/97107、
/// unionid 关联 95900/97108、客户标签 ID 转换 96169/97109、微信客服 ID 转换 97064/97110、
/// 查询迁移状态 96518/97104、大批量调用凭据 96168/96250），全部声明于本公共父接口。
/// 企业自建应用官方无本族端点（自建侧对接能力见 <see cref="IWechatWorkAccountIdInteropService"/> 与
/// <see cref="IWechatWorkAccountIdTmpExternalUserIdService"/>），故不设自建子接口；
/// 第三方子接口为空标记；代开发子接口另持「群 ID 升级」差异端点（99601，
/// 见 <see cref="IWechatWorkProviderAccountIdService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，
/// 即第三方/代开发应用在授权企业侧的应用凭证），由多应用基座按当前应用上下文（AppKey + scope）路由。
/// </para>
/// <para>
/// corpid / userid / external_userid 密文均大小写敏感，使用时不能再转为纯小写或纯大写。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAccountIdService
{
    /// <summary>
    /// userid 的转换
    /// <para>将企业主体下的明文 userid 转换为服务商主体下的密文 userid（open_userid）。</para>
    /// <para>仅代开发自建应用或第三方应用可调用；成员需要在应用的可见范围内；
    /// 传入 userid 出错次数较多会导致 1 天不可调用（具体限制阈值由授权企业的员工规模决定）。</para>
    /// </summary>
    /// <param name="request">userid 转换请求体（<see cref="UserIdToOpenUserIdRequest"/>；最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换成功的 userid 与 open_userid 映射列表（open_userid_list）及转换失败列表（invalid_userid_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96516"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97106"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/userid_to_openuserid")]
    Task<UserIdToOpenUserIdResponse> UserIdToOpenUserIdAsync(
        [Body] UserIdToOpenUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 转换 external_userid
    /// <para>将企业主体的 external_userid 转换为服务商主体加密的 external_userid；
    /// 传入新的 external_userid 则原样返回。</para>
    /// <para>客户联系和家校场景中，external_userid 对应的跟进人需要在应用可见范围内；
    /// 微信客服场景中，仅支持 48 小时内客服会话的 external_userid。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="GetNewExternalUserIdRequest"/>；建议 200 个、最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（items：external_userid + new_external_userid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97063"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97107"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_new_external_userid")]
    Task<GetNewExternalUserIdResponse> GetNewExternalUserIdAsync(
        [Body] GetNewExternalUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 转换客户群成员 external_userid
    /// <para>转换客户群中无好友关系的群成员 external_userid（「转换 external_userid」接口不支持客户群场景，
    /// 调用时需传入客户群的 chat_id）；传入新的 external_userid 则原样返回。</para>
    /// <para>客户群的群主需要在应用可见范围内。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="GetGroupChatNewExternalUserIdRequest"/>；最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（items：external_userid + new_external_userid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97063"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97107"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/get_new_external_userid")]
    Task<GetGroupChatNewExternalUserIdResponse> GetGroupChatNewExternalUserIdAsync(
        [Body] GetGroupChatNewExternalUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// unionid 转换为第三方 external_userid
    /// <para>根据微信开放平台的 unionid 与 openid 查询对应的微信用户 external_userid；
    /// 用户未成为企业客户时返回 pending_id（临时外部联系人 ID，有效期 90 天，
    /// 仅用于关联映射、无法当成 external_userid 调用接口）。</para>
    /// <para>频率限制：10 万次/小时、48 万次/天、750 万次/月（subject_type=0 时所有服务商共用企业额度，
    /// subject_type=1 时按服务商）；传入有效 mass_call_ticket 可不受业务频率限制（仍受基础频率限制）。</para>
    /// </summary>
    /// <param name="request">unionid 关联请求体（<see cref="ConvertUnionidToExternalUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该授权企业的外部联系人 ID（external_userid）或临时外部联系人 ID（pending_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95900"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97108"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/unionid_to_external_userid")]
    Task<ConvertUnionidToExternalUserIdResponse> ConvertUnionidToExternalUserIdAsync(
        [Body] ConvertUnionidToExternalUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// external_userid 查询 pending_id
    /// <para>将外部联系人 id 批量转换为临时外部联系人 ID（pending_id），
    /// 以打通 unionid = pending_id = external_userid 的映射关系。</para>
    /// <para>仅认证企业可调用；客户跟进人或客户群群主须在应用可见范围内；
    /// 必须曾通过 unionid 转换接口获取过该客户的 pending_id（超 90 天失效）；
    /// 官方契约陷阱：请求体数组字段名为 <c>external_userid</c>（不带 <c>_list</c> 后缀）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="ConvertExternalUserIdToPendingIdRequest"/>；最多可同时查询 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（result：external_userid + pending_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95900"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97108"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/batch/external_userid_to_pending_id")]
    Task<ConvertExternalUserIdToPendingIdResponse> ConvertExternalUserIdToPendingIdAsync(
        [Body] ConvertExternalUserIdToPendingIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 客户标签 ID 的转换
    /// <para>将企业主体下的客户标签 ID（含标签组 ID）转换成服务商主体下的客户标签 ID。</para>
    /// <para>应用需具有「企业客户权限-&gt;客户基础信息」权限。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ExternalTagIdConvertRequest"/>；最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（items）及无法转换列表（invalid_external_tagid_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96169"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97109"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/external_tagid")]
    Task<ExternalTagIdConvertResponse> ConvertExternalTagIdAsync(
        [Body] ExternalTagIdConvertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信客服 ID 的转换
    /// <para>将企业主体下的微信客服 ID 转换成服务商主体下的微信客服 ID。</para>
    /// <para>应用需具有「微信客服-&gt;管理账号、分配会话和收发消息」权限。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="OpenKfIdConvertRequest"/>；最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（items）及无法转换列表（invalid_open_kfid_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97064"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97110"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/open_kfid")]
    Task<OpenKfIdConvertResponse> ConvertOpenKfIdAsync(
        [Body] OpenKfIdConvertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代开发应用查询迁移状态
    /// <para>查询应用的 ID 回收状态（新授权代开发应用或设置迁移完成后，均可能返回新的 openid）。
    /// 官方契约：本端点为<b>无请求体的 POST</b>，仅以 Query 注入的 access_token 鉴权。</para>
    /// <para>仅代开发自建应用和第三方应用可以调用（区别于设置迁移完成接口的 provider_access_token）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>迁移状态列表（migration_info：openid_type 1-corpid/userid、3-external_userid；status 0-未升级、1-已升级）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96518"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97104"/></para>
    /// </remarks>
    [Post("/cgi-bin/corp/get_openid_migration")]
    Task<GetOpenIdMigrationResponse> GetOpenIdMigrationAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取接口大批量调用凭据
    /// <para>ID 转换等接口存在较严格的业务频率限制，服务商可在企业授权后 3 个月内申请一个高频调用凭据
    /// （每个企业最多仅能获取一次，有效期 7 天），调用相关接口时传入该凭据可不受业务频率限制（仍受基础频率限制）。
    /// 目前仅 unionid 转第三方 external_userid 与 unionid 转上下游 external_userid 两接口支持该凭据。</para>
    /// <para>官方授权口径：第三方应用 access_token、代开发应用 access_token，或上下游自建/代开发应用的上游应用 access_token。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>大批量调用凭据（mass_call_ticket）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96168"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96250"/></para>
    /// </remarks>
    [Get("/cgi-bin/corp/apply_mass_call_ticket")]
    Task<ApplyMassCallTicketResponse> ApplyMassCallTicketAsync(
        CancellationToken cancellationToken = default);
}
