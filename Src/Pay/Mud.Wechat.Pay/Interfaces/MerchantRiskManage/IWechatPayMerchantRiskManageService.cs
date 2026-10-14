// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantRiskManage;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户风险管理」域 SDK（<b>11 端点</b>：风险订单同步 4 + 风险共建 3 + 违规通知 URL 四件套 4）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 逐页核验，两轮全覆盖）：查询风险订单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013894221"/>（服务商）/
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>（普通商户）、回传处置结果
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013894227"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894250"/>、上报订单关联信息
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>、查询风险信息
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012467535"/>、处理结果回传
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012467540"/>、违规通知回调地址四支
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471333"/>（创建）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471330"/>（修改）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471327"/>（查询）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471334"/>（删除）。</para>
/// <para><b>11 个方法但只有 9 条 (方法, 路由)</b>：风险订单两支与处置结果两支各自「服务商面 / 普通商户面
/// 同方法同路由、字段表不同」⇒ 同路由多方法（与消息推送域「每个 msgtype 一个方法同路由」同形），
/// 端点计数按方法计、路由计数按 (方法, 路由) 去重（守卫 MRB-B1 两个数都锁）。</para>
/// <para><b>本域是支付线首次出现非 GET/POST 动词的面</b>：<b>PUT</b>（修改通知地址）与
/// <b>DELETE</b>（删除通知地址）各一支。二者对「204 还是 JSON」的答复相反：
/// 同一条 <c>/violation-notifications</c> 上 PUT/POST/GET 都是 <b>200 + <c>{notify_url}</c></b>，
/// 只有 DELETE 是 <b>204 无包体</b> ⇒ 不得按「写操作都 204」的直觉收口（守卫 MRB-B1 逐支断言）。</para>
/// <para><b>四套商户标识并存</b>：<c>mchid</c>（普通商户面）/ <c>sp_mchid</c>+<c>sub_mchid</c>（服务商面）/
/// <c>sp_mchid</c>+<c>acquiring_bank_id</c>+<c>channel_id</c>+<c>sub_mchid</c>（风险共建的从业机构与渠道面）/
/// <c>sub_orders[].mch_id</c>（合单子单，官方此处<b>带下划线</b>）⇒ 照抄任一别域的标识写法都会静默错参。</para>
/// <para><b>同名 <c>risk_type</c> 有两套类型</b>：风险订单同步面是 <b>integer</b>（1~4），
/// 风险共建面是 <b>string</b>（5 值）⇒ 两族 DTO 与常量类各一套，跨族赋值即错（守卫 MRB-B3）。</para>
/// <para><b>官方拼写陷阱照页</b>：<c>risk_discove_time</c>（缺 r）、<c>same_deviced</c>（多 d）——
/// 本域两处误拼均在服务商面页内逐字核得，修正拼写会静默丢字段。</para>
/// <para><b>敏感字段加密</b>：<c>report-trade-union-information</c> 的 <c>phone</c> 与
/// <c>certificates_number</c>（身份证后 6 位）须以微信支付公钥（推荐）或平台证书加密，
/// 该页 <c>Wechatpay-Serial</c> 是<b>参数表行</b>（必填）；本域其余页面均无该表行
/// （违规通知四页仅在示例代码里出现）⇒ 加密头由传输层与宿主协同，DTO 只承载密文字符串。</para>
/// <para><b>频率限制</b>：本域<b>全部页面均未印端点级数字</b>（两轮复核一致），只有 429
/// <c>FREQUENCY_LIMITED</c>（<c>trade-risk-information</c> / <c>trade-risk-result</c> 两页另列
/// <c>RATELIMIT_EXCEEDED</c>，官方文案「请降低频率后重试」）⇒ 注释不得凭第三方 SDK 补数。</para>
/// <para><b>业务错误码</b>：<c>PARAM_ERROR</c>(400) / <c>INVALID_REQUEST</c>(400) / <c>SIGN_ERROR</c>(401) /
/// <c>NO_AUTH</c>(403) / <c>MCH_NOT_EXISTS</c>(404) / <c>NOT_FOUND</c>(404) / <c>ORDER_NOT_EXIST</c>(404) /
/// <c>ALREADY_EXISTS</c>(409，仅创建通知地址支) / <c>SYSTEM_ERROR</c>(500) / <c>FREQUENCY_LIMITED</c>(429)，
/// 原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本域「查询风险信息」是单查不是列表</b>：官方页正文只按 <c>out_trade_no</c> 查一笔、
/// 应答无数组节点（方案 §2.7 已勘误该行的转述），列表形态在 <c>query-risk-trades</c>（<c>page_index</c> /
/// <c>page_size</c> 整型页码，<b>不是</b> <c>offset</c>/<c>limit</c> 游标）。</para>
/// <para><b>本域不含「消费者投诉」与「商户风险规则」侧栏组</b>：投诉族落 <c>PayModule.MerchantService</c>，
/// 而支付分风控（<c>/v3/pay-score</c>）、服务商支付分（<see cref="IWechatPayPartnerPayScoreService"/>）、
/// 银行面风控（<see cref="IWechatPayBankPayScoreService"/>）是各自独立的域，路由与字段均不互通。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，
/// 本域<b>零 <c>[Token]</c></b>（支付线以商户私钥签名，与公众平台令牌面无关，守卫 PAY-B1）。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantRiskManage", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantRiskManageService
{
    /// <summary>
    /// 「查询风险订单」服务商形态：按商户与时间区间分页拉取风险订单列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894221"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManagePartnerRiskTradeQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>总数与风险订单列表，见 <see cref="MerchantRiskManagePartnerRiskTradeQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades</c>；
    /// 支持商户【普通服务商】；无 path / query 参数。</para>
    /// <para><b>官方把每个字段都标选填</b>（含两个商户标识），但描述内注明「若为子商户调用，须传入对应的服务商号」
    /// ⇒ 条件必填交官方判定。</para>
    /// <para><b>分页是整型页码</b> <c>page_index</c> / <c>page_size</c>，不是 <c>offset</c> / <c>limit</c>。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades")]
    Task<MerchantRiskManagePartnerRiskTradeQueryResponse> QueryRiskTradesByPartnerAsync(
        MerchantRiskManagePartnerRiskTradeQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询风险订单」普通商户形态：与 <see cref="QueryRiskTradesByPartnerAsync"/> <b>同方法同路由</b>，标识字段换成 <c>mchid</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManageMchRiskTradeQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>总数与风险订单列表（条目内<b>无</b> <c>sp_mchid</c>），见 <see cref="MerchantRiskManageMchRiskTradeQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades</c>；
    /// 支持商户【普通商户】。</para>
    /// <para><b>两形态请求表与应答表都不相同</b>（服务商多 <c>sp_mchid</c>+<c>sub_mchid</c>，应答条目多 <c>sp_mchid</c>）
    /// ⇒ 按「表不同则分建」各建类型，请求 DTO 不可互换（守卫 MRB-B6）。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/ec-trade-risk/query-risk-trades")]
    Task<MerchantRiskManageMchRiskTradeQueryResponse> QueryRiskTradesByMchAsync(
        MerchantRiskManageMchRiskTradeQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「回传处置结果」服务商形态：把商户侧对某笔风险订单的最终判定与处置结论回传微信。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894227"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManagePartnerDisposeResultRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>单字段 <c>rsp_msg</c>，见 <see cref="MerchantRiskManageDisposeResultResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result</c>；
    /// 支持商户【普通服务商】。</para>
    /// <para><b><c>out_trade_no</c> 是本域唯一必填</b>（string(64)）⇒ 缺它即 <c>PARAM_ERROR</c>。</para>
    /// <para><b><c>disp_conclusion</c> 是 <c>array[integer]</c></b>（1-未处置,2-拦截订单,3-关闭店铺），
    /// 不是 string 枚举。</para>
    /// <para><b><c>refund_amount</c> 官方只写「退款金额」，两轮复核均未印单位与区间</b>
    /// ⇒ 本 SDK 原样透传 integer，宿主勿自行按「分」换算。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result")]
    Task<MerchantRiskManageDisposeResultResponse> RecvDisposeResultByPartnerAsync(
        MerchantRiskManagePartnerDisposeResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「回传处置结果」普通商户形态：与 <see cref="RecvDisposeResultByPartnerAsync"/> <b>同方法同路由</b>，应答类型共用。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894250"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManageMchDisposeResultRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与服务商形态<b>同一应答类型</b>，见 <see cref="MerchantRiskManageDisposeResultResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result</c>；
    /// 支持商户【普通商户】。</para>
    /// <para><b>请求分建、应答共用</b>：官方两页应答表同为单字段 <c>rsp_msg</c> string(1024)，零差异 ⇒ 归并
    /// （守卫 MRB-B6 断言两方法返回同一类型）。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result")]
    Task<MerchantRiskManageDisposeResultResponse> RecvDisposeResultByMchAsync(
        MerchantRiskManageMchDisposeResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「上报订单关联信息」（风险共建）：把订单侧的风控要素（含加密的手机号与证件后 6 位）上报微信以换取更准的风险判定。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManageTradeUnionReportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显五项商户标识，见 <see cref="MerchantRiskManageTradeUnionReportResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/report-trade-union-information</c>；
    /// 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】。</para>
    /// <para><b><c>Wechatpay-Serial</c> 在本支是参数表行（必填）</b>：<c>phone</c> 与
    /// <c>certificates_number</c>（身份证后 6 位）须加密 ⇒ 密文由宿主产出，SDK 不解密也不转换。</para>
    /// <para><b>应答无状态字段</b>：本支结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</para>
    /// <para><b>六个整型字段官方未给取值字典</b>（<c>risk_level</c> / <c>phone_from</c> /
    /// <c>high_risk_goods</c> / <c>recharge_fields.account_type</c> / <c>living_expenses_fields.type</c>）
    /// ⇒ 按 <c>int?</c> 透传，注释不补第三方数值。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/report-trade-union-information")]
    Task<MerchantRiskManageTradeUnionReportResponse> ReportTradeUnionInformationAsync(
        MerchantRiskManageTradeUnionReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询风险信息」：按商户订单号<b>单笔</b>查询微信侧给出的风险系数与处置手段（无列表、无分页）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467535"/>。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（官方 query <c>out_trade_no</c>，<b>必填</b> string(64)）。</param>
    /// <param name="spMchId">服务商商户号（官方 query <c>sp_mchid</c>，选填 string(32)）。</param>
    /// <param name="acquiringBankId">从业机构号（官方 query <c>acquiring_bank_id</c>，选填 string(32)）。</param>
    /// <param name="channelId">渠道号（官方 query <c>channel_id</c>，选填 string(32)）。</param>
    /// <param name="subMchId">子商户号（官方 query <c>sub_mchid</c>，官方标选填、描述内注明服务商/从业机构/渠道商调用时必填，string(32)）。</param>
    /// <param name="complaintsInformation">投诉信息（官方 query <c>complaints_information</c>，选填 string(255)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>风险系数与处置手段，见 <see cref="MerchantRiskManageTradeRiskInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchant-risk-manage/trade-risk-information</c>；
    /// 六个参数<b>全部在 query</b>（本域唯一纯 query 支）。</para>
    /// <para><b><c>out_trade_no</c> 是这支唯一必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>本支不是列表接口</b>：官方正文按单笔单号查，页内零个数组节点
    /// （方案 §2.7 已记录该行的转述错判）⇒ 勿按 <c>query-risk-trades</c> 的分页形态建 DTO。</para>
    /// <para><b>429 有两支码</b>：<c>FREQUENCY_LIMITED</c> 与 <c>RATELIMIT_EXCEEDED</c>（官方文案「请降低频率后重试」）。</para>
    /// </remarks>
    [Get("/v3/merchant-risk-manage/trade-risk-information")]
    Task<MerchantRiskManageTradeRiskInfoResponse> QueryTradeRiskInformationAsync(
        [Query("out_trade_no")] string outTradeNo,
        [Query("sp_mchid")] string? spMchId = null,
        [Query("acquiring_bank_id")] string? acquiringBankId = null,
        [Query("channel_id")] string? channelId = null,
        [Query("sub_mchid")] string? subMchId = null,
        [Query("complaints_information")] string? complaintsInformation = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「处理结果回传」（风险共建）：把商户或从业机构侧执行完的处置手段回传微信。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467540"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantRiskManageTradeRiskResultRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显标识与处置结论，见 <see cref="MerchantRiskManageTradeRiskResultResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/trade-risk-result</c>；
    /// 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】。</para>
    /// <para><b><c>punish_type</c> 必填</b>（10 值），<c>out_trade_no</c> 反而<b>选填</b> ⇒ 与直觉相反，照页。</para>
    /// <para><b>应答多出 <c>punish_description</c></b>（请求侧是 <c>additional_punish_type</c>）⇒ 请求与应答类型不共用。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/trade-risk-result")]
    Task<MerchantRiskManageTradeRiskResultResponse> ReportTradeRiskResultAsync(
        MerchantRiskManageTradeRiskResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「创建商户违规通知回调地址」：登记接收违规处罚 / 拦截通知的回调 URL。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471333"/>。
    /// </summary>
    /// <param name="request">请求体（单字段 <c>notify_url</c>），见 <see cref="MerchantRiskManageViolationNotificationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显 <c>notify_url</c>（HTTP 200 + JSON），见 <see cref="MerchantRiskManageViolationNotificationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-risk-manage/violation-notifications</c>；
    /// 支持商户【普通服务商】【从业机构（银行）】【从业机构（支付机构）】【渠道商】。</para>
    /// <para><b>已设置过再创建报 <c>409 ALREADY_EXISTS</c></b> ⇒ 幂等重放要走
    /// <see cref="UpdateViolationNotificationUrlAsync"/>，不要捕获 409 当成功。</para>
    /// <para><b>回调 URL 口径</b>（官方原文）：外网可访问、<b>不得携带参数</b>、公网域名必须 HTTPS（专线可用 HTTP），
    /// 通知应答须验签。</para>
    /// </remarks>
    [Post("/v3/merchant-risk-manage/violation-notifications")]
    Task<MerchantRiskManageViolationNotificationResponse> CreateViolationNotificationAsync(
        MerchantRiskManageViolationNotificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改商户违规通知回调地址」：<b>PUT</b> 同一路由，成功应答是 <b>200 + <c>{notify_url}</c>（不是 204）</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471330"/>。
    /// </summary>
    /// <param name="request">请求体（单字段 <c>notify_url</c>），见 <see cref="MerchantRiskManageViolationNotificationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显 <c>notify_url</c>，见 <see cref="MerchantRiskManageViolationNotificationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PUT</b> <c>/v3/merchant-risk-manage/violation-notifications</c>（支付线<b>第一支 PUT</b>）。</para>
    /// <para><b>与创建支共用请求与应答类型</b>：官方两页请求表、应答表逐字段相同（守卫 MRB-B6）。</para>
    /// <para><b>未设置过时修改报 <c>404 NOT_FOUND</c>（官方文案「更新的通知回调不存在」）</b>
    /// ⇒ 本支不是 upsert，须先创建。</para>
    /// </remarks>
    [Put("/v3/merchant-risk-manage/violation-notifications")]
    Task<MerchantRiskManageViolationNotificationResponse> UpdateViolationNotificationUrlAsync(
        MerchantRiskManageViolationNotificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询商户违规通知回调地址」：无参数的 GET，回当前生效的 <c>notify_url</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471327"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回 <c>notify_url</c>（选填，未配置时官方可不返回该字段），见 <see cref="MerchantRiskManageViolationNotificationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchant-risk-manage/violation-notifications</c>；无 path / query 参数。</para>
    /// <para><b>「没配过」有两种表现</b>：应答字段缺失<b>或</b> <c>404 NOT_FOUND</c> ⇒ 宿主两种都要接得住。</para>
    /// </remarks>
    [Get("/v3/merchant-risk-manage/violation-notifications")]
    Task<MerchantRiskManageViolationNotificationResponse> QueryViolationNotificationAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除商户违规通知回调地址」：<b>DELETE</b> 同一路由，成功应答是 <b>204 No Content（无包体）</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471334"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回内容（204）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>DELETE</b> <c>/v3/merchant-risk-manage/violation-notifications</c>（支付线<b>第一支 DELETE</b>）；
    /// 请求侧只有 <c>Authorization</c> 与 <c>Accept</c> 两个头，无包体、无参数。</para>
    /// <para><b>204 无包体 ⇒ 返回裸 <see cref="Task"/></b>：本域唯一无 JSON 应答的端点，
    /// 但同路由的 PUT 是 200 + JSON ⇒ 「写操作都 204」在本域不成立（守卫 MRB-B1 断言返回类型）。</para>
    /// <para><b>未设置时删除报 <c>404 NOT_FOUND</c></b>；删除后微信停止推送违规通知。</para>
    /// </remarks>
    [Delete("/v3/merchant-risk-manage/violation-notifications")]
    Task DeleteViolationNotificationAsync(
        CancellationToken cancellationToken = default);
}
