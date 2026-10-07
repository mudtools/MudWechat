// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.License;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「接口调用许可」模块订单管理域第三方应用 / 服务商代开发 SDK
/// （下单购买账号 / 下单续期账号 / 获取订单列表 / 获取订单详情 / 获取订单中的账号列表 / 取消订单 /
/// 下单购买多企业账号 / 获取多企业订单详情 / 使用余额支付订单 / 民生优惠条件查询 / 充值账户余额查询，共 15 个端点）。
/// <para>官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
/// 全部 15 个端点声明于本接口；企业自建应用官方不开放，不设自建子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：下单 / 支付类端点的下单人与支付人必须为服务商企业内具有「购买接口许可」权限的管理员
/// （该 userid 必须登录过企业微信，且企业微信已绑定微信）。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "License",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkLicenseOrderService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyLicenseOrderService : IWechatWorkLicenseOrderService
{
    /// <summary>
    /// 下单购买账号
    /// <para>服务商下单为企业购买新的账号，可以同时购买基础账号与互通账号。
    /// 下单之后，需要到服务商管理端发起支付，支付完成之后，订单才能生效；也可以通过接口使用余额支付订单。</para>
    /// <para>官方约束：自 2023 年 6 月 30 日起，服务商为企业购买接口许可时，会检查企业下的应用订单情况，
    /// 应用订单检查通过时才可购买接口许可（关联应用订单失败时，返回的 errmsg 将包含有「WARNING」字段）。</para>
    /// <para>官方约束：基础账号跟互通账号不能同时为 0；总购买时长为 (months*31+days) 天，
    /// 最少购买 1 个月（31 天），最多购买 60 个月（1860 天）；若企业为服务商测试企业，最多各购买 1000 个账号，
    /// 且只支持购买 1 个月、不支持指定天购买。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateNewLicenseOrderRequest"/>：corpid 企业id /
    /// buyer_userid 下单人 / account_count 账号个数详情 / account_duration 账号购买时长）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单号（order_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97182"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/create_new_order")]
    Task<CreateNewLicenseOrderResponse> CreateNewOrderAsync(
        [Body] CreateNewLicenseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建续期任务
    /// <para>可以下单为一批已激活账号的成员续期，续期下单分为两个步骤：传入 userid 列表创建一个任务；
    /// 根据任务 jobid 提交订单（见 <see cref="SubmitOrderJobAsync"/>）。</para>
    /// <para>官方约束：在同一个订单里，首次创建任务无须指定 jobid，后续指定同一个 jobid，
    /// 表示往同一个订单任务追加续期的成员。</para>
    /// <para>官方约束：account_list 每次最多 1000 个；同一个 jobid 最多关联 1000000 个基础账号跟 1000000 个互通账号。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateRenewLicenseOrderJobRequest"/>：corpid 企业id /
    /// account_list 续期的账号列表 / jobid 任务id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务id（jobid，请求包中未指定 jobid 时生成新 jobid 返回）/
    /// 不合法的续期账号列表（invalid_account_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97183"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/create_renew_order_job")]
    Task<CreateRenewLicenseOrderJobResponse> CreateRenewOrderJobAsync(
        [Body] CreateRenewLicenseOrderJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交续期订单
    /// <para>创建续期任务之后，需要调用该接口，以提交订单任务。
    /// 注意，提交之后，需要到服务商管理端发起支付，支付完成之后，订单才能生效；也可以通过接口使用余额支付订单。</para>
    /// <para>官方约束：account_duration 的 months 与 new_expire_time 二者填其一；
    /// new_expire_time 不可为今天和过去的时间，不可为 1860 天后的时间，须填当天的 24 时 0 分 0 秒，
    /// 否则系统自动处理为当天的 24 时 0 分 0 秒；若企业为服务商测试企业，每次续期只能续期 1 个月、
    /// 不支持指定新的到期时间来续期。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SubmitLicenseOrderJobRequest"/>：jobid 任务id /
    /// buyer_userid 下单人 / account_duration 账号购买时长）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单号（order_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97183"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/submit_order_job")]
    Task<SubmitLicenseOrderJobResponse> SubmitOrderJobAsync(
        [Body] SubmitLicenseOrderJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单列表
    /// <para>服务商查询自己某段时间内的平台能力服务订单列表。</para>
    /// <para>官方约束：start_time 跟 end_time 必须同时指定，不能单独指定，且起始时间跟结束时间不能超过 31 天；
    /// 若指定 corpid 且 corpid 为服务商测试企业，则返回的订单列表为测试订单列表，否则只返回正式订单列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListLicenseOrderRequest"/>：corpid 企业id /
    /// start_time 开始时间 / end_time 结束时间 / cursor 分页游标 / limit 返回的最大记录数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor）/ 是否有更多（has_more）/ 订单列表（order_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97184"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/list_order")]
    Task<ListLicenseOrderResponse> ListOrderAsync(
        [Body] ListLicenseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情
    /// <para>查询某个订单的详情，包括订单的状态、基础账号个数、互通账号个数、账号购买时长等。</para>
    /// <para>官方约束：该接口不返回订单中的账号激活码列表或者续期的账号成员列表，
    /// 请调用 <see cref="ListOrderAccountAsync"/> 以获取账号列表；
    /// 此接口不支持获取多企业订单详情，多企业订单请调用 <see cref="GetUnionOrderAsync"/>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLicenseOrderRequest"/>：order_id 订单id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（order：order_id / order_type / order_status / corpid / price /
    /// account_count / account_duration / create_time / pay_time）。
    /// <para>官方口径：corpid 返回加密的 corpid；pay_time 迁移订单不返回该字段。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97185"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_order")]
    Task<GetLicenseOrderResponse> GetOrderAsync(
        [Body] GetLicenseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单中的账号列表
    /// <para>查询指定订单下的平台能力服务账号列表。若为购买账号的订单或者存量企业的版本付费迁移订单，
    /// 则返回账号激活码列表；若为续期账号的订单，则返回续期账号的成员列表。</para>
    /// <para>官方约束：若是购买账号的订单，则仅订单支付完成时系统才会生成账号，
    /// 故支付完成之前该接口不会返回账号激活码；若是多企业订单，请先调用 <see cref="GetUnionOrderAsync"/>
    /// 获取到每个企业的子订单id (sub_order_id)，然后使用 sub_order_id 来调用此接口。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListLicenseOrderAccountRequest"/>：order_id 订单号 /
    /// limit 返回的最大记录数 / cursor 分页游标）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor）/ 是否有更多（has_more）/ 账号列表（account_list）。
    /// <para>官方口径：续期订单的 userid 返回加密的 userid。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97186"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/list_order_account")]
    Task<ListLicenseOrderAccountResponse> ListOrderAccountAsync(
        [Body] ListLicenseOrderAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订单
    /// <para>取消接口许可购买和续费订单，只可取消未支付且未失效的订单。</para>
    /// <para>官方约束：corpid 若为多企业新购订单时不填，否则必填。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelLicenseOrderRequest"/>：order_id 订单id / corpid 企业id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97187"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/cancel_order")]
    Task<WechatWorkResponse> CancelOrderAsync(
        [Body] CancelLicenseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建多企业新购任务
    /// <para>可以下单为多个企业购买新的账号，可以同时购买基础账号与互通账号。多企业下单分为两个步骤：
    /// 传入企业新购信息列表创建一个任务（创建之后追加参数 jobid 可以往同一个任务继续追加）；
    /// 根据任务 jobid 提交订单（见 <see cref="SubmitNewOrderJobAsync"/>）。</para>
    /// <para>官方约束：buy_list 每次最多传 10 个，每个 jobid 最多关联 100000 个 BuyInfo；
    /// 测试企业不支持多企业下单方式；auto_active_status 不填默认开启。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateNewLicenseOrderJobRequest"/>：buy_list 企业新购信息列表 /
    /// jobid 多企业新购任务id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>多企业新购任务id（jobid，请求包中未指定 jobid 时生成新 jobid 返回）/
    /// 不合法的新购信息列表（invalid_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/98887"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/create_new_order_job")]
    Task<CreateNewLicenseOrderJobResponse> CreateNewOrderJobAsync(
        [Body] CreateNewLicenseOrderJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交多企业新购订单
    /// <para>创建多企业新购任务之后，需要调用该接口，以提交多企业新购订单任务。
    /// 注意，提交之后，需要到服务商管理端发起支付，支付完成之后，订单才能生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SubmitNewLicenseOrderJobRequest"/>：jobid 多企业新购任务id /
    /// buyer_userid 下单人）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/98887"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/submit_new_order_job")]
    Task<WechatWorkResponse> SubmitNewOrderJobAsync(
        [Body] SubmitNewLicenseOrderJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取多企业新购订单提交结果
    /// <para>提交多企业新购订单之后，用于获取该订单的创建结果。</para>
    /// <para>官方约束：该结果仅在提交多企业新购订单后 7 天内可获取。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetNewLicenseOrderJobResultRequest"/>：jobid 多企业新购任务id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单创建结果（status：1 创建完成 / 2 创建中，稍后再试 / 3 创建失败）/
    /// 订单号（order_id，创建完成后返回）/ 下单失败的企业及原因（fail_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/98887"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/new_order_job_result")]
    Task<GetNewLicenseOrderJobResultResponse> GetNewOrderJobResultAsync(
        [Body] GetNewLicenseOrderJobResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取多企业订单详情
    /// <para>查询某个多企业订单的详情，包括订单的状态、购买的企业、基础账号个数、互通账号个数、账号购买时长等。</para>
    /// <para>官方约束：如需每个企业的账号列表，请使用该接口返回的 sub_order_id 调用
    /// <see cref="ListOrderAccountAsync"/> 以获取账号列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetUnionLicenseOrderRequest"/>：order_id 订单id /
    /// limit 返回的最大记录数 / cursor 分页游标）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（order）/ 是否有更多（has_more）/ 分页游标（next_cursor）/
    /// 多企业购买信息列表（buy_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/98888"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_union_order")]
    Task<GetUnionLicenseOrderResponse> GetUnionOrderAsync(
        [Body] GetUnionLicenseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交余额支付订单任务
    /// <para>使用该接口创建支付任务，该接口默认使用充值账户余额进行支付。提交成功后，该订单无法再变更支付方式。
    /// 可以支付的订单包括单企业购买、单企业续期、多企业购买创建的订单，支付成功后自动扣款，
    /// 且服务商可接收到支付成功的回调。</para>
    /// <para>官方约束：提交支付任务成功后，支付任务异步进行，服务商还需要调用
    /// <see cref="GetPayJobResultAsync"/> 以获取支付的最终结果；
    /// payer_userid 必须登录过企业微信，并且企业微信已绑定微信，且必须为服务商企业内
    /// 具有「购买接口许可」权限的管理员（用于充值账户的流水记录）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SubmitLicensePayJobRequest"/>：payer_userid 支付人 /
    /// order_id 要使用充值账户余额支付的接口许可订单id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付任务的 jobid（jobid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99420"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/submit_pay_job")]
    Task<SubmitLicensePayJobResponse> SubmitPayJobAsync(
        [Body] SubmitLicensePayJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单支付结果
    /// <para>使用该接口获取余额订单支付任务的执行结果。</para>
    /// <para>官方约束：仅在提交了「余额支付订单任务」后的 7 天内可获取。</para>
    /// <para>官方契约陷阱：顶层 errcode 表示接口调用是否成功（而非支付是否成功）——支付失败时该错误码也会返回 0，
    /// 支付是否成功须以 status 与 pay_job_result 判定。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLicensePayJobResultRequest"/>：jobid
    /// 「提交余额支付订单任务」返回的 jobid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支付任务结果（status：1 支付成功 / 2 支付任务执行中，稍后再试 / 3 支付失败）/
    /// 支付结果的信息（pay_job_result，仅在支付失败时返回）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/99420"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/pay_job_result")]
    Task<GetLicensePayJobResultResponse> GetPayJobResultAsync(
        [Body] GetLicensePayJobResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 民生优惠条件查询
    /// <para>查询企业是否满足民生行业政策的优惠条件。</para>
    /// <para>官方约束：查询的企业必须安装了服务商的第三方应用或者代开发应用；
    /// 一个企业在 30 天内最多只能查询一次。</para>
    /// <para>官方注记：民生行业接口许可优惠政策于 2023 年 3 月 31 日到期，到期后不再支持查询。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="QueryLicenseSupportPolicyRequest"/>：corpid 企业id，
    /// 支持加密和非加密的 corpid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>查询结果（query_result：<c>0</c>-不符合减免条件 / <c>1</c>-符合减免条件）/
    /// 不符合减免条件的原因错误码列表（unsatisfied_reason，701090 认证或验证状态不符合 /
    /// 701091 行业类型不符合 / 701092 统一社会信用代码不符合 / 701096 风控审核不通过 /
    /// 701110 学校不符合单校条件 / 701111 学校不符合局校条件）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97208"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/support_policy_query")]
    Task<QueryLicenseSupportPolicyResponse> QuerySupportPolicyAsync(
        [Body] QueryLicenseSupportPolicyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 充值账户余额查询
    /// <para>可以通过该接口查询服务商充值账户余额。</para>
    /// <para>官方约束：请求无业务参数、无请求体，仅经 provider_access_token 鉴权（官方本端点即 GET）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>充值账户余额（balance，单位为分）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/100138"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Get("/cgi-bin/service/get_account_balance")]
    Task<GetAccountBalanceResponse> GetAccountBalanceAsync(CancellationToken cancellationToken = default);
}
