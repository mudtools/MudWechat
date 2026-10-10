// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.License;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「接口调用许可」模块账号管理域第三方应用 / 服务商代开发 SDK
/// （激活账号 / 获取激活码详情 / 获取企业的账号列表 / 获取成员的激活详情 / 账号继承 /
/// 分配激活码给下游或下级企业，共 9 个端点）。
/// <para>官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
/// 全部 9 个端点声明于本接口；企业自建应用官方不开放，不设自建子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：本族端点以服务商凭证调用，操作对象为已购买接口许可的授权企业账号
/// （激活码 / 已激活成员 / 继承与分配关系）。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "License",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkLicenseAccountService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyLicenseAccountService : IWechatWorkLicenseAccountService
{
    /// <summary>
    /// 激活账号
    /// <para>下单购买账号并支付完成之后，先调用获取订单中的账号列表接口获取到账号激活码，
    /// 然后可以调用该接口将激活码绑定到某个企业员工，以对其激活相应的平台服务能力。</para>
    /// <para>官方约束：一个 userid 允许激活一个基础账号以及一个互通账号；若 userid 已激活，
    /// 使用同类型的激活码来激活后，则绑定关系变为新激活码，新激活码有效时长自动叠加上旧激活码剩余时长，
    /// 同时旧激活码失效；多个同类型的激活码累加后的有效期不可超过 5 年，否则接口报错 701030；
    /// 只有当旧的激活码的剩余时长小于等于 20 天，才可以使用新的同类型的激活码进行激活并续期。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ActiveLicenseAccountRequest"/>：active_code 账号激活码 /
    /// corpid 激活码所属企业corpid / userid 待绑定激活的企业成员userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97188"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/active_account")]
    Task<WechatWorkResponse> ActiveAccountAsync(
        [Body] ActiveLicenseAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量激活账号
    /// <para>可在一次请求里为一个企业的多个成员激活许可账号，便于服务商批量化处理。</para>
    /// <para>官方约束：一个 userid 允许激活一个基础账号以及一个互通账号；若 userid 已激活，
    /// 使用同类型的激活码来激活后，则绑定关系变为新激活码，新激活码有效时长自动叠加上旧激活码剩余时长，
    /// 同时旧激活码失效；多个同类型的激活码累加后的有效期不可超过 5 年，否则接口报错 701030；
    /// 只有当旧的激活码的剩余时长小于等于 20 天，才可以使用新的同类型的激活码进行激活并续期；
    /// 单次激活的员工数量不超过 1000。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchActiveLicenseAccountRequest"/>：corpid 激活码所属企业corpid /
    /// active_list 需要激活的账号列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>激活结果列表（active_result：active_code / userid / errcode，0 为成功）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97188"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/batch_active_account")]
    Task<BatchActiveLicenseAccountResponse> BatchActiveAccountAsync(
        [Body] BatchActiveLicenseAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定账号类型激活
    /// <para>从当前企业中选择一个该指定类型的激活截止时间最早的未激活的激活码进行激活。</para>
    /// <para>官方约束：userid 当前必须未激活指定类型的许可或者绑定的该类型账号已过期。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ActiveLicenseAccountByTypeRequest"/>：type 账号类型 /
    /// corpid 激活码所属企业corpid / userid 待绑定激活的企业成员userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97188"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/active_account_by_type")]
    Task<WechatWorkResponse> ActiveAccountByTypeAsync(
        [Body] ActiveLicenseAccountByTypeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取激活码详情
    /// <para>查询某个账号激活码的状态以及激活绑定情况。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLicenseActiveInfoByCodeRequest"/>：corpid 要查询的企业的corpid /
    /// active_code 激活码）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账号码信息（active_info：active_code / type / status / userid / create_time /
    /// active_time / expire_time / merge_info / share_info）。
    /// <para>官方口径：userid 返回加密的 userid，未激活则不返回该字段；
    /// active_time 与 expire_time 未激活则不返回该字段；merge_info 合并的激活码或者被合并的激活码才返回；
    /// share_info 仅上下游 / 企业互联场景下分配关系才返回。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97189"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_active_info_by_code")]
    Task<GetLicenseActiveInfoByCodeResponse> GetActiveInfoByCodeAsync(
        [Body] GetLicenseActiveInfoByCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取激活码详情
    /// <para>批量查询账号激活码的状态以及激活绑定情况。</para>
    /// <para>官方约束：active_code_list 最多不超过 1000 个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchGetLicenseActiveInfoByCodeRequest"/>：corpid 要查询的企业的corpid /
    /// active_code_list 激活码列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账号码信息列表（active_info_list）/ 无效的激活码列表（invalid_active_code_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97189"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/batch_get_active_info_by_code")]
    Task<BatchGetLicenseActiveInfoByCodeResponse> BatchGetActiveInfoByCodeAsync(
        [Body] BatchGetLicenseActiveInfoByCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业的账号列表
    /// <para>查询指定企业下的平台能力服务账号列表。</para>
    /// <para>官方约束：若为上下游场景，corpid 指定的为上游企业，仅返回上游企业激活的账号；
    /// 若 corpid 指定为下游企业，若激活码为上游企业分享过来的且已绑定，也会返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListLicenseActivedAccountRequest"/>：corpid 企业corpid /
    /// limit 返回的最大记录数 / cursor 分页游标）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor）/ 是否结束（has_more）/ 已激活成员列表（account_list，
    /// 已激活过期的也会返回）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97190"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/list_actived_account")]
    Task<ListLicenseActivedAccountResponse> ListActivedAccountAsync(
        [Body] ListLicenseActivedAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员的激活详情
    /// <para>查询某个企业成员的激活情况。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLicenseActiveInfoByUserRequest"/>：corpid 企业corpid /
    /// userid 待查询员工的userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账号激活状态（active_status：0 未激活 / 1 已激活）/ 账号列表（active_info_list，
    /// 同一个 userid 同类账号最多只能有一个）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97191"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_active_info_by_user")]
    Task<GetLicenseActiveInfoByUserResponse> GetActiveInfoByUserAsync(
        [Body] GetLicenseActiveInfoByUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 账号继承
    /// <para>在企业员工离职或者工作范围的有变更时，允许将其许可账号继承给其他员工。</para>
    /// <para>官方约束：转移成员和接收成员属于同一个企业；转移成员的账号已激活，且在有效期；
    /// 转移许可的成员为离职成员，或不在服务商应用的可见范围内时，不限制下次转移的时间间隔；
    /// 转移许可的成员为在职成员且在服务商应用的可见范围内时，转移后 30 天后才可进行下次转移；
    /// 当接收成员许可与转移成员的许可重叠时（同时拥有基础账号或者互通账号），
    /// 如果接收成员许可剩余时长小于等于 20 天则可以成功继承，否则会报错；单次转移的账号数限制在 1000 以内。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchTransferLicenseRequest"/>：corpid 待绑定激活的成员所属企业corpid /
    /// transfer_list 继承信息列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>继承结果列表（transfer_result：handover_userid / takeover_userid / errcode）。
    /// <para>官方口径：响应中的转移成员与接收成员 userid 均为加密的 userid。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97192"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/batch_transfer_license")]
    Task<BatchTransferLicenseResponse> BatchTransferLicenseAsync(
        [Body] BatchTransferLicenseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配激活码给下游/下级企业
    /// <para>服务商可调用该接口将为上游/上级企业购买的激活码分配给下游/下级企业使用。</para>
    /// <para>官方约束：目前支持的场景包括企微上下游和企业互联（含局校互联）；
    /// 上游/上级企业有共享该服务商的第三方应用或代开发应用给下游/下级企业；
    /// 分配给下游/下级企业的激活码，当前未激活，且属于上游/上级企业的，且未分配给其他下游/下级企业；
    /// 每次调用接口分配的激活码账号数，不能超过下游/下级企业在上下游/企业互联通讯录中人数上限的两倍，
    /// 且每次分配激活码不可超过 1000 个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchShareLicenseActiveCodeRequest"/>：from_corpid 上游/上级企业corpid /
    /// to_corpid 下游/下级企业corpid / share_list 分配的接口许可列表 / corp_link_type 分配的场景）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分配结果（share_result，仅分配失败的会返回：active_code / errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97193"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/batch_share_active_code")]
    Task<BatchShareLicenseActiveCodeResponse> BatchShareActiveCodeAsync(
        [Body] BatchShareLicenseActiveCodeRequest request,
        CancellationToken cancellationToken = default);
}
